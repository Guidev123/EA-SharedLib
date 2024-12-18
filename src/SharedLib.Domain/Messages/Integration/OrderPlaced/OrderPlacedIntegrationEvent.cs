using SharedLib.Domain.Messages.Integration;

namespace SharedLib.Domain.Messages.Integration.OrderPlaced
{
    public class OrderPlacedIntegrationEvent : IntegrationEvent
    {
        public OrderPlacedIntegrationEvent(Guid customerId)
        {
            AggregateId = customerId;
            CustomerId = customerId;
        }
        public Guid CustomerId { get; private set; }
    }
}
