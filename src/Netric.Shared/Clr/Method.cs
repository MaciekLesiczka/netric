namespace Netric.Shared.Clr
{
    public class Method
    {
        private readonly string _name;
        private readonly MethodStats _methodStats;

        public Method(string name, MethodStats methodStats)
        {
            _name = name;
            _methodStats = methodStats;
        }

        public string Name
        {
            get { return _name; }
        }

        public MethodStats MethodStats
        {
            get { return _methodStats; }
        }
    }
}