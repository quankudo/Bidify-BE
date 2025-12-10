using AutoMapper;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Gift;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;

namespace bidify_be.Services.Implementations
{
    public class GiftServiceImpl : IGiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GiftServiceImpl> _logger;
        private readonly IValidator<AddGiftRequest> _validatorAdd;
        private readonly IValidator<UpdateGiftRequest> _validatorUpdate;

        public GiftServiceImpl(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GiftServiceImpl> logger,
            IValidator<AddGiftRequest> validatorAdd,
            IValidator<UpdateGiftRequest> validatorUpdate)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _validatorAdd = validatorAdd;
            _validatorUpdate = validatorUpdate;
        }

        // Helper: Validate request
        private async Task ValidateAsync<T>(T request, IValidator<T> validator)
        {
            var result = await validator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(result, _logger);
        }

        // Helper: Get entity or throw
        private async Task<Gift> GetGiftOrThrowAsync(Guid id)
        {
            var gift = await _unitOfWork.GiftRepository.GetByIdAsync(id);
            if (gift == null)
            {
                _logger.LogWarning("Gift not found: {Id}", id);
                throw new GiftNotFoundException($"Gift with Id '{id}' not found.");
            }
            return gift;
        }

        // CREATE
        public async Task<GiftResponse> CreateAsync(AddGiftRequest request)
        {
            _logger.LogInformation("Creating Gift");

            await ValidateAsync(request, _validatorAdd);

            var gift = _mapper.Map<Gift>(request);
            gift.Code = await GenerateUniqueReferralCodeAsync();
            gift.CreatedAt = DateTime.UtcNow;
            gift.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GiftRepository.CreateAsync(gift);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Gift created with Code = {Code}", gift.Code);

            return _mapper.Map<GiftResponse>(gift);
        }

        // GET ALL
        //public async Task<IEnumerable<GiftResponse>> GetAllAsync()
        //{
        //    _logger.LogInformation("Getting all gifts");
        //    return await _unitOfWork.GiftRepository.GetAllAsync();
        //}

        public async Task<PagedResult<GiftResponse>> SearchAsync(GiftQueryRequest req)
        {
            _logger.LogInformation("Searching gifts...");
            return await _unitOfWork.GiftRepository.SearchAsync(req);
        }


        // GET BY ID
        public async Task<GiftResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting gift with Id = {Id}", id);

            var gift = await _unitOfWork.GiftRepository.GetByIdAsyncResponse(id);
            if(gift == null)
            {
                _logger.LogInformation("Gift not found with {Id}", id);
                throw new GiftNotFoundException($"Gift not found with {id}");
            }
            return gift;
        }

        // UPDATE
        public async Task<GiftResponse> UpdateAsync(Guid id, UpdateGiftRequest request)
        {
            _logger.LogInformation("Updating Gift {Id}", id);

            await ValidateAsync(request, _validatorUpdate);

            var gift = await GetGiftOrThrowAsync(id);

            _mapper.Map(request, gift);
            gift.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GiftRepository.UpdateAsync(gift);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<GiftResponse>(gift);
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting Gift {Id}", id);

            var gift = await GetGiftOrThrowAsync(id);

            _unitOfWork.GiftRepository.DeleteAsync(gift);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // TOGGLE ACTIVE
        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            _logger.LogInformation("Toggling Active Gift {Id}", id);

            var gift = await GetGiftOrThrowAsync(id);

            _unitOfWork.GiftRepository.ToggleActiveAsync(gift);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Generate Unique Code
        private async Task<string> GenerateUniqueReferralCodeAsync(int length = 15)
        {
            string code;
            bool exists;

            do
            {
                code = Guid.NewGuid().ToString("N")[..length].ToUpper();
                exists = await _unitOfWork.GiftRepository.ExistsByCodeAsync(code);
            }
            while (exists);

            return code;
        }
    }
}
