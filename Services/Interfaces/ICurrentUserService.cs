namespace bidify_be.Services.Interfaces
{
    public interface ICurrentUserService
    {
        public Guid? GetUserId();
        bool IsAdmin();
    }
}
