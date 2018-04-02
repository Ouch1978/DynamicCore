using System;

namespace DynamicCore.DynamicController.Common.Models
{
    public class ResponseBase
    {
        public string ReturnCode
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string SysDateTime
        {
            get
            {
                return DateTime.Now.ToString( "yyyy/MM/dd hh:mm:ss zzz" );
            }
        }

    }
}
