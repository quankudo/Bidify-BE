namespace bidify_be.DTOs.Auth
{
    public class UserRegisterResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime ExpireVerifyCode { get; set; }
    }
}
