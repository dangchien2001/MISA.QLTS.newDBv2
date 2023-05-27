using MISA.QLTS.Common.Entities;
using MISA.QLTS.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.DL.AccountSystemDL
{
    public interface IAccountSystemDL : IBaseDL<AccountSystem>
    {
        /// <summary>
        /// Lấy danh sách tài khoản theo khóa cha
        /// </summary>
        /// <param name="IdParent">khóa cha</param>
        /// <returns>danh sách tài sản theo khóa cha</returns>
        List<AccountSystem> GetAccountsByParentId(Guid IdParent);
    }
}
