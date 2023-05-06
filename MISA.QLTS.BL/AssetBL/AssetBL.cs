using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Resources;
using MISA.QLTS.DL.AssetDL;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace MISA.QLTS.BL.AssetBL
{
    public class AssetBL : BaseBL<Asset>, IAssetBL
    {
        #region Field

        private IAssetDL _assetDL;

        #endregion

        #region Constructor

        public AssetBL(IAssetDL assetDL) : base(assetDL)
        {
            _assetDL = assetDL;
        }

        #endregion

        /// <summary>
        /// Xóa nhiều tài sản
        /// </summary>
        /// <param name="assetIds"></param>
        /// <returns></returns>
        public int DeleteAssetMore(List<Guid> assetIds)
        {
            var result = _assetDL.DeleteAssetMore(assetIds);
            return result;
        }

        /// <summary>
        /// Hàm valdate ghi đè ở baseBL
        /// </summary>
        /// <param name="record">Nhân viên muốn kiểm tra validate</param>
        /// <returns>ServiceResult</returns>
        protected override ServiceResult ValidateCustom(Asset? record)
        {
            ServiceResult lstValiDateCustom = new ServiceResult();
            bool result = DuplicateCode(record);           
            if (result)
            {
                lstValiDateCustom.IsSuccess = false;
                lstValiDateCustom.ErrorCode = Common.Enums.ErrorCode.DuplicateCode;
                lstValiDateCustom.Data = Resource.ErrorDuplicate;
            }
            else
            {
                lstValiDateCustom = null;
            }
            return lstValiDateCustom;
        }

        /// <summary>
        /// Hàm valdate ghi đè ở baseBL
        /// </summary>
        /// <param name="record">Nhân viên muốn kiểm tra validate</param>
        /// <returns>ServiceResult</returns>
        protected override ServiceResult ValidateCustom(Asset? record, Guid? recordId)
        {
            ServiceResult lstValiDateCustom = new ServiceResult();
            bool result = DuplicateCode(record, recordId);
            if (result)
            {
                lstValiDateCustom.IsSuccess = false;
                lstValiDateCustom.ErrorCode = Common.Enums.ErrorCode.DuplicateCode;
                lstValiDateCustom.Data = Resource.ErrorDuplicate;
            }
            else
            {
                lstValiDateCustom = null;
            }
            return lstValiDateCustom;
        }

        /// <summary>
        /// Kiểm tra trùng mã tài sản
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool DuplicateCode(Asset asset)
        {
            List<Asset> result = new List<Asset>();
            result = _assetDL.DuplicateCode(asset);
            bool isCheck = false;
            foreach (Asset asset_result in result)
            {
                if (asset_result.asset_code == asset.asset_code && asset_result.asset_id != asset.asset_id)
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

     

        /// <summary>
        /// Kiểm tra trùng mã tài sản
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool DuplicateCode(Asset? asset, Guid? assetId)
        {
            List<Asset> result = new List<Asset>();
            result = _assetDL.DuplicateCode(asset);
            bool isCheck = false;
            foreach (Asset asset_result in result)
            {
                if (asset_result.asset_code == asset.asset_code && asset_result.asset_id != assetId)
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

        /// <summary>
        /// Lấy danh sách tài sản lọc theo trang
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public PagingResult GetAssetsByFilter([FromQuery] string? assetFilter, [FromQuery] string? departmentFilter, [FromQuery] string? assetCategoryFilter, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            var result = _assetDL.GetAssetsByFilter(assetFilter, departmentFilter, assetCategoryFilter, pageSize, pageNumber);
            return result;
        }

        /// <summary>
        /// Lấy tổng số bản ghi, số lượng, nguyên giá, hao mòn lũy kế
        /// </summary>
        /// <returns></returns>
        public TotalResult GetTotalResults()
        {
            var result = _assetDL.GetTotalResults();
            return result;
        }

        /// <summary>
        /// Lấy mã tài sản mới
        /// </summary>
        /// <returns></returns>
        public string GetMaxAssetCode()
        {
            string numCode = _assetDL.GetMaxAssetCode();
            
            return numCode;
        }

        public List<AssetExport> ExportToExcel()
        {
            var result = _assetDL.ExportToExcel();
            return result;
        }

        /// <summary>
        /// Xuất khẩu tất cả nhân viên ra excel
        /// </summary>
        /// <returns>URL tải excle xuống </returns>
        public Stream ExportAssets(string? txtSearch, Guid? DepartmentId, Guid? AssetCategoryId)
        {
            return GenerateExcelFileAsync((List<AssetExport>) _assetDL.Getpage(txtSearch, DepartmentId, AssetCategoryId));
        }

        public Stream GenerateExcelFileAsync(List<AssetExport> asset)
        {
            string excelName = $"DanhSanhTaiSan-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

            MemoryStream stream = new MemoryStream();
            var headers = new List<string>()
            {
                "STT",
                "Mã tài sản",
                "Tên tài sản",
                "Mã loại tài sản",
                "Tên loại tài sản",
                "Mã bộ phận sử dụng",
                "Tên bộ phận sử dụng",
                "Số lượng",
                "Nguyên giá",
                "HM/KH lũy kế",
                "Giá trị còn lại",

            };
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("My Sheet");
                for (int i = 1; i <= headers.Count; i++)
                {
                    sheet.Column(i).AutoFit();
                    sheet.Cells[3, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[3, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    sheet.Cells[3, i].Style.Fill.BackgroundColor.SetColor(color: System.Drawing.Color.FromArgb(25512780));
                    sheet.Cells[3, i].Value = headers[i - 1];
                }

                int row = 4;
                int index = 1;
                foreach (var i in asset)
                {
                    sheet.Cells[row, 1].Value = index;
                    sheet.Cells[row, 2].Value = i.asset_code;
                    sheet.Cells[row, 3].Value = i.asset_name;
                    sheet.Cells[row, 4].Value = i.asset_category_code;
                    sheet.Cells[row, 5].Value = i.asset_category_name;
                    sheet.Cells[row, 6].Value = i.department_code ?? "";
                    sheet.Cells[row, 7].Value = i.department_name ?? "";
                    sheet.Cells[row, 8].Value = i.quantity.ToString();
                    sheet.Cells[row, 9].Value = FomatMoney(Decimal.ToDouble(Math.Round(i.cost)));
                    sheet.Cells[row, 10].Value = FomatMoney(Decimal.ToDouble(i.depreciation_value));
                    sheet.Cells[row, 11].Value = FomatMoney(Decimal.ToDouble(i.residual_value));

                    index++;
                    row++;
                }

                row = 4;
                // Style
                foreach (var i in asset)
                {

                    sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    sheet.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    sheet.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    sheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    row++;
                }
                for (var i = 1; i <= headers.Count; i++)
                {
                    sheet.Column(i).AutoFit();
                }
                package.Save();
            }


            return stream;
        }

        /// <summary>
        /// hàm định dạng tiền 
        /// </summary>
        /// <param name="dataFormat"> tiền cần format </param>
        /// <returns>dd/mm/dd</returns>
        private string FomatMoney(double dataFormat)
        {
            var result = "";
            var a = dataFormat.ToString().Length / 3;
            var b = dataFormat.ToString().Length % 3;
            var index = 1;
            while (a != 0)
            {
                result +=
                dataFormat.ToString()[dataFormat.ToString().Length - index].ToString() +
                dataFormat.ToString()[dataFormat.ToString().Length - index - 1].ToString() +
                dataFormat.ToString()[dataFormat.ToString().Length - index - 2].ToString() +
                  ".";
                index += 3;
                a--;
            }

            if (b == 0)
            {
                result = Reverse(result.Substring(0, result.Length - 1));
            }
            else
            {
                while (b != 0)
                {
                    result += dataFormat.ToString()[b - 1];
                    b--;
                }
                result = Reverse(result);
            }
            return result;

        }

        /// <summary>
        /// hàm đảo ngược chuỗi 
        /// </summary>
        /// <param name="txt">chuỗi đầu vào </param>
        /// <returns></returns>
        string Reverse(string txt)
        {
            var strRev = "";

            for (var i = txt.Length - 1; i >= 0; i--)
            {
                strRev += txt[i];
            }

            return strRev;
        }

        /// <summary>
        /// API lấy danh sách tài sản chưa active lọc theo trang
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public PagingAssetNoActive GetAssetsNoActiveByFilter(
            [FromBody] AssetForSelect? assetCodes,
            [FromQuery] string? assetFilter, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1)
        {
            var result = _assetDL.GetAssetsNoActiveByFilter(assetCodes, assetFilter, pageSize, pageNumber);
            return result;
        }

        public List<Budget> SelectBudget(string assetCode)
        {
            var result = _assetDL.SelectBudget(assetCode);
            return result;
        }

        /// <summary>
        /// Update giá json
        /// Created by: NDCHIEN(5/5/2023)
        /// </summary>
        /// <returns></returns>
        public ServiceResult UpdateCostAsset(ForUpdateCost forUpdateCost, String assetCode)
        {
            var numberOfAffectedResult = _assetDL.UpdateCostAsset(forUpdateCost, assetCode);
            if (numberOfAffectedResult.result > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Data = numberOfAffectedResult.asset
                };
            }
            else
            {
                //Kết quả trả về
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Lỗi khi gọi vào DL",
                };
            }
        }
    }
}
