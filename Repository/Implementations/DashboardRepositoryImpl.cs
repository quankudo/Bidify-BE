using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Dashboard;
using bidify_be.DTOs.Users;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class DashboardRepositoryImpl : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        public DashboardRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        private StatCardResponse BuildStatCard(
            string title,
            int current,
            int previous,
            List<StatCardData> data)
        {
            var diff = current - previous;
            var percent = previous == 0
                ? 0
                : Math.Round((double)diff / previous * 100, 1);

            return new StatCardResponse
            {
                Title = title,
                Value = current.ToString(),
                PercentageChange = percent,
                IsUp = diff >= 0,
                Data = data
            };
        }

        private async Task<List<StatCardData>> BuildStatChart(
            TimeRange range,
            IQueryable<DateTime> source)
        {
            switch (range)
            {
                case TimeRange.Day:
                    {
                        var raw = await source
                            .GroupBy(x => x.Hour)
                            .Select(g => new
                            {
                                Key = g.Key,
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Key)
                            .ToListAsync();

                        return raw.Select(x => new StatCardData
                        {
                            Label = $"{x.Key}h",
                            Value = x.Count
                        }).ToList();
                    }

                case TimeRange.Week:
                    {
                        var raw = await source
                            .GroupBy(x => x.DayOfWeek)
                            .Select(g => new
                            {
                                Key = g.Key,
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Key)
                            .ToListAsync();

                        return raw.Select(x => new StatCardData
                        {
                            Label = x.Key.ToString().Substring(0, 3),
                            Value = x.Count
                        }).ToList();
                    }

                case TimeRange.Month:
                    {
                        var raw = await source
                            .GroupBy(x => (x.Day - 1) / 7 + 1)
                            .Select(g => new
                            {
                                Key = g.Key,
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Key)
                            .ToListAsync();

                        return raw.Select(x => new StatCardData
                        {
                            Label = $"W{x.Key}",
                            Value = x.Count
                        }).ToList();
                    }

                case TimeRange.Year:
                    {
                        var raw = await source
                            .GroupBy(x => x.Month)
                            .Select(g => new
                            {
                                Key = g.Key,
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Key)
                            .ToListAsync();

                        return raw.Select(x => new StatCardData
                        {
                            Label = $"T{x.Key}",
                            Value = x.Count
                        }).ToList();
                    }

                default:
                    return new List<StatCardData>();
            }
        }



        public async Task<ListStatCardResponse> GetListStatCardAsync(DateTime start, DateTime end, DateTime prevStart, DateTime prevEnd, TimeRange range)
        {
            var userCurrent = await _context.Users
                .CountAsync(x => x.CreateAt >= start && x.CreateAt < end);

            var userPrevious = await _context.Users
                .CountAsync(x => x.CreateAt >= prevStart && x.CreateAt < prevEnd);

            // AUCTION ACTIVE
            var auctionActiveCurrent = await _context.Auctions
                .CountAsync(x => x.Status == AuctionStatus.Approved);

            var auctionActivePrevious = await _context.Auctions
                .CountAsync(x =>
                    x.Status == AuctionStatus.Approved &&
                    x.CreatedAt >= prevStart && x.CreatedAt < prevEnd);

            // AUCTION CLOSED
            var auctionClosedCurrent = await _context.Auctions
                .CountAsync(x =>
                    x.Status == AuctionStatus.EndedNoBids || x.Status == AuctionStatus.EndedWithBids &&
                    x.EndAt >= start && x.EndAt < end);

            var auctionClosedPrevious = await _context.Auctions
                .CountAsync(x =>
                    x.Status == AuctionStatus.EndedNoBids || x.Status == AuctionStatus.EndedWithBids &&
                    x.EndAt >= prevStart && x.EndAt < prevEnd);

            // DISPUTE
            //var disputeCurrent = await _context.Disputes
            //    .CountAsync(x => x.CreatedAt >= start && x.CreatedAt < end);

            //var disputePrevious = await _context.Disputes
            //    .CountAsync(x => x.CreatedAt >= prevStart && x.CreatedAt < prevEnd);

            return new ListStatCardResponse
            {
                UserCard = BuildStatCard(
                    "Người dùng",
                    userCurrent,
                    userPrevious,
                    await BuildStatChart(range, _context.Users.Select(x => x.CreateAt))
                ),

                AuctionActiveCard = BuildStatCard(
                    "Auction đang diễn ra",
                    auctionActiveCurrent,
                    auctionActivePrevious,
                    await BuildStatChart(range, _context.Auctions.Select(x => x.CreatedAt))
                ),

                AuctionClosedCard = BuildStatCard(
                    "Auction đã kết thúc",
                    auctionClosedCurrent,
                    auctionClosedPrevious,
                    await BuildStatChart(range, _context.Auctions
                        .Where(x => x.Status == AuctionStatus.EndedNoBids || x.Status == AuctionStatus.EndedWithBids)
                        .Select(x => x.EndAt))
                ),

                //DisputeCard = BuildStatCard(
                //    "Khiếu nại",
                //    disputeCurrent,
                //    disputePrevious,
                //    await BuildStatChart(range, _context.Disputes.Select(x => x.CreatedAt))
                //)
            };
        }

        //=======================================================================================================
        private async Task<List<RevenueChartResponse>> BuildRevenueByHour(
            IQueryable<TopupTransaction> topups,
            IQueryable<TransitionPackageBid> bids)
        {
            var topupData = await topups
                .GroupBy(x => x.CreatedAt.Hour)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var bidData = await bids
                .GroupBy(x => x.CreatedAt.Hour)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Price) })
                .ToListAsync();

            return Enumerable.Range(0, 24)
                .Select(h => new RevenueChartResponse
                {
                    Label = $"{h}h",
                    Topup = topupData.FirstOrDefault(x => x.Key == h)?.Total ?? 0,
                    Bid = bidData.FirstOrDefault(x => x.Key == h)?.Total ?? 0
                })
                .ToList();
        }

        private async Task<List<RevenueChartResponse>> BuildRevenueByDayOfWeek(
            IQueryable<TopupTransaction> topups,
            IQueryable<TransitionPackageBid> bids)
        {
            var topupData = await topups
                .GroupBy(x => x.CreatedAt.DayOfWeek)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var bidData = await bids
                .GroupBy(x => x.CreatedAt.DayOfWeek)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Price) })
                .ToListAsync();

            var days = Enum.GetValues<DayOfWeek>();

            return days.Select(d => new RevenueChartResponse
            {
                Label = d.ToString().Substring(0, 3),
                Topup = topupData.FirstOrDefault(x => x.Key == d)?.Total ?? 0,
                Bid = bidData.FirstOrDefault(x => x.Key == d)?.Total ?? 0
            }).ToList();
        }

        private async Task<List<RevenueChartResponse>> BuildRevenueByWeek(
            IQueryable<TopupTransaction> topups,
            IQueryable<TransitionPackageBid> bids)
        {
            var topupData = await topups
                .GroupBy(x => (x.CreatedAt.Day - 1) / 7 + 1)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var bidData = await bids
                .GroupBy(x => (x.CreatedAt.Day - 1) / 7 + 1)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Price) })
                .ToListAsync();

            return Enumerable.Range(1, 5)
                .Select(w => new RevenueChartResponse
                {
                    Label = $"W{w}",
                    Topup = topupData.FirstOrDefault(x => x.Key == w)?.Total ?? 0,
                    Bid = bidData.FirstOrDefault(x => x.Key == w)?.Total ?? 0
                })
                .ToList();
        }

        private async Task<List<RevenueChartResponse>> BuildRevenueByMonth(
            IQueryable<TopupTransaction> topups,
            IQueryable<TransitionPackageBid> bids)
        {
            var topupData = await topups
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var bidData = await bids
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Price) })
                .ToListAsync();

            return Enumerable.Range(1, 12)
                .Select(m => new RevenueChartResponse
                {
                    Label = $"T{m}",
                    Topup = topupData.FirstOrDefault(x => x.Key == m)?.Total ?? 0,
                    Bid = bidData.FirstOrDefault(x => x.Key == m)?.Total ?? 0
                })
                .ToList();
        }


        public async Task<List<RevenueChartResponse>> GetRevenueChartAsync(
            DateTime start,
            DateTime end,
            TimeRange range)
        {
            var topupQuery = _context.TopupTransactions
                .Where(x =>
                    x.Status == TopupTransactionsStatus.Success &&
                    x.CreatedAt >= start && x.CreatedAt < end);

            var bidQuery = _context.TransitionPackagesBids
                .Where(x => x.CreatedAt >= start && x.CreatedAt < end);

            switch (range)
            {
                case TimeRange.Day:
                    return await BuildRevenueByHour(topupQuery, bidQuery);

                case TimeRange.Week:
                    return await BuildRevenueByDayOfWeek(topupQuery, bidQuery);

                case TimeRange.Month:
                    return await BuildRevenueByWeek(topupQuery, bidQuery);

                case TimeRange.Year:
                    return await BuildRevenueByMonth(topupQuery, bidQuery);

                default:
                    return new List<RevenueChartResponse>();
            }
        }

        //=======================================================================================================

        public async Task<List<CategoryChartResponse>> GetCategoryChartAsync(
            DateTime start,
            DateTime end,
            TimeRange range)
        {
            // Tổng auction
            var totalAuctions = await _context.Auctions
                .CountAsync(a => a.CreatedAt >= start && a.CreatedAt < end);

            if (totalAuctions == 0)
                return new List<CategoryChartResponse>();

            var data = await (
                from a in _context.Auctions
                join p in _context.Products on a.ProductId equals p.Id
                join c in _context.Categories on p.CategoryId equals c.Id
                where a.CreatedAt >= start && a.CreatedAt < end
                group a by c.Title into g
                select new
                {
                    CategoryName = g.Key,
                    Count = g.Count()
                }
            ).ToListAsync();

            return data.Select(x => new CategoryChartResponse
            {
                Name = x.CategoryName,
                Value = (int)Math.Round((double)x.Count * 100 / totalAuctions)
            }).ToList();
        }




        public async Task<List<AuctionHotChartResponse>> GetAuctionHotChartAsync(DateTime start, DateTime end, TimeRange range)
        {
            return new List<AuctionHotChartResponse>
            {
                new() { Name = "A1", Bids = 120 },
                new() { Name = "A2", Bids = 98 },
                new() { Name = "A3", Bids = 150 },
                new() { Name = "A4", Bids = 80 },
            };
        }

        public async Task<List<PendingAuctionTableResponse>> GetPendingAuctionTableNoFilterAsync()
        {
            return await _context.Auctions
                .AsNoTracking()
                .Where(x => x.Status == AuctionStatus.Pending)
                .Select(x => new PendingAuctionTableResponse
                {
                    Id = x.Id,
                    StartPrice = x.StartPrice,
                    StepPrice = x.StepPrice,
                    UpdatedAt = x.UpdatedAt,
                    CountDay = EF.Functions.DateDiffDay(x.StartAt, x.EndAt),
                    Title = x.Product.Name
                })
                .ToListAsync();
        }

        public async Task<List<PendingProductTableResponse>> GetPendingProductTableNoFilterAsync()
        {
            return await _context.Products.AsNoTracking().Where(x => x.Status == ProductStatus.Pending).Select(x => new PendingProductTableResponse
            {
                Id = x.Id,
                Name = x.Name,
                UpdatedAt = x.UpdatedAt
            }).ToListAsync();
        }

        public Task<List<PendingDisputeTableResponse>> GetPendingDisputeTableNoFilterAsync()
        {
            var data = new List<PendingDisputeTableResponse>
            {
                new PendingDisputeTableResponse
                {
                    Id = Guid.NewGuid(),
                    Title = "Tranh chấp đấu giá iPhone 15 Pro",
                    Reason = "Người bán không giao hàng đúng mô tả",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new PendingDisputeTableResponse
                {
                    Id = Guid.NewGuid(),
                    Title = "Tranh chấp MacBook Pro M3",
                    Reason = "Sản phẩm có dấu hiệu đã qua sử dụng",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new PendingDisputeTableResponse
                {
                    Id = Guid.NewGuid(),
                    Title = "Tranh chấp đồng hồ Rolex",
                    Reason = "Nghi ngờ hàng không chính hãng",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new PendingDisputeTableResponse
                {
                    Id = Guid.NewGuid(),
                    Title = "Tranh chấp tai nghe AirPods",
                    Reason = "Thiếu phụ kiện đi kèm",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new PendingDisputeTableResponse
                {
                    Id = Guid.NewGuid(),
                    Title = "Tranh chấp máy ảnh Sony A7 IV",
                    Reason = "Hàng bị trầy xước nặng",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };

            return Task.FromResult(data);
        }


        public async Task<int> CountPendingAuctionsAsync()
        {
            return await _context.Auctions.AsNoTracking()
                .CountAsync(x => x.Status == AuctionStatus.Pending);
        }

        public async Task<int> CountPendingProductsAsync()
        {
            return await _context.Products.AsNoTracking()
                .CountAsync(x => x.Status == ProductStatus.Pending);
        }

        public Task<int> CountPendingDisputesAsync()
        {
            return Task.FromResult(5);
        }

        public async Task<List<AuctionCompletionStat>> GetAuctionCompletionStatsAsync(
            DateTime start,
            DateTime end,
            TimeRange range)
        {
            var endedStatusesWithBids = new[]
            {
                AuctionStatus.EndedWithBids,
                AuctionStatus.Paid,
                AuctionStatus.Dispute
            };

            var stats = await _context.Auctions
                .Where(a =>
                    a.EndAt >= start &&
                    a.EndAt <= end &&
                    (
                        endedStatusesWithBids.Contains(a.Status) ||
                        a.Status == AuctionStatus.EndedNoBids
                    )
                )
                .GroupBy(a => a.Status == AuctionStatus.EndedNoBids)
                .Select(g => new
                {
                    NoBids = g.Key, // true = không có ai đấu giá
                    Count = g.Count()
                })
                .ToListAsync();

            return new List<AuctionCompletionStat>
            {
                new AuctionCompletionStat
                {
                    Name = "Có người tham gia đấu giá",
                    Value = stats
                        .Where(x => !x.NoBids)
                        .Select(x => x.Count)
                        .FirstOrDefault()
                },
                new AuctionCompletionStat
                {
                    Name = "Không có người tham gia đấu giá",
                    Value = stats
                        .Where(x => x.NoBids)
                        .Select(x => x.Count)
                        .FirstOrDefault()
                }
            };
        }



        public async Task<List<TopAuctionParticipantTableResponse>> GetTopAuctionParticipantTable(
             DateTime start,
             DateTime end,
             TimeRange range)
        {
            var validAuctionStatuses = new[]
            {
                AuctionStatus.Approved,
                AuctionStatus.EndedWithBids,
                AuctionStatus.EndedNoBids,
                AuctionStatus.Paid,
                AuctionStatus.Dispute
            };

            var query =
                from b in _context.BidsHistories
                join a in _context.Auctions on b.AuctionId equals a.Id
                join u in _context.Users on b.UserId equals u.Id
                where
                    b.CreatedAt >= start &&
                    b.CreatedAt <= end &&
                    validAuctionStatuses.Contains(a.Status)
                group b by new
                {
                    u.Id,
                    u.UserName,
                    u.Avatar,
                    u.RateStar
                }
                into g
                select new
                {
                    UserId = g.Key.Id,
                    UserName = g.Key.UserName,
                    Avatar = g.Key.Avatar,
                    RateStar = g.Key.RateStar,

                    BidCount = g.Count(),
                    AuctionCount = g
                        .Select(x => x.AuctionId)
                        .Distinct()
                        .Count()
                };

            var result = await query
                .OrderByDescending(x => x.AuctionCount) 
                .ThenByDescending(x => x.BidCount)      
                .Take(10)
                .Select(x => new TopAuctionParticipantTableResponse
                {
                    User = new UserShortResponse
                    {
                        Id = x.UserId,
                        UserName = x.UserName,
                        Avatar = x.Avatar,
                        RateStar = x.RateStar
                    },
                    AuctionCount = x.AuctionCount,
                    BidCount = x.BidCount
                })
                .ToListAsync();

            return result;
        }

    }
}
