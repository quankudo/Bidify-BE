using bidify_be.Domain.Enums;
using bidify_be.DTOs.Dashboard;

namespace bidify_be.Services.Interfaces
{
    public interface IDashboardService
    {
        public Task<ListStatCardResponse> GetListStatCardAsync(TimeRange range);
        public Task<ChartResponse> GetChartAsync(TimeRange range);

        public Task<TableNoFilterResponse> GetTablesNoFilter();
        public Task<TableWithFilterResponse> GetTablesWithFilter(TimeRange range);
    }
}
