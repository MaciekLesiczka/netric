using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Netric.Configuration.Profilee;

namespace Netric.Configuration.Console
{
    class Program
    {
        private const string StartMessage =
@"Netric Manager version {0}
@MaciekLesiczka
";
        private static SiteInstaller _profileeManager;
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;

            System.Console.WriteLine(StartMessage, version);
            using (_profileeManager = new SiteInstaller(System.Console.WriteLine))
            {
                Command command;
                if (!ParseArguments(args, out command))
                {
                    PrintUsage();
                }
                else
                {
                    command.Execute();
                }
            }

        }

        private static bool ParseArguments(string[] args, out Command command)
        {
            command = null;
            
            if(args.Length ==0)
            {
                return false;
            }
            if (!args[0].StartsWith("-"))
            {
                return false;
            }

            return CreateCommand(out command, args[0].Substring(1), args.Skip(1).ToArray());
        }

        private static bool CreateCommand(out Command command,string name, string[] args)
        {
            command = null;
            if (name == "i")
            {
                if (args.Length == 1)
                {
                    command = new InstallOnSiteCommand(args[0], _profileeManager);
                }
            }
            else if (name == "u")
            {
                if (args.Length == 1)
                {
                    command = new UninstallOnSiteCommand(args[0], _profileeManager);
                }
            }
            else if (name == "s")
            {
                command = new ShowConfigurationCommand(_profileeManager,new ClrConfigurator(System.Console.WriteLine));
            }
            else if (name == "asm")
            {
                if (args.Length == 1)
                {
                    command = new ConfigureAssemblyCommand(args[0],new ClrConfigurator(System.Console.WriteLine));
                }
            }
            else if (name == "?")
            {
                if (args.Length == 0)
                {
                    command = new HelpCommand();
                }
            }
            return command != null;
        }

        private static void PrintUsage()
        {
            const string usageText =
            @"Usage:
-i <sitename>       :: Installs monitoring on IIS Site.
-u <sitename>       :: Uninstalls monitoring on IIS Site.
-asm <expression>   :: Sets assemblies to be profiled. Use -? to learn more.
-s                  :: Shows current configuration.
-?                  :: Displays help.";

            System.Console.Write(usageText);
        }
    }

    internal class ConfigureAssemblyCommand : Command
    {
        private readonly string _asm;
        private readonly IClrConfigurator _clrConfigurator;

        public ConfigureAssemblyCommand(string asm, IClrConfigurator clrConfigurator)
        {
            _asm = asm;
            _clrConfigurator = clrConfigurator;
        }


        public override void Execute()
        {
            _clrConfigurator.SetProfiledAssemblies(_asm);
        }
    }
}
