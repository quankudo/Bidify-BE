namespace bidify_be.Domain.Enums
{
    public enum OrderStatus
    {
        PendingPayment = 0,   // Đã tạo order, chờ thanh toán
        Paid = 1,             // Đã thanh toán
        Processing = 2,       // Người bán đang xử lý / đóng gói
        Shipped = 3,          // Đã gửi hàng
        Delivered = 4,        // Đã giao thành công
        Completed = 5,        // Hoàn tất (không khiếu nại)
        Cancelled = 6,        // Bị hủy
        Refunded = 7          // Đã hoàn tiền
    }

}
