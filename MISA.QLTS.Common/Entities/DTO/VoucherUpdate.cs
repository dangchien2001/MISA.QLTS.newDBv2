using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class VoucherUpdate
    {
        public Voucher voucher { get; set; }

        public List<string> asset_code_active { get; set; }

        public List<string> asset_code_no_active { get; set; }

        public List<Guid> asset_ids { get; set; }
    }
}
