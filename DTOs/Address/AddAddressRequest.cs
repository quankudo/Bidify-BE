namespace bidify_be.DTOs.Address
{
    public class AddAddressRequest
    {
        public string? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public string LineOne { get; set; } = string.Empty;
        public string LineTwo { get; set; } = string.Empty;
    }
}
