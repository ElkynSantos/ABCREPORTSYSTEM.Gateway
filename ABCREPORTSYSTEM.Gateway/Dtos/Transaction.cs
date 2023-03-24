namespace ABCREPORTSYSTEM.Gateway.Dtos
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }

        public List<string> Errors { get; set; }

        public Transaction()
        {
            TransactionId = Guid.NewGuid();
            Errors = new List<string>();
        }
    }
}
