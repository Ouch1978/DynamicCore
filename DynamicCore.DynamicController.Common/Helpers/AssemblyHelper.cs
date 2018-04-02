using System;
using System.Configuration;
using System.IO;
using System.Linq;
using DynamicCore.DynamicController.Common.Constants;

namespace DynamicCore.DynamicController.Common.Helpers
{
    public static class AssemblyHelper
    {
        private static readonly string DynamicDllsPath = ConfigurationManager.AppSettings[ DynamicCoreConstants.DynamicControllerDllPath ];

        public static string GetAssemblyFullPath( string assemblyFileName )
        {
            string fullPath;

            if( DynamicDllsPath.Contains( "," ) == true )
            {
                fullPath = DynamicDllsPath.Split( ',' ).Select( p => p + assemblyFileName ).FirstOrDefault( p => File.Exists( p ) == true );
            }
            else
            {
                fullPath = DynamicDllsPath + assemblyFileName;
            }

            return fullPath;
        }


        public static string GetAssemblyFullName( byte[] assemblyBytes )
        {
            var tempDomain = AppDomain.CreateDomain( "TempDomain" );

            string fullName = tempDomain.Load( assemblyBytes ).FullName;

            AppDomain.Unload( tempDomain );

            return fullName;
        }
    }
}
