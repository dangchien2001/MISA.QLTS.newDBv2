using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.DL.BaseDL
{
    public interface IBaseDL<T>
    {
        /// <summary>
        /// Lấy danh sách record
        /// </summary>
        /// <returns></returns>
        public List<T> GetAllRecord();

        /// <summary>
        /// Tìm bản ghi theo ID
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        List<T> GetRecordById(Guid recordId);

        /// <summary>
        /// Thêm mới một bản ghi
        /// </summary>
        /// <param name="record"></param>
        /// <returns>
        /// 1: Nếu insert thành công
        /// 0: Nếu insert thất bại
        /// </returns>
        int InsertRecord(T record);

        /// <summary>
        /// Sửa thông tin bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="record"></param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 0: Nếu update thất bại
        /// </returns>
        int UpdateRecord(Guid recordId, T record);

        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns>
        /// 1: Nếu xóa thành công
        /// 0: Nếu xóa thất bại
        /// </returns>
        int DeleteRecord(Guid recordId);
    }
}
