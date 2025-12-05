using System.ComponentModel;

namespace bidify_be.Domain.Enums
{
    public enum ErrorCode
    {
        [Description("USER_NOT_FOUND")]
        UserNotFound,

        [Description("PRODUCT_NOT_FOUND")]
        ProductNotFound,

        [Description("VALIDATION_FAILED")]
        ValidationFailed,

        [Description("UNAUTHORIZED_ACCESS")]
        UnauthorizedAccess
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attr?.Description ?? value.ToString();
        }
    }
}
