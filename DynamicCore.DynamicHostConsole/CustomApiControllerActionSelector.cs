using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace DynamicCore.DynamicHostConsole
{
    public class CustomApiControllerActionSelector : ApiControllerActionSelector
    {

        public override HttpActionDescriptor SelectAction( HttpControllerContext controllerContext )
        {

            IHttpRouteData routeData = controllerContext.RouteData;
            bool containsAction = routeData.Values.ContainsKey( "action" );

            if( containsAction )
            {
                return base.SelectAction( controllerContext );
            }

            try
            {
                routeData.Values[ "action" ] = controllerContext.Request.Method.Method;

                return base.SelectAction( controllerContext );

            }
            finally
            {

                routeData.Values.Remove( "action" );
            }
        }
    }
}
