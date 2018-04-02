using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using DynamicCore.DynamicController.Common.Constants;
using DynamicCore.DynamicController.Common.Helpers;
using log4net;

namespace DynamicCore.HelpPageLibrary
{
    public static class HttpConfigurationImporter
    {
        private static readonly ILog Logger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

        public static readonly string ConfigurationMethodName = "Register";
        public static readonly string ConfigClassName = "WebApiConfig";
        public static readonly string HelpPageConfigClassName = "HelpPageConfig";

        public static Collection<ApiDescription> ApiDescriptionsCollection = new Collection<ApiDescription>();

        public static HttpConfiguration ImportConfigurationFromPath( string dynamicAssemblyPath )
        {
            HttpConfiguration config = new HttpConfiguration();

            string originalDirectory = Environment.CurrentDirectory;

            Assembly mainAssembly = AppDomain.CurrentDomain.GetAssemblies().First( a => a.FullName.Contains( "DynamicCore.DynamicCoreHostConsole" ) );

            Type webApiConfigType = mainAssembly.GetTypes().FirstOrDefault( t => t.Name == ConfigClassName );

            if( webApiConfigType == null )
            {
                Logger.ErrorFormat( "Cannot find the configuration class: '{0}' in {1}" , ConfigClassName , dynamicAssemblyPath );
            }

            MethodInfo registerConfigMethod = webApiConfigType.GetMethod( ConfigurationMethodName , BindingFlags.Static | BindingFlags.Public );

            if( registerConfigMethod == null )
            {
                Logger.ErrorFormat( "Cannot find the static configuration method: '{0}()' in {1}" , ConfigurationMethodName , ConfigClassName );
            }

            var availableControllerNames = new List<string>();

            if( dynamicAssemblyPath.Contains( "," ) == true )
            {
                foreach( var path in dynamicAssemblyPath.Split( ',' ) )
                {
                    LoadAssembly( path , availableControllerNames , config , registerConfigMethod );
                }
            }
            else
            {
                LoadAssembly( dynamicAssemblyPath , availableControllerNames , config , registerConfigMethod );
            }

            Environment.CurrentDirectory = originalDirectory;

            var apiDescriptions = config.Services.GetApiExplorer().ApiDescriptions.ToList();

            foreach( var apiDescription in apiDescriptions )
            {
                if( ApiDescriptionsCollection.FirstOrDefault( a =>
                    a.HttpMethod == apiDescription.HttpMethod
                    && a.ActionDescriptor.ActionName == apiDescription.ActionDescriptor.ActionName
                    && a.ActionDescriptor.ControllerDescriptor.ControllerName == apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName ) == null )
                {
                    ApiDescriptionsCollection.Add( apiDescription );

                    Logger.DebugFormat( "{0} added into ApiDescriptionsCollection." , apiDescription.RelativePath );
                }

                if( availableControllerNames.Contains( apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() ) == false )
                {
                    config.Services.GetApiExplorer().ApiDescriptions.Remove( apiDescription );

                    Logger.DebugFormat( "{0} removed from current ApiDescriptions collection." , apiDescription.RelativePath );
                }
            }

            foreach( var apiDescription in ApiDescriptionsCollection )
            {
                if( config.Services.GetApiExplorer().ApiDescriptions.FirstOrDefault( a =>
                    a.HttpMethod == apiDescription.HttpMethod
                    && a.ActionDescriptor.ActionName == apiDescription.ActionDescriptor.ActionName
                    && a.ActionDescriptor.ControllerDescriptor.ControllerName == apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName ) == null
                    && availableControllerNames.Contains( apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() ) == true )
                {
                    config.Services.GetApiExplorer().ApiDescriptions.Add( apiDescription );

                    Logger.DebugFormat( "{0} added into current ApiDescriptions collection." , apiDescription.RelativePath );
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Logger.DebugFormat( "Memory Usage : {0}" , GC.GetTotalMemory( true ) );

            return config;
        }

        private static HttpConfiguration ImportHelpPageConfiguration( HttpConfiguration config )
        {
            Assembly mainAssembly = AppDomain.CurrentDomain.GetAssemblies().First( a => a.FullName.Contains( "DynamicCore.DynamicCoreHostConsole" ) );

            Type helpPageConfigType = mainAssembly.GetTypes().FirstOrDefault( t => t.Name == HelpPageConfigClassName );

            if( helpPageConfigType != null )
            {
                MethodInfo registerConfigMethod = helpPageConfigType.GetMethod( ConfigurationMethodName , BindingFlags.Static | BindingFlags.Public );

                if( registerConfigMethod == null )
                {
                    throw new InvalidOperationException( string.Format( "Cannot find the static configuration method: '{0}()' in {1}" , ConfigurationMethodName , HelpPageConfigClassName ) );
                }

                Action<HttpConfiguration> registerConfig = Delegate.CreateDelegate( typeof( Action<HttpConfiguration> ) , registerConfigMethod ) as Action<HttpConfiguration>;

                registerConfig( config );
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Logger.DebugFormat( "Memory Usage : {0}" , GC.GetTotalMemory( true ) );

            return config;
        }

        private static void LoadAssembly( string dynamicAssemblyPath , List<string> availableControllerNames , HttpConfiguration config , MethodInfo registerConfigMethod )
        {
            foreach( string fileName in Directory.GetFiles( dynamicAssemblyPath ) )
            {
                if( fileName.EndsWith( string.Format( "{0}.dll" , DynamicCoreConstants.DllSuffix ) ) == true )
                {
                    byte[] assemblyBytes = File.ReadAllBytes( fileName );

                    string assemblyFullName = AssemblyHelper.GetAssemblyFullName( assemblyBytes );

                    var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault( a => a.FullName == assemblyFullName );

                    if( assembly == null )
                    {
                        assembly = Assembly.Load( assemblyBytes );

                        Logger.DebugFormat( "Assembly loaded: {0}." , fileName );
                    }

                    string controllerName = assembly.ManifestModule.ScopeName.Remove( assembly.ManifestModule.ScopeName.IndexOf( DynamicCoreConstants.DllSuffix , System.StringComparison.Ordinal ) ).ToLower();

                    availableControllerNames.Add( controllerName );

                    Logger.DebugFormat( "{0} added into AvailableControllerNames." , controllerName );

                    Action<HttpConfiguration> registerConfig = Delegate.CreateDelegate( typeof( Action<HttpConfiguration> ) , registerConfigMethod ) as Action<HttpConfiguration>;

                    registerConfig( config );

                    ImportHelpPageConfiguration( config );
                }
            }
        }
    }
}