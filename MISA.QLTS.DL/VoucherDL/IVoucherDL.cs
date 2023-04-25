using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Constrants;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using MISA.QLTS.DL.BaseDL;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.DL.VoucherDL
{
    public interface IVoucherDL : IBaseDL<Voucher>
    {
        /// <summary>
        /// hàm thêm mới chứng từ 
        /// </summary>
        /// <param name="record">đối tượng chứng từ</param>
        /// <returns>đối tượng VoucherResult gồm số bản ghi ảnh hưởng và id được tạo sau khi thêm</returns>
        VoucherResult InsertVoucher(Voucher record);

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
            [FromQuery] int pageNumber = 1);

        /// <summary>
        /// Thêm các mã tài sản vào bảng chi tiết chứng từ
        /// </summary>
        /// <param name="assetIds">danh sách id mã tài sản</param>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        int InserVoucherDetail(List<Guid> assetIds, Guid voucherId);

        /// <summary>
        /// Cập nhật active thành true cho tài sản trong danh sách được chọn
        /// </summary>
        /// <param name="assetIds">danh sách tài sản</param>
        /// <returns>Số bản ghi ảnh hưởng</returns>
        int UpdateAsset(List<Guid> assetIds);

        /// <summary>
        /// Lấy mã lớn nhất của voucher
        /// </summary>
        /// <returns></returns>
        string GetMaxCode();

        decimal TotalCost();
    }
}
