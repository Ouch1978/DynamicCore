using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace DynamicCore.DynamicController.Common.MessageHandlers
{
    public class ContentLogMessageHandler : DelegatingHandler
    {
        private static readonly ILog _logger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name );

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request , CancellationToken cancellationToken )
        {
            bool isHelpPageRequest = false;

            if( request.Method.Method == "POST" || request.Method.Method == "PUT" || request.Method.Method == "DELETE" )
            {
                var content = request.Content.ReadAsStringAsync().Result;
                content = content.IndexOf( "Password" , System.StringComparison.Ordinal ) > -1
                    ? content.Substring( 0 , content.IndexOf( "Password" , System.StringComparison.Ordinal ) - 2 ) + "}" : content;
                _logger.Debug( content );
            }
            else if( request.Method.Method == "GET" )
            {
                _logger.Debug( request.RequestUri.OriginalString );
            }

            if( request.Method.Method == "GET" && request.RequestUri.PathAndQuery.ToLower().StartsWith( "/help" ) )
            {
                isHelpPageRequest = true;
            }

            return base.SendAsync( request , cancellationToken )
                        .ContinueWith( task =>
                        {
                            HttpResponseMessage response = task.Result;
                            _logger.Debug( isHelpPageRequest == true ? "HelpPageResponse" : response.Content.ReadAsStringAsync().Result );
                            return response;
                        } , cancellationToken );
        }
    }
}