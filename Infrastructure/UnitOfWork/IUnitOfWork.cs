using bidify_be.Repository.Interfaces;

namespace bidify_be.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

}
