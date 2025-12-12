namespace bidify_be.DTOs.Users
{
    public class UserQueryRequest
    {
        public string? Search {  get; set; }
        public string? Role { get; set; }
        public bool? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
