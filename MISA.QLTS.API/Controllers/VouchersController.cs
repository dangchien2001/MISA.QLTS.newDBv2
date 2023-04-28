using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.DepartmentBL;
using MISA.QLTS.BL.VoucherBL;
using MISA.QLTS.BL.VoucherDetailBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Resources;
using MySqlConnector;

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

        [HttpGet("Code")]
        public IActionResult GetVoucherByCode(string voucherCode) {
            try
            {
                var result = _voucherBL.GetVoucherByCode(voucherCode);
                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, result);
                }
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
                var voucherResult = _voucherBL.InsertVoucher(voucherInsert);

                if (voucherResult.IsSuccess)
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
                else if (!voucherResult.IsSuccess && voucherResult.ErrorCode == Common.Enums.ErrorCode.NoData)
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
                else if (!voucherResult.IsSuccess && voucherResult.ErrorCode == Common.Enums.ErrorCode.DuplicateCode)
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

        [HttpGet("Asset")]
        public IActionResult GetVoucherDetailByVoucherCode(string voucherCode)
        {
            try
            {
                dynamic record;
                record = _voucherBL.GetVoucherDetailByVoucherCode(voucherCode);
                return StatusCode(StatusCodes.Status200OK, record);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }

        [HttpPut("Detail/{voucherId}")]
        public virtual IActionResult UpdateVoucher(VoucherUpdate voucherUpdate, Guid voucherId)
        {
            try
            {
                var result = _voucherBL.UpdateVoucher(voucherUpdate, voucherId);
                if (result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status200OK, 1);
                }
                else if (!result.IsSuccess && result.ErrorCode == Common.Enums.ErrorCode.IsValidData)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = result.ErrorCode,
                        MoreInfo = result.Data,
                        DevMsg = result.Message,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
                else if (!result.IsSuccess && result.ErrorCode == Common.Enums.ErrorCode.DuplicateCode)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new ErrorResult
                    {
                        ErrorCode = result.ErrorCode,
                        MoreInfo = result.Data,
                        DevMsg = result.Message,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
                else if (!result.IsSuccess && result.ErrorCode == Common.Enums.ErrorCode.NoData)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = result.ErrorCode,
                        DevMsg = Resource.SystemError,
                        UserMsg = result.Message,
                        MoreInfo = result.Data,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                    {
                        ErrorCode = result.ErrorCode,
                        DevMsg = Resource.SystemError,
                        MoreInfo = result.Data,
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

        [HttpDelete("List")]
        public IActionResult DeleteVoucher(List<string> voucher_codes)
        {
            try
            {
                int numberOfAffectedRows;
                numberOfAffectedRows = _voucherBL.DeleteVoucher(voucher_codes);
                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, 1);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                    {
                        ErrorCode = Common.Enums.ErrorCode.NoData,
                        DevMsg = "Có lỗi xảy ra vui lòng liên hệ MISA để được trợ giúp",
                    });
                }
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
