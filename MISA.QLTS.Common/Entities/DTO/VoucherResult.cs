using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    /// <summary>
    /// class phục vụ thêm chứng từ
    /// </summary>
    public class VoucherResult
    {
        /// <summary>
        /// số bản ghi ảnh hưởng sau khi thêm
        /// </summary>
        public int numberOfAffectRows { get; set; }

        /// <summary>
        /// id chứng từ
        /// </summary>
        public Guid voucher_id { get; set; }
    }
}
