namespace bidify_be.Domain.Contracts
{
    public class OtpModel
    {
        public string Code { get; set; }
        public int ExpireMinutes { get; set; }
    }
}
