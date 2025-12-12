using bidify_be.DTOs.Tags;

namespace bidify_be.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagResponse>> GetAllTagsAsync(TagQueryRequest req);
        Task<TagResponse> GetTagByIdAsync(Guid id);
        Task<TagResponse> CreateTagAsync(AddTagRequest tag);
        Task<TagResponse> UpdateTagAsync(Guid id, UpdateTagRequest tag);
        Task DeleteTagAsync(Guid id);
        Task ToggleActiveAsync(Guid id);
    }
}
