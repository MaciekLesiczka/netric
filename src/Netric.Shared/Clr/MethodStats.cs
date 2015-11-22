namespace Netric.Shared.Clr
{
    public class MethodStats
    {
        public int StackLevel { get; set; }
        public long ElapsedInclusive { get; set; }
        public long? ElapsedExclusive { get; set; }
        public bool HandledException { get; set; }
    }
}
