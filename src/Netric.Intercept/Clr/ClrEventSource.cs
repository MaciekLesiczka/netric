using Microsoft.Diagnostics.Tracing;

namespace Netric.Intercept.Clr
{
    [EventSource(Name = ProviderName)]
    public sealed class ClrEventSource : EventSource
    {
        public const string ProviderName = "Netric.Intercept.Clr";
        public void OnEnter(string Method, long CallId) { WriteEvent(1, Method, CallId); }
        public void OnLeave(string Method, long CallId) { WriteEvent(2, Method, CallId); }
        
        public static ClrEventSource Log = new ClrEventSource();
    }
}
