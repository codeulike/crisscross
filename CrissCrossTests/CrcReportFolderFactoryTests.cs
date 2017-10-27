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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib.Hierarchical;
using CrissCrossLib.ReportWebService;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for HierarchicalTests
    /// </summary>
    [TestClass]
    public class CrcReportFolderFactoryTests
    {
        public CrcReportFolderFactoryTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CanReturnHierarchyForEmptyList()
        {

            var serviceMock = MakeMockSsrsService(); 
            CatalogItem[] empty = new CatalogItem[0];
            
            var returnResponse = new ListChildrenResponse(null, empty);
            serviceMock.Expect(s => s.ListChildren( Arg<ListChildrenRequest>.Matches(lcr => lcr.ItemPath == @"/"))).Return(returnResponse);

            var fac = new CrcReportFolderFactory();
            var ret = fac.Create(serviceMock);

            Assert.IsNotNull(ret);
            Assert.AreEqual(String.Empty, ret.FolderName);
            Assert.AreEqual(@"/", ret.Path);
            Assert.AreEqual(0, ret.SubFolders.Count());
            Assert.AreEqual(0, ret.Reports.Count());

        }

        [TestMethod]
        public void CanReturnHierarchyForReportList()
        {

            var serviceMock = MakeMockSsrsService();
            var item1 = new CatalogItem(){
                Path = @"/Test1",
                Name = @"Test1",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report
            };
            var item2 = new CatalogItem(){
                Path = @"/Test2",
                Name = @"Test2",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report
            };
            CatalogItem[] twoitems = new CatalogItem[] { item1, item2 };
            
            var returnResponse = new ListChildrenResponse(null, twoitems);
            serviceMock.Expect(s => s.ListChildren( Arg<ListChildrenRequest>.Matches(lcr => lcr.ItemPath == @"/")) ).Return(returnResponse);
            
            var fac = new CrcReportFolderFactory();
            var ret = fac.Create(serviceMock);

            Assert.IsNotNull(ret);
            Assert.AreEqual(String.Empty, ret.FolderName);
            Assert.AreEqual(@"/", ret.Path);
            Assert.AreEqual(0, ret.SubFolders.Count());
            Assert.AreEqual(2, ret.Reports.Count());
            Assert.IsTrue(ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test1") != null);
            Assert.AreEqual(@"/Test1", ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test1").ReportPath);
            Assert.IsTrue(ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test2") != null);
            Assert.AreEqual(@"/Test2", ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test2").ReportPath);

        }

        [TestMethod]
        public void CanReturnHierarchyWithDescriptions()
        {

            var serviceMock = MakeMockSsrsService();
            var item1 = new CatalogItem()
            {
                Path = @"/Test1",
                Name = @"Test1",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report,
                Description = "Test1 Description"
            };
            var item2 = new CatalogItem()
            {
                Path = @"/Test2",
                Name = @"Test2",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report,
                Description = "Test2 Description"
            };
            CatalogItem[] twoitems = new CatalogItem[] { item1, item2 };

            var returnResponse = new ListChildrenResponse(null, twoitems);
            serviceMock.Expect(s => s.ListChildren(Arg<ListChildrenRequest>.Matches(lcr => lcr.ItemPath == @"/"))).Return(returnResponse);

            var fac = new CrcReportFolderFactory();
            var ret = fac.Create(serviceMock);

            Assert.IsNotNull(ret);
            Assert.AreEqual(String.Empty, ret.FolderName);
            Assert.AreEqual(@"/", ret.Path);
            Assert.AreEqual(0, ret.SubFolders.Count());
            Assert.AreEqual(2, ret.Reports.Count());
            Assert.IsTrue(ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test1") != null);
            Assert.AreEqual("Test1 Description", ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test1").Description);
            Assert.IsTrue(ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test2") != null);
            Assert.AreEqual("Test2 Description", ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test2").Description);

        }

        [TestMethod]
        public void CanReturnHierarchyAndOmitHiddenReports()
        {

            var serviceMock = MakeMockSsrsService();
            var item1 = new CatalogItem()
            {
                Path = @"/Test1",
                Name = @"Test1",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report,
                Hidden = true
            };
            var item2 = new CatalogItem()
            {
                Path = @"/Test2",
                Name = @"Test2",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report,
                Hidden = false
            };
            CatalogItem[] twoitems = new CatalogItem[] { item1, item2 };

            var returnResponse = new ListChildrenResponse(null, twoitems);
            serviceMock.Expect(s => s.ListChildren(Arg<ListChildrenRequest>.Matches(lcr => lcr.ItemPath == @"/"))).Return(returnResponse);

            var fac = new CrcReportFolderFactory();
            var ret = fac.Create(serviceMock);

            Assert.IsNotNull(ret);
            Assert.AreEqual(String.Empty, ret.FolderName);
            Assert.AreEqual(@"/", ret.Path);
            Assert.AreEqual(0, ret.SubFolders.Count());
            Assert.AreEqual(1, ret.Reports.Count());
            Assert.IsTrue(ret.Reports.FirstOrDefault(r => r.DisplayName == @"Test2") != null);
            

        }


        [TestMethod]
        public void CanReturnHierarchyForFolderWithReports()
        {

            var serviceMock = MakeMockSsrsService();
            var folder1 = new CatalogItem()
            {
                Path = "/Foo",
                Name = "Foo",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Folder,
            };
            CatalogItem[] onefolder = new CatalogItem[] { folder1 };
            var item1 = new CatalogItem()
            {
                Path = @"/Foo/Test1",
                Name = @"Test1",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report,
            };
            var item2 = new CatalogItem()
            {
                Path = @"/Foo/Test2",
                Name = @"Test2",
                TypeName = CrcReportFolderFactory.ReportServiceItemTypes.Report,
            };
            CatalogItem[] twoitems = new CatalogItem[] { item1, item2 };
            
            var returnResponse1 = new ListChildrenResponse(null, onefolder);
            serviceMock.Expect(s => s.ListChildren(Arg<ListChildrenRequest>.Matches(lcr => lcr.ItemPath == @"/") )).Return(returnResponse1);
            var expectedRequest2 = new ListChildrenRequest(null, @"/Foo", false); // first param null?
            var returnResponse2 = new ListChildrenResponse(null, twoitems);
            serviceMock.Expect(s => s.ListChildren(Arg<ListChildrenRequest>.Matches(lcr => lcr.ItemPath == @"/Foo"))).Return(returnResponse2);
            
            
            var fac = new CrcReportFolderFactory();
            var ret = fac.Create(serviceMock);

            Assert.IsNotNull(ret);
            Assert.AreEqual(String.Empty, ret.FolderName);
            Assert.AreEqual(@"/", ret.Path);
            Assert.AreEqual(1, ret.SubFolders.Count());
            Assert.AreEqual(0, ret.Reports.Count());
            var subFolder = ret.SubFolders[0];
            Assert.AreEqual("/Foo", subFolder.Path);
            Assert.AreEqual("Foo", subFolder.FolderName);
            Assert.IsTrue(subFolder.Reports.FirstOrDefault(r => r.DisplayName == @"Test1") != null);
            Assert.AreEqual(@"/Foo/Test1", subFolder.Reports.FirstOrDefault(r => r.DisplayName == @"Test1").ReportPath);
            Assert.IsTrue(subFolder.Reports.FirstOrDefault(r => r.DisplayName == @"Test2") != null);
            Assert.AreEqual(@"/Foo/Test2", subFolder.Reports.FirstOrDefault(r => r.DisplayName == @"Test2").ReportPath);

        }


        private ReportingService2010Soap MakeMockSsrsService()
        {
            
            var serviceMock = MockRepository.GenerateStub<ReportingService2010Soap>();

            return serviceMock;
        }
    }
}
