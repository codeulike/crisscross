// CrissCross - alternative user interface for running SSRS reports
// Copyright (C) 2011-2017 Ian Finch
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;


namespace CrissCross.Code
{
    public class CrissCrossWebHelper
    {
        public static string SerializeObjectIntoJson<T>(T obj)
        {
            DataContractJsonSerializer serializer = new
                         DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Flush();

                byte[] bytes = ms.GetBuffer();
                string jsonString = Encoding.UTF8.GetString(bytes, 0, bytes.Length).Trim('\0');
                jsonString = "(" + jsonString + ")";
                return jsonString;
            }
        }

    }
}
