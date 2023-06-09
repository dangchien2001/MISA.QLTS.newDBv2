﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Constrants;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.BaseDL;
using MISA.QLTS.DL.Datacontext;
using MISA.QLTS.DL.VoucherDL;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MISA.QLTS.DL.AssetDL
{

    public class AssetDL : BaseDL<Asset>, IAssetDL
    {
        
        /// <summary>
        /// Hàm xóa nhiều bản ghi
        /// </summary>
        /// <param name="assetIds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int DeleteAssetMore(List<Guid> assetIds)
        {
            // Khởi tạo câu lệnh sql
            var result = 0;
            var sql = $"DELETE FROM asset WHERE asset_id IN @p_list";
            var parameters = new DynamicParameters();
            parameters.Add("p_list", assetIds);
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.connectionString))
            {
                mySqlConnection.Open();

                using (var transaction = mySqlConnection.BeginTransaction())
                {
          
                    
                        result = mySqlConnection.Execute(sql, parameters, transaction: transaction, commandType: CommandType.Text);

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
            }
        }



        /// <summary>
        /// Kiểm tra trùng mã tài sản
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<Asset> DuplicateCode(Asset Asset)
        {
            var storedProcedureName = "Proc_Asset_Duplicate_Code";
            var parameters = new DynamicParameters();
            parameters.Add("p_asset_code", Asset.asset_code);
            dynamic result;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                var multy = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                result = multy.Read<Asset>().ToList();
            }
            return result;
        }

        /// <summary>
        /// API lấy danh sách tài sản lọc theo trang
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public PagingResult GetAssetsByFilter([FromQuery] string? assetFilter, [FromQuery] string? departmentFilter, [FromQuery] string? assetCategoryFilter, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            string storedProcedureName = string.Format(ProcedureName.Filter, typeof(Asset).Name);

            var parameters = new DynamicParameters();
            parameters.Add("p_page_size", pageSize);
            parameters.Add("p_page_number", pageNumber);
            parameters.Add("p_asset_filter", assetFilter);
            parameters.Add("p_department_filter", departmentFilter);
            parameters.Add("p_asset_category_filter", assetCategoryFilter);

            var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString);
            mySqlConnection.Open();

            var getAssetFilter = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);

            int totalAllRecords = getAssetFilter.Read<int>().Single();
            var AssetFilters = getAssetFilter.Read<AssetPaging>().ToList();
            int totalRecords = getAssetFilter.Read<int>().Single();
            int totalQuantity = getAssetFilter.Read<int>().Single();
            decimal totalCost = getAssetFilter.Read<decimal>().Single();
            decimal totalDepreciationValue = getAssetFilter.Read<decimal>().Single();
            decimal totalResidualValue = getAssetFilter.Read<decimal>().Single();

            double totalPage = Convert.ToDouble(totalAllRecords) / pageSize;
            mySqlConnection.Close();
            return new PagingResult
            {
                CurrentPage = pageNumber,
                CurrentPageRecords = pageSize,
                TotalPage = Convert.ToInt32(Math.Ceiling(totalPage)),
                TotalQuantity = totalQuantity,
                TotalCost= totalCost,
                TotalDepreciationValue=totalDepreciationValue,
                TotalResidualValue=totalResidualValue,
                TotalRecord = totalAllRecords,
                Data = AssetFilters
            };
        }

        /// <summary>
        /// Lấy tổng số bản ghi, số lượng, nguyên giá, hao mòn lũy kế
        /// </summary>
        /// <returns></returns>
        public TotalResult GetTotalResults()
        {
            // Chuẩn bị tên stored
            string storedProcedureName = String.Format(ProcedureName.Get, typeof(Asset).Name, "All");
            // Khởi tạo kết nối tới Database
            List<Asset> listRecords;
            var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString);
            mySqlConnection.Open();

            var result = mySqlConnection.QueryMultiple(storedProcedureName, commandType: CommandType.StoredProcedure);
            var totalRecords = result.Read<int>().Single();
            listRecords = result.Read<Asset>().ToList();
            var totalQuantity = result.Read<int>().Single();
            var totalDepreciationValue = result.Read<decimal>().Single();
            var totalPrice = result.Read<decimal>().Single();
            mySqlConnection.Close();
            return new TotalResult
            {
                TotalRecord = totalRecords,
                TotalQuantity = totalQuantity,
                TotalDepreciationValue = totalDepreciationValue,
                TotalPrice = totalPrice
            };
        }

        /// <summary>
        /// Lấy mã nhân viên mới
        /// </summary>
        /// <returns></returns>
        public string GetMaxAssetCode()
        {
            var sqlcmd = $"SELECT asset_code FROM asset ORDER BY created_date DESC LIMIT 1";
            var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString);
            var data = mySqlConnection.QueryFirstOrDefault<string>(sql: sqlcmd);
            data = data.Substring(0, 3).ToString();

            var dynamicParams = new DynamicParameters();
            if (data == null)
            {
                data = "TS";
            }
            data = data + "%";
            dynamicParams.Add("@txt", data);
            var sqlcmd2 = $"SELECT SUBSTR(asset_code, 4) FROM asset WHERE asset_code LIKE @txt ORDER BY CAST(SUBSTR(asset_code, 4) AS SIGNED) DESC LIMIT 1";
            var data2 = mySqlConnection.QueryFirstOrDefault<string>(sql: sqlcmd2, param: dynamicParams);
            return (data + (Int32.Parse(data2) + 1).ToString()).Replace("%","");
        }

        /// <summary>
        /// Lấy danh sách tài sản sau khi lọc phục vụ export excel
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="departmentFilter"></param>
        /// <param name="assetCategoryFilter"></param>
        /// <returns></returns>
        public IEnumerable<AssetExport> Getpage(string? txtSearch, Guid? DepartmentId, Guid? AssetCategoryId)
        {
            if (txtSearch == null)
            {
                txtSearch = "";
            }
            var sqlcmd = $"Proc_Asset_Export";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("p_asset_filter", txtSearch);
            dynamicParams.Add("p_department_filter", DepartmentId);
            dynamicParams.Add("p_asset_category_filter", AssetCategoryId);

            var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString);
            var page = mySqlConnection.Query<AssetExport>(sql: sqlcmd, param: dynamicParams, commandType: System.Data.CommandType.StoredProcedure);
            return page;

        }

        /// <summary>
        /// Export dữ liệu ra file excel
        /// </summary>
        /// <returns></returns>
        public List<AssetExport> ExportToExcel()
        {
            var result = new List<AssetExport>();
            string storedProcedureName = "Proc_Employee_Export";
            var mySqlConnection = new MySqlConnection(DataBaseContext.connectionString);
            var multy = mySqlConnection.QueryMultiple(storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);
            result = multy.Read<AssetExport>().ToList();
            return result;
        }

        /// <summary>
        /// API lấy danh sách tài sản chưa active lọc theo trang
        /// </summary>
        /// <param name="assetFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public PagingAssetNoActive GetAssetsNoActiveByFilter(
            [FromBody] AssetForSelect? assetCodes,
            [FromQuery] string? assetFilter, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1)            
        {
            // Khởi tạo câu lệnh sql
            var result = 0;
            var stringFormatNoActive = $"('{string.Join("','", assetCodes.asset_no_active)}')";
            var stringFormatActive = $"('{string.Join("','", assetCodes.asset_active)}')";

            // chuẩn bị tên procedure
            string storedProcedureName = "Proc_Asset_NonActive_Search_Paging";

            // chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("p_page_size", pageSize);
            parameters.Add("p_page_number", pageNumber);
            parameters.Add("p_asset_filter", assetFilter);
            parameters.Add("p_list", stringFormatNoActive);
            parameters.Add("p_list_active", stringFormatActive);

            // gọi db
            var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString);
            mySqlConnection.Open();

            var getAssetFilter = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);

            int totalAllRecords = getAssetFilter.Read<int>().Single();
            var AssetFilters = getAssetFilter.Read<AssetExport>().ToList();

            double totalPage = Convert.ToDouble(totalAllRecords) / pageSize;
            mySqlConnection.Close();

            return new PagingAssetNoActive
            {
                CurrentPage = pageNumber,
                CurrentPageRecords = pageSize,
                TotalPage = Convert.ToInt32(Math.Ceiling(totalPage)),
                TotalRecord = totalAllRecords,
                Data = AssetFilters
            };
        }

        /// <summary>
        /// Update giá json
        /// Created by: NDCHIEN(5/5/2023)
        /// </summary>
        /// <returns></returns>
        public ResultUpdateAssetCost UpdateCostAsset(ForUpdateCost forUpdateCost, String assetCode)
        {
            // chuẩn bị tên procedure
            string storedProcedureName = "Proc_Asset_UpdateJSON";
            // chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("p_json", forUpdateCost.json);
            parameters.Add("p_total_cost", forUpdateCost.total_cost);
            parameters.Add("p_asset_code", assetCode);

            // chuẩn bị tên procedure
            string storedProcedureName2 = "Proc_Voucher_UpdateCost";
            // chuẩn bị tham số đầu vào
            var parameters2 = new DynamicParameters();
            parameters2.Add("p_voucher_id", forUpdateCost.voucher_id);
            parameters2.Add("p_cost", forUpdateCost.voucher_cost);

            // chuẩn bị tên procedure
            string storedProcedureName3 = "Proc_Asset_GetByAssetCode";
            // chuẩn bị tham số đầu vào
            var parameters3 = new DynamicParameters();
            parameters3.Add("p_asset_code", assetCode);

            int numberOfAffectedRows;
            int numberOfAffectedRows2;
            dynamic asset;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    numberOfAffectedRows = mySqlConnection.Execute(storedProcedureName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
                    if (forUpdateCost.voucher_id == new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        numberOfAffectedRows2 = 1;
                    }
                    else
                    {
                        numberOfAffectedRows2 = mySqlConnection.Execute(storedProcedureName2, parameters2, transaction: transaction, commandType: CommandType.StoredProcedure);
                    }

                    asset = mySqlConnection.QueryFirstOrDefault<AssetExport>(storedProcedureName3, parameters3, transaction: transaction, commandType: CommandType.StoredProcedure);

                    if (numberOfAffectedRows == 1 &&
                        numberOfAffectedRows2 == 1)
                    {
                        transaction.Commit();

                        return new ResultUpdateAssetCost
                        {
                            result = 1,
                            asset = asset
                        };
                    }
                    else
                    {
                        transaction.Rollback();

                        return new ResultUpdateAssetCost
                        {
                            result = 0,
                            asset = null
                        };
                    }
                }
            }
        }

        

        /// <summary>
        /// Lấy dữ liệu nguồn hình thành giá 
        /// </summary>
        /// <param name="assetCode">Mã tài sản</param>
        /// <returns>Tên tài sản, tên phòng ban, thông tin nguồn hình thành giá</returns>
        public List<Budget> SelectBudget(string assetCode)
        {
            // chuẩn bị tên procedure
            string storedProcedureName = "Proc_Asset_GetBudget";
            // chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("p_asset_code", assetCode);

            dynamic record;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                var result = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                record = result.Read<Budget>().ToList();
            }
            return record;
        }
    }
}
