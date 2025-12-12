using AutoMapper;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Tags;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;

namespace bidify_be.Services.Implementations
{
    public class TagServiceImpl : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TagServiceImpl> _logger;
        private readonly IValidator<AddTagRequest> _validatorAdd;
        private readonly IValidator<UpdateTagRequest> _validatorUpdate;
        private readonly IMapper _mapper;

        public TagServiceImpl(
            IUnitOfWork unitOfWork,
            IValidator<AddTagRequest> validatorAdd,
            IValidator<UpdateTagRequest> validatorUpdate,
            ILogger<TagServiceImpl> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _validatorAdd = validatorAdd;
            _validatorUpdate = validatorUpdate;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<TagResponse> CreateTagAsync(AddTagRequest request)
        {
            _logger.LogInformation("Creating tag: {Title}", request.Title);

            var validationResult = await _validatorAdd.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var title = request.Title.Trim();

            // Check trùng tên
            var exists = await _unitOfWork.TagRepository.ExistsByTitleAsync(title);
            if (exists)
            {
                _logger.LogWarning("Tag with title {Title} already exists", title);
                throw new InvalidOperationException($"Tag with title '{title}' already exists.");
            }

            var tag = _mapper.Map<Tag>(request);
            tag.CreatedAt = DateTime.UtcNow;
            tag.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TagRepository.CreateTagAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tag created successfully with ID: {Id}", tag.Id);

            return _mapper.Map<TagResponse>(tag);
        }

        
        public async Task<TagResponse> UpdateTagAsync(Guid id, UpdateTagRequest request)
        {
            _logger.LogInformation("Updating tag with ID: {Id}", id);

            var validationResult = await _validatorUpdate.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var tag = await _unitOfWork.TagRepository.GetTagByIdAsync(id);
            if (tag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found", id);
                throw new TagNotFoundException($"Tag with ID {id} not found.");
            }

            var title = request.Title.Trim();

            // Check trùng tên với tag khác
            var existsOther = await _unitOfWork.TagRepository.ExistsOtherWithTitleAsync(id, title);
            if (existsOther)
            {
                _logger.LogWarning("Another tag with title {Title} already exists", title);
                throw new InvalidOperationException($"Tag with title '{title}' already exists.");
            }

            tag.UpdatedAt = DateTime.UtcNow;

            // Map request vào entity
            _mapper.Map(request, tag);

            _unitOfWork.TagRepository.UpdateTagAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tag with ID {Id} updated successfully", id);

            return _mapper.Map<TagResponse>(tag);
        }


        public async Task<IEnumerable<TagResponse>> GetAllTagsAsync(TagQueryRequest req)
        {
            _logger.LogInformation("Retrieving all tags...");

            return await _unitOfWork.TagRepository.GetAllTagsAsync(req);
        }


        public async Task<TagResponse> GetTagByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving tag with ID: {Id}", id);

            var tag = await _unitOfWork.TagRepository.GetTagByIdAsync(id);
            if (tag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found", id);
                throw new TagNotFoundException($"Tag with ID {id} not found.");
            }

            return _mapper.Map<TagResponse>(tag);
        }


        public async Task DeleteTagAsync(Guid id)
        {
            _logger.LogInformation("Deleting tag with ID: {Id}", id);

            var tag = await _unitOfWork.TagRepository.GetTagByIdAsync(id);

            if (tag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found", id);
                throw new TagNotFoundException($"Tag with ID {id} not found.");
            }

            tag.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TagRepository.DeleteTagAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tag with ID {Id} deleted (soft) successfully", id);
        }


        public async Task ToggleActiveAsync(Guid id)
        {
            _logger.LogInformation("Toggling active status for tag with ID: {Id}", id);

            var tag = await _unitOfWork.TagRepository.GetTagByIdAsync(id);

            if (tag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found", id);
                throw new TagNotFoundException($"Tag with ID {id} not found.");
            }

            tag.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TagRepository.ToggleActiveAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tag with ID {Id} toggled status to: {Status}", id, tag.Status);
        }

    }
}
