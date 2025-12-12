using bidify_be.Domain.Entities;
using bidify_be.DTOs.Tags;

namespace bidify_be.Repository.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<TagResponse>> GetAllTagsAsync(TagQueryRequest req);
        Task<Tag?> GetTagByIdAsync(Guid id);

        Task<bool> ExistsByTitleAsync(string title);
        Task<bool> ExistsOtherWithTitleAsync(Guid id, string title);

        Task CreateTagAsync(Tag tag);
        void UpdateTagAsync(Tag tag);
        void DeleteTagAsync(Tag tag);
        void ToggleActiveAsync(Tag tag);
    }
}
