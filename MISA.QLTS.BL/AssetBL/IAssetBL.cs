using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.AssetBL
{
    public interface IAssetBL : IBaseBL<Asset>
    {
        /// <summary>
        /// API lấy danh sách nhân viên lọc theo trang
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        PagingResult GetAssetsByFilter(
            [FromQuery] string? assetFilter,
            [FromQuery] string? departmentFilter, 
            [FromQuery] string? assetCategoryFilter,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1);

        /// <summary>
        /// Lấy mã tài sản mới
        /// </summary>
        /// <returns></returns>
        string GetMaxAssetCode();

        /// <summary>
        /// Hàm xóa nhiều bản ghi
        /// </summary>
        /// <param name="assetIds"></param>
        /// <returns></returns>
        public int DeleteAssetMore(List<Guid> assetIds);

        /// <summary>
        /// Kiểm tra trùng mã tài sản
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        bool DuplicateCode(Asset asset);


        /// <summary>
        /// Lấy tổng số bản ghi, số lượng, nguyên giá, hao mòn lũy kế
        /// </summary>
        /// <returns></returns>
        TotalResult GetTotalResults();

        /// <summary>
        /// Export dữ liệu ra file excel
        /// </summary>
        /// <returns></returns>
        List<AssetExport> ExportToExcel();

        /// <summary>
        /// Xuất khẩu tất cả nhân viên ra excel
        /// </summary>
        /// <returns>URL tải excle xuống </returns>
        public Stream ExportAssets(string? txtSearch, Guid? DepartmentId, Guid? AssetCategoryId);

        /// <summary>
        /// API lấy danh sách tài sản chưa active lọc theo trang
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        PagingAssetNoActive GetAssetsNoActiveByFilter(
            [FromBody] AssetForSelect? assetCodes,
            [FromQuery] string? assetFilter,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1);

        /// <summary>
        /// Update giá json
        /// Created by: NDCHIEN(5/5/2023)
        /// </summary>
        /// <returns></returns>
        ServiceResult UpdateCostAsset(ForUpdateCost forUpdateCost, String assetCode);

        List<Budget> SelectBudget(string assetCode);
    }
}
