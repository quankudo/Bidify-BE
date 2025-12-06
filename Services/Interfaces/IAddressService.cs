using bidify_be.DTOs.Address;

namespace bidify_be.Services.Interfaces
{
    public interface IAddressService
    {
        public Task<AddressResponse> AddAddressAsync(AddAddressRequest address);
        public Task<AddressResponse> GetAddressByIdAsync(Guid id);
        public Task<List<AddressResponse>> GetAddressesByUserIdAsync(string userId);
        public Task<AddressResponse> UpdateAddress(Guid id, UpdateAddressRequest address);
        public Task DeleteAddress(Guid id);
        public Task SetDefaultAddress(Guid id);
        public Task<AddressResponse> GetDefaultAddress(string userId);
    }
}
