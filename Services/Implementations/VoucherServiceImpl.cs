using AutoMapper;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Voucher;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;

namespace bidify_be.Services.Implementations
{
    public class VoucherServiceImpl : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VoucherServiceImpl> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<AddVoucherRequest> _validatorAdd;
        private readonly IValidator<UpdateVoucherRequest> _validatorUpdate;

        public VoucherServiceImpl(
            IUnitOfWork unitOfWork,
            ILogger<VoucherServiceImpl> logger,
            IMapper mapper,
            IValidator<AddVoucherRequest> validatorAdd,
            IValidator<UpdateVoucherRequest> validatorUpdate)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _validatorAdd = validatorAdd;
            _validatorUpdate = validatorUpdate;
        }

        private async Task ValidateAsync<T>(T request, IValidator<T> validator)
        {
            var result = await validator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(result, _logger);
        }

        private async Task<Voucher> GetVoucherOrThrowAsync(Guid id)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                _logger.LogWarning("Voucher not found: {Id}", id);
                throw new VoucherNotFoundException($"Voucher with Id '{id}' not found.");
            }
            return voucher;
        }

        public async Task<VoucherResponse> AddVoucherAsync(AddVoucherRequest request)
        {
            _logger.LogInformation("Creating voucher for PackageBid: {PackageBidId}", request.PackageBidId);

            await ValidateAsync(request, _validatorAdd);

            var voucher = _mapper.Map<Voucher>(request);
            voucher.Code = await GenerateUniqueReferralCodeAsync();
            voucher.CreatedAt = DateTime.UtcNow;
            voucher.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.VoucherRepository.AddAsync(voucher);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Voucher created successfully with Id {Id}", voucher.Id);
            return _mapper.Map<VoucherResponse>(voucher);
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            _logger.LogInformation("Deleting voucher: {Id}", id);

            var voucher = await GetVoucherOrThrowAsync(id);
            voucher.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.VoucherRepository.DeleteAsync(voucher);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Voucher {Id} deleted successfully", id);
            return true;
        }

        public async Task<IEnumerable<VoucherResponse>> GetAllVouchersAsync()
        {
            _logger.LogInformation("Fetching all vouchers");

            var vouchers = await _unitOfWork.VoucherRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VoucherResponse>>(vouchers);
        }

        public async Task<VoucherResponse> GetVoucherByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching voucher with Id: {Id}", id);

            var voucher = await GetVoucherOrThrowAsync(id);
            return _mapper.Map<VoucherResponse>(voucher);
        }

        public async Task<IEnumerable<VoucherResponse>> GetVouchersByPackageBidIdAsync(Guid packageBidId)
        {
            _logger.LogInformation("Fetching vouchers by PackageBidId: {PackageBidID}", packageBidId);

            var vouchers = await _unitOfWork.VoucherRepository.GetByPackageBidIdAsync(packageBidId);
            return _mapper.Map<IEnumerable<VoucherResponse>>(vouchers);
        }

        public async Task<IEnumerable<VoucherResponse>> GetVouchersByStatusAsync(VoucherStatus status)
        {
            _logger.LogInformation("Fetching vouchers by Status: {Status}", status);

            var vouchers = await _unitOfWork.VoucherRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<VoucherResponse>>(vouchers);
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            _logger.LogInformation("Activating voucher with Id: {Id}", id);

            var voucher = await GetVoucherOrThrowAsync(id);
            voucher.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.VoucherRepository.ToggleActiveAsync(voucher);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Voucher {Id} is now Active", id);
            return true;
        }

        public async Task<VoucherResponse> UpdateVoucherAsync(Guid id, UpdateVoucherRequest request)
        {
            _logger.LogInformation("Updating voucher with Id: {Id}", id);

            var voucher = await GetVoucherOrThrowAsync(id);
            await ValidateAsync(request, _validatorUpdate);

            _mapper.Map(request, voucher);
            voucher.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.VoucherRepository.UpdateAsync(voucher);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Voucher {Id} updated successfully", id);
            return _mapper.Map<VoucherResponse>(voucher);
        }

        private async Task<string> GenerateUniqueReferralCodeAsync(int length = 15)
        {
            string code;
            bool exists;

            do
            {
                code = Guid.NewGuid().ToString("N")[..length].ToUpper();
                exists = await _unitOfWork.VoucherRepository.ExistsByCodeAsync(code);
            }
            while (exists);

            return code;
        }

        public async Task<PagedResult<VoucherResponse>> QueryAsync(VoucherQueryRequest req)
        {
            _logger.LogInformation("Querying vouchers with filters");
            return await _unitOfWork.VoucherRepository.QueryAsync(req);
        }
    }
}
