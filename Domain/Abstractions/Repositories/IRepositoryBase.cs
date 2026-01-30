namespace bidify_be.Domain.Abstractions.Repositories
{
    public interface IRepositoryBase<TEntity, in TKey> where TEntity : class
    {
        Task<TEntity> FindByIdAsync(TKey id);
        //FindSingleAsync
        //FindAll
        //Add
        //Update
        //Remove
        //RemoveMultiple
    }
}
