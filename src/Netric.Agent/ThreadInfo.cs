using Microsoft.Diagnostics.Tracing;

namespace Netric.Agent
{
    public struct ThreadInfo
    {
        public int ProcessId { get; set; }
        public int ThreadId { get; set; }

        public static ThreadInfo Of(TraceEvent traceEvent)
        {
            return new ThreadInfo { ProcessId = traceEvent.ProcessID, ThreadId = traceEvent.ThreadID };
        }

        public override string ToString()
        {
            return string.Format("Process={0}, Thread={1}", ProcessId, ThreadId);
        }
    }
}