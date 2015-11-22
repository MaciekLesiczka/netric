using System.Globalization;
using System.Linq;
using Akka.Actor;
using Flamegraph;
using Netric.Shared;

namespace Netric.EventConsumer.Flamegraphs
{
    public class RequestConsumer : ReceiveActor
    {
        private readonly MarkupWriter _markupWriter;

        public RequestConsumer()
        {
            _markupWriter = new MarkupWriter();
            Receive<Request>(r => HandleRequest(r));
        }

        private void HandleRequest(Request request)
        {
            var stackFolder = new ClrCallStackFolder();
            var foldedStack = request.CallStack.Select(stackFolder.Fold).ToList();

            var flamegraph = new Markup(foldedStack, new Flamegraph.Settings
            {
                CountName = "ms",
                CountFormat = v => (new decimal(v)/1000).ToString(CultureInfo.InvariantCulture),
                Title = request.Url,
            });

            using (var writer = _markupWriter.GetWriter(request))
            {
                flamegraph.Write(writer);    
            }            
        }
    }
}
