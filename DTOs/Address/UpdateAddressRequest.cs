namespace bidify_be.DTOs.Address
{
    public class UpdateAddressRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string LineOne { get; set; } = string.Empty;
        public string LineTwo { get; set; } = string.Empty;
    }
}
