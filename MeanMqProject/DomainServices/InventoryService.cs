using System;
using Domain;
using Domain.InventoryDomain;
using Newtonsoft.Json;
using RabbitMQPubSubNode;
using Repository;

namespace DomainServices
{
    public class InventoryService
    {
        private readonly InventoryRepository _inventoryRepository;
        private readonly RabbitMqPubSubNode _rabbitMqPubSubNode;
        public InventoryService(bool pubOnly = false)
        {
            _inventoryRepository = new InventoryRepository();
            _rabbitMqPubSubNode = new RabbitMqPubSubNode(
                    "InventoryService",
                    (x) => Console.WriteLine("received: " + x.ToString()),
                    pubOnly);
        }

        public StorageFacility GetStorageFacility(string facilityId)
        {
            return _inventoryRepository.GetStorageFacility(facilityId);
        }

        public void HandlePostBak(InventoryDomainEventCheckoutItems domainEventDataFromPost)
        {
            if (domainEventDataFromPost.FacilityId == null ||
               domainEventDataFromPost.ItemIdsToBeRemovedList == null)
            {
                return;
            }

            _rabbitMqPubSubNode.Publish(JsonConvert.SerializeObject(domainEventDataFromPost));
        }

        public void HandlePost(DomainEventBase domainEventDataFromPost)
        {
            if (domainEventDataFromPost.SourceDomain == null ||
               domainEventDataFromPost.TargetDomain == null ||
                domainEventDataFromPost.EventType == DomainEventType.IndeterminateType)
            {
                return;
            }

            if (domainEventDataFromPost.EventType == DomainEventType.InventoryDomainEventCheckoutItems)
            {
                var checkoutDomainEvent = (InventoryDomainEventCheckoutItems)domainEventDataFromPost;
                _rabbitMqPubSubNode.Publish(JsonConvert.SerializeObject(checkoutDomainEvent));
            }
            else if (domainEventDataFromPost.EventType == DomainEventType.InventoryDomainEventRefreshItems)
            {
                var checkoutDomainEvent = (InventoryDomainEventRefreshItems)domainEventDataFromPost;
                _rabbitMqPubSubNode.Publish(JsonConvert.SerializeObject(checkoutDomainEvent));
            }
        }
    }
}
