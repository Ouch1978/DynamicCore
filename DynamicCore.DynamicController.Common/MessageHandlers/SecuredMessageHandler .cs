using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicCore.DynamicController.Common.Extensions;
using log4net;

namespace DynamicCore.DynamicController.Common.MessageHandlers
{
    /// <summary>
    /// Decrypt and encrypt Http request body between client and server.
    /// </summary>
    public class SecuredMessageHandler : DelegatingHandler
    {
        private static readonly ILog _logger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name );
        private string _key;
        private string _iv;

        public SecuredMessageHandler( string key , string iv )
        {
            _key = key;
            _iv = iv;
        }

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request , CancellationToken cancellationToken )
        {
            request = DecryptMessage( request );
            return base.SendAsync( request , cancellationToken )
                .ContinueWith( task =>
                {
                    HttpResponseMessage response = task.Result;
                    response = EncryptMessage( response );
                    return response;
                } , cancellationToken );
        }

        private HttpResponseMessage EncryptMessage( HttpResponseMessage response )
        {
            if( response.RequestMessage == null )
            {
                return response;
            }

            string data = response.Content.ReadAsStringAsync().Result;
            var content = new StringContent( data.Encrypt( _key , _iv ) , Encoding.UTF8 , response.Content.Headers.ContentType.MediaType );
            response.Content.Dispose();
            response.Content = content;

            return response;
        }

        private HttpRequestMessage DecryptMessage( HttpRequestMessage request )
        {
            if( request.Method == HttpMethod.Post )
            {
                var encryptedMessage = request.Content.ReadAsStringAsync().Result;
                var data = encryptedMessage.Decrypt( _key , _iv );
                var content = new StringContent( data , Encoding.UTF8 , request.Content.Headers.ContentType.MediaType );
                request.Content.Dispose();
                request.Content = content;
            }
            return request;
        }
    }
}