using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorService.Dtos
{
   public class DATAS
    {

        public Guid TransactionId { get; set; }

        public List<string> Errors { get; set; }

        public double threads { get; set; }
    }
}
