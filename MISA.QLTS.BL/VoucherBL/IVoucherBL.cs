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
        ServiceResult InsertVoucher(VoucherInsert voucherInsert);

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

        /// <summary>
        /// Lấy mã lớn nhất của voucher
        /// </summary>
        /// <returns></returns>
        string GetMaxCode();

        /// <summary>
        /// Cập nhật nguyên giá chứng từ
        /// </summary>
        /// <param name="assetIds">danh sách id tài sản</param>
        /// <param name="voucherId">id chứng từ</param>
        /// <returns>thành công: isSuccess = true, thất bại: isSuccess = false</returns>
        ServiceResult UpdateCost(List<Guid> assetIds, Guid voucherId);

        /// <summary>
        /// Check trùng mã 
        /// </summary>
        /// <param name="voucher"></param>
        /// <returns></returns>
        bool DuplicateCode(Voucher voucher);

        /// <summary>
        /// Lấy voucher theo code
        /// </summary>
        /// <param name="voucherCode">mã chứng từ</param>
        /// <returns>Đối tượng chứng từ</returns>
        VoucherGetByCode GetVoucherByCode(string voucherCode);

        /// <summary>
        /// lấy danh sách tài sản (chi tiết chứng từ) dựa trên mã chứng từ
        /// </summary>
        /// <param name="voucherId">mã chứng từ</param>
        /// <returns>danh sách tài sản (chi tiết chứng từ)</returns>
        List<VoucherDetail> GetVoucherDetailByVoucherCode(string voucherCode);

        /// <summary>
        /// Cập nhật chứng từ
        /// Created by: NDCHIEN(27/4/2023)
        /// </summary>
        /// <param name="voucher">đối tượng chứng từ</param>
        /// <param name="assetCodeActive">mảng chứa mã tài sản dùng để active</param>
        /// <param name="assetCodeNoActive">mảng chứa mã tài sản dùng để hủy active</param>
        /// <returns></returns>
        ServiceResult UpdateVoucher(VoucherUpdate voucherUpdate, Guid voucherId);

        /// <summary>
        /// Xóa chứng từ
        /// </summary>
        /// <param name="voucher_ids">list id chứng từ</param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        int DeleteVoucher(List<string> voucher_codes);
    }
}
