using Netric.Configuration.Profilee;

namespace Netric.Configuration.Console
{
    public class UninstallOnSiteCommand : Command
    {
        private readonly string _siteName;
        private readonly ISiteInstaller _profileeManager;

        public UninstallOnSiteCommand(string siteName, ISiteInstaller profileeManager)
        {
            _siteName = siteName;
            _profileeManager = profileeManager;
        }

        public override void Execute()
        {
            _profileeManager.Uninstall(_siteName);
        }
    }
}