using System.Collections.Generic;

namespace Domain.InventoryDomain
{
    public class InventoryDomainEventCheckoutItems : DomainEventBase
    {
        public string FacilityId { get; set; }

        public IList<string> ItemIdsToBeRemovedList { get; set; }
    }

    public class InventoryDomainEventRefreshItems : DomainEventBase
    {
        public string FacilityId { get; set; }
    }
}
