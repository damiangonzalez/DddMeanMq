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
        private readonly RabbitMqPubNode _rabbitMqPubSubNode;
        public InventoryService()
        {
            _inventoryRepository = new InventoryRepository();
            _rabbitMqPubSubNode = new RabbitMqPubNode("InventoryService");
        }

        public StorageFacility GetStorageFacility(string facilityId)
        {
            return _inventoryRepository.GetStorageFacility(facilityId);
        }

        public void HandlePost(DomainEventBase domainEventDataFromPost)
        {
            if (domainEventDataFromPost == null ||
                domainEventDataFromPost.SourceDomain == null ||
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
