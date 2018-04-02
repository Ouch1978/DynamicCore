using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DynamicCore.DynamicController.Common.Helpers;
using log4net;
using Topshelf;

namespace DynamicCore.DynamicHostConsole
{
    public class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

        private static readonly IDictionary<string , Assembly> AdditionalAssemblies = new Dictionary<string , Assembly>();

        static int Main( string[] args )
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler( DynamicCoreResolveEventHandler );
            currentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler( DynamicCoreResolveEventHandler );

            return ( int ) HostFactory.Run( x =>
               {
                   x.Service<OwinService>( s =>
                 {
                       s.ConstructUsing( () => new OwinService() );
                       s.WhenStarted( service => service.Start() );
                       s.WhenStopped( service => service.Stop() );
                   } );
               } );
        }

        private static Assembly DynamicCoreResolveEventHandler( object sender , ResolveEventArgs args )
        {
            Assembly referencedAssembly = null;

            string referencedAssemblyFullPath = args.Name;

            if( referencedAssemblyFullPath.Contains( "," ) == true )
            {
                var fileName = args.Name.Substring( 0 , args.Name.IndexOf( "," , StringComparison.Ordinal ) ) + ".dll";

                referencedAssemblyFullPath = AssemblyHelper.GetAssemblyFullPath( fileName );

                if( File.Exists( referencedAssemblyFullPath ) == true )
                {
                    bool isAssemblyLoaded = AppDomain.CurrentDomain.GetAssemblies().Any( a => a.FullName == args.Name );

                    if( AdditionalAssemblies.TryGetValue( args.Name , out referencedAssembly ) == true )
                    {
                        return referencedAssembly;
                    }

                    if( isAssemblyLoaded == true )
                    {
                        referencedAssembly = AppDomain.CurrentDomain.GetAssemblies().First( a => a.FullName == args.Name );
                    }
                    else
                    {
                        byte[] referencedAssemblyBytes = File.ReadAllBytes( referencedAssemblyFullPath );

                        //if( args.Name.Contains( "Common.Logging" ) )
                        //{
                        //    referencedAssembly = Assembly.ReflectionOnlyLoadFrom( referencedAssemblyFullPath );
                        //}
                        //else
                        //{
                            referencedAssembly = Assembly.Load( referencedAssemblyBytes );
                        //}

                        AdditionalAssemblies.Add( args.Name , referencedAssembly );
                    }
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Logger.DebugFormat( "Memory Usage : {0}" , GC.GetTotalMemory( true ) );

            return referencedAssembly;
        }

        static void CurrentDomain_UnhandledException( object sender , UnhandledExceptionEventArgs e )
        {
            var exception = e.ExceptionObject as Exception;

            if( exception != null )
            {
                Logger.ErrorFormat( "CurrentDomain_UnhandledException - {0}" , exception.StackTrace );
            }
        }

    }
}
