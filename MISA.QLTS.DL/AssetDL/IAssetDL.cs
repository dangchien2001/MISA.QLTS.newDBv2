using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.DL.AssetDL
{
    public interface IAssetDL : IBaseDL<Asset>
    {
        /// <summary>
        /// API lấy danh sách tài sản lọc theo trang
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
        List<Asset> DuplicateCode(Asset asset);

        /// <summary>
        /// Lấy tổng số bản ghi, số lượng, nguyên giá, hao mòn lũy kế
        /// </summary>
        /// <returns></returns>
        TotalResult GetTotalResults();

        /// <summary>
        /// lấy thông tin dành cho phân trang 
        /// </summary>
        /// <param name="assetFilter">từ khóa tìm kiếm</param>
        /// <param name="departmentFilter">mã phòng ban tìm kiếm</param>
        /// <param name="assetCategoryFilter">Mã loại tài sản tìm kiếm</param>
        /// <returns></returns>
        public IEnumerable<AssetExport> Getpage(string? txtSearch, Guid? DepartmentId, Guid? AssetCategoryId);

        /// <summary>
        /// Export dữ liệu ra file excel
        /// </summary>
        /// <returns></returns>
        List<AssetExport> ExportToExcel();


    }
}
