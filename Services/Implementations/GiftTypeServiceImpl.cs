using AutoMapper;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.GiftType;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;

namespace bidify_be.Services.Implementations
{
    public class GiftTypeServiceImpl : IGiftTypeService
    {
        private readonly ILogger<GiftTypeServiceImpl> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddGiftTypeRequest> _validatorAdd;
        private readonly IValidator<UpdateGiftTypeRequest> _validatorUpdate;
        private readonly IMapper _mapper;

        public GiftTypeServiceImpl(
            ILogger<GiftTypeServiceImpl> logger,
            IUnitOfWork unitOfWork,
            IValidator<AddGiftTypeRequest> validatorAdd,
            IValidator<UpdateGiftTypeRequest> validatorUpdate,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _validatorAdd = validatorAdd;
            _validatorUpdate = validatorUpdate;
            _mapper = mapper;
        }


        private async Task ValidateAsync<T>(T request, IValidator<T> validator)
        {
            var result = await validator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(result, _logger);
        }

        private async Task<GiftType> GetGiftTypeOrThrowAsync(Guid id)
        {
            var gift = await _unitOfWork.GiftTypeRepository.GetByIdAsync(id);
            if (gift == null)
            {
                _logger.LogWarning("Gift Type not found: {Id}", id);
                throw new GiftTypeNotFoundException($"Gift type not found with Id = {id}");
            }
            return gift;
        }


        public async Task<GiftTypeResponse> CreateAsync(AddGiftTypeRequest request)
        {
            _logger.LogInformation("Creating Gift Type: {Name}", request.Name);

            await ValidateAsync(request, _validatorAdd);

            var existsGiftTypeByCode = await _unitOfWork.GiftTypeRepository.ExistsWithCodeAsync(request.Code);
            if (existsGiftTypeByCode)
            {
                _logger.LogWarning("Gift Type with title '{Code}' already exists", request.Code);
                throw new InvalidOperationException($"Gift Type with title '{request.Code}' already exists.");
            }

            var giftType = _mapper.Map<GiftType>(request);
            giftType.CreatedAt = DateTime.UtcNow;
            giftType.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GiftTypeRepository.CreateAsync(giftType);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<GiftTypeResponse>(giftType);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting Gift Type {Id}", id);

            var gift = await GetGiftTypeOrThrowAsync(id);

            _unitOfWork.GiftTypeRepository.Delete(gift);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<GiftTypeResponse>> GetAllAsync(GiftTypeQueryRequest req)
        {
            _logger.LogInformation("Getting all gift types with filters");
            return await _unitOfWork.GiftTypeRepository.GetAllAsync(req);
        }


        public async Task<GiftTypeResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting gift type by Id = {Id}", id);

            var gift = await GetGiftTypeOrThrowAsync(id);

            return _mapper.Map<GiftTypeResponse>(gift);
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            _logger.LogInformation("Toggling active Gift Type {Id}", id);

            var gift = await GetGiftTypeOrThrowAsync(id);

            _unitOfWork.GiftTypeRepository.ToggleActive(gift);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<GiftTypeResponse> UpdateAsync(Guid id, UpdateGiftTypeRequest request)
        {
            _logger.LogInformation("Updating Gift Type {Id}", id);

            await ValidateAsync(request, _validatorUpdate);

            var gift = await GetGiftTypeOrThrowAsync(id);

            var existsGiftTypeByCode = await _unitOfWork.GiftTypeRepository.ExistsWithCodeAsync(id, request.Code);
            if (existsGiftTypeByCode)
            {
                _logger.LogWarning("Gift Type with code '{Code}' already exists", request.Code);
                throw new InvalidOperationException($"Gift Type with title '{request.Code}' already exists.");
            }

            _mapper.Map(request, gift);
            gift.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GiftTypeRepository.Update(gift);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<GiftTypeResponse>(gift);
        }
    }

}
