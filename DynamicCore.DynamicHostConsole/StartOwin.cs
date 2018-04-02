using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using DynamicCore.DynamicController.Common.Constants;
using DynamicCore.DynamicController.Common.Filters;
using DynamicCore.DynamicController.Common.MessageHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace DynamicCore.DynamicHostConsole
{
    public class StartOwin
    {
        public void Configuration( IAppBuilder appBuilder )
        {
            var config = new HttpConfiguration();

            config.Services.Replace( typeof( IHttpControllerTypeResolver ) , new CustomHttpControllerTypeResolver() );

            config.Services.Replace( typeof( IHttpControllerSelector ) , new BypassCacheSelector( config ) );

            config.Routes.MapHttpRoute(
                name: "DefaultApi" ,
                routeTemplate: "{controller}/{action}/{id}" ,
                defaults: new
                {
                    action = RouteParameter.Optional,
                    id = RouteParameter.Optional
                } ,
                constraints: new
                {
                    controller = @"[^\.]*"
                }
            );

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
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore ,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "text/html" ) );

            config.Filters.Add( new JsonCallbackAttribute() );

            config.Filters.Add( new MessageLogAttribute() );

            if( DynamicCoreConstants.IsEncryptionEnabled )
            {
                config.MessageHandlers.Add( new SecuredMessageHandler( DynamicCoreConstants.Key , DynamicCoreConstants.Iv ) );
            }

            HelpPageConfig.Register( config );

            appBuilder.UseWebApi( config );
        }
    }
}

