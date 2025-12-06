using bidify_be.DTOs.Category;

namespace bidify_be.Services.Interfaces
{
    public interface ICategoryServices
    {
        Task<CategoryResponse> GetByIdAsync(Guid id);
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse> CreateAsync(AddCategoryRequest request);
        Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
    }
}
