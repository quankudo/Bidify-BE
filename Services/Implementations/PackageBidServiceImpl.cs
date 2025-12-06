using AutoMapper;
using bidify_be.DTOs.PackageBid;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;

namespace bidify_be.Services.Implementations
{
    public class PackageBidServiceImpl : IPackageBidService
    {
        private readonly ILogger<PackageBidServiceImpl> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddPackageBidRequest> _validatorAdd;
        private readonly IValidator<UpdatePackageBidRequest> _validatorUpdate;

        public PackageBidServiceImpl(
            ILogger<PackageBidServiceImpl> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IValidator<AddPackageBidRequest> validatorAdd,
            IValidator<UpdatePackageBidRequest> validatorUpdate)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validatorAdd = validatorAdd;
            _validatorUpdate = validatorUpdate;
        }

        public async Task<PackageBidResponse> CreateAsync(AddPackageBidRequest request)
        {
            _logger.LogInformation("Creating PackageBid: {Title}", request.Title);

            var validationResult = await _validatorAdd.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            // Check duplicate name
            var exists = await _unitOfWork.PackageBids.ExistsAsync(request.Title.Trim());
            if (exists)
            {
                _logger.LogWarning("PackageBid '{Title}' already exists", request.Title);
                throw new InvalidOperationException($"PackageBid with title '{request.Title}' already exists.");
            }

            var entity = _mapper.Map<Domain.Entities.PackageBid>(request);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.PackageBids.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("PackageBid created successfully with ID: {Id}", entity.Id);

            return _mapper.Map<PackageBidResponse>(entity);
        }

        public async Task<IEnumerable<PackageBidResponse>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all package bids");

            var list = await _unitOfWork.PackageBids.GetAllAsync();

            return _mapper.Map<IEnumerable<PackageBidResponse>>(list);
        }

        public async Task<PackageBidResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving PackageBid with ID: {Id}", id);

            var entity = await _unitOfWork.PackageBids.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("PackageBid with ID {Id} not found", id);
                throw new PackageBidNotFoundException($"PackageBid with ID {id} not found.");
            }

            return _mapper.Map<PackageBidResponse>(entity);
        }

        public async Task<PackageBidResponse> UpdateAsync(Guid id, UpdatePackageBidRequest request)
        {
            _logger.LogInformation("Updating PackageBid with ID: {Id}", id);

            var validationResult = await _validatorUpdate.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var entity = await _unitOfWork.PackageBids.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("PackageBid with ID {Id} not found", id);
                throw new PackageBidNotFoundException($"PackageBid with ID {id} not found.");
            }

            // Check duplicate name but exclude itself
            var exists = await _unitOfWork.PackageBids.ExistsAsync(id, request.Title.Trim());
            if (exists)
            {
                _logger.LogWarning("PackageBid with name {Title} already exists", request.Title);
                throw new InvalidOperationException($"PackageBid with title '{request.Title}' already exists.");
            }

            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PackageBids.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("PackageBid with ID {Id} updated successfully", id);

            return _mapper.Map<PackageBidResponse>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting PackageBid with ID: {Id}", id);

            var entity = await _unitOfWork.PackageBids.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("PackageBid with ID {Id} not found", id);
                throw new PackageBidNotFoundException($"PackageBid with ID {id} not found.");
            }

            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PackageBids.Delete(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("PackageBid with ID {Id} deleted successfully", id);
            return true;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            _logger.LogInformation("Toggling active status for package bid with ID: {Id}", id);

            var packageBid = await _unitOfWork.PackageBids.GetByIdAsync(id);

            if (packageBid == null)
            {
                _logger.LogWarning("Package bid with ID {Id} not found", id);
                throw new PackageBidNotFoundException($"Package bid with ID {id} not found.");
            }

            packageBid.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PackageBids.ToggleActive(packageBid);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Package bid with ID {Id} active status updated to: true",
                id
            );

            return true;
        }

    }
}
