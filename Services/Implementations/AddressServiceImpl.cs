using AutoMapper;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Address;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Repository.Interfaces;
using bidify_be.Services.Interfaces;
using FluentValidation;
using UnauthorizedAccessException = bidify_be.Exceptions.UnauthorizedAccessException;

namespace bidify_be.Services.Implementations
{
    public class AddressServiceImpl : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddressServiceImpl> _logger;
        private readonly IValidator<AddAddressRequest> _addValidator;
        private readonly IValidator<UpdateAddressRequest> _updateValidator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAddressRepository _addressRepository;

        public AddressServiceImpl(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AddressServiceImpl> logger,
            IValidator<AddAddressRequest> addValidator,
            IValidator<UpdateAddressRequest> updateValidator,
            IAddressRepository addressRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
            _currentUserService = currentUserService;
            _addressRepository = addressRepository;
        }
        
        public async Task<AddressResponse> AddAddressAsync(AddAddressRequest request)
        {
            var currentUserId = _currentUserService.GetUserId();

            if (currentUserId == null)
            {
                _logger.LogError("Current user ID is null");
                throw new UnauthorizedAccessException("User must be logged in to add an address");
            }

            request.UserId = currentUserId;

            _logger.LogInformation("Creating address for user {UserId}", request.UserId);

            int addressCount = await _addressRepository.GetAddressCountByUserAsync(currentUserId.Value);

            if (addressCount >= 4)
            {
                _logger.LogWarning(
                    "User {UserId} reached max address count {Max}",
                    currentUserId,
                    4
                );

                throw new AddressLimitExceededException(
                    $"Maximum {4} addresses allowed"
                );
            }


            var validation = await _addValidator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validation, _logger);

            var address = _mapper.Map<Address>(request);

            // Nếu là default -> clear default cũ
            if (address.IsDefault)
            {
                var oldDefault = await _addressRepository.GetDefaultAddress(request.UserId.Value);
                if (oldDefault != null)
                {
                    oldDefault.IsDefault = false;
                    _addressRepository.UpdateAddress(oldDefault);
                }
            }

            address.CreatedAt = DateTime.UtcNow;
            address.UpdatedAt = DateTime.UtcNow;

            await _addressRepository.AddAddressAsync(address);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Address created with ID: {Id}", address.Id);

            return _mapper.Map<AddressResponse>(address);
        }

        // READ BY ID
        public async Task<AddressResponse> GetAddressByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving address {Id}", id);

            var address = await _addressRepository.GetAddressByIdAsync(id);
            if (address == null)
            {
                _logger.LogWarning("Address {Id} not found", id);
                throw new AddressNotFoundException($"Address {id} not found");
            }

            var currentUserId = _currentUserService.GetUserId();

            AuthorizationHelper.EnsureSameUser(currentUserId, address.UserId);

            return _mapper.Map<AddressResponse>(address);
        }

        // READ ALL BY USER
        public async Task<List<AddressResponse>> GetAddressesByUserIdAsync()
        {
            var currentUserId = _currentUserService.GetUserId();

            _logger.LogInformation("Retrieving addresses for user {UserId}", currentUserId);

            var addresses = await _addressRepository.GetAddressesByUserIdAsync(currentUserId.Value);

            return _mapper.Map<List<AddressResponse>>(addresses);
        }

        // READ DEFAULT
        public async Task<AddressResponse> GetDefaultAddress()
        {
            var currentUserId = _currentUserService.GetUserId();

            _logger.LogInformation("Retrieving default address for user {UserId}", currentUserId);

            var address = await _addressRepository.GetDefaultAddress(currentUserId.Value);
            if (address == null)
            {
                throw new AddressNotFoundException("Default address not found");
            }
            
            return _mapper.Map<AddressResponse>(address);
        }

        // UPDATE
        public async Task<AddressResponse> UpdateAddress(Guid id, UpdateAddressRequest request)
        {
            _logger.LogInformation("Updating address {Id}", id);

            var validation = await _updateValidator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validation, _logger);

            var address = await _addressRepository.GetAddressByIdAsync(id);
            if (address == null)
            {
                throw new AddressNotFoundException($"Address {id} not found");
            }

            var currentUserId = _currentUserService.GetUserId();
            AuthorizationHelper.EnsureSameUser(currentUserId, address.UserId);

            bool becomingDefault = request.IsDefault && !address.IsDefault;

            // Map update
            _mapper.Map(request, address);
            address.UpdatedAt = DateTime.UtcNow;

            // If set default -> remove old default
            if (becomingDefault)
            {
                var oldDefault = await _addressRepository.GetDefaultAddress(address.UserId);
                if (oldDefault != null && oldDefault.Id != address.Id)
                {
                    oldDefault.IsDefault = false;
                    _addressRepository.UpdateAddress(oldDefault);
                }
            }

            _addressRepository.UpdateAddress(address);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AddressResponse>(address);
        }

        // DELETE
        public async Task DeleteAddress(Guid id)
        {
            _logger.LogInformation("Deleting address {Id}", id);

            var address = await _addressRepository.GetAddressByIdAsync(id);
            if (address == null)
            {
                _logger.LogWarning("Address {Id} not found", id);
                throw new AddressNotFoundException($"Address {id} not found");
            }

            var currentUserId = _currentUserService.GetUserId();
            AuthorizationHelper.EnsureSameUser(currentUserId, address.UserId);

            _addressRepository.DeleteAddress(address);
            await _unitOfWork.SaveChangesAsync();
        }

        // SET DEFAULT
        public async Task SetDefaultAddress(Guid id)
        {
            _logger.LogInformation("Setting default address: {Id}", id);

            var address = await _addressRepository.GetAddressByIdAsync(id);
            if (address == null)
            {
                _logger.LogWarning("Address {Id} not found", id);
                throw new AddressNotFoundException($"Address {id} not found");
            }

            var currentUserId = _currentUserService.GetUserId();
            AuthorizationHelper.EnsureSameUser(currentUserId, address.UserId);

            if (address.IsDefault)
            {
                _logger.LogInformation("Address {Id} is already default", id);
                return;
            }

            var oldDefault = await _addressRepository.GetDefaultAddress(address.UserId);
            if (oldDefault != null && oldDefault.Id != id)
            {
                oldDefault.IsDefault = false;
                _addressRepository.UpdateAddress(oldDefault);
            }

            address.UpdatedAt = DateTime.UtcNow;

            _addressRepository.SetDefaultAddress(address);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
