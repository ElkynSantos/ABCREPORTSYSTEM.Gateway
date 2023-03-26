namespace RECOLLECTOR2.Dtos
{
    public class Sales
    {
        public string username { get; set; }

        public Guid car_id { get; set; }

        public double price { get; set; }

        public Guid vin { get; set; }

        public int branch_office_id { get; set; }
        public string buyer_first_name { get; set; }

        public string buyer_last_name { get; set; }

        public Guid buyer_id { get; set; }
    }
}
