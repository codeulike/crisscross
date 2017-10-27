using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib;
using CrissCrossLib.Configuration;
using rws = CrissCrossLib.ReportWebService;
using Rhino.Mocks;


namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for CrcReportDefinitionFactoryTests
    /// </summary>
    [TestClass]
    public class CrcReportDefinitionFactoryTests
    {
        public CrcReportDefinitionFactoryTests()
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
        public void CanMakeSimpleReport()
        {
         

            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeSimpleTestParameters();

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);

            
            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(2, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(2, p1check.ValidValues.Count());
            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(2, p2check.ValidValues.Count());


        
        }

        [TestMethod]
        public void CanMakeSimpleReportWithCatalogItem()
        {


            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeSimpleTestParameters();

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            rws.CatalogItem catItem = new rws.CatalogItem()
            {
                Path = "TestReport",
                Name = "TestReportName",
                Description = "TestReportDescription"
            };

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", catItem, paramArray, configMock);


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
        public void CanMakeSimpleReportWithNullDefaults()
        {


            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeSimpleTestParameters();
            paramArray[0].DefaultValues = null;

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(2, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(2, p1check.ValidValues.Count());
            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(2, p2check.ValidValues.Count());



        }

        [TestMethod]
        public void CanMakeReportWithNoReportConfigButIncludeDefaultEmptyEquivalents()
        {

            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamOne";
            p1.ValidValues = new rws.ValidValue[]{ new rws.ValidValue(){Label = "--All--", Value = "%%"},
                                    new rws.ValidValue(){Label = "Label1", Value = "Value1"},
                                    new rws.ValidValue(){Label = "Label2", Value = "Value2"}};
            p1.DefaultValues = new string[] { };
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();
            configMock.Expect(m => m.DefaultEmptyEquivalentValues).Return(new List<string>() { "", "%%" });
            configMock.Expect(m => m.GetReportConfig(null)).IgnoreArguments().Return(null);

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(1, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(3, p1check.ValidValues.Count());
            Assert.IsTrue(p1check.EmptyEquivalentValues.Count() > 0);


        }

        [TestMethod]
        public void CanMakeReportAndInterpretSSRSDependencies()
        {


            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeDependantTestParameters();

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(3, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(2, p1check.DependantParameterNames.Count());
            Assert.IsTrue(p1check.DependantParameterNames.Contains("ParamTwo"));
            Assert.IsTrue(p1check.DependantParameterNames.Contains("ParamThree"));

            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(0, p2check.DependantParameterNames.Count());

            var p3check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamThree");
            Assert.IsNotNull(p3check);
            Assert.AreEqual(0, p3check.DependantParameterNames.Count());


        }

        [TestMethod]
        public void CanMakeReportWithExtraConfigDependants()
        {


            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeDependantTestParameters();

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();
            var repConfig = new CrcReportConfig();
            repConfig.Path = "TestReport";
            repConfig.CrcParamConfigs = new List<CrcReportConfig.CrcParamConfig>();
            repConfig.CrcParamConfigs.Add(new CrcReportConfig.CrcParamConfig()
            {
                ParamName = "ParamOne",
                DependantParams = new List<string>() { "ParamThree" }
            });
            configMock.Expect(m => m.GetReportConfig(null)).IgnoreArguments().Return(repConfig);
            configMock.Expect(m => m.DefaultEmptyEquivalentValues).Return(new List<string>());

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(3, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(1, p1check.DependantParameterNames.Count());
            Assert.IsTrue(p1check.DependantParameterNames.Contains("ParamThree"));

            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(0, p2check.DependantParameterNames.Count());

            var p3check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamThree");
            Assert.IsNotNull(p3check);
            Assert.AreEqual(0, p3check.DependantParameterNames.Count());


        }

        [TestMethod]
        public void CanMakeReportAndIgnoreSSRSDependencies()
        {


            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeDependantTestParameters();

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();
            configMock.Expect(m => m.IgnoreSsrsParameterDependencies).Return(true);

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(3, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(0, p1check.DependantParameterNames.Count());
            
            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(0, p2check.DependantParameterNames.Count());

            var p3check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamThree");
            Assert.IsNotNull(p3check);
            Assert.AreEqual(0, p3check.DependantParameterNames.Count());


        }

        [TestMethod]
        public void CanMakeReportWithExtraConfigSpecificEmptyEquivalents()
        {


            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeSimpleTestParameters();

            var extraConfig = new CrcExtraConfiguration();
            var repConfig = new CrcReportConfig();
            repConfig.Path = "TestReport";
            repConfig.CrcParamConfigs = new List<CrcReportConfig.CrcParamConfig>();
            repConfig.CrcParamConfigs.Add(new CrcReportConfig.CrcParamConfig()
            {
                ParamName = "ParamOne",
                EmptyEquivalentValues = new List<string>() { "Value1" }
            });
            extraConfig.CrcReportConfigs.Add(repConfig);
            
            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, extraConfig);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(2, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(1, p1check.EmptyEquivalentValues.Count());
            Assert.IsTrue(p1check.EmptyEquivalentValues.Contains("Value1"));

           


        }

        [TestMethod]
        public void CanMakeReportAndApplyRequiredFromUserToMultipicks()
        {


            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamWithDefault";
            p1.ValidValues = new rws.ValidValue[]{ new rws.ValidValue(){Label = "--All--", Value = "%%"},
                                    new rws.ValidValue(){Label = "Label1", Value = "Value1"},
                                    new rws.ValidValue(){Label = "Label2", Value = "Value2"}};
            p1.DefaultValues = new string[] {"%%"};
            p1.Nullable = false;
            rws.ItemParameter p2 = new rws.ItemParameter();
            p2.Name = "ParamWithoutDefault";
            p2.ValidValues = new rws.ValidValue[]{ 
                                    new rws.ValidValue(){Label = "Label3", Value = "Value3"},
                                    new rws.ValidValue(){Label = "Label4", Value = "Value4"}};
            p2.DefaultValues = new string[] { };
            p2.Nullable = false;
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1, p2 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(2, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamWithDefault");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(false, p1check.RequiredFromUser, "RequiredFromUser expected false");
            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamWithoutDefault");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(true, p2check.RequiredFromUser, "RequiredFromUser expected true");



        }

        [TestMethod]
        public void CanMakeReportAndApplyRequiredFromUserToDateParams()
        {


            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamWithDefault";
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.DateTime;
            p1.DefaultValues = new string[] { "01/Jan/2011" };
            p1.Nullable = false;
            rws.ItemParameter p2 = new rws.ItemParameter();
            p2.Name = "ParamWithoutDefault";
            p2.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.DateTime;
            p2.DefaultValues = new string[] { };
            p2.Nullable = false;
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1, p2 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(2, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamWithDefault");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(false, p1check.RequiredFromUser, "RequiredFromUser expected false");
            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamWithoutDefault");
            Assert.IsNotNull(p2check);
            Assert.AreEqual(true, p2check.RequiredFromUser, "RequiredFromUser expected true");



        }

        [TestMethod]
        public void CanMakeReportAndDetectFreeTextField()
        {
            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamFreeText";
            p1.Nullable = false;
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.String;
            p1.ValidValuesQueryBased = false;
            p1.ValidValues = null;
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(1, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamFreeText");
            Assert.IsNotNull(p1check);
            Assert.AreEqual( CrcParameterType.Text, p1check.ParameterType);
        }

        [TestMethod]
        public void CanMakeReportAndDetectNormalSelect()
        {
            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamSelect";
            p1.Nullable = false;
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.String;
            p1.ValidValuesQueryBased = false;
            p1.ValidValues = new rws.ValidValue[]{ new rws.ValidValue(){Label = "--All--", Value = "%%"},
                                    new rws.ValidValue(){Label = "Label1", Value = "Value1"},
                                    new rws.ValidValue(){Label = "Label2", Value = "Value2"}};
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(1, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamSelect");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(CrcParameterType.Select, p1check.ParameterType);
        }

        [TestMethod]
        public void CanMakeReportAndDetectMultipickSelect()
        {
            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamSelect";
            p1.Nullable = false;
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.String;
            p1.ValidValuesQueryBased = false;
            p1.ValidValues = new rws.ValidValue[]{ new rws.ValidValue(){Label = "--All--", Value = "%%"},
                                    new rws.ValidValue(){Label = "Label1", Value = "Value1"},
                                    new rws.ValidValue(){Label = "Label2", Value = "Value2"}};
            p1.MultiValue = true;
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(1, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamSelect");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(CrcParameterType.MultiSelect, p1check.ParameterType);
        }

        [TestMethod]
        public void CanMakeReportAndDetectEmptySelect()
        {
            // sometimes dependant selects can be empty at first
            // they are always query based though
            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamSelect";
            p1.Nullable = false;
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.String;
            p1.ValidValuesQueryBased = true;
            p1.ValidValues = null;
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(1, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamSelect");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(CrcParameterType.Select, p1check.ParameterType);
        }

        [TestMethod]
        public void CanMakeReportWithHintFromExtraConfig()
        {


            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeSimpleTestParameters();

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();
            var repConfig = new CrcReportConfig();
            repConfig.Path = "TestReport";
            repConfig.ReportHint = "This is a report hint";
            configMock.Expect(m => m.GetReportConfig(null)).IgnoreArguments().Return(repConfig);
            
            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual("This is a report hint", repDefn.ReportHint);
            


        }

        [TestMethod]
        public void CanMakeReportAndDetectNormalBoolField()
        {
            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamBool";
            p1.Nullable = false;
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.Boolean;
            p1.ValidValuesQueryBased = false;
            p1.ValidValues = null;
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(1, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamBool");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(CrcParameterType.Boolean, p1check.ParameterType);
        }

        [TestMethod]
        public void CanMakeReportAndDetectBoolFieldWithYesNoValues()
        {
            rws.ItemParameter p1 = new rws.ItemParameter();
            p1.Name = "ParamBool";
            p1.Nullable = false;
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.Boolean;
            p1.ValidValuesQueryBased = false;
            p1.ValidValues = new rws.ValidValue[] {
                new rws.ValidValue() { Label = "Yes", Value="true" },
                new rws.ValidValue() { Label = "No", Value="false" }};
            rws.ItemParameter[] paramArray = new rws.ItemParameter[] { p1 };

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(1, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamBool");
            Assert.IsNotNull(p1check);
            Assert.AreEqual(CrcParameterType.Select, p1check.ParameterType);
            Assert.AreEqual(2, p1check.ValidValues.Count());
            var v1 = p1check.ValidValues.FirstOrDefault(v => v.Value == "true");
            Assert.IsNotNull(v1);
            Assert.AreEqual("Yes" ,v1.Label);
            var v2 = p1check.ValidValues.FirstOrDefault(v => v.Value == "false");
            Assert.IsNotNull(v2);
            Assert.AreEqual("No", v2.Label);
            
        }

        [TestMethod]
        public void CanMakeSimpleReportWithDefaultDate()
        {
            // test to catch default date formatting bug

            var mf = new TestDoubles.MockSsrsWebServiceFactory();
            var paramArray = mf.MakeDateTestParameters();
            // set a default date in the format that comes out of ssrs
            paramArray[0].DefaultValues = new string[] { "2/1/2010 12:00:00 AM"};

            var configMock = MockRepository.GenerateMock<CrcExtraConfiguration>();

            // make main service obj
            var factory = new CrcReportDefinitionFactory();
            var repDefn = factory.Create("TestReport", paramArray, configMock);


            Assert.IsNotNull(repDefn);
            Assert.AreEqual("TestReport", repDefn.ReportPath);
            Assert.AreEqual(2, repDefn.ParameterDefinitions.Count());
            var p1check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamOne");
            Assert.IsNotNull(p1check);
            Assert.AreEqual("2010-02-01", p1check.ParameterChoice.SingleValue);
            var p2check = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "ParamTwo");
            Assert.IsNotNull(p2check);
            Assert.IsNull(p2check.ParameterChoice.SingleValue);



        }

    }
}
