namespace bidify_be.Domain.Enums
{
    public enum AuctionStatus
    {
        Pending,          // Chờ phê duyệt
        Approved,         // Đã phê duyệt

        UserCancelled,    // Người dùng tự hủy
        Cancelled,        // Admin / hệ thống hủy

        EndedWithBids,    // Kết thúc (có đấu giá)
        EndedNoBids,      // Kết thúc (không ai đấu giá)
        Paid,             // Đã thanh toán
        Dispute           // Bị khiếu nại
    }
}
