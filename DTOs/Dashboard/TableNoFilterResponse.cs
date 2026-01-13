namespace bidify_be.DTOs.Dashboard
{
    public class TableNoFilterResponse
    {
        public int CountPendingProdut { get; set; }
        public int CountPendingAuction { get; set; }
        public int CountPendingDispute { get; set; }
        public List<PendingProductTableResponse> PendingProductTable { get; set; }
        public List<PendingAuctionTableResponse> PendingAuctionTable { get; set; }
        public List<PendingDisputeTableResponse> PendingDisputeTable { get; set; }
    }
}
