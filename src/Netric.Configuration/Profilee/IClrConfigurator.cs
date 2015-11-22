using System;
using System.Linq;
using Microsoft.Win32;

namespace Netric.Configuration.Profilee
{
    public interface IClrConfigurator
    {
        string GetProfiledAssemblies();
        void SetProfiledAssemblies(string asm);
    }

    public class ClrConfigurator : IClrConfigurator
    {
        private const string ValueName = "Environment";
        private readonly Action<string> _logger;

        public ClrConfigurator(Action<string> logger=null)
        {
            _logger = logger;
        }

        public string GetProfiledAssemblies()
        {
            string result = null;

            using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\WAS", true))
            {

                if (key != null)
                {
                    var value = key.GetValue(ValueName) as string[];
                    if (value != null)
                    {
                        var valuesList = value.ToList();

                        var asmSetting = valuesList.FirstOrDefault(vl => vl.StartsWith("PROFILED_ASSEMBLY="));

                        if (asmSetting != null)
                        {
                            result = asmSetting.Substring("PROFILED_ASSEMBLY=".Length);
                        }
                        else
                        {
                            LogTrace("PROFILED_ASSEMBLY does not exist! CLR NullReferenceException may occur!");
                        }                        
                    }
                    else
                    {
                        LogTrace(string.Format("Value {0} does not exist", ValueName));
                    }
                }
                else
                {
                    LogTrace(string.Format("Key HKLM\\{0} was not found", @"SYSTEM\CurrentControlSet\Services\WAS"));
                }
            }

            return result;
        }

        public void SetProfiledAssemblies(string asm)
        {
            if (SetProfiledAssemblies(asm, @"SYSTEM\CurrentControlSet\Services\W3SVC") &&

                SetProfiledAssemblies(asm, @"SYSTEM\CurrentControlSet\Services\WAS"))
            {
                Log("Setting was changed. To apply changes, please reset IIS");
            }
        }

        private bool SetProfiledAssemblies(string asm, string serviceKey)
        {
            using (var key = Registry.LocalMachine.OpenSubKey(serviceKey, true))
            {
                if (key != null)
                {
                    var value = key.GetValue(ValueName) as string[];
                    if (value != null)
                    {
                        var valuesList = value.ToList();

                        var asmSetting = valuesList.FirstOrDefault(vl => vl.StartsWith("PROFILED_ASSEMBLY="));

                        var newValue = string.Format("PROFILED_ASSEMBLY={0}", asm);

                        if (asmSetting != null)
                        {
                            valuesList[valuesList.IndexOf(asmSetting)] = newValue;
                        }
                        else
                        {
                            valuesList.Add(newValue);
                        }
                        key.SetValue(ValueName, valuesList.ToArray());
                        return true;
                    }
                    else
                    {
                        LogTrace(string.Format("Value {0} does not exist in key {1}", ValueName,serviceKey));
                    }
                }
                else
                {
                    LogTrace(string.Format("Key HKLM\\{0} was not found", serviceKey));
                }
            }
            return false;
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
    }
}