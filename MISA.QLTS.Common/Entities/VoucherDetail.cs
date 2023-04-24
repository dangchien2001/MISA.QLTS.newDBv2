using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities
{
    public class VoucherDetail
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? row_index { get; set; }

        /// <summary>
        /// Khoá chính
        /// </summary>
        public Guid voucher_detail_id { get; set; }

        /// <summary>
        /// id chứng từ
        /// </summary>
        public Guid voucher_id { get; set; }

        /// <summary>
        /// mã tài sản
        /// </summary>
        public string asset_code { get; set; }

        /// <summary>
        /// tên tài sản
        /// </summary>
        public string asset_name { get; set; }

        /// <summary>
        /// tên bộ phận sử dụng
        /// </summary>
        public string department_name { get; set; }

        /// <summary>
        /// nguyên giá
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// hao mòn năm
        /// </summary>
        public decimal depreciation_value { get; set; }

        /// <summary>
        /// giá trị còn lại
        /// </summary>
        public decimal residual_value { get; set; }
    }
}
