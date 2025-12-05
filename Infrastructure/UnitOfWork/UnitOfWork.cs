using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;

namespace bidify_be.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Categories { get; }

        public UnitOfWork(ApplicationDbContext context, ICategoryRepository categories)
        {
            _context = context;
            Categories = categories;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
