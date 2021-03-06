using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.NetricInterceptClr;
using Microsoft.Diagnostics.Tracing.Parsers.NetricInterceptWebRequest;
using Netric.Agent.Clr;
using Netric.Shared;
using Netric.Shared.Clr;
using NLog;

namespace Netric.Agent
{
    /// <summary>
    /// Collects all profilee ETW events and wrap them up into one request statistics object
    /// </summary>
    public class EtwEventProcessingActor : ReceiveActor
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IActorRef _requestStatsConsumer;        
        readonly Dictionary<ThreadInfo, RequestBegin> _currentRequests = new Dictionary<ThreadInfo, RequestBegin>();
        
        readonly Dictionary<ThreadInfo,RequestCallStack> _currentCallstacks = new Dictionary<ThreadInfo, RequestCallStack>();

        public EtwEventProcessingActor(IActorRef requestStatsConsumer)
        {
            _requestStatsConsumer = requestStatsConsumer;            
            Receive<MethodEnter>(x => OnMethodEnter(x));
            Receive<MethodLeave>(x => OnMethodLeave(x));            
            Receive<RequestBegin>(x => OnRequestBegin(x));
            Receive<RequestEnd>(x => OnRequestEnd(x));
        }

        private void OnMethodEnter(MethodEnter args)
        {
            if (_currentCallstacks.ContainsKey(args.Thread))
            {
                _currentCallstacks[args.Thread].RegisterMethodEnter(args);
                Logger.Trace("Enter:{0}", args.Name);
            }
        }

        private void OnMethodLeave(MethodLeave args)
        {
            if (_currentCallstacks.ContainsKey(args.Thread))
            {
                _currentCallstacks[args.Thread].RegisterMethodLeave(args);                
                Logger.Trace("Leave:{0}", args.Name);
            }            
        }
        
        private void OnRequestBegin(RequestBegin args)
        {            
            EnsureBeginUniqueCall(args);
            _currentRequests[args.Thread] = args;
            _currentCallstacks[args.Thread] = new RequestCallStack();

            Logger.Trace("BEGIN Request {1}{0}", args.Url, args.Thread);            
        }

        private void EnsureBeginUniqueCall(RequestBegin args)
        {
            if (_currentRequests.ContainsKey(args.Thread) && _currentRequests[args.Thread].Id == args.Id)
            {
                throw new InvalidOperationException(string.Format("Request with id = {0} already begun.", args.Id));
            }
        }

        private void OnRequestEnd(RequestEnd args)
        {
            if (_currentRequests.ContainsKey(args.Thread))
            {
                if (_currentRequests[args.Thread].Id == args.Id)
                {
                    Logger.Trace("END Request {1}{0}", _currentRequests[args.Thread].Url, args.Thread);
                    var request = CreateRequestStats(_currentRequests[args.Thread],args);
                    _requestStatsConsumer.Tell(request);
                    _currentRequests.Remove(args.Thread);                    
                }
                else
                {                    
                    Logger.Warn("Current Thread with id = {0} is handling different request. Expected: {1}, actual: {2}", args.Thread.ThreadId, args.Id, _currentRequests[args.Thread].Id);
                }
            }            
        }

        private Request CreateRequestStats( RequestBegin begin, RequestEnd end)
        {
            List<Method> methods = null;
            if (_currentCallstacks.ContainsKey(end.Thread))
            {
                methods = _currentCallstacks[end.Thread].ToList();
                _currentCallstacks.Remove(end.Thread);
            }
            return new Request(begin.Id,begin.Url,begin.Time,(end.Time - begin.Time).Ticks,methods);
        }

        public class ProfileeEvent
        {
            private readonly ThreadInfo _thread;
            private DateTime _time;

            public ProfileeEvent(TraceEvent traceEvent)                
            {
                _thread = ThreadInfo.Of(traceEvent);
                Time = traceEvent.TimeStamp;
            }

            public ThreadInfo Thread
            {
                get { return _thread; }
            }

            public DateTime Time
            {
                get { return _time; }
                set { _time = value; }
            }
        }

        public class RequestBegin : ProfileeEvent
        {
            private readonly Guid _id;
            private readonly string _site;
            private readonly string _url;

            public RequestBegin(OnBeginArgs args)
                : base(args)
            {
                _url = args.Url;
                _site = args.Site;
                _id = new Guid(args.Id);
            }

            public Guid Id
            {
                get { return _id; }
            }

            public string Site
            {
                get { return _site; }
            }

            public string Url
            {
                get { return _url; }
            }
        }

        public class RequestEnd : ProfileeEvent
        {
            private readonly Guid _id;

            public RequestEnd(OnEndArgs args) : base(args)
            {
                _id = new Guid(args.Id);
            }

            public Guid Id
            {
                get { return _id; }
            }
        }

        public class MethodEvent : ProfileeEvent, IMethodEvent
        {
            private readonly string _name;
            private readonly long _callId;
            private readonly long _ticks;

            protected MethodEvent(string name,long callId,TraceEvent traceEvent)
                : base(traceEvent)
            {
                _name = name;
                _callId = callId;
                _ticks = traceEvent.TimeStamp.Ticks;
            }

            public long CallId
            {
                get { return _callId; }
            }

            public long Ticks
            {
                get { return _ticks; }
            }

            public string Name
            {
                get { return _name; }
            }
        }

        public class MethodEnter : MethodEvent
        {
            public MethodEnter(OnEnterArgs args)
                : base(args.Method,args.CallId, args)
            {
            }
        }

        public class MethodLeave : MethodEvent
        {
            public MethodLeave(OnLeaveArgs args)
                : base(args.Method,args.CallId, args)
            {
            }
        }
    }
}