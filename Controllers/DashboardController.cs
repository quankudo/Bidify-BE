using bidify_be.Domain.Contracts;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Dashboard;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("cards")]
        public async Task<IActionResult> GetDashboardCards([FromQuery] TimeRange range = TimeRange.Day)
        {
            var result = await _dashboardService.GetListStatCardAsync(range);
            return Ok(ApiResponse<ListStatCardResponse>.SuccessResponse(result));
        }

        [HttpGet("charts")]
        public async Task<IActionResult> GetDashboardCharts([FromQuery] TimeRange range = TimeRange.Day)
        {
            var result = await _dashboardService.GetChartAsync(range);
            return Ok(ApiResponse<ChartResponse>.SuccessResponse(result));
        }

        [HttpGet("tables-no-filter")]
        public async Task<IActionResult> GetDashboardTableNoFilter()
        {
            var result = await _dashboardService.GetTablesNoFilter();
            return Ok(ApiResponse<TableNoFilterResponse>.SuccessResponse(result));
        }

        [HttpGet("tables-with-filter")]
        public async Task<IActionResult> GetDashboardTableWithFilter([FromQuery] TimeRange range = TimeRange.Day)
        {
            var result = await _dashboardService.GetTablesWithFilter(range);
            return Ok(ApiResponse<TableWithFilterResponse>.SuccessResponse(result));
        }
    }
}
