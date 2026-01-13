using bidify_be.Domain.Enums;
using bidify_be.DTOs.Dashboard;
using bidify_be.Repository.Interfaces;
using bidify_be.Services.Interfaces;
using System;

namespace bidify_be.Services.Implementations
{
    public class DashboardServiceImpl : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        private (DateTime start, DateTime end, DateTime prevStart, DateTime prevEnd)
        ResolveTimeRange(TimeRange range)
        {
            var now = DateTime.UtcNow;
            DateTime start, end, prevStart, prevEnd;

            switch (range)
            {
                case TimeRange.Day:
                    start = now.Date;
                    end = start.AddDays(1);
                    prevStart = start.AddDays(-1);
                    prevEnd = start;
                    break;

                case TimeRange.Week:
                    start = now.Date.AddDays(-(int)now.DayOfWeek + 1);
                    end = start.AddDays(7);
                    prevStart = start.AddDays(-7);
                    prevEnd = start;
                    break;

                case TimeRange.Month:
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1);
                    prevStart = start.AddMonths(-1);
                    prevEnd = start;
                    break;

                case TimeRange.Year:
                    start = new DateTime(now.Year, 1, 1);
                    end = start.AddYears(1);
                    prevStart = start.AddYears(-1);
                    prevEnd = start;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (start, end, prevStart, prevEnd);
        }

        private (DateTime start, DateTime end)
        ResolveTimeRangeNoPrev(TimeRange range)
        {
            var now = DateTime.UtcNow;
            DateTime start, end;

            switch (range)
            {
                case TimeRange.Day:
                    start = now.Date;
                    end = start.AddDays(1);
                    break;

                case TimeRange.Week:
                    start = now.Date.AddDays(-(int)now.DayOfWeek + 1);
                    end = start.AddDays(7);
                    break;

                case TimeRange.Month:
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1);
                    break;

                case TimeRange.Year:
                    start = new DateTime(now.Year, 1, 1);
                    end = start.AddYears(1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (start, end);
        }


        public DashboardServiceImpl(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public Task<ListStatCardResponse> GetListStatCardAsync(TimeRange range)
        {
            var (start, end, prevStart, prevEnd) = ResolveTimeRange(range);
            return _dashboardRepository.GetListStatCardAsync(start, end, prevStart, prevEnd, range);
        }

        public async Task<ChartResponse> GetChartAsync(TimeRange range)
        {
            var (start, end) = ResolveTimeRangeNoPrev(range);
            var revenueChart = await _dashboardRepository.GetRevenueChartAsync(start, end, range);
            var categoryChart = await _dashboardRepository.GetCategoryChartAsync(start, end, range);
            var auctionHotChart = await _dashboardRepository.GetAuctionHotChartAsync(start, end, range);
            var auctionCompletionStats = await _dashboardRepository.GetAuctionCompletionStatsAsync(start, end, range);
            return new ChartResponse
            {
                AuctionHotChart = auctionHotChart,
                CategoryChart = categoryChart,
                RevenueChart = revenueChart,
                AuctionCompletionStats = auctionCompletionStats,
            };
        }

        public async Task<TableNoFilterResponse> GetTablesNoFilter()
        {
            var countPendingProduct = await _dashboardRepository.CountPendingProductsAsync();
            var countPendingAuction = await _dashboardRepository.CountPendingAuctionsAsync();
            var countPendingDispute = await _dashboardRepository.CountPendingDisputesAsync();

            var pendingProductTable = await _dashboardRepository.GetPendingProductTableNoFilterAsync();
            var pendingAuctionTable = await _dashboardRepository.GetPendingAuctionTableNoFilterAsync();
            var pendingDisputeTable = await _dashboardRepository.GetPendingDisputeTableNoFilterAsync();

            return new TableNoFilterResponse
            {
                CountPendingAuction = countPendingAuction,
                CountPendingDispute = countPendingDispute,
                CountPendingProdut = countPendingProduct,
                PendingAuctionTable = pendingAuctionTable,
                PendingDisputeTable = pendingDisputeTable,
                PendingProductTable = pendingProductTable,
            };
        }

        public async Task<TableWithFilterResponse> GetTablesWithFilter(TimeRange range)
        {
            var (start, end) = ResolveTimeRangeNoPrev(range);
            var topAuctionParticipantTable = await _dashboardRepository.GetTopAuctionParticipantTable(start, end, range);
            return new TableWithFilterResponse
            {
                TopAuctionParticipantTables = topAuctionParticipantTable,
            };
        }
    }
}
