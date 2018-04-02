using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using DynamicCore.DynamicController.Common.Constants;
using DynamicCore.DynamicController.Common.Helpers;
using log4net;

namespace DynamicCore.DynamicHostConsole
{
    public class BypassCacheSelector : DefaultHttpControllerSelector
    {
        private static readonly ILog Logger = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType.Name );

        private readonly HttpConfiguration _configuration;

        private static string _dynamicDllsPath;

        private string _enabledControllers;

        public BypassCacheSelector( HttpConfiguration configuration )
            : base( configuration )
        {
            _configuration = configuration;

            _dynamicDllsPath = ConfigurationManager.AppSettings[ "DynamicControllerDllPath" ];
        }

        public override HttpControllerDescriptor SelectController( HttpRequestMessage request )
        {

            var controllerName = base.GetControllerName( request );

            if( controllerName.ToLower() == "help" )
            {
                return new HttpControllerDescriptor( _configuration , controllerName , typeof( HelpController ) );
            }

            Logger.DebugFormat( "Incoming request is calling to controller : {0}." , controllerName );

            if( ControllersHelper.IsControllerEnabled( controllerName ) == false )
            {
                string errorMessage = "Incoming request is calling to invalid controller.";

                Logger.ErrorFormat( errorMessage );
                throw new Exception( errorMessage );
            }

            try
            {
                var assemblyFullPath = AssemblyHelper.GetAssemblyFullPath( string.Format( "{0}{1}.dll" , controllerName , DynamicCoreConstants.DllSuffix ) );

                Logger.DebugFormat( "Loading assembly : {0}" , assemblyFullPath );

                byte[] assemblyBytes = File.ReadAllBytes( assemblyFullPath );

                string assemblyFullName = AssemblyHelper.GetAssemblyFullName( assemblyBytes );

                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault( a => a.FullName == assemblyFullName );

                if( assembly == null )
                {
                    assembly = Assembly.Load( assemblyBytes );
                }

                var referencedAssemblies = assembly.GetReferencedAssemblies();

                foreach( var referencedAssembly in referencedAssemblies )
                {
                    string referencedAssemblyFullPath = string.Format( "{0}{1}.dll" , _dynamicDllsPath , referencedAssembly.Name );

                    if( File.Exists( referencedAssemblyFullPath ) == true )
                    {
                        byte[] referencedaAsemblyBytes = File.ReadAllBytes( referencedAssemblyFullPath );

                        assemblyFullName = AssemblyHelper.GetAssemblyFullName( referencedaAsemblyBytes );

                        var referencedaAsembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault( a => a.FullName == assemblyFullName );

                        if( referencedaAsembly == null )
                        {
                            Assembly.Load( referencedaAsemblyBytes );
                        }
                    }
                }

                var types = assembly.GetTypes();

                var matchedTypes = types.Where( i => typeof( IHttpController ).IsAssignableFrom( i ) ).ToList();

                Logger.DebugFormat( "Assembly {0} loaded, file size is {1} bytes, {2} types found and {3} types matched." , assemblyFullPath , assemblyBytes.Length , types.Length , matchedTypes.Count );

                var matchedController =
                    matchedTypes.FirstOrDefault( i => i.Name.ToLower() == controllerName.ToLower() + "controller" );

                if( matchedController != null )
                {
                    Logger.DebugFormat( "Controller matched." );
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Logger.DebugFormat( "Memory Usage : {0}" , GC.GetTotalMemory( true ) );

                return new HttpControllerDescriptor( _configuration , controllerName , matchedController );

            }
            catch( ReflectionTypeLoadException ex )
            {
                var stringBuilder = new StringBuilder();

                foreach( Exception exception in ex.LoaderExceptions )
                {
                    stringBuilder.AppendLine( exception.Message );

                    var fileNotFoundException = exception as FileNotFoundException;

                    if( fileNotFoundException != null )
                    {
                        if( !string.IsNullOrEmpty( fileNotFoundException.FusionLog ) )
                        {
                            stringBuilder.AppendLine( "Fusion Log:" );
                            stringBuilder.AppendLine( fileNotFoundException.FusionLog );
                        }
                    }
                    stringBuilder.AppendLine();
                }

                Logger.ErrorFormat( "Fail to load assembly : {0}" , stringBuilder );

                throw;
            }
        }
    }
}
