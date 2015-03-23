using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQPubSubNode;

namespace RabbitMQPubSubNodeTest
{
    [TestClass]
    public class MongoIoTests
    {
        [TestMethod]
        public void TestWriteThenReadToDb()
        {
            MongoIo test = new MongoIo("myTestDbName");
            const string sampleString = "This is a string for testing";
            string identifier = MongoIo.GetNewId();
            test.InsertToDb(identifier, sampleString);
            string stringFromRead = test.ReadFromDb(identifier);
            Assert.AreEqual(sampleString, stringFromRead);
        }
    }

}
