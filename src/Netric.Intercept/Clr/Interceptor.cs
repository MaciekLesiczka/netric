using System;

namespace Netric.Intercept.Clr
{
    public static class Interceptor
    {   
        [ThreadStatic]        
        private static ulong _cycle;

        public static ulong OnEnter(string method)
        {
            ulong cycle = _cycle++;
            ClrEventSource.Log.OnEnter(method, (long)cycle);            
            return cycle;
        }
        public static void OnExit(string method, ulong enterCycle)
        {
            ClrEventSource.Log.OnLeave(method, (long)enterCycle);
        }
    }
}
