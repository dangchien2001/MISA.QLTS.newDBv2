using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Constrants;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.BaseDL;
using MISA.QLTS.DL.DepartmentDL;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.DL.VoucherDL
{
    public class VoucherDL : BaseDL<Voucher>, IVoucherDL
    {
        public PagingVoucherResult GetVoucherByFilter(
            [FromQuery] string? voucherFilter,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1)
        {
            string storedProcedureName = string.Format(ProcedureName.Filter, typeof(Voucher).Name);

            var parameters = new DynamicParameters();
            parameters.Add("p_page_size", pageSize);
            parameters.Add("p_page_number", pageNumber);
            parameters.Add("p_voucher_filter", voucherFilter);

            var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString);
            mySqlConnection.Open();

            var getVoucherFilter = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);

            int totalAllRecords = getVoucherFilter.Read<int>().Single();
            var voucherFilters = getVoucherFilter.Read<Voucher>().ToList();
            decimal totalCost = getVoucherFilter.Read<decimal>().Single();

            double totalPage = Convert.ToDouble(totalAllRecords) / pageSize;
            mySqlConnection.Close();

            return new PagingVoucherResult
            {
                CurrentPage = pageNumber,
                CurrentPageRecords = pageSize,
                TotalPage = Convert.ToInt32(Math.Ceiling(totalPage)),
                TotalCost = totalCost,
                MoreInfo = new decimal[]{ totalCost },
                TotalRecord = totalAllRecords,
                Data = voucherFilters
            };
        }
    }
}
