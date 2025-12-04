namespace bidify_be.DTOs.Users
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Avatar { get; set; }
    }
}
