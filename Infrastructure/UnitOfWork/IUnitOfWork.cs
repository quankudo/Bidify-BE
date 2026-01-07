using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace bidify_be.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICategoryRepository Categories { get; }
        IPackageBidRepository PackageBids { get; }
        ITagRepository TagRepository { get; }
        IAddressRepository Addresses { get; }
        IGiftTypeRepository GiftTypeRepository { get; }
        IGiftRepository GiftRepository { get; }
        IVoucherRepository VoucherRepository { get; }
        IProductRepository ProductRepository { get; }
        IFileStorageRepository FileStorageRepository { get; }
        ITopupTransactionRepository TopupTransactionRepository { get; }
        IWalletTransactionRepository WalletTransactionRepository { get; }
        ITransitionPackageBidRepository TransitionPackageBidRepository { get; }
        IAuctionRepository AuctionRepository { get; }
        INotificationRepository NotificationRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync();
    }

}
