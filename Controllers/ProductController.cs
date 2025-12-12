using Microsoft.AspNetCore.Mvc;
using bidify_be.Services.Interfaces;
using bidify_be.DTOs.Product;
using bidify_be.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // ------------------ CREATE ------------------
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> Add([FromBody] AddProductRequest request)
        {
            var result = await _productService.AddProductAsync(request);
            return Ok(ApiResponse<ProductResponse>.SuccessResponse(result, "Product created successfully"));
        }

        // ------------------ GET DETAIL ------------------
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetDetail(Guid id)
        {
            var result = await _productService.GetProductDetailAsync(id);
            return Ok(ApiResponse<ProductResponse>.SuccessResponse(result, "Fetched product detail successfully"));
        }

        // ------------------ UPDATE ------------------
        [HttpPut("{id:guid}")]
        [Authorize] 
        public async Task<ActionResult<ApiResponse<ProductResponse>>> Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            var result = await _productService.UpdateProductAsync(id, request);
            return Ok(ApiResponse<ProductResponse>.SuccessResponse(result, "Product updated successfully"));
        }

        // ------------------ DELETE ------------------
        [HttpDelete("hidden/{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteByUser(Guid id)
        {
            var result = await _productService.DeleteProductAsyncByUser(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Product deleted successfully"));
        }

        [HttpDelete("cancle/{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteByAdmin(Guid id)
        {
            var result = await _productService.DeleteProductAsyncByAdmin(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Product deleted successfully"));
        }

        // ------------------ FILTER + PAGINATION ------------------
        [HttpGet("filter")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductShortResponse>>>> Filter([FromQuery] ProductFilterRequest request)
        {
            var result = await _productService.FilterProductsAsync(request);
            return Ok(ApiResponse<PagedResult<ProductShortResponse>>.SuccessResponse(result, "Fetched products successfully"));
        }
    }
}
