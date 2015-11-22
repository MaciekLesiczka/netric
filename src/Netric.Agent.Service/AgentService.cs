using System.Threading.Tasks;
using Akka.Actor;
using Topshelf;
using Netric.EventConsumer.Flamegraphs;

namespace Netric.Agent.Service
{
    class AgentService : ServiceControl
    {
        private ActorSystem _agentSystem;
        private Task _ongoingTask;

        public bool Start(HostControl hostControl)
        {
            _agentSystem = ActorSystem.Create("netricagent");

            var consumer = _agentSystem.ActorOf<RequestConsumer>("consumer");
            var receiver = _agentSystem.ActorOf(Props.Create(() => new EtwEventProcessingActor(consumer)),"receiver");
            _ongoingTask = new Task(()=>new EventReceiver(receiver).Start());
            _ongoingTask.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _agentSystem.Shutdown();
            return true;
        }
    }
}