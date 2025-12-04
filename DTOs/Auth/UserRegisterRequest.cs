namespace bidify_be.DTOs.Auth
{
    public class UserRegisterRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? ReferredBy { get; set; }
    }
}
