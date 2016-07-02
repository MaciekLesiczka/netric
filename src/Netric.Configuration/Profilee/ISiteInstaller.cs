using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Web.Administration;

namespace Netric.Configuration.Profilee
{
    public interface ISiteInstaller
    {
        List<string> GetAllSites();
        List<string> GetSitesWithInstalledApm();
        /// <summary>
        /// Adds handler and module web.config site. If one exists, its attributes are updated, so this method can be used for repair as well
        /// </summary>
        /// <param name="siteName"></param>
        void Install(string siteName);

        void Uninstall(string siteName);
    }

    public class SiteInstaller : ISiteInstaller, IDisposable
    {
        private readonly Action<string> _logger;
        private readonly ServerManager _manager;

        private const string ElementName = "Netric";

        /// <param name="logger">If it is null exceptions will be thrown outside class methods</param>
        public SiteInstaller(Action<string> logger=null)
        {
            _logger = logger;
            _manager = new ServerManager();
        }

        public List<string> GetAllSites()
        {
            return _manager.Sites.Select(x => x.Name).ToList();
        }

        public List<string> GetSitesWithInstalledApm()
        {
            var result = new List<string>();
            foreach (var site in _manager.Sites)
            {
                if (ModuleExist(site.Name))
                {
                    result.Add(site.Name);
                }
            }

            return result.ToList();
        }

        private bool ModuleExist(string siteName)
        {
            try
            {
                var modulesCollection = GetCollection("system.webServer/modules", siteName);
    
                Func<ConfigurationElementCollection, bool> isInstalled = coll => coll.Any(configurationElement =>
                    configurationElement.Attributes["name"].Value.Equals(ElementName));
                
                return isInstalled(modulesCollection);
            }
            //todo handle FileNotFoundException
            catch (Exception ex)
            {
                //todo refactor logging. This information should be logged only with -v(erbose) option
                if (_logger != null)
                {
                    LogTrace( string.Format("Cannot read configuration of site {0}. {1}", siteName, ex.Message));
                    return false;
                }
                throw;
            }
        }

        public void Install(string siteName)
        {
            try
            {   
                var site =
                    _manager.Sites.FirstOrDefault(
                        s => String.Equals(s.Name, siteName, StringComparison.InvariantCultureIgnoreCase));
                if (site == null)
                {
                    Log(string.Format("Site {0} does not exists", siteName));
                }
                else
                {
                    AddModule(siteName);
                    _manager.CommitChanges();
                }
                Log("Profiling was added. To apply changes, please recycle application pool");
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    Log(ex.ToString());
                }
                else
                {
                    throw;    
                }                
            }       
        }

        public void Uninstall(string siteName)
        {
            try
            {
                RemoveFrom("system.webServer/modules", siteName, "Module");
                _manager.CommitChanges();
                Log("Profiling was removed. To apply changes, please recycle application pool");
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    Log(ex.ToString());
                }
                else
                {
                    throw;
                }
            }
        }

        private void RemoveFrom(string collection, string siteName, string type)
        {
            ConfigurationElementCollection modulesCollection = GetCollection(collection, siteName);

            ConfigurationElement addElement = FindElement(modulesCollection, "add", "name", ElementName);
            if (addElement == null)
            {
                LogTrace(string.Format("{1} was not found in site {0}", siteName, type));
            }
            else
            {
                modulesCollection.Remove(addElement);
            }
        }

        private void LogTrace(string message)
        {
            Log(message);
        }

        private void Log(string msg)
        {
            if (_logger != null)
            {
                _logger(msg);
            }
        }

        private void AddModule(string siteName)
        {
            var collection = GetCollection("system.webServer/modules", siteName);

            var element =
                collection.FirstOrDefault(
                    configurationElement => configurationElement.Attributes["name"].Value.Equals(ElementName));

            var isNew = element == null;
            if (isNew)
            {
                element = collection.CreateElement("add");
                element["name"] = ElementName;
            }
            
            element["type"] = string.Format(@"Netric.Intercept.Web.HttpModule, {0}", GetAssemblyNameString());
            if (isNew)
            {
                collection.AddAt(0, element);
            }
        }

        private ConfigurationElementCollection GetCollection(string sectionPath, string siteName)
        {
            return _manager.GetApplicationHostConfiguration()
                .GetSection(sectionPath, siteName)
                .GetCollection();
        }

        private string GetAssemblyNameString()
        {
            return string.Format(
                "Netric.Intercept, Version={0}, Culture=neutral, PublicKeyToken=4D5ABCE15D499C91",
                Assembly.GetExecutingAssembly().GetName().Version);
        }

        public void Dispose()
        {
            _manager.Dispose();
        }

        private static ConfigurationElement FindElement(IEnumerable<ConfigurationElement> collection, string elementTagName, params string[] keyValues)
        {
            foreach (ConfigurationElement element in collection)
            {
                if (String.Equals(element.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase))
                {
                    bool matches = true;

                    for (int i = 0; i < keyValues.Length; i += 2)
                    {
                        object o = element.GetAttributeValue(keyValues[i]);
                        string value = null;
                        if (o != null)
                        {
                            value = o.ToString();
                        }

                        if (!String.Equals(value, keyValues[i + 1], StringComparison.OrdinalIgnoreCase))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return element;
                    }
                }
            }
            return null;
        }
    }
}
