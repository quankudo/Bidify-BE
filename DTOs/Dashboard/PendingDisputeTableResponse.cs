namespace bidify_be.DTOs.Dashboard
{
    public class PendingDisputeTableResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; }
    }
}
