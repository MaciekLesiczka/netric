using System;

namespace Netric.Configuration.Service
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public IObservable<ServiceState> GetState()
        {
            throw new NotImplementedException();
        }

        public IObservable<LogEntry> GetLogs()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }

    public interface IServiceConfiguration
    {
        IObservable<ServiceState> GetState();
        IObservable<LogEntry> GetLogs();

        void Start();
        void Stop();
    }

    public class LogEntry
    {
        public string Message { get; set; }
    }

    public enum ServiceState
    {
        Stopped,
        Started,
    }
}