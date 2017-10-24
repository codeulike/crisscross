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
using rws = CrissCrossLib.ReportWebService;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for CrcParameterRefresherTests
    /// </summary>
    [TestClass]
    public class CrcParameterRefresherTests
    {
        public CrcParameterRefresherTests()
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
        public void CanUpdateValidValues()
        {
            var pd = new CrcParameterDefinition();
            pd.Name = "TestParam";
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label1", Value = "Value1" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label2", Value = "Value2" });

            var newVV = new List<rws.ValidValue>();
            newVV.Add(new rws.ValidValue(){ Label = "Label3", Value = "Value3"});

            var r = new CrcParameterRefresher();
            r.UpdateValidValues(pd, newVV.ToArray());

            Assert.AreEqual(1, pd.ValidValues.Count());
            var checkVV = pd.ValidValues[0];
            Assert.AreEqual("Label3", checkVV.Label);
            Assert.AreEqual("Value3", checkVV.Value);
        }

        [TestMethod]
        public void CanUpdateDefaultValues()
        {
            var pd = new CrcParameterDefinition();
            pd.Name = "TestParam";
            pd.ParameterChoice = new CrcParameterChoice(pd.Name);
            pd.ParameterChoice.Values.Add("Value1");
            pd.ParameterChoice.Values.Add("Value2");
            pd.ParameterChoice.Values.Add("Value3");

            var newDV = new List<string>();
            newDV.Add("Value4");
            newDV.Add("Value5");

            var r = new CrcParameterRefresher();
            r.UpdateDefaultValues(pd, newDV.ToArray());

            Assert.AreEqual(2, pd.ParameterChoice.Values.Count());
            var check4 = pd.ParameterChoice.Values.FirstOrDefault(d => d == "Value4");
            Assert.IsNotNull(check4);
            var check5 = pd.ParameterChoice.Values.FirstOrDefault(d => d == "Value5");
            Assert.IsNotNull(check5);



        }

        [TestMethod]
        public void RefreshParameter_CanHandleNulls()
        {
            var pd = new CrcParameterDefinition();
            pd.Name = "TestParam";
            pd.ParameterChoice = new CrcParameterChoice(pd.Name);
            pd.ParameterChoice.Values.Add("Value1");
            pd.ParameterChoice.Values.Add("Value2");
            pd.ParameterChoice.Values.Add("Value3");

            var latestParam = new rws.ReportParameter();
            latestParam.Name = "TestParam";
            latestParam.ValidValues = null;
            latestParam.DefaultValues = null;

            
            var r = new CrcParameterRefresher();
            r.RefreshParameter(pd, latestParam);

            Assert.IsNotNull(pd);
            Assert.AreEqual("TestParam", pd.Name);
            Assert.AreEqual(0, pd.ParameterChoice.Values.Count());
            

        }

        [TestMethod]
        public void IsChoiceValid_CanDetectValid()
        {
            var pd = new CrcParameterDefinition();
            pd.Name = "TestParam";
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label1", Value = "Value1" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label2", Value = "Value2" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label3", Value = "Value3" });

            pd.ParameterChoice = new CrcParameterChoice(pd.Name);
            pd.ParameterChoice.Values.Add("Value1");
            pd.ParameterChoice.Values.Add("Value2");

            var r = new CrcParameterRefresher();
            bool valid = r.IsChoiceValid(pd);

            Assert.IsTrue(valid);

        }

        [TestMethod]
        public void IsChoiceValid_CanDetectInValid()
        {
            var pd = new CrcParameterDefinition();
            pd.Name = "TestParam";
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label1", Value = "Value1" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label2", Value = "Value2" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label3", Value = "Value3" });

            pd.ParameterChoice = new CrcParameterChoice(pd.Name);
            pd.ParameterChoice.Values.Add("Value1");
            pd.ParameterChoice.Values.Add("Value5");

            var r = new CrcParameterRefresher();
            bool valid = r.IsChoiceValid(pd);

            Assert.IsFalse(valid);

        }

        [TestMethod]
        public void IsChoiceValid_CanHandleMissingChoice()
        {
            var pd = new CrcParameterDefinition();
            pd.Name = "TestParam";
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label1", Value = "Value1" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label2", Value = "Value2" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label3", Value = "Value3" });

            
            var r = new CrcParameterRefresher();
            bool valid = r.IsChoiceValid(pd);

            Assert.IsTrue(valid);

        }

        [TestMethod]
        public void IsChoiceValid_CanHandleEmptyChoice()
        {
            var pd = new CrcParameterDefinition();
            pd.Name = "TestParam";
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label1", Value = "Value1" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label2", Value = "Value2" });
            pd.ValidValues.Add(new CrcValidValue() { Label = "Label3", Value = "Value3" });

            pd.ParameterChoice = new CrcParameterChoice(pd.Name);
            
            var r = new CrcParameterRefresher();
            bool valid = r.IsChoiceValid(pd);

            Assert.IsTrue(valid);

        }
    }
}
