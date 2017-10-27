using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib;
using CrissCrossLib.ReportWebService;
using CrissCrossLib.Caching;
using CrissCrossLib.Configuration;
using Rhino.Mocks;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for CrissCrossServicesTests
    /// </summary>
    [TestClass]
    public class CrissCrossServicesTests
    {
        public CrissCrossServicesTests()
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
        public void CanGetReportDefn()
        {

            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var serviceMock = mf.MakeMockReportingService2010Soap(
                                    mf.MakeSimpleTestParameters());
            mf.SetListChildrenExpectation(serviceMock, @"/", new CatalogItem[] { new CatalogItem() {
                Path = "TestReport",
                Name = "TestReportName",
                Description = "TestReportDescription",
                TypeName = CrissCrossLib.Hierarchical.CrcReportFolderFactory.ReportServiceItemTypes.Report
                }});
            var soapClientFactory = mf.MakeMockSoapClientFactory(serviceMock);
                                

            var cacheMock = MockRepository.GenerateMock<CrcCacheManager>();
            // return empty cache when asked
            cacheMock.Expect(m => m.AllReportsCacheByUsername).Return(new TimedCache<CatalogItem[]>("test1",10,10));
            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var ccs = new CrissCrossServices(soapClientFactory, cacheMock, configMock, null);

            var repDefn = ccs.GetReportDefn("TestReport", "TestUser");

            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual("TestReportName", repDefn.DisplayName);
            Assert.AreEqual("TestReportDescription", repDefn.Description);
            Assert.AreEqual(2, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(2, p1check.ValidValues.Count());
            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(2, p2check.ValidValues.Count());


        }

        [TestMethod]
        public void CanRefreshDependantParameters_Simple()
        {
            // make report defn
            var repDefn = new CrcReportDefinition();
            repDefn.ReportPath = "Test/Report";
            var pd1 = new CrcParameterDefinition();
            pd1.Name = "ParamOne";
            pd1.ValidValues.Add(new CrcValidValue() { Label = "Label1", Value = "Value1" });
            pd1.ValidValues.Add(new CrcValidValue() { Label = "Label2", Value = "Value2" });
            pd1.ValidValues.Add(new CrcValidValue() { Label = "Label3", Value = "Value3" });
            pd1.DependantParameterNames.Add("ParamTwo");
            var pd2 = new CrcParameterDefinition();
            pd2.Name = "ParamTwo";
            pd2.ValidValues.Add(new CrcValidValue() { Label = "SubLabel1_1", Value = "SubValue1_1" });
            pd2.ValidValues.Add(new CrcValidValue() { Label = "SubLabel1_2", Value = "SubValue1_2" });
            pd2.ValidValues.Add(new CrcValidValue() { Label = "SubLabel2_1", Value = "SubValue2_1" });
            pd2.ValidValues.Add(new CrcValidValue() { Label = "SubLabel2_2", Value = "SubValue2_2" });
            pd2.ValidValues.Add(new CrcValidValue() { Label = "SubLabel3_1", Value = "SubValue3_1" });
            repDefn.ParameterDefinitions.Add(pd1);
            repDefn.ParameterDefinitions.Add(pd2);

            
            // make choice collection
            var choiceCollection = new CrcParameterChoiceCollection();
            var paramChoice = new CrcParameterChoice("ParamOne");
            paramChoice.Values.Add("Value2");
            choiceCollection.ParameterChoiceList.Add( paramChoice);

            // make server side params to return
            ItemParameter p1 = new ItemParameter();
            p1.Name = "ParamOne";
            p1.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label1", Value = "Value1"},
                                    new ValidValue(){Label = "Label2", Value = "Value2"},
                                    new ValidValue(){Label = "Label3", Value = "Value3"}};
            p1.DefaultValues = new string[] { };
            ItemParameter p2 = new ItemParameter();
            p2.Name = "ParamTwo";
            p2.ValidValues = new ValidValue[]{ new ValidValue(){Label = "SubLabel2_1", Value = "SubValue2_1"},
                                    new ValidValue(){Label = "SubLabel2_2", Value = "SubValue2_2"}};
            p2.DefaultValues = new string[] { };
            ItemParameter[] paramArrayToReturn = new ItemParameter[] { p1, p2 };


            // make mocks for ccs
            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var soapClientFactory = mf.MakeMockSoapClientFactory(
                        mf.MakeMockReportingService2010Soap("Value2", paramArrayToReturn));
                                
            var cacheMock = MockRepository.GenerateMock<CrcCacheManager>();
            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make ccs
            var ccs = new CrissCrossServices(soapClientFactory, cacheMock, configMock, null);


            ccs.RefreshDependantParameters(repDefn, choiceCollection);

            Assert.AreEqual(2,repDefn.ParameterDefinitions.Count());
            var pd1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(pd1check);
            Assert.AreEqual("Value2", pd1check.ParameterChoice.SingleValue);
            var pd2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(pd2check);
            Assert.AreEqual(2, pd2check.ValidValues.Count());
            Assert.IsNotNull(pd2check.ValidValues.FirstOrDefault( vv => vv.Value == "SubValue2_1"));
            Assert.IsNotNull(pd2check.ValidValues.FirstOrDefault( vv => vv.Value == "SubValue2_2"));
            

        }
    }
}
