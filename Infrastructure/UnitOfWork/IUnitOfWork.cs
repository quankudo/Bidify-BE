using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace bidify_be.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IPackageBidRepository PackageBids { get; }
        ITagRepository TagRepository { get; }
        IAddressRepository Addresses { get; }
        IGiftTypeRepository GiftTypeRepository { get; }
        IGiftRepository GiftRepository { get; }
        IVoucherRepository VoucherRepository { get; }
        IProductRepository ProductRepository { get; }
        IFileStorageRepository FileStorageRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync();
    }

}
