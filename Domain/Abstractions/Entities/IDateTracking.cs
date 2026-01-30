namespace bidify_be.Domain.Abstractions.Entities
{
    public interface IDateTracking
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }
    }
}
