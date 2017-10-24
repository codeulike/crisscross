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
using CrissCrossLib;


namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for ParameterParsing
    /// </summary>
    [TestClass]
    public class CrcParameterChoiceMapperTests
    {
        public CrcParameterChoiceMapperTests()
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
        public void CanMapDateParam()
        {
            var repDefn = this.MakeTestReportDefn();
            var choiceColl = new CrcParameterChoiceCollection();
            var dateChoice = new CrcParameterChoice();
            dateChoice.Name = "PretendDateParam";
            dateChoice.SingleValue = "2010-06-02";
            choiceColl.ParameterChoiceList.Add(dateChoice);

            var mapper = new CrcParameterChoiceMapper();
            var mapResult = mapper.MapParameterChoices(repDefn, choiceColl);

            Assert.IsTrue(mapResult.MappingValid);
            Assert.AreEqual(0, mapResult.Complaints.Count());

            var dateParamDefn = repDefn.ParameterDefinitions.FirstOrDefault(p=> p.Name == "PretendDateParam");
            Assert.IsNotNull(dateParamDefn);
            Assert.IsNotNull(dateParamDefn.ParameterChoice);
            Assert.AreEqual("2010-06-02", dateParamDefn.ParameterChoice.SingleValue);
    

        }


        [TestMethod]
        public void CanMapDateParamFromLog()
        {
            var repDefn = this.MakeTestReportDefn();
            var choiceColl = new CrcParameterChoiceCollection();
            var dateChoice = new CrcParameterChoice();
            dateChoice.Name = "PretendDateParam";
            // dates from the log can have odd formats
            dateChoice.SingleValue = "5/2/2011 12:00";
            choiceColl.ParameterChoiceList.Add(dateChoice);

            var mapper = new CrcParameterChoiceMapper();
            var mapResult = mapper.MapParameterChoices(repDefn, choiceColl);

            Assert.IsTrue(mapResult.MappingValid);
            Assert.AreEqual(0, mapResult.Complaints.Count());

            var dateParamDefn = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "PretendDateParam");
            Assert.IsNotNull(dateParamDefn);
            Assert.IsNotNull(dateParamDefn.ParameterChoice);
            Assert.AreEqual("2011-05-02", dateParamDefn.ParameterChoice.SingleValue);


        }


        [TestMethod]
        public void CanMapSingleSelectParam()
        {
            var repDefn = this.MakeTestReportDefn();
            var choiceColl = new CrcParameterChoiceCollection();
            var singleSelect = new CrcParameterChoice();
            singleSelect.Name = "PretendSingleSelect";
            singleSelect.SingleValue = "S2";
            choiceColl.ParameterChoiceList.Add(singleSelect);

            var mapper = new CrcParameterChoiceMapper();
            var mapResult = mapper.MapParameterChoices(repDefn, choiceColl);

            Assert.IsTrue(mapResult.MappingValid);
            Assert.AreEqual(0, mapResult.Complaints.Count());

            var singleParamDefn = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "PretendSingleSelect");
            Assert.IsNotNull(singleParamDefn);
            Assert.IsNotNull(singleParamDefn.ParameterChoice);
            Assert.AreEqual("S2", singleParamDefn.ParameterChoice.SingleValue);
    
        }


        [TestMethod]
        public void CanMapMultiSelectParam()
        {
            var repDefn = this.MakeTestReportDefn();
            var choiceColl = new CrcParameterChoiceCollection();
            var multiSelect = new CrcParameterChoice();
            multiSelect.Name = "PretendMultiSelect";
            multiSelect.Values = new List<string>() { "M2", "M3" };

            choiceColl.ParameterChoiceList.Add(multiSelect);

            var mapper = new CrcParameterChoiceMapper();
            var mapResult = mapper.MapParameterChoices(repDefn, choiceColl);

            Assert.IsTrue(mapResult.MappingValid);
            Assert.AreEqual(0, mapResult.Complaints.Count());

            var multiParamDefn = repDefn.ParameterDefinitions.FirstOrDefault(p => p.Name == "PretendMultiSelect");
            Assert.IsNotNull(multiParamDefn);
            Assert.IsNotNull(multiParamDefn.ParameterChoice);
            Assert.AreEqual(2, multiParamDefn.ParameterChoice.Values.Count());
            Assert.IsNotNull(multiParamDefn.ParameterChoice.Values.FirstOrDefault(v => v == "M2"));
            Assert.IsNotNull(multiParamDefn.ParameterChoice.Values.FirstOrDefault(v => v == "M3"));

        }

        [TestMethod]
        public void CanMapDetectWrongName()
        {
            var repDefn = this.MakeTestReportDefn();
            var choiceColl = new CrcParameterChoiceCollection();
            var multiSelect = new CrcParameterChoice();
            multiSelect.Name = "WrongName";
            multiSelect.Values = new List<string>() { "M2", "M3" };

            choiceColl.ParameterChoiceList.Add(multiSelect);

            var mapper = new CrcParameterChoiceMapper();
            var mapResult = mapper.MapParameterChoices(repDefn, choiceColl);

            Assert.IsFalse(mapResult.MappingValid);
            Assert.IsTrue(mapResult.Complaints.Contains("Param WrongName not found"));

            
        }

        [TestMethod]
        public void CanParseEnUsDateAndGenericDate()
        {
            var mapper = new CrcParameterChoiceMapper();

            var pres1 = mapper.ParseDateStringVariousWays("8/4/2010 12:00:00 AM", "EN-US");
            // internal format used by CrissCross irrespective of culture
            var pres2 = mapper.ParseDateStringVariousWays("2012-03-09", "EN-US");
            Assert.AreEqual( true, pres1.CouldParse);
            Assert.AreEqual( new DateTime(2010, 8, 4), pres1.DateTime);
            Assert.AreEqual(true, pres2.CouldParse);
            Assert.AreEqual( new DateTime(2012, 3, 9), pres2.DateTime);

        }

        [TestMethod]
        public void CanParseEnGbDateAndGenericDate()
        {
            var mapper = new CrcParameterChoiceMapper();

            var pres1 = mapper.ParseDateStringVariousWays("04/08/2010 00:00:00", "EN-GB");
            // internal format used by CrissCross irrespective of culture
            var pres2 = mapper.ParseDateStringVariousWays("2012-03-09", "EN-GB");
            Assert.AreEqual(true, pres1.CouldParse);
            Assert.AreEqual(new DateTime(2010, 8, 4), pres1.DateTime);
            Assert.AreEqual(true, pres2.CouldParse);
            Assert.AreEqual(new DateTime(2012, 3, 9), pres2.DateTime);

        }

        [TestMethod]
        public void CanParseEnUsDateEnGbDateAndGenericDate()
        {
            var mapper = new CrcParameterChoiceMapper();
            var pres1 = mapper.ParseDateStringVariousWays("8/4/2010 12:00:00 AM", "Flexi-Gb-Us");
            var pres2 = mapper.ParseDateStringVariousWays("04/08/2010 00:00:00", "Flexi-Gb-Us");
            // internal format used by CrissCross irrespective of culture
            var pres3 = mapper.ParseDateStringVariousWays("2012-03-09", "Flexi-Gb-Us");
            Assert.AreEqual(true, pres1.CouldParse);
            Assert.AreEqual(new DateTime(2010, 8, 4), pres1.DateTime, "EN-US failed");
            Assert.AreEqual(true, pres2.CouldParse);
            Assert.AreEqual(new DateTime(2010, 8, 4), pres2.DateTime, "EN-GB failed");
            
            Assert.AreEqual(true, pres3.CouldParse);
            Assert.AreEqual(new DateTime(2012, 3, 9), pres3.DateTime);

        }



        public CrcReportDefinition MakeTestReportDefn()
        {
            var repDefn = new CrcReportDefinition();
            repDefn.ReportPath = "/Pretend/Path";
            repDefn.DisplayName = "Pretend Report";
            var dateParam = new CrcParameterDefinition();
            dateParam.Name = "PretendDateParam";
            dateParam.DisplayName = "Pretend Date Param";
            dateParam.ParameterType = CrcParameterType.Date;
            repDefn.ParameterDefinitions.Add(dateParam);
            var singleChoiceParam = new CrcParameterDefinition();
            singleChoiceParam.Name = "PretendSingleSelect";
            singleChoiceParam.DisplayName = "Pretend Single Select";
            singleChoiceParam.ParameterType = CrcParameterType.Select;
            singleChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "S1", Label = "Single1" });
            singleChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "S2", Label = "Single2" });
            singleChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "S3", Label = "Single3" });
            repDefn.ParameterDefinitions.Add(singleChoiceParam);
            var multiChoiceParam = new CrcParameterDefinition();
            multiChoiceParam.Name = "PretendMultiSelect";
            multiChoiceParam.DisplayName = "Pretend Multi Select";
            multiChoiceParam.ParameterType = CrcParameterType.MultiSelect;
            multiChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "M1", Label = "Multi1" });
            multiChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "M2", Label = "Multi2" });
            multiChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "M3", Label = "Multi3" });
            multiChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "M4", Label = "Multi4" });
            multiChoiceParam.ValidValues.Add(new CrcValidValue() { Value = "M5", Label = "Multi5" });
            repDefn.ParameterDefinitions.Add(multiChoiceParam);

            return repDefn;

        }
    }
}
