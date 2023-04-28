using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class AssetForSelect
    {
        public List<string> asset_no_active { get; set; }

        public List<string> asset_active { get; set; }
    }
}
