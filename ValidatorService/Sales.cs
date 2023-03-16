using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorService
{
    public class Sales
    {
        public string Username { get; set; }

        public Guid car_id { get; set; }

        public double price { get; set; }

        public Guid Vin { get; set; }

        public int branch_office_id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid BuyerId { get; set; }


    }
}
