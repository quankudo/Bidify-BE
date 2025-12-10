using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Voucher;

namespace bidify_be.Services.Interfaces
{
    public interface IVoucherService
    {
        // Thêm voucher mới
        Task<VoucherResponse> AddVoucherAsync(AddVoucherRequest request);

        // Cập nhật voucher
        Task<VoucherResponse> UpdateVoucherAsync(Guid id, UpdateVoucherRequest request);

        // Xóa voucher
        Task<bool> DeleteVoucherAsync(Guid id);

        // Lấy tất cả voucher
        Task<IEnumerable<VoucherResponse>> GetAllVouchersAsync();

        // Lấy voucher theo Id
        Task<VoucherResponse> GetVoucherByIdAsync(Guid id);

        Task<bool> ToggleActiveAsync(Guid id);

        // Lấy voucher theo trạng thái
        Task<IEnumerable<VoucherResponse>> GetVouchersByStatusAsync(Domain.Enums.VoucherStatus status);

        // Lấy voucher theo PackageBidId
        Task<IEnumerable<VoucherResponse>> GetVouchersByPackageBidIdAsync(Guid packageBidId);

        Task<PagedResult<VoucherResponse>> QueryAsync(VoucherQueryRequest req);
    }
}
