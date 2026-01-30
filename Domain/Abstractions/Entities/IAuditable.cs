namespace bidify_be.Domain.Abstractions.Entities
{
    public interface IAuditable : ISoftDelete, IUserTracking, IDateTracking
    {

    }
}
