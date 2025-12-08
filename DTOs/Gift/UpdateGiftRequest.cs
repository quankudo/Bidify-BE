namespace bidify_be.DTOs.Gift
{
    public class UpdateGiftRequest
    {
        public int QuantityBid { get; set; }
        public Guid GiftTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
