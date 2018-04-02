using System.Configuration;
using System.Diagnostics;

namespace DynamicCore.DynamicController.Common.Helpers
{
    public static class CustomConfigurationHelper
    {
        public static string GetAppSettingValue( string key )
        {
            var callerType = new StackFrame( 1 , true ).GetMethod().DeclaringType;

            if( callerType == null )
            {
                return string.Empty;
            }

            var DynamicControllerDllPath = AssemblyHelper.GetAssemblyFullPath( callerType.Assembly.ManifestModule.ToString() );

            var dynamicAssemblyConfiguration = ConfigurationManager.OpenExeConfiguration( DynamicControllerDllPath );

            var appSettingsSection = ( AppSettingsSection ) dynamicAssemblyConfiguration.GetSection( "appSettings" );

            return appSettingsSection.Settings[ key ].Value;
        }

        public static string GetSectionSettingValue( string sectionName , string key )
        {
            var callerType = new StackFrame( 1 , true ).GetMethod().DeclaringType;

            if( callerType == null )
            {
                return string.Empty;
            }

            var DynamicControllerDllPath = AssemblyHelper.GetAssemblyFullPath( callerType.Assembly.ManifestModule.ToString() );

            var dynamicAssemblyConfiguration = ConfigurationManager.OpenExeConfiguration( DynamicControllerDllPath );

            var appSettingsSection = ( AppSettingsSection ) dynamicAssemblyConfiguration.GetSection( sectionName );

            return appSettingsSection.Settings[ key ].Value;
        }
    }
}
