using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.AssetBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Resources;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace MISA.QLTS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : BasesController<Asset>
    {
        #region Field

        private IAssetBL _assetBL;

        #endregion

        #region Constructor

        public AssetsController(IAssetBL assetBL) : base(assetBL)
        {
            _assetBL = assetBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy mã tài sản mới
        /// </summary>
        /// <returns></returns>
        [HttpGet("MaxAssetCode")]
        public IActionResult GetMaxAssetCode()
        {
            try
            {
                string result = _assetBL.GetMaxAssetCode();
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách tài sản lọc theo trang
        /// </summary>
        /// <param name="employeeFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("filter")]
        public IActionResult GetAssetsByFilter(
            [FromQuery] string? assetFilter,
            [FromQuery] string? departmentFilter,
            [FromQuery] string? assetCategoryFilter,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = new PagingResult();
                result = _assetBL.GetAssetsByFilter(assetFilter, departmentFilter, assetCategoryFilter, pageSize, pageNumber);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Hàm xóa nhiều bản ghi
        /// </summary>
        /// <param name="assetIds"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteAssetMore(List<Guid> assetIds)
        {
            try
            {
                DeleteResult numberOfAffectedRows = _assetBL.DeleteAssetMore(assetIds);
                if (numberOfAffectedRows.rowAffected > 0 && numberOfAffectedRows.result == true)
                {
                    return StatusCode(StatusCodes.Status200OK, 1);
                }
                if (numberOfAffectedRows.rowAffected > 0 && numberOfAffectedRows.result == false)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"{numberOfAffectedRows.rowAffected} tài sản được chọn không thể xóa. Vui lòng kiểm tra lại tài sản trước khi thực hiện xóa.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                    {
                        ErrorCode = Common.Enums.ErrorCode.NoData,
                        DevMsg = Resource.SystemError,
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
        /// Lấy tổng số bản ghi, số lượng, nguyên giá, hao mòn lũy kế
        /// </summary>
        /// <returns></returns>
        [HttpGet("totalResults")]
        public IActionResult GetTotalResults()
        {
            try
            {
                TotalResult listRecords;
                listRecords = _assetBL.GetTotalResults();
                // Xử lý kết quả trả về
                // Thành công
                return StatusCode(StatusCodes.Status200OK, listRecords);
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

        /// <summary>
        /// xuất khẩu danh sách tài sản
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export")]
        public IActionResult Export(string? txtSearch, Guid? DepartmentId, Guid? AssetCategoryId)
        {
            try
            {
                var stream = _assetBL.ExportAssets(txtSearch, DepartmentId, AssetCategoryId);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        [HttpPost("NoActive")]
        public IActionResult GetAssetsNoActiveByFilter(
            [FromBody] AssetForSelect? assetCodes,
            [FromQuery] string? assetFilter, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = new PagingAssetNoActive();
                result = _assetBL.GetAssetsNoActiveByFilter(assetCodes, assetFilter, pageSize, pageNumber);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }

        [HttpPut("CostAsset/{assetCode}")]
        public IActionResult UpdateCostAsset(ForUpdateCost forUpdateCost, string assetCode)
        {
            try
            {
                var result = _assetBL.UpdateCostAsset(forUpdateCost, assetCode);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ErrorException(ex);
            }
        }

        [HttpGet("Budget")]
        public IActionResult SelectBudget(string assetCode)
        {
            try
            {
                var result = _assetBL.SelectBudget(assetCode);
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
