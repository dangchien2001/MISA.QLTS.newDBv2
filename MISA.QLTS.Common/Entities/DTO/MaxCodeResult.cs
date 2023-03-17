using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class MaxCodeResult
    {
        /// <summary>
        /// Phần chữ max code
        /// </summary>
        public string TextCode { get; set; }

        /// <summary>
        /// Phần số max code
        /// </summary>
        public int NumCode { get; set; }
    }
}
