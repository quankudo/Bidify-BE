using AutoMapper;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Category;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;

namespace bidify_be.Services.Implementations
{
    public class CategoryServiceImpl : ICategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryServiceImpl> _logger;
        private readonly IValidator<AddCategoryRequest> _validatorAdd;
        private readonly IValidator<UpdateCategoryRequest> _validatorUpdate;
        private readonly IMapper _mapper;

        public CategoryServiceImpl(
            IUnitOfWork unitOfWork, 
            IValidator<UpdateCategoryRequest> validatorUpdate, 
            IValidator<AddCategoryRequest> validatorAdd, 
            ILogger<CategoryServiceImpl> logger,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _validatorUpdate = validatorUpdate;
            _validatorAdd = validatorAdd;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<CategoryResponse> CreateAsync(AddCategoryRequest request)
        {
            _logger.LogInformation("Creating category: {Title}", request.Title);

            var validationResult = await _validatorAdd.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var isExisting = await _unitOfWork.Categories.ExistsAsyncByName(request.Title.Trim());
            if(isExisting)
            {
                _logger.LogWarning("Category with title {Title} already exists", request.Title);
                throw new InvalidOperationException($"Category with title {request.Title} already exists.");
            }

            var newCat = _mapper.Map<Category>(request);

            newCat.CreatedAt = DateTime.UtcNow;
            newCat.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Categories.AddAsync(newCat);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Category added with ID: {Id}", newCat.Id);

            return _mapper.Map<CategoryResponse>(newCat);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting category with ID: {Id}", id);

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found", id);
                throw new CategoryNotFoundException($"Category with ID {id} not found.");
            }

            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Category with ID {Id} deleted successfully", id);
            return true;
        }


        public async Task<PagedResult<CategoryResponse>> GetAllAsync(CategoryQueryRequest req)
        {
            _logger.LogInformation("Get all categories with paging/search/filter");
            return await _unitOfWork.Categories.GetAllAsync(req);
        }




        public async Task<CategoryResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving category with ID: {Id}", id);

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found", id);
                throw new CategoryNotFoundException($"Category with ID {id} not found.");
            }

            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            _logger.LogInformation("Toggling active status for category with ID: {Id}", id);

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found", id);
                throw new CategoryNotFoundException($"Category with ID {id} not found.");
            }
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Category with ID {Id} updated active status to true",
                id
            );

            return true;
        }


        public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request)
        {
            _logger.LogInformation("Updating category with ID: {Id}", id);

            // Validate request
            var validationResult = await _validatorUpdate.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var title = request.Title.Trim();

            // Check if category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found", id);
                throw new CategoryNotFoundException($"Category with ID {id} not found.");
            }

            // Check duplicate title (excluding itself)
            var existsByName = await _unitOfWork.Categories.ExistsAsyncByName(title, id);
            if (existsByName)
            {
                _logger.LogWarning("Category with title {Title} already exists", title);
                throw new InvalidOperationException($"Category with title '{title}' already exists.");
            }

            // Map updated fields into existing entity
            _mapper.Map(request, category);
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Category with ID {Id} updated successfully", id);

            return _mapper.Map<CategoryResponse>(category);
        }

    }
}
