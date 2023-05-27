using MISA.QLTS.BL.AssetCategoryBL;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.AccountSystemDL;
using MISA.QLTS.DL.AssetCategoryDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.AccountSystemBL
{
    public class AccountSystemBL : BaseBL<AccountSystem>, IAccountSystemBL
    {
        #region Field

        private IAccountSystemDL _accountSystemDL;

        #endregion

        #region Constructor

        public AccountSystemBL(IAccountSystemDL accountSystemDL) : base(accountSystemDL)
        {
            _accountSystemDL = accountSystemDL;
        }



        #endregion

        #region Methods

        public List<AccountSystem> GetAccountsByParentId(Guid IdParent)
        {
            List<AccountSystem> listRecords;
            listRecords = _accountSystemDL.GetAccountsByParentId(IdParent);

            //Kết quả trả về
            return listRecords;
        }
        protected override ServiceResult ValidateCustom(AccountSystem? record)
        {
            ServiceResult lstValiDateCustom = new ServiceResult();
            lstValiDateCustom = null;
            return lstValiDateCustom;
        }

            #endregion
        }
}
