namespace bidify_be.DTOs.Product
{
    public class RejectProductRequest
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }
}
