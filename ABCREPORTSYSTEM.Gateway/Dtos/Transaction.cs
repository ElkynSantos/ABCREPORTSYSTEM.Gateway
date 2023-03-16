namespace ABCREPORTSYSTEM.Gateway.Dtos
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }

        public IEnumerable<Error>? Errors { get; set; }
    }
}
