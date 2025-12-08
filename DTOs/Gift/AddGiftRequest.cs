namespace bidify_be.DTOs.Gift
{
    public class AddGiftRequest
    {
        public int QuantityBid { get; set; }
        public Guid GiftTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
