namespace Netric.Agent.Clr
{
    public interface IMethodLeave
    {
        string Name { get; }
        long CallId { get; }
        long Ticks { get; }
        ThreadInfo Thread { get; }
    }
}