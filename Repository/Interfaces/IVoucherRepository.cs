using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Voucher;
using System.Runtime.CompilerServices;

namespace bidify_be.Repository.Interfaces
{
    public interface IVoucherRepository
    {
        // Lấy tất cả voucher
        Task<IEnumerable<Voucher>> GetAllAsync();

        // Lấy voucher theo Id
        Task<Voucher?> GetByIdAsync(Guid id);

        // Thêm voucher mới
        Task AddAsync(Voucher voucher);

        // Cập nhật voucher
        void UpdateAsync(Voucher voucher);

        void DeleteAsync(Voucher voucher);

        void ToggleActiveAsync(Voucher voucher);

        // Tìm theo PackageBidId hoặc Status
        Task<IEnumerable<Voucher>> GetByPackageBidIdAsync(Guid packageBidId);
        Task<IEnumerable<Voucher>> GetByStatusAsync(VoucherStatus status);

        Task<PagedResult<VoucherResponse>> QueryAsync(VoucherQueryRequest req);

        Task<bool> ExistsByCodeAsync(string code);
    }
}
