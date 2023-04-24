﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities.DTO
{
    public class PagingAssetNoActive
    {
        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Số bản ghi trên 1 trang hiện tại
        /// </summary>
        public int CurrentPageRecords { get; set; }

        /// <summary>
        /// Danh sách dữ liệu
        /// </summary>
        public List<AssetExport> Data { get; set; }
    }
}
