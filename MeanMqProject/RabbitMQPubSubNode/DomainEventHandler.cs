using Domain;
using Domain.InventoryDomain;
using Newtonsoft.Json;
using Repository;
using System;

namespace RabbitMQPubSubNode
{
    public class DomainEventHandler
    {
        InventoryRepository _inventoryRepository = new InventoryRepository();

        // Contains business logic to identify event type and target domain
        // Will respond to events that are of the expected type targeting the current node type
        public string HandleDomainEvent(string nodeType, string jsonMessage)
        {
            DomainEventBase resolvedDomainEvent = ResolveDomainEvent(jsonMessage);
            if (resolvedDomainEvent.EventType == DomainEventType.InventoryDomainEventCheckoutItems &&
                resolvedDomainEvent.TargetDomain == nodeType)
            {
                var resolvedInventoryDomainEvent = JsonConvert.DeserializeObject<InventoryDomainEventCheckoutItems>(jsonMessage);
                //remove items from this facility using the InventoryRepository
                _inventoryRepository.RemoveItemsFromStorageFacility(resolvedInventoryDomainEvent);
                return "DomainEvent: InventoryDomainEventCheckoutItems handled successfully";
            }

            return "DomainEvent: Not handled";
        }

        public DomainEventBase ResolveDomainEvent(string jsonMessage)
        {
            DomainEventBase resolvedDomainEvent;
            try
            {
                resolvedDomainEvent = JsonConvert.DeserializeObject<DomainEventBase>(jsonMessage);
            }
            catch (Exception)
            {
                resolvedDomainEvent = new DomainEventBase
                                        {
                                            EventType = DomainEventType.IndeterminateType,
                                            SourceDomain = "Json Parsing Error",
                                            TargetDomain = "Json Parsing Error"
                                        };
            }

            return resolvedDomainEvent;
        }
    }
}
