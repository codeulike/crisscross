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
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reporting.WebForms;
using CrissCrossLib;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for ReportDefinition
    /// </summary>
    [TestClass]
    public class CrcParameterDefinitionTests
    {
        public CrcParameterDefinitionTests()
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
        public void CanCloneCrcParameterDefinition()
        {
            var firstP = new CrcParameterDefinition();
            firstP.Name = "Foo";
            firstP.ParameterType = CrcParameterType.Select;
            firstP.AllowNull = true;
            firstP.RequiredFromUser = true;
            firstP.ValidValues.Add(new CrcValidValue() { Label = "Label1", Value = "Value1" });
            firstP.ValidValues.Add(new CrcValidValue() { Label = "Label2", Value = "Value2" });

            var cloneP = firstP.DeepClone();

            Assert.AreEqual("Foo", cloneP.Name);
            Assert.AreEqual(CrcParameterType.Select, cloneP.ParameterType);
            Assert.AreEqual(true, cloneP.AllowNull);
            Assert.AreEqual(true, cloneP.RequiredFromUser);
            Assert.AreEqual(2, cloneP.ValidValues.Count());
            Assert.IsNotNull(cloneP.ValidValues.FirstOrDefault(v => v.Value == "Value1"));
            Assert.AreEqual("Label1", cloneP.ValidValues.FirstOrDefault(v => v.Value == "Value1").Label);
            Assert.IsNotNull(cloneP.ValidValues.FirstOrDefault(v => v.Value == "Value2"));
            Assert.AreEqual("Label2", cloneP.ValidValues.FirstOrDefault(v => v.Value == "Value2").Label);

            // change original
            firstP.Name = "FooChanged";
            firstP.ValidValues[0].Label = "Label1Changed";

            // clone should not be affected
            Assert.AreEqual("Foo", cloneP.Name);
            Assert.IsNotNull(cloneP.ValidValues.FirstOrDefault(v => v.Value == "Value1"));
            Assert.AreEqual("Label1", cloneP.ValidValues.FirstOrDefault(v => v.Value == "Value1").Label);
            
        }

        
    }
}
