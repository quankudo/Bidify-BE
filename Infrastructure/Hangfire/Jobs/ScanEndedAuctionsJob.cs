using bidify_be.Infrastructure.UnitOfWork;
using Hangfire;

namespace bidify_be.Infrastructure.Hangfire.Jobs
{
    public class ScanEndedAuctionsJob
    {
        private readonly IUnitOfWork _uow;

        public ScanEndedAuctionsJob(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ExecuteAsync()
        {
            var now = DateTime.UtcNow;

            var auctions = await _uow.AuctionRepository
                .GetEndedButNotProcessedAsync(now);

            foreach (var auction in auctions)
            {
                BackgroundJob.Enqueue<EndAuctionJob>(
                    job => job.EndAuctionAsync(auction.Id)
                );
            }
        }
    }

}
