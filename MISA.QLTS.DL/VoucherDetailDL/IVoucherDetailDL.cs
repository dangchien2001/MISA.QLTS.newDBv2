using MISA.QLTS.Common.Entities;
using MISA.QLTS.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.DL.VoucherDetailDL
{
    public interface IVoucherDetailDL : IBaseDL<VoucherDetail>
    {
        /// <summary>
        /// lấy danh sách tài sản (chi tiết chứng từ) dựa trên id chứng từ
        /// </summary>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>danh sách tài sản (chi tiết chứng từ)</returns>
        List<VoucherDetail> GetVoucherDetailByIdVoucher(Guid voucherId);
    }
}
