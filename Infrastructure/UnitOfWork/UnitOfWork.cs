using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace bidify_be.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository UserRepository { get; set; }
        public ICategoryRepository Categories { get; }
        public IPackageBidRepository PackageBids { get; }

        public ITagRepository TagRepository { get; }
        public IAddressRepository Addresses { get; }
        public IGiftTypeRepository GiftTypeRepository { get; }
        public IGiftRepository GiftRepository { get; }
        public IVoucherRepository VoucherRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IFileStorageRepository FileStorageRepository { get; }
        public IWalletTransactionRepository WalletTransactionRepository { get; }
        public ITopupTransactionRepository TopupTransactionRepository { get; }
        public ITransitionPackageBidRepository TransitionPackageBidRepository { get; }
        public IAuctionRepository AuctionRepository { get; }
        public INotificationRepository NotificationRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context, 
            IUserRepository userRepository,
            ICategoryRepository categories, 
            IPackageBidRepository packageBids, 
            ITagRepository tagRepository, 
            IAddressRepository addresses,
            IGiftTypeRepository giftTypeRepository,
            IGiftRepository giftRepository,
            IVoucherRepository voucherRepository,
            IProductRepository productRepository,
            IFileStorageRepository fileStorageRepository,
            ITopupTransactionRepository topupTransactionRepository,
            IWalletTransactionRepository walletTransactionRepository,
            ITransitionPackageBidRepository transitionPackageBidRepository,
            IAuctionRepository auctionRepository,
            INotificationRepository notificationRepository)
        {
            _context = context;
            UserRepository = userRepository;
            Categories = categories;
            PackageBids = packageBids;
            TagRepository = tagRepository;
            Addresses = addresses;
            GiftRepository = giftRepository;
            GiftTypeRepository = giftTypeRepository;
            VoucherRepository = voucherRepository;
            ProductRepository = productRepository;
            FileStorageRepository = fileStorageRepository;
            TopupTransactionRepository = topupTransactionRepository;
            WalletTransactionRepository = walletTransactionRepository;
            TransitionPackageBidRepository = transitionPackageBidRepository;
            AuctionRepository = auctionRepository;
            NotificationRepository = notificationRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await _context.Database.BeginTransactionAsync();
    }

}
