using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class DeleteResult
    {
        public Boolean result { get; set; }

        public int rowAffected { get; set; }
    }
}
