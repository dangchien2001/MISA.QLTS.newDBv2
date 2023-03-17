using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Entities
{
    public class Department
    {
        /// <summary>
        /// Id phòng ban
        /// </summary>
        public Guid department_id { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        public string department_code { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        public string department_name { get;set; }

        /// <summary>
        /// ghi chú
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// có phải là cha không
        /// </summary>
        public int? is_parent { get; set; }


        /// <summary>
        /// id phòng ban cha
        /// </summary>
        public Guid? parent_id { get; set; }

        /// <summary>
        /// id của đơn vị
        /// </summary>
        public Guid? organization_id { get; set; }

        /// <summary>
        /// người tạo
        /// </summary>
        public string? created_by { get; set; }

        /// <summary>
        /// ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// người sửa
        /// </summary>
        public string? modified_by { get; set; }

        /// <summary>
        /// ngày sửa
        /// </summary>
        public DateTime? modified_date { get; set; }
    }
}
