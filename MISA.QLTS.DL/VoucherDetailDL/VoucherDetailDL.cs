using Dapper;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.DL.BaseDL;
using MISA.QLTS.DL.VoucherDL;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.DL.VoucherDetailDL
{
    public class VoucherDetailDL : BaseDL<VoucherDetail>, IVoucherDetailDL
    {
        /// <summary>
        /// lấy danh sách tài sản (chi tiết chứng từ) dựa trên id chứng từ
        /// </summary>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>danh sách tài sản (chi tiết chứng từ)</returns>
        public List<VoucherDetail> GetVoucherDetailByIdVoucher(Guid voucherId)
        {
            // chuẩn bị tên procedure
            string storedProcedureName = "Proc_VoucherDetail_GetByIdVoucher";

            // chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("p_voucher_id", voucherId);

            // khởi tạo kết nối db
            dynamic record;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                // Thực hiện gọi vào Database để chạy stored procedure
                var result = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                record = result.Read<VoucherDetail>().ToList();
            }
            return record;        
        }
    }
}
