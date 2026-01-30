namespace bidify_be.Domain.Abstractions.Entities
{
    public interface IEntityAuditBase<TKey> : IEntityBase<TKey>, IAuditable
    {
    }
}
