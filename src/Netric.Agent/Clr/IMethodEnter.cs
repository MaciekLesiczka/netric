namespace Netric.Agent.Clr
{
    public interface IMethodEnter
    {
        string Name { get; }
        long CallId { get; }
        long Ticks { get; }
        ThreadInfo Thread { get; }
    }
}