namespace bidify_be.Domain.Abstractions.Entities
{
    public interface IEntityBase<TKey>
    {
        TKey Id { get; }
    }
}
