using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using DynamicCore.DynamicController.Common.Models;

namespace DynamicCore.DynamicController.Common.Helpers
{
    public static class SerializerHelper
    {
        public static string Serialize<T>( T value , MessageMediaTypes mediaType = MessageMediaTypes.Json )
        {
            var formatter = ( mediaType == MessageMediaTypes.Json ) ? ( MediaTypeFormatter ) new JsonMediaTypeFormatter() : ( MediaTypeFormatter ) new XmlMediaTypeFormatter();

            Stream stream = new MemoryStream();
            var content = new StreamContent( stream );
            formatter.WriteToStreamAsync( typeof( T ) , value , stream , content , null ).Wait();
            stream.Position = 0;
            return content.ReadAsStringAsync().Result;
        }

        public static T Deserialize<T>( string content , MessageMediaTypes mediaType = MessageMediaTypes.Json ) where T : class
        {
            var formatter = ( mediaType == MessageMediaTypes.Json ) ? ( MediaTypeFormatter ) new JsonMediaTypeFormatter() : ( MediaTypeFormatter ) new XmlMediaTypeFormatter();

            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter( stream );
            writer.Write( content );
            writer.Flush();
            stream.Position = 0;
            return formatter.ReadFromStreamAsync( typeof( T ) , stream , null , null ).Result as T;
        }
    }
}