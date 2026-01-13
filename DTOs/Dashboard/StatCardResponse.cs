namespace bidify_be.DTOs.Dashboard
{
    public class ListStatCardResponse
    {
        public StatCardResponse UserCard { get; set; }
        public StatCardResponse AuctionActiveCard { get; set; }
        public StatCardResponse AuctionClosedCard { get; set; }
        public StatCardResponse DisputeCard { get; set; }
    }
    public class StatCardResponse
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public List<StatCardData> Data { get; set; }
        public double PercentageChange { get; set; } = 0;
        public bool IsUp { get; set; } = true;

    }

    public class StatCardData
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }
}
