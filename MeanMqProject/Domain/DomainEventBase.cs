
namespace Domain
{
    public class DomainEventBase
    {
        public DomainEventType EventType  { get; set; }
        public string SourceDomain { get; set; }
        public string TargetDomain { get; set; }
    }

    public enum DomainEventType
    {
        InventoryDomainEventCheckoutItems,
        InventoryDomainEventRefreshItems,
        IndeterminateType
    }
}
