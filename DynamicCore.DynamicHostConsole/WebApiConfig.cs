using System.Web.Http;
using DynamicCore.DynamicController.Common.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DynamicCore.DynamicHostConsole
{
    public static class WebApiConfig
    {
        public static void Register( HttpConfiguration config )
        {
            if( config.Routes.ContainsKey( "DefaultApi" ) == false )
            {
                config.Routes.MapHttpRoute(
                    name: "DefaultApi" ,
                    routeTemplate: "{controller}/{action}/{id}" ,
                    defaults: new
                    {
                        action = "Get" ,
                        id = RouteParameter.Optional
                    }
                    , constraints: new
                    {
                        controller = @"[^\.]*"
                    }
                    );

                config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore ,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                config.Filters.Add( new JsonCallbackAttribute() );

            }

            if( config.Routes.ContainsKey( "ApiByAction" ) == false )
            {
                config.Routes.MapHttpRoute(
                    name: "ApiByAction" ,
                    routeTemplate: "{controller}/{action}" ,
                    defaults: new
                    {
                        action = "Get"
                    } ,
                    constraints: new
                    {
                        controller = @"[^\.]*"
                    }
                    );

                config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore ,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                config.Filters.Add( new JsonCallbackAttribute() );

            }

        }
    }
}
