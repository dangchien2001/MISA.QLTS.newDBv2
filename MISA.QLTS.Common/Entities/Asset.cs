using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MISA.QLTS.Common.Attributes.QLTSAttributes;

namespace MISA.QLTS.Common.Entities
{
    public class Asset
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? row_index { get; set; }

        /// <summary>
        /// Id tài sản
        /// </summary>
        [PrimaryKey]
        public Guid asset_id { get; set; }

        /// <summary>
        /// Mã tài sản
        /// </summary>
        [Required(ErrorMessage = "Mã tài sản không được bỏ trống")]
        [StringLength(4, ErrorMessage = "Mã tài sản không vượt quá 20 ký tự")]
        public string asset_code { get; set; }

        /// <summary>
        /// Tên tài sản
        /// </summary>
        [Required(ErrorMessage = "Tên tài sản không được bỏ trống")]
        [StringLength(3, ErrorMessage = "Tên tài sản không vượt quá 20 ký tự")]
        public string asset_name { get; set; }

        /// <summary>
        /// id đơn vị
        /// </summary>
        public Guid? organization_id { get; set; }

        /// <summary>
        /// mã đơn vị
        /// </summary>
        public string? organization_code { get; set; }

        /// <summary>
        /// tên đơn vị
        /// </summary>
        public string? organization_name { get; set;}

        /// <summary>
        /// id phòng ban
        /// </summary>
        [ForeignKey("Mã phòng ban không được để trống")]
        public Guid department_id { get; set; }

        /// <summary>
        /// mã phòng ban
        /// </summary>
        public string? department_code { get; set; }

        /// <summary>
        /// tên phòng ban
        /// </summary>
        public string? department_name { get; set; }

        /// <summary>
        /// id loại tài sản
        /// </summary>
        [ForeignKey("Mã loại tài sản không được để trống")]
        public Guid asset_category_id { get; set; }

        /// <summary>
        /// mã loại tài sản
        /// </summary>
        public string? asset_category_code { get; set; }

        /// <summary>
        /// tên loại tài sản
        /// </summary>
        public string? asset_category_name { get; set; }

        /// <summary>
        /// ngày mua
        /// </summary>
        [Required(ErrorMessage = "Ngày mua không được bỏ trống")]
        public DateTime? purchase_date { get; set; }

        /// <summary>
        /// nguyên giá
        /// </summary>
        [Required(ErrorMessage = "Nguyên giá không được bỏ trống")]
        public decimal? cost { get; set; }


        /// <summary>
        /// số lượng
        /// </summary>
        [Required(ErrorMessage = "Số lượng không được bỏ trống")]
        public int? quantity { get; set; }

        /// <summary>
        /// tỉ lệ hao mòn
        /// </summary>
        [Required(ErrorMessage = "Tỉ lệ hao mòn không được bỏ trống")]
        public float? depreciation_rate { get; set; }

        /// <summary>
        /// năm bắt đầu theo dõi sản phẩm trên phần mềm
        /// </summary>
        [Required(ErrorMessage = "Năm bắt đầu theo dõi sản phẩm trên phần mềm không được bỏ trống")]
        public DateTime? tracked_year { get; set; }

        /// <summary>
        /// số năm sử dụng
        /// </summary>
        [Required(ErrorMessage = "Số năm sử dụng không được bỏ trống")]
        public int? life_time { get; set; }

        /// <summary>
        /// năm sử dụng
        /// </summary>
        [Required(ErrorMessage = "Ngày sử dụng không được bỏ trống")]
        public DateTime? production_year { get; set; }

        /// <summary>
        /// sử dụng
        /// </summary>
        public int? active { get; set; }

        /// <summary>
        /// người tạo
        /// </summary>
        public string? created_by { get; set; }

        /// <summary>
        /// ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// người sửa
        /// </summary>
        public string? modified_by { get; set; }

        /// <summary>
        /// ngày sửa
        /// </summary>
        public DateTime? modified_date { get; set; }
    }
}
