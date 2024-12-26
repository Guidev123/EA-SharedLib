namespace SharedLib.Domain.Messages.Integration.AppliedVoucher
{
    public class AppliedVoucherResponse
    {
        public AppliedVoucherResponse(decimal? percentual, decimal? discountValue, string code, int discountType)
        {
            Percentual = percentual;
            DiscountValue = discountValue;
            Code = code;
            DiscountType = discountType;
        }

        public decimal? Percentual { get; private set; }
        public decimal? DiscountValue { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public int DiscountType { get; private set; }
    }
}
