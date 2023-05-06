using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BudgetPlaceBL;
using MISA.QLTS.BL.DepartmentBL;
using MISA.QLTS.Common.Entities;

namespace MISA.QLTS.API.Controllers
{

    public class BudgetPlacesController : BasesController<BudgetPlace>
    {
        #region Field

        private IBudgetPlaceBL _budgetPlaceBL;

        #endregion

        #region Constructor

        public BudgetPlacesController(IBudgetPlaceBL budgetPlaceBL) : base(budgetPlaceBL)
        {
            _budgetPlaceBL = budgetPlaceBL;
        }

        #endregion

        #region Method
        #endregion
    }
}
