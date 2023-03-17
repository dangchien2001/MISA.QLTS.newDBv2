using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Common.Attributes
{
    public class QLTSAttributes
    {
        /// <summary>
        /// Attribute dùng để xác định 1 property là khóa chính
        /// </summary>
        /// Author: NCCONG (09/02/2023)
        [AttributeUsage(AttributeTargets.Property)]
        public class PrimaryKeyAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute dùng để xác định 1 property không được để trống
        /// </summary>
        /// Author: NCCONG (09/02/2023)
        public class IsNotNullOrEmptyAttribute : Attribute
        {
            #region Field
            /// <summary>
            /// Message lỗi trả về cho client
            /// </summary>
            public string ErrorMessage;

            #endregion

            #region Constructor
            /// <summary>
            /// Contructor có tham số
            /// </summary>
            /// <param name="errorMessage">Tin nhắn lỗi trả về</param>
            /// Author: NCCONG (09/02/2023)
            public IsNotNullOrEmptyAttribute(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }
            #endregion
        }

        /// <summary>
        /// Attribute dùng để xác định kiểu dữ liệu DateTime
        /// </summary>
        /// Author: NCCONG (14/02/2022)
        public class DataTypeDate : Attribute
        {
            #region Field
            /// <summary>
            /// Message lỗi trả về cho client
            /// </summary>
            public string ErrorMessage;

            #endregion

            #region Constructor
            /// <summary>
            /// Contructor có tham số
            /// </summary>
            /// <param name="errorMessage">Tin nhắn lỗi trả về</param>
            /// Author: NCCONG (09/02/2023)
            public DataTypeDate(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }
            #endregion
        }
        /// <summary>
        /// Attribute để xác định khóa ngoại không được để trống
        /// </summary>
        public class ForeignKey : Attribute
        {
            #region Field

            /// <summary>
            /// Message lỗi trả về cho client
            /// </summary>
            public string ErrorMessage;

            #endregion

            #region Contructor

            /// <summary>
            /// Contructor có tham số
            /// </summary>
            /// <param name="errorMessage">Tin nhắn lỗi trả về</param>
            /// Author: NCCONG (16/02/2022)
            public ForeignKey(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }

            #endregion
        }
    }
}
