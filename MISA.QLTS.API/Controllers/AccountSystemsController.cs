using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.AccountSystemBL;
using MISA.QLTS.BL.AssetCategoryBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Resources;

namespace MISA.QLTS.API.Controllers
{
    public class AccountSystemsController : BasesController<AccountSystem>
    {
        #region Field

        private IAccountSystemBL _accountSystemBL;

        #endregion

        #region Constructor

        public AccountSystemsController(IAccountSystemBL accountSystemBL) : base(accountSystemBL)
        {
            _accountSystemBL = accountSystemBL;
        }

        #endregion

        #region Methods

        [HttpGet("Detail/{idParent}")]
        public IActionResult GetAccountsByParentId(Guid idParent)
        {
            try
            {
                dynamic record;
                record = _accountSystemBL.GetAccountsByParentId(idParent);
                if (record == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, record);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }

        private IActionResult ErrorException(Exception ex)
        {
            var errorExp = new
            {
                errorCode = Common.Enums.ErrorCode.Exception,
                devMsg = ex.Message,
                userMsg = Resource.SystemError,
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorExp);
        }

        #endregion
    }
}
