using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.AccountSystemBL
{
    public interface IAccountSystemBL : IBaseBL<AccountSystem>
    {
        List<AccountSystem> GetAccountsByParentId(Guid IdParent);
    }
}
