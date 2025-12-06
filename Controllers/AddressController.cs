using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Address;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // GET /api/address/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> GetAddressByIdAsync(Guid id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            return Ok(ApiResponse<AddressResponse>.SuccessResponse(address, "Get address successfully"));
        }

        // GET /api/address/user/{userId}/list
        [HttpGet("user/{userId}/list")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<AddressResponse>>>> GetAddressesByUserIdAsync(string userId)
        {
            var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
            return Ok(ApiResponse<List<AddressResponse>>.SuccessResponse(addresses, "Get addresses successfully"));
        }

        // POST /api/address
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> AddAddressAsync([FromBody] AddAddressRequest request)
        {
            var address = await _addressService.AddAddressAsync(request);
            return Ok(ApiResponse<AddressResponse>.SuccessResponse(address, "Add address successfully"));
        }

        // PUT /api/address/{id}
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> UpdateAddressAsync(Guid id, [FromBody] UpdateAddressRequest request)
        {
            var address = await _addressService.UpdateAddress(id, request);
            return Ok(ApiResponse<AddressResponse>.SuccessResponse(address, "Update address successfully"));
        }

        // DELETE /api/address/{id}
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAddressAsync(Guid id)
        {
            await _addressService.DeleteAddress(id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Delete address successfully"));
        }

        // PUT /api/address/{id}/default
        [HttpPut("{id:guid}/default")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> SetDefaultAddressAsync(Guid id)
        {
            await _addressService.SetDefaultAddress(id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Set default successfully"));
        }
    }
}
