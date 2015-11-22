using System;
using System.Collections;
using System.Collections.Generic;
using Netric.Shared.Clr;

namespace Netric.Agent.Clr
{
    /// <summary>
    /// Holds methods profiled by request, esorted by execution order
    /// </summary>
    public class RequestCallStack : IEnumerable<Method>
    {
        private readonly List<long> _idList = new List<long>();
        private readonly Dictionary<long,Method> _methodDicitonary = new Dictionary<long, Method>();
        private readonly ShadowStackBuilder _shadowStackBuilder = new ShadowStackBuilder();
        private long _lastMethodCall = -1;

        public void RegisterMethodEnter(IMethodEnter methodEnter)
        {            
            if (methodEnter.CallId < 0)
            {
                throw new ArgumentException("CallId cannot be lower than zero"); 
            }
            if (methodEnter.CallId == -1 || _lastMethodCall >= methodEnter.CallId)
            {
                throw new ArgumentException(string.Format("Methods must be registered in call order. Last method callId: {0}, callId attempt:{1}", _lastMethodCall, methodEnter.CallId)); 
            }
            _lastMethodCall = methodEnter.CallId;

            _idList.Add(methodEnter.CallId);
            _shadowStackBuilder.TraceEnter(methodEnter);
        }

        public void RegisterMethodLeave(IMethodLeave methodLeave)
        {
            var stats = _shadowStackBuilder.TraceLeave(methodLeave);
            _methodDicitonary[methodLeave.CallId] = new Method(methodLeave.Name,stats);
        }

        public IEnumerable<Method> GetAllMethods()
        {
            //methods are returned by CallId - in order that they were called. Let's leverage the fact that RegisterMethodEnter is called in proper order, instad of sorting collection by CallId
            foreach (var l in _idList)
            {
                if (_methodDicitonary.ContainsKey(l))//some methods may have no leave event if there was unhandled exception in it
                {
                    yield return _methodDicitonary[l];    
                }                
            }            
        }

        public IEnumerator<Method> GetEnumerator()
        {
            return GetAllMethods().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}