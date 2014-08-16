using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Databox.Integration.Test
{
    [TestClass]
    public class InitalizationTests
    {
        [TestMethod]
        public void TestExceptionOnNoConfigurationFile()
        {
            try
            {
                new DataboxService();
                Assert.Fail("Exception expected.");
            }
            catch (Databox.Integration.Exceptions.InvalidAPIKeyException)
            {
                // this is expected
            }
        }

        [TestMethod]
        public void TestConstructorFromApiKey()
        {
            var randomKey = Guid.NewGuid().ToString();
            var randomUrl = Guid.NewGuid().ToString();

            var svc = new DataboxService(randomKey, randomUrl);

            Assert.IsNotNull(svc);
            Assert.AreEqual(randomKey, svc.APIKey);
            Assert.AreEqual(randomUrl, svc.UniqueUrl);
        }
    }
}
