
/****************************** ghost1372.github.io ******************************\
*	Module Name:	Members.cs
*	Project:		MasterCry
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2017, 9, 21, 02:47 ب.ظ
*	
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterCry
{
    class Members
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            return "ID: " + Id + "   Name: " + Name + "   Path: " + Path;
        }
    }
}
