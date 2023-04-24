using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.BL.VoucherBL
{
    public interface IVoucherBL : IBaseBL<Voucher>
    {
        /// <summary>
        /// hàm thêm mới chứng từ 
        /// </summary>
        /// <param name="record">đối tượng chứng từ</param>
        /// <returns>đối tượng VoucherResult gồm số bản ghi ảnh hưởng và id được tạo sau khi thêm</returns>
        ServiceResultForVoucher InsertVoucher(Voucher record);

        /// <summary>
        /// lấy danh sách chứng từ lọc và phân trang
        /// </summary>
        /// <param name="voucherFilter">từ khóa tìm kiếm (mã chứng từ)</param>
        /// <param name="pageSize">kích thước trang (số bản ghi trên 1 trang)</param>
        /// <param name="pageNumber">số trang (trang hiện tại)</param>
        /// <returns>danh sách chứng từ đã qua lọc và phân trang</returns>
        PagingVoucherResult GetVoucherByFilter(
            [FromQuery] string? voucherFilter,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1);

        /// <summary>
        /// thêm mới bảng chi tiết chứng từ
        /// </summary>
        /// <param name="assetIds">danh sách id tài sản</param>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>đối tượng ServiceResult chứa các trạng thái thành công, thất bại, trùng mã, ...</returns>
        ServiceResult InserDetail(List<Guid> assetIds, Guid voucherId);

        /// <summary>
        /// Cập nhật active thành true cho tài sản trong danh sách được chọn
        /// </summary>
        /// <param name="assetIds">danh sách tài sản</param>
        /// <returns>Số bản ghi ảnh hưởng</returns>
        ServiceResult UpdateAsset(List<Guid> assetIds);
    }
}
