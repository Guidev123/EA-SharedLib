using SharedLib.Domain.Messages.Integration;

namespace SharedLib.Domain.Messages.Integration.OrderPlaced
{
    public class OrderPlacedIntegrationEvent : IntegrationEvent
    {
        public OrderPlacedIntegrationEvent(string customerId)
        {
            AggregateId = Guid.Parse(customerId);
            CustomerId = customerId;
        }
        public string CustomerId { get; private set; }
    }
}
