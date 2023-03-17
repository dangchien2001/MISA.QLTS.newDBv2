using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.DL.AssetCategoryDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.AssetCategoryBL
{
    public class AssetCategoryBL : BaseBL<AssetCategory>, IAssetCategoryBL
    {
        #region Field

        private IAssetCategoryDL _assetCategoryDL;

        #endregion

        #region Constructor

        public AssetCategoryBL(IAssetCategoryDL assetCategoryDL) : base(assetCategoryDL)
        {
            _assetCategoryDL = assetCategoryDL;
        }

        #endregion
    }
}
