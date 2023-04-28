using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Constrants;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.AssetDL;
using MISA.QLTS.DL.BaseDL;
using MISA.QLTS.DL.Datacontext;
using MISA.QLTS.DL.DepartmentDL;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
        public int InsertVoucher(VoucherInsert voucherInsert)
        {
            // tạo parameter để insert bảng voucher
            string storedProcedureNameForInsertVoucher = string.Format(ProcedureName.Insert, typeof(Voucher).Name);
            var parametersForvoucher = new DynamicParameters();
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
                    propertyValue = property.GetValue(voucherInsert.voucher, null);
                }
                parametersForvoucher.Add($"p_{propertyName}", propertyValue);
            }

            // nối chuỗi để insert nhiều id tài sản vào bảng chi tiết chứng từ
            string insertVoucherDetail = "INSERT INTO voucher_detail (voucher_detail_id, asset_id, voucher_id) VALUES ";
            foreach (var id in voucherInsert.assetIds)
            {
                var voucherDetailId = Guid.NewGuid();
                if (id == voucherInsert.assetIds[voucherInsert.assetIds.Count - 1])
                {
                    insertVoucherDetail = insertVoucherDetail + $"('{voucherDetailId}', '{id}', '{newRecordID}');";
                }
                else
                {
                    insertVoucherDetail = insertVoucherDetail + $"('{voucherDetailId}', '{id}', '{newRecordID}'),";
                }
            }

            // nối chuỗi để update tài sản sau khi thêm ch tiết chứng từ
            var stringFormat = $"('{string.Join("','", voucherInsert.assetIds)}')";
            var storedProcedureNameForUpdateAsset = "Proc_Asset_UpdateActive";
            var parametersForUpdateAsset = new DynamicParameters();
            parametersForUpdateAsset.Add("p_list", stringFormat);
            parametersForUpdateAsset.Add("p_active", 1);

            // update tổng nguyên giá chứng từ
            // chuẩn bị câu lệnh proc
            var storedProcedureName = "Proc_Voucher_UpdateCost";
            // chuẩn bị parameter
            var parameters = new DynamicParameters();
            parameters.Add("p_voucher_id", newRecordID);
            parameters.Add("p_cost", TotalCost(voucherInsert.assetIds));

            // các biến lưu số bản ghi ảnh hưởng
            int numberOfAffectedRowsVoucher;
            int numberOfAffectedRowsVoucherDetail;
            int numberOfAffectedRowsAsset;
            int numberOfAffectedRowsAfterUpdateVoucherCost;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    // truy vấn csdl để insert voucher
                    numberOfAffectedRowsVoucher = mySqlConnection.Execute(storedProcedureNameForInsertVoucher, parametersForvoucher, transaction: transaction, commandType: CommandType.StoredProcedure);
                    // truy vấn csdl để insert voucher detail
                    numberOfAffectedRowsVoucherDetail = mySqlConnection.Execute(insertVoucherDetail, transaction: transaction);
                    // truy vấn csdl để update asset
                    numberOfAffectedRowsAsset = mySqlConnection.Execute(storedProcedureNameForUpdateAsset, parametersForUpdateAsset, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                    // truy vấn để update lại tổng nguyên giá chứng từ
                    numberOfAffectedRowsAfterUpdateVoucherCost = mySqlConnection.Execute(storedProcedureName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
                    // điều kiện 
                    if (numberOfAffectedRowsVoucherDetail == voucherInsert.assetIds.Count && 
                        numberOfAffectedRowsAsset == voucherInsert.assetIds.Count &&
                        numberOfAffectedRowsVoucher == 1 &&
                        numberOfAffectedRowsAfterUpdateVoucherCost == 1)
                    {
                        transaction.Commit();
                        return 1;
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                mySqlConnection.Close();
            }
            return 0;
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

        /// <summary>
        /// Lấy mã lớn nhất của voucher
        /// </summary>
        /// <returns></returns>
        public string GetMaxCode()
        {
            var result = "";
            // Chuẩn bị tên stored procedure
            string storedProcedureName = "Proc_Voucher_GetFirst";
            // Khởi tạo kết nối tới Database
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                result = mySqlConnection.QueryFirstOrDefault<string>(storedProcedureName, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        /// <summary>
        /// Hàm tính tổng nguyên giá tài sản
        /// Created by: NDCHIEN(25/4/2023)
        /// </summary>
        /// <param name="assetIds">danh sách tài sản</param>
        /// <returns>tổng nguyên giá các tài sản truyền vào</returns>
        public decimal TotalCost(List<Guid> assetIds)
        {
            // khởi tạo câu lệnh sql
            decimal result = 0;
            if (assetIds.Count == 0)
            {
                return result;
            }
            var stringFormat = $"('{string.Join("','", assetIds)}')";
            var storedProcedureName = "Proc_Asset_TotalCost";
            var parameters = new DynamicParameters();
            parameters.Add("p_list", stringFormat);
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                result = mySqlConnection.QueryFirstOrDefault<decimal>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        /// <summary>
        /// Hàm update nguyên giá cho chứng từ
        /// </summary>
        /// <param name="assetIds">danh sách id tài sản</param>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        public int UpdateCost(List<Guid> assetIds, Guid voucherId)
        {
            // chuẩn bị câu lệnh proc
            var storedProcedureName = "Proc_Voucher_UpdateCost";
            // chuẩn bị parameter
            var parameters = new DynamicParameters();
            parameters.Add("p_voucher_id", voucherId);
            parameters.Add("p_cost", TotalCost(assetIds));
            // kết nối mysql
            int numberOfAffectedRows;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();
                numberOfAffectedRows = mySqlConnection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            return numberOfAffectedRows;
        }

        /// <summary>
        /// Lấy chứng từ trong db theo code phục vụ check trùng
        /// Created by: NDCHIEN(26/4/2023)
        /// </summary>
        /// <param name="voucher">đối tượng voucher</param>
        /// <returns>danh sách voucher có mã cần tìm</returns>
        public List<Voucher> DuplicateCode(Voucher voucher)
        {
            var storedProcedureName = "Proc_Voucher_Duplicate_Code";
            var parameters = new DynamicParameters();
            parameters.Add("p_voucher_code", voucher.voucher_code);
            dynamic result;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                var multy = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                result = multy.Read<Voucher>().ToList();
            }
            return result;
        }

        /// <summary>
        /// Lấy voucher theo voucher_code
        /// Created by: NDCHIEN(26/4/2023)
        /// </summary>
        /// <param name="voucherCode">code chứng từ</param>
        /// <returns>Đối tượng chứng từ</returns>
        public VoucherGetByCode GetVoucherByCode(string voucherCode)
        {
            // proc lấy voucher bằng code
            var storedProcedureNameVoucher = "Proc_Voucher_GetByCode";
            var parameters = new DynamicParameters();
            parameters.Add("p_voucher_code", voucherCode);
            Voucher resultVoucher;
            dynamic resultAsset;
            // proc lấy danh sách tài sản theo mã chứng từ
            string storedProcedureNameAsset = "Proc_Asset_GetByVoucherCode";
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                resultVoucher = mySqlConnection.QueryFirstOrDefault<Voucher>(storedProcedureNameVoucher, parameters, commandType: System.Data.CommandType.StoredProcedure);
                var result = mySqlConnection.QueryMultiple(storedProcedureNameAsset, parameters, commandType: CommandType.StoredProcedure);
                resultAsset = result.Read<AssetExport>().ToList();
            }
            return new VoucherGetByCode()
            {
                assets = resultAsset,
                voucher = resultVoucher
            };
        }

        /// <summary>
        /// lấy danh sách tài sản (chi tiết chứng từ) dựa trên mã chứng từ
        /// </summary>
        /// <param name="voucherId">mã chứng từ</param>
        /// <returns>danh sách tài sản (chi tiết chứng từ)</returns>
        public List<VoucherDetail> GetVoucherDetailByVoucherCode(string voucherCode)
        {
            // chuẩn bị tên procedure
            string storedProcedureName = "Proc_Asset_GetByVoucherCode";

            // chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("p_voucher_code", voucherCode);

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

        /// <summary>
        /// Cập nhật chứng từ
        /// Created by: NDCHIEN(27/4/2023)
        /// </summary>
        /// <param name="voucher">đối tượng chứng từ</param>
        /// <param name="assetCodeActive">mảng chứa mã tài sản dùng để active</param>
        /// <param name="assetCodeNoActive">mảng chứa mã tài sản dùng để hủy active</param>
        /// <returns></returns>
        public int UpdateVoucher(VoucherUpdate voucherUpdate, Guid voucherId)
        {
            // update chứng từ
            int numberOfAffectedRows;
            string storedProcedureName = "Proc_Voucher_Update";
            var parameters = new DynamicParameters();
            var properties = typeof(Voucher).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;

                var primaryKeyAttribute = (PrimaryKeyAttribute?)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));

                if (primaryKeyAttribute != null)
                {
                    propertyValue = voucherId;
                }
                else
                {
                    propertyValue = property.GetValue(voucherUpdate.voucher, null);
                }
                parameters.Add($"p_{propertyName}", propertyValue);
            }

            // update tài sản active
            var resultAssetActiveCode = 0;
            var stringFormatAssetActiveCode = $"('{string.Join("','", voucherUpdate.asset_code_active)}')";
            var storedProcedureNameAssetActiveCode = "Proc_Asset_UpdateActive";
            var parametersAssetActiveCode = new DynamicParameters();
            parametersAssetActiveCode.Add("p_list", stringFormatAssetActiveCode);
            parametersAssetActiveCode.Add("p_active", 1);

            //update tài sản bỏ active
            var resultAssetNoActiveCode = 0;
            var stringFormatAssetNoActiveCode = $"('{string.Join("','", voucherUpdate.asset_code_no_active)}')";
            var storedProcedureNameAssetNoActiveCode = "Proc_Asset_UpdateActive";
            var parametersAssetNoActiveCode = new DynamicParameters();
            parametersAssetNoActiveCode.Add("p_list", stringFormatAssetNoActiveCode);
            parametersAssetNoActiveCode.Add("p_active", 0);

            // update danh sách tài sản trong chứng từ
            // xóa tất cả các tài sản hiện có
            var resultDeletedAsset = 0;
            var storeProcedureNameDeletedAsset = "Proc_VoucherDetail_Delete";
            var parametersDeletedAsset = new DynamicParameters();
            parametersDeletedAsset.Add("p_voucher_id", voucherId);



            // thêm danh sách tài sản mới
            // Chuẩn bị câu lệnh insert
            var resultInsertedAsset = 0;
            string insertVoucherDetail = "INSERT INTO voucher_detail (voucher_detail_id, asset_id, voucher_id) VALUES ";
            foreach (var id in voucherUpdate.asset_ids)
            {
                var voucherDetailId = Guid.NewGuid();
                if (id == voucherUpdate.asset_ids[voucherUpdate.asset_ids.Count - 1])
                {
                    insertVoucherDetail = insertVoucherDetail + $"('{voucherDetailId}', '{id}', '{voucherId}');";
                }
                else
                {
                    insertVoucherDetail = insertVoucherDetail + $"('{voucherDetailId}', '{id}', '{voucherId}'),";
                }
            }

            // Update giá cho chứng từ
            var numberOfAffectedRowsAfterUpdateVoucherCost = 0;
            var storedProcedureNameTotalCost = "Proc_Voucher_UpdateCost";
            // chuẩn bị parameter
            var parametersTotalCost = new DynamicParameters();
            parametersTotalCost.Add("p_voucher_id", voucherId);
            parametersTotalCost.Add("p_cost", TotalCost(voucherUpdate.asset_ids));

            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {                    
                    numberOfAffectedRows = mySqlConnection.Execute(storedProcedureName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
                    resultAssetActiveCode = mySqlConnection.Execute(storedProcedureNameAssetActiveCode, parametersAssetActiveCode, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                    resultAssetNoActiveCode = mySqlConnection.Execute(storedProcedureNameAssetNoActiveCode, parametersAssetNoActiveCode, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                    resultDeletedAsset = mySqlConnection.Execute(storeProcedureNameDeletedAsset, parametersDeletedAsset, transaction: transaction, commandType: CommandType.StoredProcedure);
                    resultInsertedAsset = mySqlConnection.Execute(insertVoucherDetail, transaction: transaction);
                    // truy vấn để update lại tổng nguyên giá chứng từ
                    numberOfAffectedRowsAfterUpdateVoucherCost = mySqlConnection.Execute(storedProcedureNameTotalCost, parametersTotalCost, transaction: transaction, commandType: CommandType.StoredProcedure);
                    if (numberOfAffectedRows == 1 &&
                        resultAssetActiveCode == voucherUpdate.asset_code_active.Count &&
                        resultAssetNoActiveCode == voucherUpdate.asset_code_no_active.Count &&
                        resultInsertedAsset == voucherUpdate.asset_ids.Count &&
                        numberOfAffectedRowsAfterUpdateVoucherCost == 1)
                    {
                        transaction.Commit();

                        return 1;
                    }
                    else
                    {
                        transaction.Rollback();

                        return 0;
                    }
                }                    
            }
        }

        /// <summary>
        /// Xóa chứng từ
        /// </summary>
        /// <param name="voucher_ids">list id chứng từ</param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        public int DeleteVoucher(List<string> voucher_codes)
        {
            // lấy danh sách id tài sản theo mảng chứng từ
            var stringFormat1 = $"('{string.Join("','", voucher_codes)}')";
            var selectAsset = "Proc_Voucher_GetListAssetId";
            var parameters1 = new DynamicParameters();
            parameters1.Add("p_list", stringFormat1);
            dynamic assetIds;
            dynamic voucherIds;

            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();
                var result = mySqlConnection.QueryMultiple(selectAsset, parameters1, commandType: CommandType.StoredProcedure);
                assetIds = result.Read<Guid>().ToList();
                voucherIds = result.Read<Guid>().Distinct().ToList();
            }

            // update tài sản thành active = 0
            var resultUpdate = 0;
            var stringFormat2 = $"('{string.Join("','", assetIds)}')";
            var updateAsset = "Proc_Asset_UpdateActive";
            var parameters2t = new DynamicParameters();
            parameters2t.Add("p_list", stringFormat2);
            parameters2t.Add("p_active", 0);

            // xóa chứng từ
            var resultDelete = 0;
            var stringFormat3 = $"('{string.Join("','", voucherIds)}')";
            var deleteVoucher = "Proc_Voucher_DeleteList";
            var parameters3 = new DynamicParameters();
            parameters3.Add("p_list", stringFormat3);

            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    resultUpdate = mySqlConnection.Execute(updateAsset, parameters2t, transaction: transaction, commandType: CommandType.StoredProcedure);
                    resultDelete = mySqlConnection.Execute(deleteVoucher, parameters3, transaction: transaction, commandType: CommandType.StoredProcedure);
                    if (resultUpdate == assetIds.Count &&
                       resultDelete == voucherIds.Count)
                    {
                        transaction.Commit();

                        return 1;
                    }
                    else
                    {
                        transaction.Rollback();

                        return 0;
                    }
                }
            }
        }
    }       
}
