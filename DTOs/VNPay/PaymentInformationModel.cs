namespace bidify_be.DTOs.VNPay
{
    public class PaymentInformationModel
    {
        public string ClientOrderId { get; set; }
        public string OrderType { get; set; }
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
    }

}
