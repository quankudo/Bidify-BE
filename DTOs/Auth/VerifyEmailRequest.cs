namespace bidify_be.DTOs.Auth
{
    public class VerifyEmailRequest
    {
        public Guid Id { get; set; }
        public string Code { get; set; } 
        public string Email { get; set; }
    }
}
