namespace bidify_be.DTOs.Auth
{
    public class ResendCodeRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
