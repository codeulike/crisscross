// CrissCross - alternative user interface for running SSRS reports
// Copyright (C) 2011 Ian Richardson
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
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using log4net;

namespace CrissCrossLib.Configuration
{
    [Serializable]
    public class CrcExtraConfiguration
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CrcExtraConfiguration));
        
        private static int ms_cacheMinutes = 45;

        private static CrcExtraConfiguration ms_cachedConfig = null;

        private DateTime m_loadedOn;

        public virtual int Version { get ; set; }
        public virtual List<CrcReportConfig> CrcReportConfigs { get; set; }

        [XmlArrayItem("DefaultEmptyEquivalentValue")]
        public virtual List<string> DefaultEmptyEquivalentValues { get; set; }


        public virtual bool IgnoreSsrsParameterDependencies { get; set; }

        // singleton instance, gets refreshed after set time period
        public static CrcExtraConfiguration Instance
        {
            get
            {
                if (ms_cachedConfig == null)
                    ms_cachedConfig = GetDefault();
                else
                {
                    // reload if too old
                    if (DateTime.Now.Subtract(ms_cachedConfig.GetLoadedTime()).TotalMinutes > ms_cacheMinutes)
                    {
                        logger.DebugFormat("Extraconfig was loaded on {0} so reloading", ms_cachedConfig.GetLoadedTime().ToString("HH:mm:ss"));
                        ms_cachedConfig = GetDefault();
                    }
                }
                return ms_cachedConfig;
            }
        }

        private static CrcExtraConfiguration GetDefault()
        {
            string location = ConfigurationManager.AppSettings["crisscriss.ExtraConfigPath"];
            if (location.StartsWith("~"))
            {
                string aspnetbase = System.Web.HttpContext.Current.ApplicationInstance.Server.MapPath("~");
                location = Path.Combine(aspnetbase, location.Substring(2));
            }
            logger.DebugFormat("Loading extraconfig from {0}", location);
            return Deserialize(location);
        }

        public CrcExtraConfiguration()
        {
            CrcReportConfigs = new List<CrcReportConfig>();
            DefaultEmptyEquivalentValues = new List<string>();
            m_loadedOn = DateTime.Now;
        }

        public static void Serialize(string file, CrcExtraConfiguration c)
        {
            XmlSerializer xs = new XmlSerializer(typeof(CrcExtraConfiguration));
            using (StreamWriter writer = File.CreateText(file))
            {
                xs.Serialize(writer, c);
            }
        }

        public static CrcExtraConfiguration Deserialize(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(CrcExtraConfiguration));
            CrcExtraConfiguration ret = null;
            using (StreamReader reader = File.OpenText(file))
            {
                ret = (CrcExtraConfiguration)xs.Deserialize(reader);
            }
            
            return ret;
        }

        public virtual CrcReportConfig GetReportConfig(string path)
        {
            return CrcReportConfigs.FirstOrDefault(r => r.Path == path);
        }

        public virtual List<string> GetFeaturedReportPaths()
        {
            return CrcReportConfigs.Where(r => r.IsFeatured).Select(r => r.Path).ToList<string>();
        }

        /// <summary>
        /// returns the time that the config was loaded from xml
        /// used to check for expiry
        /// </summary>
        public virtual DateTime GetLoadedTime()
        {
            return m_loadedOn;
        }
    }
}
