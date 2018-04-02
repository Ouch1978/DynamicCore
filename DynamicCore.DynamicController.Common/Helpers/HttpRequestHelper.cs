using System.Net.Http;

namespace DynamicCore.DynamicController.Common.Helpers
{
    public class HttpRequestHelper
    {
        private static string SendAsyncAndGetStringResult( HttpRequestMessage request , HttpClient client )
        {
            string response = client.SendAsync( request ).Result.Content.ReadAsStringAsync().Result;
            return response;
        }
    }
}
