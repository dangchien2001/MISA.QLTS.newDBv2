using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.DepartmentBL;
using MISA.QLTS.BL.VoucherBL;
using MISA.QLTS.BL.VoucherDetailBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Resources;

namespace MISA.QLTS.API.Controllers
{
    public class VouchersController : BasesController<Voucher>
    {
        #region Field

        private IVoucherBL _voucherBL;
        private IVoucherDetailBL _voucherDetailBL;

        #endregion

        #region Constructor

        public VouchersController(IVoucherBL voucherBL) : base(voucherBL)
        {
            _voucherBL = voucherBL;
        }

        #endregion

        #region Method


        [HttpGet("filter")]
        public IActionResult GetVoucherByFilter(
            [FromQuery] string? voucherFilter,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = new PagingVoucherResult();
                result = _voucherBL.GetVoucherByFilter(voucherFilter, pageSize, pageNumber);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }

        [HttpPost("Detail")]
        public IActionResult InsertVoucher(VoucherInsert voucherInsert)
        {
            try
            {
                var voucherResult = _voucherBL.InsertVoucher(voucherInsert.voucher);
                var voucherDetailResult = _voucherBL.InserDetail(voucherInsert.assetIds, voucherResult.voucher_id);
                var updateAssetResult = _voucherBL.UpdateAsset(voucherInsert.assetIds);
                if (voucherResult.IsSuccess && voucherDetailResult.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status201Created, 1);
                }
                else if (!voucherResult.IsSuccess && voucherResult.ErrorCode == Common.Enums.ErrorCode.IsValidData)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = voucherResult.ErrorCode,
                        DevMsg = Resource.SystemError,
                        UserMsg = voucherResult.Message,
                        MoreInfo = voucherResult.Data,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
                else if (!voucherDetailResult.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = voucherDetailResult.ErrorCode,
                        UserMsg = Resource.SystemError,
                        DevMsg = voucherDetailResult.Message
                    });
                }
                else if (!updateAssetResult.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = voucherDetailResult.ErrorCode,
                        UserMsg = Resource.SystemError,
                        DevMsg = voucherDetailResult.Message
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                    {
                        ErrorCode = voucherResult.ErrorCode,
                        DevMsg = Resource.SystemError,
                        MoreInfo = voucherResult.Data,
                        TraceId = HttpContext.TraceIdentifier
                    });
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

        [HttpGet("maxCode")]
        public IActionResult GetMaxCode()
        {
            try
            {
                string result = _voucherBL.GetMaxCode();
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }


        #endregion
    }
}
