using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Resources;
using MISA.QLTS.DL.AssetDL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
