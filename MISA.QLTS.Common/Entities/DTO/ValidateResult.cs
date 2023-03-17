using MISA.QLTS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class ValidateResult
    {
        /// <summary>
        /// Kết quả validate: true là không có lỗi, false là có lỗi
        /// </summary>
        public bool IsSuccess { get; set; }

        public ErrorCode? Error { get; set; }
        
        public string MessageError { get; set; }
    }
}
