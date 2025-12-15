using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace bidify_be.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Categories { get; }
        public IPackageBidRepository PackageBids { get; }

        public ITagRepository TagRepository { get; }
        public IAddressRepository Addresses { get; }
        public IGiftTypeRepository GiftTypeRepository { get; }
        public IGiftRepository GiftRepository { get; }
        public IVoucherRepository VoucherRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IFileStorageRepository FileStorageRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context, 
            ICategoryRepository categories, 
            IPackageBidRepository packageBids, 
            ITagRepository tagRepository, 
            IAddressRepository addresses,
            IGiftTypeRepository giftTypeRepository,
            IGiftRepository giftRepository,
            IVoucherRepository voucherRepository,
            IProductRepository productRepository,
            IFileStorageRepository fileStorageRepository)
        {
            _context = context;
            Categories = categories;
            PackageBids = packageBids;
            TagRepository = tagRepository;
            Addresses = addresses;
            GiftRepository = giftRepository;
            GiftTypeRepository = giftTypeRepository;
            VoucherRepository = voucherRepository;
            ProductRepository = productRepository;
            FileStorageRepository = fileStorageRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await _context.Database.BeginTransactionAsync();
    }

}
