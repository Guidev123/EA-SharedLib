namespace SharedLib.Domain.Messages.Integration.AppliedVoucher
{
    public class AppliedVoucherIntegrationEvent : IntegrationEvent
    {
        public AppliedVoucherIntegrationEvent(string code)
        {
            AggregateId = Guid.NewGuid();
            Code = code;
        }

        public string Code { get; private set; } = string.Empty;
    }
}
