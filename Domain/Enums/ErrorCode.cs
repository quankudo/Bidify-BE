using System.ComponentModel;

namespace bidify_be.Domain.Enums
{
    public enum ErrorCode
    {
        [Description("USER_NOT_FOUND")]
        UserNotFound,

        [Description("AUCTION_NOT_FOUND")]
        AuctionNotFound,

        [Description("PRODUCT_NOT_FOUND")]
        ProductNotFound,
        
        [Description("CATEGORY_NOT_FOUND")]
        CategoryNotFound,

        [Description("TAG_NOT_FOUND")]
        TagNotFound,

        [Description("ADDRESS_NOT_FOUND")]
        AddressNotFound,

        [Description("PACKAGE_BID_NOT_FOUND")]
        PackageBidNotFound,

        [Description("GIFT_TYPE_NOT_FOUND")]
        GiftTypeNotFound,

        [Description("GIFT_NOT_FOUND")]
        GiftNotFound,

        [Description("VOUCHER_NOT_FOUND")]
        VoucherNotFound,

        [Description("VALIDATION_FAILED")]
        ValidationFailed,

        [Description("INVALID_CREDENTIALS")]
        InvalidCredentials,

        [Description("RESEND_CODE_TOO_SOON")]
        ResendCodeTooSoon,

        [Description("UNAUTHORIZED_ACCESS")]
        UnauthorizedAccess,

        [Description("EMAIL_MISMATCH")]
        EmailMismatch,

        [Description("INVALID_VERIFY_CODE")]
        InvalidVerifyCode,

        [Description("VERIFY_CODE_EXPIRED")]
        VerifyCodeExpired,

        [Description("ADDRESS_LIMIT_EXCEEDED")]
        AddressLimitExceeded,

        [Description("INVALID_PRODUCT_STATE")]
        InvalidProductState,

        [Description("INSUFFICIENT_BALANCE")]
        InsufficientBalance,

        [Description("INSUFFICIENT_BID")]
        InsufficientBid,

        [Description("USER_ALREADY_VERIFIED")]
        UserAlreadyVerified
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
