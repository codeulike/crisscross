using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for CrcParameterChoiceFactoryTests
    /// </summary>
    [TestClass]
    public class CrcParameterChoiceFactoryTests
    {
        public CrcParameterChoiceFactoryTests()
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
        public void CanConvertSimpleParamString()
        {
            string testParam = "District=44";
            var fac = new CrcParameterChoiceFactory();
            var pcCol = fac.Create(testParam);

            Assert.IsNotNull(pcCol);
            Assert.AreEqual(1, pcCol.ParameterChoiceList.Count());
            Assert.AreEqual("District", pcCol.ParameterChoiceList[0].Name);
            Assert.AreEqual("44", pcCol.ParameterChoiceList[0].SingleValue);
        }

        [TestMethod]
        public void CanConvertParamStringWithSeveralClauses()
        {
            string testParam = "District=44&Customer=55445&Product=8889";
            var fac = new CrcParameterChoiceFactory();
            var pcCol = fac.Create(testParam);

            Assert.IsNotNull(pcCol);
            Assert.AreEqual(3, pcCol.ParameterChoiceList.Count());
            var p1 = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == "District");
            Assert.IsNotNull(p1);
            Assert.AreEqual("44", p1.SingleValue);
            var p2 = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == "Customer");
            Assert.IsNotNull(p2);
            Assert.AreEqual("55445", p2.SingleValue);
            var p3 = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == "Product");
            Assert.AreEqual("8889", p3.SingleValue);
        }

        [TestMethod]
        public void CanConvertParamStringWithRepeatingName()
        {
            string testParam = "District=44&Customer=55445&District=77";
            var fac = new CrcParameterChoiceFactory();
            var pcCol = fac.Create(testParam);

            Assert.IsNotNull(pcCol);
            Assert.AreEqual(2, pcCol.ParameterChoiceList.Count());
            var p1 = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == "District");
            Assert.IsNotNull(p1);
            Assert.AreEqual(2, p1.Values.Count());
            Assert.IsNotNull(p1.Values.FirstOrDefault(v => v == "44"));
            Assert.IsNotNull(p1.Values.FirstOrDefault(v => v == "77"));
            var p2 = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == "Customer");
            Assert.IsNotNull(p2);
            Assert.AreEqual("55445", p2.SingleValue);

        }

        [TestMethod]
        public void CanConvertIsNullParamString()
        {
            string testParam = "SomeDate:isnull=true";
            var fac = new CrcParameterChoiceFactory();
            var pcCol = fac.Create(testParam);

            Assert.IsNotNull(pcCol);
            Assert.AreEqual(1, pcCol.ParameterChoiceList.Count());
            Assert.AreEqual("SomeDate", pcCol.ParameterChoiceList[0].Name);
            Assert.IsNull(pcCol.ParameterChoiceList[0].SingleValue);
        }


        [TestMethod]
        public void CanConvertEmptyParamString()
        {
            string testParam = "";
            var fac = new CrcParameterChoiceFactory();
            var pcCol = fac.Create(testParam);

            Assert.IsNotNull(pcCol);
            Assert.AreEqual(0, pcCol.ParameterChoiceList.Count());

        }

        [TestMethod]
        public void CanConvertParamStringWithUrlEncodedContent()
        {
            // this is the kind of encoding that comes back from the execution log
            string testParam = "District=one%3Dtwo&Customer=Red %26 Black";
            
            var fac = new CrcParameterChoiceFactory();
            var pcCol = fac.Create(testParam);

            Assert.IsNotNull(pcCol);
            Assert.AreEqual(2, pcCol.ParameterChoiceList.Count());
            var p1 = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == "District");
            Assert.IsNotNull(p1);
            Assert.AreEqual(1, p1.Values.Count());
            Assert.AreEqual("one=two", p1.SingleValue);
            var p2 = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == "Customer");
            Assert.IsNotNull(p2);
            Assert.AreEqual(1,p2.Values.Count());
            Assert.AreEqual("Red & Black", p2.SingleValue);

        }

    }
}
