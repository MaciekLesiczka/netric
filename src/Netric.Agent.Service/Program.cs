using Topshelf;

namespace Netric.Agent.Service
{
    class Program
    {
        static int Main()
        {
            return (int)HostFactory.Run(x =>
            {
                x.SetServiceName("NetricAgent");
                x.SetDisplayName("Netric Agent");
                x.SetDescription("Netric Agent - service collecting website performance metrics");
                x.UseAssemblyInfoForServiceInfo();
                x.RunAsLocalSystem();
                x.StartAutomatically();                
                x.Service<AgentService>();
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
