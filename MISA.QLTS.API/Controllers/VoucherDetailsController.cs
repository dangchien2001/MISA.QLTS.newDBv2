using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.VoucherBL;
using MISA.QLTS.BL.VoucherDetailBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Resources;

namespace MISA.QLTS.API.Controllers
{
    public class VoucherDetailsController : BasesController<VoucherDetail>
    {
        #region Field

        private IVoucherDetailBL _voucherVoucherBL;

        #endregion

        #region Constructor

        public VoucherDetailsController(IVoucherDetailBL voucherDetailBL) : base(voucherDetailBL)
        {
            _voucherVoucherBL = voucherDetailBL;
        }

        #endregion

        #region methods
        [HttpGet("filter")]
        public IActionResult GetVoucherDetailByIdVoucher(Guid voucherId)
        {
            try
            {
                dynamic record;
                record = _voucherVoucherBL.GetVoucherDetailByIdVoucher(voucherId);
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

        /// <summary>
        /// Xử lý ngoại lệ trả về lỗi
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
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
