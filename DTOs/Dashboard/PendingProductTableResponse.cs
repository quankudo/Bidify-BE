namespace bidify_be.DTOs.Dashboard
{
    public class PendingProductTableResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
