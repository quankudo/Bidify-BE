using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Users
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public Gender Gender { get; set; }
        public string? Avatar { get; set; }

        public string? PublicId { get; set; }
    }
}
