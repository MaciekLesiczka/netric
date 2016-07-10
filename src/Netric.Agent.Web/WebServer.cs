using System;
using Microsoft.Owin.Hosting;

namespace Netric.Agent.Web
{
    public class WebServer
    {
        private readonly string _address;
        private IDisposable _webapp;

        public WebServer(string address)
        {
            _address = address;
        }

        public void Start()
        {
            _webapp = WebApp.Start<Startup>(_address);
        }

        public void Stop()
        {
            if (_webapp != null)
            {
                _webapp.Dispose();    
            }            
        }
    }
}