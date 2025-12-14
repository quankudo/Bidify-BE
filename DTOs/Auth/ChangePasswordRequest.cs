namespace bidify_be.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
