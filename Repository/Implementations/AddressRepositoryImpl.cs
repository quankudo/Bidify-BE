using bidify_be.Domain.Entities;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class AddressRepositoryImpl : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetAddressCountByUserAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .CountAsync();
        }


        public async Task AddAddressAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
        }

        public async Task<Address?> GetAddressByIdAsync(Guid id)
        {
            return await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Address>> GetAddressesByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Address?> GetDefaultAddress(string userId)
        {
            return await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IsDefault && a.UserId == userId);
        }
        
        public void UpdateAddress(Address address)
        {
            _context.Addresses.Update(address);
        }
        
        public void DeleteAddress(Address address)
        {
            _context.Addresses.Remove(address);
        }

        public void SetDefaultAddress(Address address)
        {
            address.IsDefault = true;
            _context.Addresses.Update(address);
        }
    }
}
