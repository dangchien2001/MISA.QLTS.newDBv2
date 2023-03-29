using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class TotalResult
    {
        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// Tổng số lượng
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// Tổng nguyên giá
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Tổng hao mòn lũy kế
        /// </summary>
        public decimal TotalDepreciationValue { get; set; }
    }
}
