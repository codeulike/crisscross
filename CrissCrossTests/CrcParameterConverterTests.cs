using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib;
using viewer = Microsoft.Reporting.WebForms;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for CrcParameterConverterTests
    /// </summary>
    [TestClass]
    public class CrcParameterConverterTests
    {
        public CrcParameterConverterTests()
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
        public void GetReportParametersForSsrsReportViewer_CanConvertSimple()
        {
            var repDefn = MakeTestReportDefn("2010-06-02", "S1");
            var converter = new CrcParameterConverter();

            var result = converter.GetReportParametersForSsrsReportViewer(repDefn);

            Assert.AreEqual(2, result.Count());
            viewer.ReportParameter param1 = result.FirstOrDefault(p => p.Name == "PretendDateParam");
            Assert.IsNotNull(param1);
            Assert.AreEqual(1, param1.Values.Count);
            Assert.AreEqual("2010-06-02", param1.Values[0]);
            viewer.ReportParameter param2 = result.FirstOrDefault(p => p.Name == "PretendSingleSelect");
            Assert.IsNotNull(param2);
            Assert.AreEqual(1, param2.Values.Count);
            Assert.AreEqual("S1", param2.Values[0]);

            
        }

        [TestMethod]
        public void GetReportParametersForSsrsReportViewer_CanReturnNullDate()
        {
            var repDefn = MakeTestReportDefn(null, "S1");
            var converter = new CrcParameterConverter();

            var result = converter.GetReportParametersForSsrsReportViewer(repDefn);

            Assert.AreEqual(2, result.Count());
            viewer.ReportParameter param1 = result.FirstOrDefault(p => p.Name == "PretendDateParam");
            Assert.IsNotNull(param1);
            Assert.AreEqual(1, param1.Values.Count);
            Assert.AreEqual(null, param1.Values[0]);
            viewer.ReportParameter param2 = result.FirstOrDefault(p => p.Name == "PretendSingleSelect");
            Assert.IsNotNull(param2);
            Assert.AreEqual(1, param2.Values.Count);
            Assert.AreEqual("S1", param2.Values[0]);


        }

        public CrcReportDefinition MakeTestReportDefn(string dateValue, string selectValue)
        {
            var repDefn = new CrcReportDefinition();
            repDefn.ReportPath = "/Pretend/Path";
            repDefn.DisplayName = "Pretend Report";

            var dateParam = new CrcParameterDefinition();
            dateParam.Name = "PretendDateParam";
            dateParam.DisplayName = "Pretend Date Param";
            dateParam.ParameterType = CrcParameterType.Date;

            dateParam.ParameterChoice = new CrcParameterChoice("PretendDateParam");
            dateParam.ParameterChoice.SingleValue = dateValue;
            
            repDefn.ParameterDefinitions.Add(dateParam);

            var singleChoiceParam = new CrcParameterDefinition();
            singleChoiceParam.Name = "PretendSingleSelect";
            singleChoiceParam.DisplayName = "Pretend Single Select";
            singleChoiceParam.ParameterType = CrcParameterType.Select;
            singleChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "S1", Label = "Single1" });
            singleChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "S2", Label = "Single2" });
            singleChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "S3", Label = "Single3" });
            singleChoiceParam.ParameterChoice = new CrcParameterChoice("PretendSingleSelect");
            singleChoiceParam.ParameterChoice.SingleValue = selectValue;
            repDefn.ParameterDefinitions.Add(singleChoiceParam);

            
            
            return repDefn;

        }
    }
}
