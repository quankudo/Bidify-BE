using bidify_be.DTOs.Users;

namespace bidify_be.DTOs.Dashboard
{
    public class TopAuctionParticipantTableResponse
    {
        public UserShortResponse User { get; set; }
        public int AuctionCount { get; set; }
        public int BidCount { get; set; }
    }
}
