using System;
using System.Collections.Generic;
using Netric.Shared.Clr;

namespace Netric.Shared
{
    public class Request
    {
        private readonly Guid _id;
        private readonly string _url;
        private readonly DateTime _startTime;
        private readonly long _elapsedTicks;
        private readonly List<Method> _callStack;

        public Request(Guid id,string url, DateTime startTime, long elapsedTicks, List<Method> callStack)
        {
            _id = id;
            _url = url;
            _startTime = startTime;
            _elapsedTicks = elapsedTicks;
            _callStack = callStack;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
        } 

        public long ElapsedTicks
        {
            get { return _elapsedTicks; }
        }

        /// <summary>
        /// Methods sorted by execution order
        /// </summary>
        public IReadOnlyCollection<Method> CallStack
        {
            get { return _callStack; }
        }

        public string Url
        {
            get { return _url; }
        }
    }
}