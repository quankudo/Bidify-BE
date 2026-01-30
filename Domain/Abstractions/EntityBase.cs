using bidify_be.Domain.Abstractions.Entities;

namespace bidify_be.Domain.Abstractions
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    { 
        public TKey Id { get; set; }
    }
}
