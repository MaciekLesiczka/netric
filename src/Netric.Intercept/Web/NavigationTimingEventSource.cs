using Microsoft.Diagnostics.Tracing;

namespace Netric.Intercept.Web
{
    [EventSource(Name = ProviderName)]
    public sealed class NavigationTimingEventSource : EventSource
    {
        public const string ProviderName = "Netric.Intercept.Web.NavigationTiming";

        public void Statistics(string Url, long Network, long Server, long Browser, long Time, string Site,string Id)
        {
            WriteEvent(1, Url, Network, Server, Browser, Time, Site,Id);
        }

        public static NavigationTimingEventSource Log = new NavigationTimingEventSource();
    }
}