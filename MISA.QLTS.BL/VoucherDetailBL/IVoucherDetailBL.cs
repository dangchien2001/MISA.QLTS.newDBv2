using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.VoucherDetailBL
{
    public interface IVoucherDetailBL : IBaseBL<VoucherDetail>
    {
        List<VoucherDetail> GetVoucherDetailByIdVoucher(Guid voucherId);
    }
}
