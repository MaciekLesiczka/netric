using Netric.Configuration.Profilee;

namespace Netric.Configuration.Console
{
    public class InstallOnSiteCommand : Command
    {
        private readonly string _siteName;
        private readonly ISiteInstaller _profileeManager;

        public InstallOnSiteCommand(string siteName, ISiteInstaller profileeManager)
        {
            _siteName = siteName;
            _profileeManager = profileeManager;
        }

        public override void Execute()
        {
            _profileeManager.Install(_siteName);
        }
    }
}