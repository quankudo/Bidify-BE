using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface IAddressRepository
    {
        public Task AddAddressAsync(Address address);
        public Task<Address?> GetAddressByIdAsync(Guid id);
        public Task<List<Address>> GetAddressesByUserIdAsync(string userId);
        public void UpdateAddress(Address address);
        public void DeleteAddress(Address address);
        public void SetDefaultAddress(Address address);
        public Task<Address?> GetDefaultAddress(string userId);
    }
}
