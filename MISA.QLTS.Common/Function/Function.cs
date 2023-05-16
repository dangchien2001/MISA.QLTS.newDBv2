using MISA.QLTS.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Function
{
    public class Function
    {
        public static string insertVoucherDetail(List<Guid> assetIds, Guid voucherId)
        {
            string insertVoucherDetail = "INSERT INTO voucher_detail (voucher_detail_id, asset_id, voucher_id) VALUES ";
            foreach (var id in assetIds)
            {
                var voucherDetailId = Guid.NewGuid();
                if (id == assetIds[assetIds.Count - 1])
                {
                    insertVoucherDetail = insertVoucherDetail + $"('{voucherDetailId}', '{id}', '{voucherId}');";
                }
                else
                {
                    insertVoucherDetail = insertVoucherDetail + $"('{voucherDetailId}', '{id}', '{voucherId}'),";
                }
            }
            return insertVoucherDetail;
        }
    }
}
