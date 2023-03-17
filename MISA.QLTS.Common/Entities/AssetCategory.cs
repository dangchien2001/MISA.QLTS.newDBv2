using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities
{
    public class AssetCategory
    {
        /// <summary>
        /// Id loại tài sản
        /// </summary>
        public Guid asset_category_id { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public string asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        public string asset_category_name { get; set; }

        /// <summary>
        /// id của đơn vị
        /// </summary>
        public Guid? organization_id { get; set; }

        /// <summary>
        /// tỉ lệ hao mòn
        /// </summary>
        public float depreciation_rate { get; set; }

        /// <summary>
        /// số năm sử dụng
        /// </summary>
        public int life_time { get; set; }

        /// <summary>
        /// ghi chú
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? created_by { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        public string? modified_by { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? modified_date { get; set; }
    }
}
