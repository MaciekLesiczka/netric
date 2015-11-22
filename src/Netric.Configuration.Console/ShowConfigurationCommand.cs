using Netric.Configuration.Profilee;

namespace Netric.Configuration.Console
{
    public class ShowConfigurationCommand : Command
    {
        private readonly ISiteInstaller _profileeManager;
        private readonly ClrConfigurator _clrConfigurator;

        public ShowConfigurationCommand(ISiteInstaller profileeManager, ClrConfigurator clrConfigurator)
        {
            _profileeManager = profileeManager;
            _clrConfigurator = clrConfigurator;
        }

        public override void Execute()
        {
            var installedOnSites = _profileeManager.GetSitesWithInstalledApm();
            if (installedOnSites.Length > 0)
            {
                System.Console.WriteLine("Sites with installed monitoring:");
                foreach (var s in installedOnSites)
                {
                    System.Console.WriteLine(s);
                }    
            }
            else
            {
                System.Console.WriteLine("Monitoring is not installed on any site");
            }

            var asm = _clrConfigurator.GetProfiledAssemblies();
            if (string.IsNullOrEmpty(asm))
            {
                System.Console.WriteLine("WARNING:Assemblies to profile are not specified.");
            }
            else
            {
                System.Console.WriteLine("Assemblies to profile");
                System.Console.WriteLine("{0}", asm);
            }
        }
    }
}