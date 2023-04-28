using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class VoucherGetByCode
    {
        /// <summary>
        /// mảng chứa danh sách id tài sản
        /// </summary>
        public List<AssetExport> assets { get; set; }

        /// <summary>
        /// id chứng từ
        /// </summary>
        public Voucher voucher { get; set; }
    }
}
