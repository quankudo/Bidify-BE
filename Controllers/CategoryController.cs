using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Category;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;

        public CategoryController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> CreateCategory([FromBody] AddCategoryRequest request)
        {
            var response = await _categoryServices.CreateAsync(request);
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(
                response, "Category created successfully"
            ));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var response = await _categoryServices.UpdateAsync(id, request);
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(
                response, "Category updated successfully"
            ));
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategoryById([FromRoute] Guid id)
        {
            var response = await _categoryServices.GetByIdAsync(id);
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(
                response, "Category retrieved successfully"
            ));
        }

        [HttpGet("search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryResponse>>>> GetAllPagingAsync([FromQuery] CategoryQueryRequest req)
        {
            var result = await _categoryServices.FilterAsync(req);
            return Ok(ApiResponse<PagedResult<CategoryResponse>>.SuccessResponse(result));
        }

        
        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryResponse>>>> GetAllAsync()
        {
            var result = await _categoryServices.GetAllAsync();
            return Ok(ApiResponse<List<CategoryShortResponse>>.SuccessResponse(result));
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory([FromRoute] Guid id)
        {
            var response = await _categoryServices.DeleteAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(
                response, "Category deleted successfully"
            ));
        }

        [HttpPatch("toggle-active/{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleCategoryActiveStatus([FromRoute] Guid id)
        {
            var response = await _categoryServices.ToggleActiveAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(
                response, "Category active status toggled successfully"
            ));
        }
    }
}
