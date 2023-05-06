using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class ForUpdateCost
    {
        public string json { get; set; }

        public decimal total_cost { get; set; }

        public Guid voucher_id { get; set; }

        public decimal voucher_cost { get; set; }
    }
}
