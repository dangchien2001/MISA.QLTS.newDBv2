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
            List<string> lstDuplicate = new List<string>();
            ServiceResult lstValiDateCustom = new ServiceResult();
            bool result = DuplicateCode(record);
            if (result)
            {
                lstValiDateCustom.IsSuccess = false;
                lstValiDateCustom.ErrorCode = Common.Enums.ErrorCode.DuplicateCode;
                lstDuplicate.Add(Resource.ErrorDuplicate);
                lstValiDateCustom.Data = lstDuplicate;
            }
            else
            {
                lstValiDateCustom = null;
            }
            return lstValiDateCustom;
        }

        /// <summary>
        /// hàm thêm mới chứng từ 
        /// </summary>
        /// <param name="record">đối tượng chứng từ</param>
        /// <returns>đối tượng VoucherResult gồm số bản ghi ảnh hưởng và id được tạo sau khi thêm</returns>
        public ServiceResult InsertVoucher(VoucherInsert voucherInsert)
        {
            // Validate
            ServiceResult validateResults = new ServiceResult();
            validateResults = ValidateData(voucherInsert.voucher);

            if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.IsValidData)
            {
                return validateResults;
            }
            else if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.DuplicateCode)
            {
                return validateResults;
            }
            else if (voucherInsert.assetIds.Count < 1)
            {
                return new ServiceResult()
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = Resource.needAssetError
                };
            }

            // Thành công -- gọi vào DL để chạy store

            var result = _voucherDL.InsertVoucher(voucherInsert);

            // Xử lý kết quả trả về

            if (result > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = Resource.ErrorToDL,
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
                    Message = Resource.needAssetError,
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = Resource.ErrorToDL,
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
                    Message = Resource.updateAssetFall,
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = Resource.ErrorToDL,
                };
            }
        }

        /// <summary>
        /// Lấy mã lớn nhất của voucher
        /// </summary>
        /// <returns></returns>
        public string GetMaxCode()
        {
            // gỡ chữ 
            var text = "";
            // mã gần nhất
            var result = _voucherDL.GetMaxCode(text.Length, text);

            if(result == null)
            {
                return "GT000001";
            }

            var regex = new Regex(@"(\D)");
            // vòng lặp tách chữ và số
            for(int i = result.Length - 1; i > 0; i--)
            {
                if (regex.IsMatch(result[i].ToString()))
                {
                    text = result.Substring(0, i + 1);
                    break;
                }                
            }

            // lấy số lớn nhất dựa trên chữ ở đầu mã
            var result2 = _voucherDL.GetMaxCode(text.Length, text);

            // logic lấy mã lớn nhất bằng cách cộng 1 với số vừa tách và cho vào chuỗi có số 0 ở đầu
            int newNumber = Int32.Parse(result2) + 1;
            string stringZero = "";
            for(int i = 0;i<(result.Length - text.Length);i++)
            {
                stringZero = string.Concat(stringZero, 0);
            }
            return string.Concat(text, newNumber.ToString(stringZero));
        }

        /// <summary>
        /// Cập nhật nguyên giá chứng từ
        /// </summary>
        /// <param name="assetIds">danh sách id tài sản</param>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>thành công: isSuccess = true, thất bại: isSuccess = false</returns>
        public ServiceResult UpdateCost(List<Guid> assetIds, Guid voucherId)
        {
            var numberOfAffectedResult = _voucherDL.UpdateCost(assetIds, voucherId);
            if (numberOfAffectedResult > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else
            {
                //Kết quả trả về
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = Resource.ErrorToDL,
                };
            }
        }

        public bool DuplicateCode(Voucher voucher)
        {
            Guid defaultGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            List<Voucher> result = new List<Voucher>();
            result = _voucherDL.DuplicateCode(voucher);
            bool isCheck = false;
            if(voucher.voucher_id != defaultGuid)
            {
                foreach (Voucher voucher_result in result)
                {
                    if (voucher_result.voucher_code == voucher.voucher_code && voucher_result.voucher_id != voucher.voucher_id)
                    {
                        isCheck = true;
                    }
                    else
                    {
                        isCheck = false;
                    }
                }
                return isCheck;
            }
            else
            {
                foreach (Voucher voucher_result in result)
                {
                    if (voucher_result.voucher_code == voucher.voucher_code)
                    {
                        isCheck = true;
                    }
                    else
                    {
                        isCheck = false;
                    }
                }
                return isCheck;
            }
        }

        /// <summary>
        /// Lấy voucher theo code
        /// </summary>
        /// <param name="voucherCode">mã chứng từ</param>
        /// <returns>Đối tượng chứng từ</returns>
        public VoucherGetByCode GetVoucherByCode(string voucherCode)
        {
            VoucherGetByCode result;
            result = _voucherDL.GetVoucherByCode(voucherCode);
            return result;
        }

        /// <summary>
        /// lấy danh sách tài sản (chi tiết chứng từ) dựa trên mã chứng từ
        /// </summary>
        /// <param name="voucherId">mã chứng từ</param>
        /// <returns>danh sách tài sản (chi tiết chứng từ)</returns>
        public List<VoucherDetail> GetVoucherDetailByVoucherCode(string voucherCode)
        {
            dynamic result;
            result = _voucherDL.GetVoucherDetailByVoucherCode(voucherCode);
            return result;
        }

        /// <summary>
        /// Cập nhật chứng từ
        /// Created by: NDCHIEN(27/4/2023)
        /// </summary>
        /// <param name="voucher">đối tượng chứng từ</param>
        /// <param name="assetCodeActive">mảng chứa mã tài sản dùng để active</param>
        /// <param name="assetCodeNoActive">mảng chứa mã tài sản dùng để hủy active</param>
        /// <returns></returns>
        public ServiceResult UpdateVoucher(VoucherUpdate voucherUpdate, Guid voucherId)
        {
            // validate
            ServiceResult validateResults = new ServiceResult();
            validateResults = ValidateData(voucherUpdate.voucher);

            if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.IsValidData)
            {
                return validateResults;
            }
            else if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.DuplicateCode)
            {
                return validateResults;
            }
            else if (voucherUpdate.asset_code_active.Count < 1 || voucherUpdate.asset_ids.Count < 1)
            {
                return new ServiceResult()
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = Resource.needAssetError
                };
            }
            var numberOfAffectedResult = _voucherDL.UpdateVoucher(voucherUpdate, voucherId);
            if (numberOfAffectedResult > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else
            {
                //Kết quả trả về
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = Resource.ErrorToDL,
                };
            }
        }

        /// <summary>
        /// Xóa chứng từ
        /// </summary>
        /// <param name="voucher_ids">list id chứng từ</param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        public int DeleteVoucher(List<string> voucher_codes)
        {
            int result;
            result = _voucherDL.DeleteVoucher(voucher_codes);
            return result;
        }


        #endregion
    }
}
