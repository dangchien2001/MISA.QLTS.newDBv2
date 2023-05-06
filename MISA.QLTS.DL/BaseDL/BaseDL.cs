using Dapper;
using MISA.QLTS.Common.Constrants;
using MISA.QLTS.Common.Entities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MISA.QLTS.Common.Attributes.QLTSAttributes;

namespace MISA.QLTS.DL.BaseDL
{
    public class BaseDL<T> : IBaseDL<T>
    {


        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns>
        /// 1: Nếu xóa thành công
        /// 0: Nếu xóa thất bại
        /// </returns>
        public int DeleteRecord(Guid recordId)
        {
            //Chuẩn bị tên stored procedure
            string storedProcedureName = string.Format(ProcedureName.Delete, typeof(T).Name);

            // Chuẩn bị tham số đầu vào cho stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("p_" + typeof(T).Name + "_id", recordId);

            // Khởi tạo kết nối tới Database
            int numberOfAffectedRows;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                // Thực hiện gọi vào Database để chạy stored procedure
                numberOfAffectedRows = mySqlConnection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            return numberOfAffectedRows;
        }

        /// <summary>
        /// Lấy danh sách record
        /// </summary>
        /// <returns></returns>
        public List<T> GetAllRecord()
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.Get, typeof(T).Name, "All");

            // Khởi tạo kết nối tới Database
            List<T> listRecords;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                var result = mySqlConnection.QueryMultiple(storedProcedureName, commandType: CommandType.StoredProcedure);
                var totalRecords = result.Read<int>().Single();
                listRecords = result.Read<T>().ToList();
            }
            // Xử lý kết quả trả về
            // Thành công
            return listRecords;
        }

        /// <summary>
        /// Tìm bản ghi theo ID
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public List<T> GetRecordById(Guid recordId)
        {
            //Chuẩn bị tên stored procedure
            string storedProcedureName = string.Format(ProcedureName.Get, typeof(T).Name, "ById");

            // Chuẩn bị tham số đầu vào cho stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("p_" + typeof(T).Name + "_id", recordId);

            // Khởi tạo kết nối tới Database
            dynamic record;
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                var result = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                record = result.Read<T>().ToList();
            }
            return record;
        }

        /// <summary>
        /// Thêm mới một bản ghi
        /// </summary>
        /// <param name="record"></param>
        /// <returns>
        /// 1: Nếu insert thành công
        /// 0: Nếu insert thất bại
        /// </returns>
        public int InsertRecord(T record)
        {
            string storedProcedureName = string.Format(ProcedureName.Insert, typeof(T).Name);
            var parameters = new DynamicParameters();
            var newRecordID = Guid.NewGuid();
            var properties = typeof(T).GetProperties();
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

            return numberOfAffectedRows;
        }

        /// <summary>
        /// Sửa thông tin bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="record"></param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 0: Nếu update thất bại
        /// </returns>
        public int UpdateRecord(Guid recordId, T record)
        {
            string storedProcedureName = string.Format(ProcedureName.Update, typeof(T).Name);
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;

                var primaryKeyAttribute = (PrimaryKeyAttribute?)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));

                if (primaryKeyAttribute != null)
                {
                    propertyValue = recordId;
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

            return numberOfAffectedRows;
        }

        
    }
}
