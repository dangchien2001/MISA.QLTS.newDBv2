using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class PagingResult
    {
        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// Tổng số lượng tài sản
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// Tổng nguyên giá
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Tổng hao mòn lũy kế
        /// </summary>
        public decimal TotalDepreciationValue { get; set; }

        /// <summary>
        /// Tổng giá trị còn lại
        /// </summary>
        public decimal TotalResidualValue { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Số bản ghi trên 1 trang hiện tại
        /// </summary>
        public int CurrentPageRecords { get; set; }

        /// <summary>
        /// Danh sách dữ liệu
        /// </summary>
        public List<AssetPaging> Data { get; set; }


    }
}
