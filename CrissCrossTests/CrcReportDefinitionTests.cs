using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for CrcReportDefinitionTests
    /// </summary>
    [TestClass]
    public class CrcReportDefinitionTests
    {
        public CrcReportDefinitionTests()
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
        public void HasDependantParameters_CanDetectTrue()
        {
            var crcRepDefn = new CrcReportDefinition();
            crcRepDefn.ParameterDefinitions.Add(new CrcParameterDefinition()
            {
                Name = "ParamOne",
                DependantParameterNames = new List<string>() { "ParamTwo" }
            });
            crcRepDefn.ParameterDefinitions.Add(new CrcParameterDefinition()
            {
                Name = "ParamTwo"
            });

            Assert.IsTrue(crcRepDefn.HasDependantParameters);

        }

        [TestMethod]
        public void HasDependantParameters_CanDetectFalse()
        {
            var crcRepDefn = new CrcReportDefinition();
            crcRepDefn.ParameterDefinitions.Add(new CrcParameterDefinition()
            {
                Name = "ParamOne"
            });
            crcRepDefn.ParameterDefinitions.Add(new CrcParameterDefinition()
            {
                Name = "ParamTwo"
            });

            Assert.IsFalse(crcRepDefn.HasDependantParameters);

        }

        [TestMethod]
        public void GetAvailableParameterNames_CanOmitHidden()
        {
            var crcRepDefn = new CrcReportDefinition();
            crcRepDefn.ParameterDefinitions.Add(new CrcParameterDefinition()
            {
                Name = "ParamOne",
                DisplayName = "ParamOneDesc",
                Hidden = false
            });
            crcRepDefn.ParameterDefinitions.Add(new CrcParameterDefinition()
            {
                Name = "ParamTwo",
                DisplayName = "ParamTwoDesc",
                Hidden = true
            });

            var paramDictionary = crcRepDefn.GetAvailableParameterNames();

            Assert.AreEqual(1, paramDictionary.Count());
            Assert.IsTrue(paramDictionary.Keys.Contains("ParamOne"));
            Assert.AreEqual("ParamOneDesc", paramDictionary["ParamOne"]);


        }
    }
}
