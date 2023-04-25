using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.BL.DepartmentBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Enums;
using MISA.QLTS.Common.Resources;
using MISA.QLTS.DL.DepartmentDL;
using MISA.QLTS.DL.VoucherDL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.VoucherBL
{
    public class VoucherBL : BaseBL<Voucher>, IVoucherBL
    {
        #region Field

        private IVoucherDL _voucherDL;

        #endregion

        #region Constructor

        public VoucherBL(IVoucherDL voucherDL) : base(voucherDL)
        {
            _voucherDL = voucherDL;
        }

        /// <summary>
        /// lấy danh sách chứng từ lọc và phân trang
        /// </summary>
        /// <param name="voucherFilter">từ khóa tìm kiếm (mã chứng từ)</param>
        /// <param name="pageSize">kích thước trang (số bản ghi trên 1 trang)</param>
        /// <param name="pageNumber">số trang (trang hiện tại)</param>
        /// <returns>danh sách chứng từ đã qua lọc và phân trang</returns>
        public PagingVoucherResult GetVoucherByFilter([FromQuery] string? voucherFilter, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            var result = _voucherDL.GetVoucherByFilter(voucherFilter, pageSize, pageNumber);
            return result;
        }

        protected override ServiceResult ValidateCustom(Voucher? record)
        {
            ServiceResult lstValiDateCustom = new ServiceResult();
            lstValiDateCustom = null;
            return lstValiDateCustom;
        }

        /// <summary>
        /// hàm thêm mới chứng từ 
        /// </summary>
        /// <param name="record">đối tượng chứng từ</param>
        /// <returns>đối tượng VoucherResult gồm số bản ghi ảnh hưởng và id được tạo sau khi thêm</returns>
        public ServiceResultForVoucher InsertVoucher(Voucher record)
        {
            // Validate
            ServiceResult validateResults = new ServiceResult();
            validateResults = ValidateData(record);
            ServiceResultForVoucher validateResultsForVoucher = new ServiceResultForVoucher
            {
                IsSuccess = validateResults.IsSuccess,
                ErrorCode = validateResults.ErrorCode,
                Data = validateResults.Data,
                Message = validateResults.Message,
                voucher_id = record.voucher_id
            };
            if (!validateResultsForVoucher.IsSuccess && validateResultsForVoucher.ErrorCode == ErrorCode.IsValidData)
            {
                return validateResultsForVoucher;
            }
            else if (!validateResultsForVoucher.IsSuccess && validateResultsForVoucher.ErrorCode == ErrorCode.DuplicateCode)
            {
                return validateResultsForVoucher;
            }

            // Thành công -- gọi vào DL để chạy store

            var VoucherResult = _voucherDL.InsertVoucher(record);

            // Xử lý kết quả trả về

            if (VoucherResult.numberOfAffectRows > 0)
            {
                return new ServiceResultForVoucher
                {
                    IsSuccess = true,
                    voucher_id = VoucherResult.voucher_id
                };
            }
            else
            {
                return new ServiceResultForVoucher
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Lỗi thêm mới khi gọi vào DL",
                };
            }
        }

        /// <summary>
        /// thêm mới bảng chi tiết chứng từ
        /// </summary>
        /// <param name="assetIds">danh sách id tài sản</param>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>đối tượng ServiceResult chứa các trạng thái thành công, thất bại, trùng mã, ...</returns>
        public ServiceResult InserDetail(List<Guid> assetIds, Guid voucherId)
        {
            var numberOfAddertedRows = _voucherDL.InserVoucherDetail(assetIds, voucherId);

            if (numberOfAddertedRows > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else if (numberOfAddertedRows == (int)Common.Enums.StatusData.DataNull)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Phải thêm ít nhất 1 tài sản",
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Lỗi thêm mới khi gọi vào DL",
                };
            }
        }

        /// <summary>
        /// Cập nhật active thành true cho tài sản trong danh sách được chọn
        /// </summary>
        /// <param name="assetIds">danh sách tài sản</param>
        /// <returns>Số bản ghi ảnh hưởng</returns>
        public ServiceResult UpdateAsset(List<Guid> assetIds)
        {
            var numberOfAddertedRows = _voucherDL.UpdateAsset(assetIds);
            if (numberOfAddertedRows > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else if (numberOfAddertedRows == (int)Common.Enums.StatusData.DataNull)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Tài sản update thất bại",
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Lỗi thêm mới khi gọi vào DL",
                };
            }
        }

        /// <summary>
        /// Lấy mã lớn nhất của voucher
        /// </summary>
        /// <returns></returns>
        public string GetMaxCode()
        {
            // gỡ chữ và số riêng
            var text = "";
            var number = "";
            var result = _voucherDL.GetMaxCode();
            var regex = new Regex(@"(\D)");
            for(int i = result.Length - 1; i > 0; i--)
            {
                if (regex.IsMatch(result[i].ToString()))
                {
                    text = result.Substring(0, i + 1);
                    break;
                }
                number = result[i].ToString() + number;
            }

            // logic lấy mã lớn nhất bằng cách cộng 1 với số vừa tách và cho vào chuỗi có số 0 ở đầu
            int newNumber = Int32.Parse(number) + 1;
            string stringZero = "";
            for(int i = 0;i<(result.Length - text.Length);i++)
            {
                stringZero = string.Concat(stringZero, 0);
            }
            return string.Concat(text, newNumber.ToString(stringZero));
        }

        #endregion
    }
}
