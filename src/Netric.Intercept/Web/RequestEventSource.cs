using Microsoft.Diagnostics.Tracing;

namespace Netric.Intercept.Web
{
    [EventSource(Name = ProviderName)]
    public sealed class RequestEventSource : EventSource
    {
        public const string ProviderName = "Netric.Intercept.Web.Request";
        public void OnBegin(string Url,string Site,string Id) { WriteEvent(1, Url,Site,Id); }
        public void OnEnd(string Url, string Site,string Id) { WriteEvent(2, Url,Site,Id); }

        public static RequestEventSource Log = new RequestEventSource();
    }
}