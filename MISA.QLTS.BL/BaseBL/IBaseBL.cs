using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.BaseBL
{
    public interface IBaseBL<T>
    {
        #region Method

        /// <summary>
        /// Lấy danh sách record
        /// </summary>
        /// <returns>Danh sách record</returns>
        /// Created by: NVTan (09/02/2023)
        public List<T> GetAllRecord();

        /// <summary>
        /// Tìm bản ghi theo ID
        /// </summary>
        /// <param name="recordId">ID bản ghi cần tìm kiếm</param>
        /// <returns>Bản ghi cần tìm kiếm</returns>
        /// Created by: NVTan (16/01/2023)
        List<T> GetRecordById(Guid recordId);

        /// <summary>
        /// Hàm thêm mới tài sản
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        ServiceResult InsertRecord(T record);

        /// <summary>
        /// Sửa thông tin bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        ServiceResult UpdateRecord(Guid recordId, T record);

        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn xóa</param>
        /// <returns>
        /// 1: Nếu insert thành công
        /// 0: Nếu insert thất bại
        /// </returns>
        /// Created by: NVTan (09/02/2023)
        int DeleteRecord([FromRoute] Guid recordId);

        #endregion
    }
}
