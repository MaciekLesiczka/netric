using System;
using Microsoft.Owin.Hosting;

namespace Netric.Agent.Web
{
    class Program
    {
        static void Main()
        {
            using (WebApp.Start<Startup>("http://localhost:8080"))
            {
                Console.ReadLine();
            }            
        }
    }
}
