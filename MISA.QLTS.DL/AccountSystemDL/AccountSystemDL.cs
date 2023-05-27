using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Common.Entities;
using MISA.QLTS.DL.AssetCategoryDL;
using MISA.QLTS.DL.BaseDL;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MISA.QLTS.DL.AccountSystemDL
{
    public class AccountSystemDL : BaseDL<AccountSystem>, IAccountSystemDL
    {
        public List<AccountSystem> GetAccountsByParentId(Guid IdParent)
        {
            dynamic result;
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("p_parent_id", IdParent);
            string storedProcedureName = "Proc_AccountSystem_GetByParentId";
            using (var mySqlConnection = new MySqlConnection(Datacontext.DataBaseContext.connectionString))
            {
                var query = mySqlConnection.QueryMultiple(storedProcedureName, dynamicParams, commandType: CommandType.StoredProcedure);
                result = query.Read<AccountSystem>().ToList();
            }
            return result;
        }
    }
}
