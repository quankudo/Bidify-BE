namespace bidify_be.DTOs.Gift
{
    public class GiftResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int QuantityBid { get; set; }
        public Guid GiftTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
