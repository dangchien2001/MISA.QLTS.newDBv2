using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Enums
{
    /// <summary>
    /// Mã lỗi
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        Exception = 0,

        /// <summary>
        /// Không có dữ liệu
        /// </summary>
        NoData = 1,

        /// <summary>
        /// Kết quả validate
        /// </summary>
        IsValidData = 2,

        /// <summary>
        /// Trùng mã
        /// </summary>
        DuplicateCode = 4,
    }
}
