using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQPubSubNode;
using System;
using System.Threading;

namespace RabbitMQPubSubNodeTest
{
    [TestClass]
    public class RabbitMqPubSubNodeTests
    {
        [TestMethod]
        public void CreateNodesAndVerifyPublishing()
        {
            const string initialText = "Initial Text";
            const string nodeOneEventText = "Node One event text";

            String myTestString = initialText;

            var autoResetEvent = new AutoResetEvent(false);
            var rabbitMqPubSubNode1 =
                new RabbitMqPubSubNode("Test",
                    (x) =>
                    {
                        //Do something in the callback 
                        autoResetEvent.Set();
                        myTestString += x;
                    });

            // A moment for Rabbit to establish queue etc.
            Thread.Sleep(2000);
            rabbitMqPubSubNode1.Publish(nodeOneEventText);
            Assert.IsTrue(autoResetEvent.WaitOne());
            Assert.AreEqual(
                string.Format("{0}Message Received by Test: {1}", initialText, nodeOneEventText),
                myTestString);
        }

        [TestMethod]
        public void TestEventTypeResolverEmptyString()
        {
            DomainEventHandler domainEventHandler = new DomainEventHandler();
            const string testJson = @"";
            DomainEventBase parsedEvent = domainEventHandler.ResolveDomainEvent(testJson);
            Assert.AreEqual(DomainEventType.IndeterminateType, parsedEvent.EventType);
            Assert.AreEqual("Json Parsing Error", parsedEvent.SourceDomain);
            Assert.AreEqual("Json Parsing Error", parsedEvent.TargetDomain);
        }

        [TestMethod]
        public void TestEventTypeResolverProperJsonString()
        {
            DomainEventHandler domainEventHandler = new DomainEventHandler();
            const string testJson = @"{
                                        'EventType': 'InventoryDomainEventCheckoutItems',
                                        'SourceDomain': 'ClientApi',
                                        'TargetDomain': 'Inventory',
                                      }";
            DomainEventBase parsedEvent = domainEventHandler.ResolveDomainEvent(testJson);
            Assert.AreEqual(DomainEventType.InventoryDomainEventCheckoutItems, parsedEvent.EventType);
            Assert.AreEqual("ClientApi", parsedEvent.SourceDomain);
            Assert.AreEqual("Inventory", parsedEvent.TargetDomain);
        }
    }
}
