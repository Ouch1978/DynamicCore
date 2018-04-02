using System;
using System.Configuration;

namespace DynamicCore.DynamicController.Common.Constants
{
    public class DynamicCoreConstants
    {
        public const string DllSuffix = "ApiLibrary";

        public const string DynamicControllerDllPath = "DynamicControllerDllPath";

        public static readonly bool IsMessageLogEnabled = Convert.ToBoolean( ConfigurationManager.AppSettings[ "IsMessageLogEnabled" ] );

        public static readonly bool IsEncryptionEnabled = Convert.ToBoolean( ConfigurationManager.AppSettings[ "IsEncryptionEnabled" ] );

        public static readonly string Key = ConfigurationManager.AppSettings[ "Key" ];
        public static readonly string Iv = ConfigurationManager.AppSettings[ "IV" ];

    }
}
