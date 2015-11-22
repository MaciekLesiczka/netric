using Akka.Actor;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace Netric.Agent
{
    public class EventReceiver
    {
        private readonly IActorRef _receiverActor;

        public EventReceiver(IActorRef receiverActor)
        {
            _receiverActor = receiverActor;
        }

        public void Start()
        {
            using (var session = new TraceEventSession("netric"))
            {
                session.EnableProvider(NetricInterceptWebRequestTraceEventParser.ProviderName);
                session.EnableProvider(NetricInterceptWebNavigationTimingTraceEventParser.ProviderName);
                session.EnableProvider(NetricInterceptClrTraceEventParser.ProviderName);

                using (var source = new ETWTraceEventSource("netric", TraceEventSourceType.Session))
                {
                    var requestTraceEventParser = new NetricInterceptWebRequestTraceEventParser(source);
                    requestTraceEventParser.OnBegin+= x => _receiverActor.Tell(new EtwEventProcessingActor.RequestBegin(x));
                    requestTraceEventParser.OnEnd += x => _receiverActor.Tell(new EtwEventProcessingActor.RequestEnd(x));

                    var navigationTimingTraceEventParser = new NetricInterceptWebNavigationTimingTraceEventParser(source);
                    navigationTimingTraceEventParser.Statistics += x => _receiverActor.Tell(x);

                    var clrTraceEventParser = new NetricInterceptClrTraceEventParser(source);
                    clrTraceEventParser.OnEnter += x => _receiverActor.Tell(new EtwEventProcessingActor.MethodEnter(x));
                    clrTraceEventParser.OnLeave += x => _receiverActor.Tell(new EtwEventProcessingActor.MethodLeave(x));    
                    source.Process();
                }               
            }
        }
    }
}
