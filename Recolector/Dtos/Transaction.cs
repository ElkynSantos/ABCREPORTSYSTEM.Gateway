using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recolector.Dtos
{
    public class Transaction
    {

        public Guid TransactionId { get; set; }

        public List<string> Errors { get; set; }
     
    }
}
