using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class AssetExport
    {
        /// <summary>
        /// Mã tài sản
        /// </summary>
        public String asset_code { get; set; }

        /// <summary>
        /// Tên tài sản
        /// </summary>
        public String asset_name { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public String asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        public String asset_category_name { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        public String department_code { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        public String department_name { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Nguyên giá
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// Hao mòn lũy kế
        /// </summary>
        public decimal depreciation_value { get; set; }

        /// <summary>
        /// Giá trị còn lại
        /// </summary>
        public decimal residual_value { get; set; }
    }
}
