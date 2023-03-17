using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.AssetCategoryBL;
using MISA.QLTS.BL.DepartmentBL;
using MISA.QLTS.Common.Entities;

namespace MISA.QLTS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetCategoriesController : BasesController<AssetCategory>
    {
        #region Field

        private IAssetCategoryBL _assetCategotyBL;

        #endregion

        #region Constructor

        public AssetCategoriesController(IAssetCategoryBL assetCategotyBL) : base(assetCategotyBL)
        {
            _assetCategotyBL = assetCategotyBL;
        }

        #endregion
    }
}
