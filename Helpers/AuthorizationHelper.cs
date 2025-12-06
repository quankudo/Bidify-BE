namespace bidify_be.Helpers
{
    public static class AuthorizationHelper
    {
        public static string EnsureSameUser(string? currentUserId, string targetUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                throw new UnauthorizedAccessException("You must be logged in to perform this action.");
            }

            if (currentUserId != targetUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to perform this action.");
            }

            return currentUserId;
        }
    }

}
