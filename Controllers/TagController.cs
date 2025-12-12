using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Tags;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    //[Authorize(Roles = "admin")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<TagResponse>>> GetTagById([FromRoute] Guid id)
        {
            var response = await _tagService.GetTagByIdAsync(id);
            return Ok(ApiResponse<TagResponse>.SuccessResponse(response, "Tag retrieved successfully" ));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagResponse>>>> GetAllTags([FromQuery] TagQueryRequest queryRequest)
        {
            var response = await _tagService.GetAllTagsAsync(queryRequest);
            return Ok(ApiResponse<IEnumerable<TagResponse>>.SuccessResponse(
                response, "Tags retrieved successfully"
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TagResponse>>> CreateTag([FromBody] AddTagRequest request)
        {
            var response = await _tagService.CreateTagAsync(request);
            return Ok(ApiResponse<TagResponse>.SuccessResponse(
                response, "Tag created successfully"
            ));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<TagResponse>>> UpdateTag([FromRoute] Guid id, [FromBody] UpdateTagRequest request)
        {
            var response = await _tagService.UpdateTagAsync(id, request);
            return Ok(ApiResponse<TagResponse>.SuccessResponse(
                response, "Tag updated successfully"
            ));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTag([FromRoute] Guid id)
        {
            await _tagService.DeleteTagAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Tag deleted successfully"
            ));
        }

        [HttpPatch("toggle-active/{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleTagActiveStatus([FromRoute] Guid id)
        {
            await _tagService.ToggleActiveAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Tag active status toggled successfully"
            ));
        }
    }
}
