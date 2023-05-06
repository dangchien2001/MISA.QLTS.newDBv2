using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.BL.DepartmentBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.DL.BudgetPlaceDL;
using MISA.QLTS.DL.DepartmentDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.BudgetPlaceBL
{
    public class BudgetPlaceBL : BaseBL<BudgetPlace>, IBudgetPlaceBL
    {
        #region Field

        private IBudgetPlaceDL _budgetPlaceDL;

        #endregion

        #region Constructor

        public BudgetPlaceBL(IBudgetPlaceDL budgetPlaceDL) : base(budgetPlaceDL)
        {
            _budgetPlaceDL = budgetPlaceDL;
        }



        #endregion
    }
}
