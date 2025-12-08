namespace bidify_be.DTOs.GiftType
{
    public class UpdateGiftTypeRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
