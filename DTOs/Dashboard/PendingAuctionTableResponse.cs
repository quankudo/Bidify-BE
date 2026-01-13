namespace bidify_be.DTOs.Dashboard
{
    public class PendingAuctionTableResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal StartPrice { get; set; }
        public decimal StepPrice { get; set; }
        public int CountDay { get; set; }
    }
}
