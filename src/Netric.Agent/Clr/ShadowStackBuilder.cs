using System.Collections.Generic;
using Netric.Shared.Clr;

namespace Netric.Agent.Clr
{
    public class ShadowStackBuilder
    {
        private struct CallToken
        {
            public long Id { get; set; }
            public long TicksTime { get; set; }
            public long NestedElapsed { get; set; }
        }

        private readonly Dictionary<ThreadInfo, Stack<CallToken>> _stackLevels = new Dictionary<ThreadInfo, Stack<CallToken>>();

        public void TraceEnter(IMethodEnter methodEnter)
        {
            if (!_stackLevels.ContainsKey(methodEnter.Thread))
            {
                _stackLevels[methodEnter.Thread] = new Stack<CallToken>();
            }
            _stackLevels[methodEnter.Thread].Push(new CallToken() { TicksTime = methodEnter.Ticks, Id = methodEnter.CallId });
        }

        public MethodStats TraceLeave(IMethodLeave clrEvent)
        {
            
            var stack = _stackLevels[clrEvent.Thread];
            var lastEnter = stack.Pop();
            var handledException = false;
            while (lastEnter.Id != clrEvent.CallId)
            {
                handledException = true;
                lastEnter = stack.Pop();
            }
            var elapsedInclusive = clrEvent.Ticks - lastEnter.TicksTime;
            var result = new MethodStats
            {
                StackLevel = stack.Count, ElapsedInclusive = elapsedInclusive, HandledException = handledException
            };
            if (!result.HandledException)
            {
                result.ElapsedExclusive = elapsedInclusive - lastEnter.NestedElapsed;
            }

            if (result.StackLevel > 0)
            {
                var parentToken = stack.Pop();
                stack.Push(new CallToken { Id = parentToken.Id, TicksTime = parentToken.TicksTime, NestedElapsed = (parentToken.NestedElapsed + elapsedInclusive) });
            }

            return result;
        }
    }
}