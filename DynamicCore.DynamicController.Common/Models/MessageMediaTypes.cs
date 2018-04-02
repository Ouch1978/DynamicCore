using System.ComponentModel;


namespace DynamicCore.DynamicController.Common.Models
{
    public enum MessageMediaTypes
    {
        [Description( "application/json" )]
        Json ,
        [Description( "application/xml" )]
        Xml
    }
}
