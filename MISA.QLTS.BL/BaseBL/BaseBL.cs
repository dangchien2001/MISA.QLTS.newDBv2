using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.Common.Enums;
using MISA.QLTS.Common.Resources;
using MISA.QLTS.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.BaseBL
{
    public class BaseBL<T> : IBaseBL<T>
    {
        #region Field

        private IBaseDL<T> _baseDL;

        #endregion

        #region Constructor

        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }

        #endregion

        #region Method

        /// <summary>
        /// Xóa một bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int DeleteRecord([FromRoute] Guid recordId)
        {
            var result = _baseDL.DeleteRecord(recordId);
            return result;
        }

        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <returns></returns>
        public List<T> GetAllRecord()
        {
            List<T> listRecords;
            listRecords = _baseDL.GetAllRecord();

            //Kết quả trả về
            return listRecords;
        }

        /// <summary>
        /// Tìm bản ghi theo id
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<T> GetRecordById(Guid recordId)
        {
            dynamic record;
            record = _baseDL.GetRecordById(recordId);
            return record;
        }

        /// <summary>
        /// Hàm thêm mới bản ghi
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ServiceResult InsertRecord(T record)
        {
            // Validate
            ServiceResult validateResults = new ServiceResult();
            validateResults = ValidateData(record);
            if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.IsValidData)
            {
                return validateResults;
            }
            else if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.DuplicateCode)
            {
                return validateResults;
            }

            // Thành công -- gọi vào DL để chạy store

            var numberOfAffectedRows = _baseDL.InsertRecord(record);

            // Xử lý kết quả trả về

            if (numberOfAffectedRows > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Lỗi thêm mới khi gọi vào DL",
                };
            }
        }

        /// <summary>
        /// Hàm validate required
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual ServiceResult ValidateData(T? record)
        {
            //Lấy toàn bộ property của class asset
            var properties = typeof(T).GetProperties();

            //Kiểm tra xem property nào có attribute là required
            ServiceResult lstValidate = new ServiceResult();

            lstValidate.IsSuccess = true;
            List<string> lstEmpty = new List<string>();
            lstEmpty.Clear();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(record);
                var requiredAttribute = (RequiredAttribute)property.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
                if (requiredAttribute != null && string.IsNullOrEmpty(propertyValue.ToString()))
                {
                    lstEmpty.Add(requiredAttribute.ErrorMessage);
                }

                if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString()))
                {
                    DateTime datetime;
                    if (DateTime.TryParse(propertyValue.ToString(), out datetime))
                    {
                        DateTime time = DateTime.Parse(propertyValue.ToString());
                        if (time > DateTime.Now)
                        {
                            lstEmpty.Add(Resource.FormatDate);
                        }
                    }
                }
            }
            var result = ValidateCustom(record);
            if (result != null)
            {
                lstValidate.IsSuccess = false;
                lstValidate.ErrorCode = result.ErrorCode;
                lstValidate.Data = result.Data;
            }
            if (lstEmpty.Count > 0)
            {
                lstValidate.IsSuccess = false;
                lstValidate.ErrorCode = ErrorCode.IsValidData;
                lstValidate.Data = lstEmpty;
            }
            return lstValidate;
        }

        protected virtual ServiceResult ValidateCustom(T? record)
        {
            return new ServiceResult();
        }

        public ServiceResult UpdateRecord(Guid recordId, T record)
        {
            ServiceResult validateResults = new ServiceResult();
            validateResults = ValidateData(record);
            // Thất bại -- return lỗi

            if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.IsValidData)
            {
                return validateResults;
            }
            else if (!validateResults.IsSuccess && validateResults.ErrorCode == ErrorCode.DuplicateCode)
            {
                return validateResults;
            };
            var numberOfAffectedResult = _baseDL.UpdateRecord(recordId, record);
            if (numberOfAffectedResult > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else
            {
                //Kết quả trả về
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = Common.Enums.ErrorCode.NoData,
                    Message = "Lỗi khi gọi vào DL",
                };
            }
        } 

        #endregion
    }
}
