namespace bidify_be.Helpers
{
    public static class AuthorizationHelper
    {
        public static Guid EnsureSameUser(Guid? currentUserId, Guid targetUserId)
        {
            if (currentUserId == null || currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("You must be logged in to perform this action.");
            }

            if (currentUserId.Value != targetUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to perform this action.");
            }

            return currentUserId.Value;
        }
    }
}
