using System.Configuration;
using System.Web.Http;
using DynamicCore.DynamicController.Common.Constants;
using DynamicCore.HelpPageLibrary;
using DynamicCore.HelpPageLibrary.Extensions;

namespace DynamicCore.DynamicHostConsole
{
    public static class HelpPageConfig
    {
        public static void Register( HttpConfiguration config )
        {
            config.SetDocumentationProvider( new XmlDocumentationProvider( ConfigurationManager.AppSettings[ DynamicCoreConstants.DynamicControllerDllPath ] ) );
        }
    }
}
