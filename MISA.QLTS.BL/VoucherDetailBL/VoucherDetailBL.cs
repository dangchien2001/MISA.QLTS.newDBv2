using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.BL.VoucherBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.VoucherDetailDL;
using MISA.QLTS.DL.VoucherDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.VoucherDetailBL
{
    public class VoucherDetailBL : BaseBL<VoucherDetail>, IVoucherDetailBL
    {
        #region Field

        private IVoucherDetailDL _voucherDetailDL;

        #endregion

        #region Constructor

        public VoucherDetailBL(IVoucherDetailDL voucherDetailDL) : base(voucherDetailDL)
        {
            _voucherDetailDL = voucherDetailDL;
        }

        #endregion

        #region method

        public List<VoucherDetail> GetVoucherDetailByIdVoucher(Guid voucherId)
        {
            dynamic record;
            record = _voucherDetailDL.GetVoucherDetailByIdVoucher(voucherId);
            return record;
        }

        #endregion
    }
}
