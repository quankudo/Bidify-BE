namespace bidify_be.Domain.Enums
{
    public enum ProductCondition
    {
        New = 0,                // Mới 100%, chưa dùng
        LikeNew = 1,            // Như mới, dùng rất ít
        UsedGood = 2,           // Đã qua sử dụng, còn tốt
        UsedAcceptable = 3,     // Đã dùng, có dấu hiệu hao mòn
        Refurbished = 4,        // Đã tân trang, kiểm định lại
        OpenBox = 5,            // Hàng mở hộp nhưng mới
        Damaged = 6,            // Bị hư nhẹ, trầy xước
        ForParts = 7            // Chỉ dùng để lấy linh kiện
    }
}
