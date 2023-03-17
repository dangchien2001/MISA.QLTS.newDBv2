using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.AssetDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (asset_result.asset_id == asset.asset_id)
                {
                    isCheck = false;
                }
                else
                {
                    isCheck = true;
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
        public PagingResult GetAssetsByFilter([FromQuery] string? assetFilter, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            var result = _assetDL.GetAssetsByFilter(assetFilter, pageSize, pageNumber);
            return result;
        }

        /// <summary>
        /// Lấy mã tài sản mới
        /// </summary>
        /// <returns></returns>
        public string GetMaxAssetCode()
        {
            int numCode = _assetDL.GetMaxAssetCode();
            string textCode = "TS";
            string newEmployeeCode = $"{textCode}-{++numCode}";
            return newEmployeeCode;
        }
    }
}
