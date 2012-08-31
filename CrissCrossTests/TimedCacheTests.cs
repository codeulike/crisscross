using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrissCrossLib.Caching;

namespace CrissCrossTests
{
    /// <summary>
    /// Summary description for TimedCacheTests
    /// </summary>
    [TestClass]
    public class TimedCacheTests
    {
        public TimedCacheTests()
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
        public void Add_CanAdd()
        {
            var cache = new TimedCache<string>("test1", 10, 10);
            cache.Add("newkey", "something");

            var retreive = cache["newkey"];
            Assert.AreEqual("something", retreive);
        }

        [TestMethod]
        public void Item_CanReturnNullIfNotFound()
        {
            var cache = new TimedCache<string>("test2", 10, 10);
            cache.Add("newkey", "something");

            var retreive = cache["newkey"];
            var retreiveUnlikely = cache["unusedkey"];
            Assert.AreEqual("something", retreive);
            Assert.IsNull(retreiveUnlikely);
        }

        [TestMethod]
        public void ContainsKey_CanDetectValid()
        {
            var cache = new TimedCache<string>("test3", 10, 10);
            cache.Add("newkey", "something");

            Assert.IsTrue(cache.ContainsKey("newkey"));
        }

        [TestMethod]
        public void ContainsKey_CanDetectInValid()
        {
            var cache = new TimedCache<string>("test4", 10, 10);
            cache.Add("newkey", "something");

            Assert.IsFalse(cache.ContainsKey("wrongkey"));
        }

        // this test takes a couple of minutes so commented out TestMethod attribute
        // comment it back in if you're looking at cache behaviour
        //[TestMethod]
        public void ItemsCanExpire()
        {
            var cache = new TimedCache<string>("test4", 1, 1);
            cache.Add("newkey", "something");
            var retreiveEarly = cache["newkey"];
            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(1.5));
            var retreiveLate = cache["newkey"];

            Assert.AreEqual("something", retreiveEarly);
            Assert.IsNull(retreiveLate);

        }

    }
}
