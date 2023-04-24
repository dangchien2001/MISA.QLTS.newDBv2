using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Constrants;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.BaseDL;
using MISA.QLTS.DL.Datacontext;
using MISA.QLTS.DL.DepartmentDL;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MISA.QLTS.Common.Attributes.QLTSAttributes;

namespace MISA.QLTS.DL.VoucherDL
{
    public class VoucherDL : BaseDL<Voucher>, IVoucherDL
    {
        /// <summary>
        /// hàm thêm mới chứng từ 
        /// </summary>
        /// <param name="record">đối tượng chứng từ</param>
        /// <returns>đối tượng VoucherResult gồm số bản ghi ảnh hưởng và id được tạo sau khi thêm</returns>
        public VoucherResult InsertVoucher(Voucher record)
        {
            string storedProcedureName = string.Format(ProcedureName.Insert, typeof(Voucher).Name);
            var parameters = new DynamicParameters();
            var newRecordID = Guid.NewGuid();
            var properties = typeof(Voucher).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;

                var primaryKeyAttribute = (PrimaryKeyAttribute?)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));

                if (primaryKeyAttribute != null)
                {
                    propertyValue = newRecordID;
                }
                else
                {
                    propertyValue = property.GetValue(record, null);
                }
                parameters.Add($"p_{propertyName}", propertyValue);
            }

            int numberOfAffectedRows;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();
                numberOfAffectedRows = mySqlConnection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            }

            return new VoucherResult
            {
                numberOfAffectRows = numberOfAffectedRows,
                voucher_id = newRecordID
            };
        } 

        /// <summary>
        /// Lấy chứng từ phân trang và tìm kiếm
        /// </summary>
        /// <param name="voucherFilter">Từ khóa tìm kiếm</param>
        /// <param name="pageSize">kích thước trang (số bản ghi trên 1 trang)</param>
        /// <param name="pageNumber">số trang (trang hiện tại)</param>
        /// <returns>Danh sách gồm các bản ghi chứng từ sau khi lọc và phân trang</returns>
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
                MoreInfo = new decimal[] { totalCost },
                TotalRecord = totalAllRecords,
                Data = voucherFilters
            };
        }

        /// <summary>
        /// Thêm các mã tài sản vào bảng chi tiết chứng từ
        /// </summary>
        /// <param name="assetIds">danh sách id mã tài sản</param>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        public int InserVoucherDetail(List<Guid> assetIds, Guid voucherId)
        {
            // khai báo biến
            int numberOfAffectedRows = 0;

            if(assetIds.Count == 0)
            {
                return (int)Common.Enums.StatusData.DataNull;
            }

            // Chuẩn bị câu lệnh insert
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
            // Kết nối tới CSDL
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();

                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    numberOfAffectedRows = mySqlConnection.Execute(insertVoucherDetail, transaction: transaction);

                    if (numberOfAffectedRows == assetIds.Count)
                    {
                        transaction.Commit();
                        return numberOfAffectedRows;
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                mySqlConnection.Close();
            }

            return numberOfAffectedRows;
        }

        /// <summary>
        /// Cập nhật active thành true cho tài sản trong danh sách được chọn
        /// </summary>
        /// <param name="assetIds">danh sách tài sản</param>
        /// <returns>Số bản ghi ảnh hưởng</returns>
        public int UpdateAsset(List<Guid> assetIds)
        {
            // khởi tạo câu lệnh sql
            var result = 0;
            if (assetIds.Count == 0)
            {
                return (int)Common.Enums.StatusData.DataNull;
            }
            var stringFormat = $"('{string.Join("','", assetIds)}')";
            var storedProcedureName = "Proc_Asset_UpdateActive";
            var parameters = new DynamicParameters();
            parameters.Add("p_list", stringFormat);
            parameters.Add("p_active", 1);
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.connectionString))
            {
                mySqlConnection.Open();

                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    result = mySqlConnection.Execute(storedProcedureName, parameters, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                    if (result == assetIds.Count)
                    {
                        transaction.Commit();

                        return result;

                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                mySqlConnection.Close();
            }
            return result;
        }
    }       
}
