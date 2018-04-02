using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DynamicCore.DynamicController.Common.Helpers
{
    public static class ControllersHelper
    {
        private static List<string> _enabledControllers = new List<string>();

        private static void ReadConfigForEnabledControllers()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            KeyValueConfigurationCollection settings = config.AppSettings.Settings;   

            _enabledControllers = settings[ "EnabledControllers" ].Value.Split( ',' ).ToList();
        }

        public static bool IsControllerEnabled( string controllerName )
        {
            ReadConfigForEnabledControllers();

            return _enabledControllers.Contains( controllerName );
        }

    }
}
