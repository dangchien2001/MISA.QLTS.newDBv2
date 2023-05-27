using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MISA.QLTS.Common.Attributes.QLTSAttributes;

namespace MISA.QLTS.Common.Entities
{
    public class AccountSystem
    {
        [PrimaryKey]
        public Guid AccountId { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string Property { get; set; }

        public string AccountNameEN { get; set; }

        public string Explain { get; set; }

        public string State { get; set; }

        public Guid? AccountParentId { get; set; }

        public int HaveChild { get; set; }
    }
}
