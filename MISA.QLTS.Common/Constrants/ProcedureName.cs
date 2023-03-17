using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Constrants
{
    public class ProcedureName
    {
        /// <summary>
        /// ProcedureName getById
        /// </summary>
        public static string Get = "Proc_{0}_Get{1}";

        /// <summary>
        /// ProcName Insert
        /// </summary>
        public static string Insert = "Proc_{0}_Insert";

        /// <summary>
        /// ProcName Update
        /// </summary>
        public static string Update = "Proc_{0}_Update";

        /// <summary>
        /// ProcName Delete
        /// </summary>
        public static string Delete = "Proc_{0}_Delete";

        /// <summary>
        /// ProcName phân trang và tìm kiếm
        /// </summary>
        public static string Filter = "Proc_{0}_Search_Paging";
    }
}
