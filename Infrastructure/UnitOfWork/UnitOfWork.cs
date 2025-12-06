using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;

namespace bidify_be.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Categories { get; }
        public IPackageBidRepository PackageBids { get; }

        public ITagRepository TagRepository { get; }
        public IAddressRepository Addresses { get; }

        public UnitOfWork(ApplicationDbContext context, ICategoryRepository categories, IPackageBidRepository packageBids, ITagRepository tagRepository, IAddressRepository addresses)
        {
            _context = context;
            Categories = categories;
            PackageBids = packageBids;
            TagRepository = tagRepository;
            Addresses = addresses;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
