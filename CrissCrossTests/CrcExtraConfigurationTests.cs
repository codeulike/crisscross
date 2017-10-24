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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib.Configuration;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for ConfigTests
    /// </summary>
    [TestClass]
    public class CrcExtraConfigurationTests
    {
        public CrcExtraConfigurationTests()
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
        public void CanSerializeAndDeserializeConfig()
        {
            var confTest = new CrcExtraConfiguration();
            confTest.Version = 2;
            var repTest = new CrcReportConfig();
            repTest.Path = "Test";
            repTest.IsFeatured = true;
            confTest.CrcReportConfigs = new List<CrcReportConfig>();
            confTest.CrcReportConfigs.Add(repTest);
            CrcExtraConfiguration.Serialize("TestConfig.xml", confTest);

            var getItBack = CrcExtraConfiguration.Deserialize("TestConfig.xml");
            Assert.AreEqual(2, getItBack.Version);
            Assert.AreEqual(1, getItBack.CrcReportConfigs.Count());
            Assert.AreEqual("Test", getItBack.CrcReportConfigs[0].Path);
            Assert.AreEqual(true, getItBack.CrcReportConfigs[0].IsFeatured);
        }

        [TestMethod]
        public void CanSerializeWithParams()
        {
            var confTest = new CrcExtraConfiguration();
            confTest.Version = 2;
            var repTest = new CrcReportConfig();
            repTest.Path = "Test";
            repTest.IsFeatured = true;
            var paramTest = new CrcReportConfig.CrcParamConfig();
            paramTest.ParamName = "TestParam";
            paramTest.ShowByDefault = true;
            repTest.CrcParamConfigs = new List<CrcReportConfig.CrcParamConfig>();
            repTest.CrcParamConfigs.Add(paramTest);
            confTest.CrcReportConfigs = new List<CrcReportConfig>();
            confTest.CrcReportConfigs.Add(repTest);

            CrcExtraConfiguration.Serialize("TestConfigWithParams.xml", confTest);

        }

        [TestMethod]
        public void CanSerializeAndDeserializeConfigWithEmptyEquivalentValues()
        {
            var confTest = new CrcExtraConfiguration();
            confTest.Version = 2;
            var repTest = new CrcReportConfig();
            repTest.Path = "Test";
            repTest.IsFeatured = true;
            confTest.CrcReportConfigs = new List<CrcReportConfig>();
            confTest.CrcReportConfigs.Add(repTest);

            List<string> empties = new List<string>() { "", "%%", null };
            confTest.DefaultEmptyEquivalentValues = empties;

            CrcExtraConfiguration.Serialize("TestConfig2.xml", confTest);

            var getItBack = CrcExtraConfiguration.Deserialize("TestConfig2.xml");
            Assert.AreEqual(2, getItBack.Version);
            Assert.AreEqual(1, getItBack.CrcReportConfigs.Count());
            Assert.AreEqual("Test", getItBack.CrcReportConfigs[0].Path);
            Assert.AreEqual(true, getItBack.CrcReportConfigs[0].IsFeatured);
            Assert.AreEqual(3, getItBack.DefaultEmptyEquivalentValues.Count());
        }

        [TestMethod]
        public void CanDetectDependantParams()
        {
            var repConfig = new CrcReportConfig();
            repConfig.Path = "TestReport";
            repConfig.CrcParamConfigs = new List<CrcReportConfig.CrcParamConfig>();
            repConfig.CrcParamConfigs.Add(new CrcReportConfig.CrcParamConfig()
            {
                ParamName = "ParamOne",
                DependantParams = new List<string>() { "ParamThree" }
            });

            Assert.IsTrue(repConfig.DependantParamsSpecified);

        }
    }
}
