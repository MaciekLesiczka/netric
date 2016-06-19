namespace Netric.Agent.Clr
{
    public interface IMethodEvent
    {
        string Name { get; }
        long CallId { get; }
        long Ticks { get; }
        ThreadInfo Thread { get; }
    }
}