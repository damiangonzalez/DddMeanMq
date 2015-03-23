using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQPubSubNode;

namespace DomainTests
{
    [TestClass]
    public class DomainTests
    {
        [TestMethod]
        public void AddItemToCart_AndAssertOnSubscriberHandling()
        {
            const string purchasingDomainEvent1 = "AddItemToCart";

            string myPurchasingTestString = string.Empty;
            string myInventoryTestString =string.Empty;

            var autoResetEventPurchasing = new AutoResetEvent(false); // used to assert async callback 
            var autoResetEventInventory = new AutoResetEvent(false); // used to assert async callback 

            // kick off the two pub sub domains as background workers
            // one for inventory and one for purchasing
            var purchasingPubSubNode =
                new RabbitMqPubSubNode("Purchasing",
                                        messageReceivedByPurchasingNode =>
                                        {
                                            autoResetEventPurchasing.Set();
                                            myPurchasingTestString += messageReceivedByPurchasingNode + Environment.NewLine;
                                        });

            var inventoryPubSubNode =
                new RabbitMqPubSubNode("Inventory",
                                        messageReceivedByInventoryNode =>
                                        {
                                            autoResetEventInventory.Set();
                                            myInventoryTestString += messageReceivedByInventoryNode + Environment.NewLine;
                                        });

            // Publish First Domain Event from purchasing
            // Publish first Domain event
            purchasingPubSubNode.Publish(purchasingDomainEvent1);

            Assert.IsTrue(autoResetEventPurchasing.WaitOne()); // Leverage AutoResetEvent to assert async callback
            // Should assert for message being received twice, once by inventory and once by purchasing itself
            Assert.AreEqual(
                string.Format(
                    "Message received by Purchasing: {0}" + Environment.NewLine,
                    purchasingDomainEvent1),
                myPurchasingTestString);

            Assert.IsTrue(autoResetEventInventory.WaitOne()); // Leverage AutoResetEvent to assert async callback
            Assert.AreEqual(
                string.Format(
                    "Message received by Inventory: {0}" + Environment.NewLine,
                    purchasingDomainEvent1),
                myInventoryTestString);
        }
        
        [TestMethod]
        public void AddItemToCartThenCompleteOrder_AndAssertOnSubscriberHandling()
        {
            const string purchasingDomainEvent1 = "AddItemToCart";
            const string purchasingDomainEvent2 = "CompleteOrder";
            const string inventoryDomainEvent2 = "ItemShipped";

            string myPurchasingTestString = string.Empty;
            string myInventoryTestString = string.Empty;

            var autoResetEventPurchasing = new AutoResetEvent(false); // used to assert async callback 
            var autoResetEventInventory = new AutoResetEvent(false); // used to assert async callback 

            // kick off the two pub sub domains as background workers
            // one for inventory and one for purchasing
            var purchasingPubSubNode =
                new RabbitMqPubSubNode("Purchasing",
                                        messageReceivedByPurchasingNode =>
                                        {
                                            autoResetEventPurchasing.Set();
                                            myPurchasingTestString += messageReceivedByPurchasingNode + Environment.NewLine;
                                        });

            var inventoryPubSubNode =
                new RabbitMqPubSubNode("Inventory",
                                        messageReceivedByInventoryNode =>
                                        {
                                            autoResetEventInventory.Set();
                                            myInventoryTestString += messageReceivedByInventoryNode + Environment.NewLine;
                                        });

            // Publish First Domain Event from purchasing
            purchasingPubSubNode.Publish(purchasingDomainEvent1);
            

            Assert.IsTrue(autoResetEventPurchasing.WaitOne()); // Leverage AutoResetEvent to assert async callback
            Assert.IsTrue(autoResetEventInventory.WaitOne()); // Leverage AutoResetEvent to assert async callback

            // Should assert for message being received twice, once by inventory and once by purchasing itself
            Assert.AreEqual(
                string.Format(
                    "Message received by Purchasing: {0}" + Environment.NewLine,
                    purchasingDomainEvent1),
                myPurchasingTestString);

            Assert.AreEqual(
                string.Format(
                    "Message received by Inventory: {0}" + Environment.NewLine,
                    purchasingDomainEvent1),
                myInventoryTestString);
            
            autoResetEventPurchasing.Reset();
            autoResetEventInventory.Reset();

            // Publish second domain event from purchasing
            purchasingPubSubNode.Publish(purchasingDomainEvent2);

            Assert.IsTrue(autoResetEventPurchasing.WaitOne()); // Leverage AutoResetEvent to assert async callback
            Assert.IsTrue(autoResetEventInventory.WaitOne()); // Leverage AutoResetEvent to assert async callback

            Thread.Sleep(2000);
            // Should assert for message being received twice, once by inventory and once by purchasing itself
            Assert.AreEqual(
                string.Format(
                    "Message received by Purchasing: {0}" + Environment.NewLine +
                    "Message received by Purchasing: {1}" + Environment.NewLine,
                    purchasingDomainEvent1,
                    purchasingDomainEvent2),
                myPurchasingTestString);

            Assert.AreEqual(
                string.Format(
                    "Message received by Inventory: {0}" + Environment.NewLine +
                    "Message received by Inventory: {1}" + Environment.NewLine,
                    purchasingDomainEvent1, 
                    purchasingDomainEvent2),
                myInventoryTestString);
        }
    }
}
