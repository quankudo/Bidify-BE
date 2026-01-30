namespace bidify_be.Domain.Abstractions.Entities
{
    public interface IUserTracking
    {
        Guid CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }
    }
}
