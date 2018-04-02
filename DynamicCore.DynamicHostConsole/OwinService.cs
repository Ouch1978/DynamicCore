using System;
using Microsoft.Owin.Hosting;

namespace DynamicCore.DynamicHostConsole
{
    public class OwinService
    {
        private IDisposable _webApp;
 
        public void Start()
        {
            _webApp = WebApp.Start<StartOwin>("http://localhost:9000");
        }
 
        public void Stop()
        {
            _webApp.Dispose();
        }
    }
}
