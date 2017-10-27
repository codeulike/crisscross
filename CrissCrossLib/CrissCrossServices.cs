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
using System.Text;
using CrissCrossLib.ReportWebService;
using CrissCrossLib.Caching;
using CrissCrossLib.Configuration;
using CrissCrossLib.History;
using System.Configuration;
using log4net;

namespace CrissCrossLib
{
    public class CrissCrossServices
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CrissCrossServices));
        
        private CrcExtraConfiguration m_extraConfiguration;
        private CrcSsrsSoapClientFactory m_ssrsSoapClientFactory;
        private CrcCacheManager m_cacheManager;
        private CrcReportHistoryFetcherFactory m_reportHistoryFetcherFactory;

        // convenience constructor that calls the dependency injection one
        public CrissCrossServices():this(new CrcSsrsSoapClientFactory(), CrcCacheManager.Instance,
            CrcExtraConfiguration.Instance, new CrcReportHistoryFetcherFactory())
        {
        }

        // constructor to allow dependency injection in unit tests with mocks, etc
        public CrissCrossServices(CrcSsrsSoapClientFactory ssrsSoapClientFactory, CrcCacheManager cacheManager,
            CrcExtraConfiguration extraConfig, CrcReportHistoryFetcherFactory fetcherFactory)
        {
            m_ssrsSoapClientFactory = ssrsSoapClientFactory;
            m_cacheManager = cacheManager;
            m_extraConfiguration = extraConfig;
            m_reportHistoryFetcherFactory = fetcherFactory;
        }

        public CatalogItem[] GetAllReports(string username)
        {
            var isitcached = m_cacheManager.AllReportsCacheByUsername[username];
            if (isitcached != null)
                return isitcached;
            
            var catItems = GetAllReportsNoCache(username);
            m_cacheManager.AllReportsCacheByUsername.Add(username, catItems);
            return catItems;
        }

        
        public CatalogItem[] GetAllReportsNoCache(string username)
        {
            ReportingService2010Soap rService = m_ssrsSoapClientFactory.MakeSsrsSoapClient();

            var lcRequest = new ListChildrenRequest()
            {
                //TrustedUserHeader = new TrustedUserHeader(),
                ItemPath = "/",
                Recursive = true
            };
            //CatalogItem[] catalogItems = null;
            var lcResponse = rService.ListChildren(lcRequest);

            var reportsOnly = lcResponse.CatalogItems.Where(c => c.TypeName != null && c.TypeName.Equals(Hierarchical.CrcReportFolderFactory.ReportServiceItemTypes.Report)).ToArray();
            
            return reportsOnly;
        }

        public CrissCrossLib.Hierarchical.CrcReportFolder GetAllReportsHierarchical(string username)
        {
            var isitcached = m_cacheManager.AllReportsHierarchicalCacheByUsername[username];
            if (isitcached != null)
                return isitcached;
            
            var hierItems = GetAllReportsHierarchicalNoCache(username);
            m_cacheManager.AllReportsHierarchicalCacheByUsername.Add(username, hierItems);
            return hierItems;
        }

        public CrissCrossLib.Hierarchical.CrcReportFolder GetAllReportsHierarchicalNoCache(string username)
        {
            ReportingService2010Soap rService = m_ssrsSoapClientFactory.MakeSsrsSoapClient();
            var fac = new CrissCrossLib.Hierarchical.CrcReportFolderFactory();
            return fac.Create(rService);
        }


        

        public List<string> GetPopularReportPathsForUser(string username)
        {
            var hist = m_reportHistoryFetcherFactory.Create();
            var reps = hist.GetUsersFavourites(username);
            return reps;
        }

        public CatalogItem[] GetFeaturedReports(string username)
        {
            var allReps = this.GetAllReports(username);
            var config = m_extraConfiguration;
            List<string> featuredPaths = null;
            if (config == null)
                featuredPaths = new List<string>();
            else
                featuredPaths = config.GetFeaturedReportPaths();

            var filteredOrdered = new List<CatalogItem>();
            foreach (string path in featuredPaths)
            {
                var match = allReps.FirstOrDefault(c => c.Path == path);
                if (match == null)
                {
                    logger.DebugFormat("GetFeaturedReports: featured report {1} not in the catalog for user {0} - maybe deleted or renamed?", username, path);
                    continue;
                }
                filteredOrdered.Add(match);
            }
            return filteredOrdered.ToArray();
        }

        public CatalogItem[] GetPopularReportsForUser(string username)
        {
            var isitcached = m_cacheManager.PopularReportsCacheByUsername[username];
            if (isitcached != null)
                return isitcached;
            
            var catItems = GetPopularReportsForUserNoCache(username);
            m_cacheManager.PopularReportsCacheByUsername.Add(username, catItems);
            return catItems;
        }

        public List<string> GetPopularFoldersForUser(string username)
        {
            var hist = m_reportHistoryFetcherFactory.Create();
            var popFolders = hist.GetUsersFavouriteFolders(username);
            return popFolders;

        }

        public CatalogItem[] GetGloballyPopularReports(string username)
        {
            var isitcached = m_cacheManager.GlobalPopularReportsCacheByUsername[username];
            if (isitcached != null)
                return isitcached;
            
            var catItems = GetGloballyPopularReportsNoCache(username);
            m_cacheManager.GlobalPopularReportsCacheByUsername.Add(username, catItems);
            return catItems;
        }

        private CatalogItem[] GetGloballyPopularReportsNoCache(string username)
        {
            var allReps = this.GetAllReports(username);
            var hist = m_reportHistoryFetcherFactory.Create();
            var popPaths = hist.GetGlobalFavourites();

            var filteredOrdered = new List<CatalogItem>();
            foreach (string path in popPaths)
            {
                var match = allReps.FirstOrDefault(c => c.Path == path);
                if (match == null)
                {
                    logger.DebugFormat("GetGloballyPopularReports: User {0} had popular report {1} but its not in the catalog - maybe deleted or renamed?", username, path);
                    continue;
                }
                filteredOrdered.Add(match);
            }
            return filteredOrdered.ToArray();
        }

        public List<CrcReportHistory> GetUsersRecentRuns(string username, int max)
        {
            var hist = m_reportHistoryFetcherFactory.Create();
            var rec = hist.GetUsersRecentRuns(username, max);
            return rec;
        }

        private CatalogItem[] GetPopularReportsForUserNoCache(string username)
        {
            var allReps = this.GetAllReports(username);
            var popPaths = this.GetPopularReportPathsForUser(username);
            var filteredOrdered = new List<CatalogItem>();
            foreach (string path in popPaths)
            {
                var match = allReps.FirstOrDefault(c => c.Path == path);
                if (match == null)
                {
                    logger.DebugFormat("GetPopularReports: User {0} had popular report {1} but its not in the catalog - maybe deleted or renamed?", username, path);
                    continue;
                }
                filteredOrdered.Add(match);
            }
            return filteredOrdered.ToArray();
        }

        public CatalogItem GetReportCatalogItem(string reportPath, string username)
        {
            var catList = this.GetAllReports(username);
            var catMatch = catList.FirstOrDefault(c => c.Path == reportPath);
            if (catMatch == null)
            {
                throw new ApplicationException(string.Format("GetReportCatalogItem {0} {1} couldn't find report in catalog",
                    reportPath, username));
            }
            return catMatch;
        }

        public CrcReportDefinition GetReportDefn(string reportPath, string username)
        {
            // first, use the username to get description
            var reportCat = this.GetReportCatalogItem(reportPath, username);

            ReportingService2010Soap rService = m_ssrsSoapClientFactory.MakeSsrsSoapClient();

            GetItemParametersRequest req = new GetItemParametersRequest()
            {
                ItemPath = reportPath,
                ForRendering = true
            };
            var resp = rService.GetItemParameters(req);

            var factory = new CrcReportDefinitionFactory();
            return factory.Create(reportPath, reportCat, resp.Parameters, m_extraConfiguration);
        }

        /// <summary>
        /// Applies the specified choices to the already-existing report definition
        /// and updates the dependant parameters
        /// </summary>
        public void RefreshDependantParameters(CrcReportDefinition repDefn, CrcParameterChoiceCollection newChoices)
        {
            
            var mapResult = repDefn.MapParameterChoices(newChoices);
            if (!mapResult.MappingValid)
            {
                // todo - friendlier message back to ui
                throw new ApplicationException(string.Format("invalid params - could not map supplied values to definitions for report {0}. complaints: {1}",
                    repDefn.DisplayName, string.Join(", ", mapResult.Complaints.ToArray())));
            }
            var conv = new CrcParameterConverter();
            List<ParameterValue> valueList = conv.GetParametersValuesForSsrsWebService(repDefn);
            // get new params from web service
            ReportingService2010Soap rService = m_ssrsSoapClientFactory.MakeSsrsSoapClient();
            logger.DebugFormat("RefreshDependantParameters: rep {0} calling WS to get new validvalid. Passing {1} values", repDefn.DisplayName, valueList.Count());
            
            var grpRequest = new GetItemParametersRequest(new TrustedUserHeader(), repDefn.ReportPath, null, true, valueList.ToArray(), null);
            var grpResponse = rService.GetItemParameters(grpRequest);

            // work out which params to refresh
            List<string> paramsToRefresh = new List<string>();
            foreach (string updatedParam in newChoices.ParameterChoiceList.Select(p => p.Name))
            {
                var paramDefn = repDefn.ParameterDefinitions.First(p => p.Name == updatedParam);
                paramsToRefresh = paramsToRefresh.Union(paramDefn.DependantParameterNames).ToList();
            }
            logger.DebugFormat("RefreshDependantParameters: rep {0} based on choices, have {1} parameters that need refreshing", repDefn.DisplayName, paramsToRefresh.Count());

            var refresher = new CrcParameterRefresher();
            foreach (string paramLoop in paramsToRefresh)
            {
                var paramDefn = repDefn.ParameterDefinitions.First(p => p.Name == paramLoop);
                var latestParamDetails = grpResponse.Parameters.FirstOrDefault(p => p.Name == paramLoop);
                if (latestParamDetails == null)
                    throw new ApplicationException(String.Format("Was expecting web service to return new details for parameter {0} but none found",
                        paramLoop));
                refresher.RefreshParameter(paramDefn, latestParamDetails);
            }

        }

        

    }
}
