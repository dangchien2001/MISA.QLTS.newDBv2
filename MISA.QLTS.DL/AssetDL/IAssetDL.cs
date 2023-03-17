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
        /// API lấy danh sách nhân viên lọc theo trang
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        PagingResult GetAssetsByFilter([FromQuery] string? assetFilter,[FromQuery] int pageSize = 10,[FromQuery] int pageNumber = 1);

        /// <summary>
        /// Lấy mã tài sản mới
        /// </summary>
        /// <returns></returns>
        int GetMaxAssetCode();

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
    }
}
