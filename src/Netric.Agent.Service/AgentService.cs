using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Netric.Agent.Api;
using Topshelf;
using Netric.EventConsumer.Flamegraphs;

namespace Netric.Agent.Service
{
    class AgentService : ServiceControl
    {
        private ActorSystem _agentSystem;
        private Task _ongoingTask;
        private CancellationTokenSource _webApp;

        public bool Start(HostControl hostControl)
        {
            _agentSystem = ActorSystem.Create("netricagent");

            var consumer = _agentSystem.ActorOf<RequestConsumer>("consumer");
            var receiver = _agentSystem.ActorOf(Props.Create(() => new EtwEventProcessingActor(consumer)),"receiver");
            _ongoingTask = new Task(()=>new EventReceiver(receiver).Start());
            _ongoingTask.Start();
            _webApp = App.start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _agentSystem.Shutdown();
            _webApp.Cancel();
            return true; 
        }
    }
}