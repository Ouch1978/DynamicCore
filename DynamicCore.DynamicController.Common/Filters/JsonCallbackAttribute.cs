using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace DynamicCore.DynamicController.Common.Filters
{
    public class JsonCallbackAttribute : ActionFilterAttribute
    {
        private const string CallbackQueryParameter = "callback";

        public override void OnActionExecuted( HttpActionExecutedContext context )
        {
            var callback = string.Empty;

             NameValueCollection queries = HttpUtility.ParseQueryString(context.Request.RequestUri.Query );

            callback = queries[CallbackQueryParameter];

            if( callback != null )
            {
                var jsonBuilder = new StringBuilder( callback );

                jsonBuilder.AppendFormat( "({0})" , context.Response.Content.ReadAsStringAsync().Result );

                context.Response.Content = new StringContent( jsonBuilder.ToString() );
            }

            base.OnActionExecuted( context );
        }
    }
}