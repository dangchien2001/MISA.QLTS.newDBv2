using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MISA.QLTS.Common.Attributes.QLTSAttributes;

namespace MISA.QLTS.Common.Entities
{
    public class Voucher
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? row_index { get; set; }

        /// <summary>
        /// id chứng từ
        /// </summary>
        [PrimaryKey]
        public Guid voucher_id { get; set; }

        /// <summary>
        /// mã chứng từ
        /// </summary>
        [Required(ErrorMessage = "Mã chứng từ không được bỏ trống")]
        public string voucher_code { get; set; }

        /// <summary>
        /// ngày chứng từ
        /// </summary>
        [Required(ErrorMessage = "Ngày chứng từ không được bỏ trống")]
        public DateTime voucher_date { get; set; }

        /// <summary>
        /// ngày ghi tăng
        /// </summary>
        [Required(ErrorMessage = "Ngày ghi tăng không được bỏ trống")]
        public DateTime increment_date { get; set; }

        /// <summary>
        /// mô tả
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// nguyên giá
        /// </summary>
        public decimal price { get; set; }

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
