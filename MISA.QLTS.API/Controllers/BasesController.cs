using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Resources;

namespace MISA.QLTS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesController<T> : ControllerBase
    {
        #region Field

        private IBaseBL<T> _baseBL;

        #endregion

        #region Constructor

        public BasesController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy danh sách record
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllRecord()
        {
            try
            {
                List<T> listRecords;
                listRecords = _baseBL.GetAllRecord();
                // Xử lý kết quả trả về
                // Thành công
                if (listRecords != null)
                {
                    return StatusCode(StatusCodes.Status200OK, listRecords);
                }
                // Thất bại
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        ErrorCode = Common.Enums.ErrorCode.NoData,  
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
                userMsg = Resource.SystemError
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorExp);
        }

        /// <summary>
        /// Tìm bản ghi theo Id
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet("{recordId}")]
        public IActionResult GetRecordById(Guid recordId)
        {
            try
            {
                dynamic record;
                record = _baseBL.GetRecordById(recordId);
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
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult InsertRecord(T record)
        {
            try
            {
                var result = _baseBL.InsertRecord(record);
                //Xử lý kết quả trả về
                if (result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status201Created, 1);
                }
                else if (!result.IsSuccess && result.ErrorCode == Common.Enums.ErrorCode.IsValidData)
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

        /// <summary>
        /// Sửa 1 bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        [HttpPut("{recordId}")]
        public virtual IActionResult UpdateRecord(Guid recordId, T record)
        {
            try
            {
                var result = _baseBL.UpdateRecord(recordId, record);
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

        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete("{recordId}")]
        public virtual IActionResult DeleteRecord(Guid recordId)
        {

            try
            {
                int numberOfAffectedRows;
                numberOfAffectedRows = _baseBL.DeleteRecord(recordId);
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
