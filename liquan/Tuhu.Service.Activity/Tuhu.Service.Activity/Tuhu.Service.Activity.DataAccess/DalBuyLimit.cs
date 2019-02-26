using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalBuyLimit
    {
        public static async Task<bool> AddBuyLimitInfo(BuyLimitDetailModel model)
        {
            const string sqlStr = @"
select PKID
from Activity..UserLimitBuyDetail as T with (nolock)
where T.ModuleName = @name
      and T.ModuleProductId = @id
      and T.LimitObjectId = @objectId
      and T.ObjectType = @type
      and T.Reference = @reference
      and T.IsDelete = 0;";
            const string sqlStr2 = @"
insert into Activity.dbo.UserLimitBuyDetail
(
    ModuleName,
    ModuleProductId,
    LimitObjectId,
    ObjectType,
    Reference,
    Remark
)
values
(@name, @id, @objectId, @type, @reference, @remark);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@name", model.ModuleName);
                cmd.Parameters.AddWithValue("@id", model.ModuleProductId);
                cmd.Parameters.AddWithValue("@objectId", model.LimitObjectId);
                cmd.Parameters.AddWithValue("@type", model.ObjectType);
                cmd.Parameters.AddWithValue("@reference", model.Reference);
                var checkResult = await DbHelper.ExecuteScalarAsync(cmd);
                if (checkResult == null)
                {
                    cmd.CommandText = sqlStr2;
                    cmd.Parameters.AddWithValue("@remark", model.Remark);
                    var num = await DbHelper.ExecuteNonQueryAsync(cmd);
                    return num > 0;
                }
                return false;
            }
        }
        public static async Task<BuyLimitInfoModel> SelectBuyLimitInfo(BuyLimitModel model,bool readOnly=true)
        {
            const string sqlStr = @"select COUNT(*) as CurrentCount
from Activity..UserLimitBuyDetail as T with (nolock)
where T.ModuleName = @name
      and T.ModuleProductId = @id
      and T.LimitObjectId = @objectId
      and T.ObjectType = @type
      and T.IsDelete = 0;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@name", model.ModuleName);
                cmd.Parameters.AddWithValue("@id", model.ModuleProductId);
                cmd.Parameters.AddWithValue("@objectId", model.LimitObjectId);
                cmd.Parameters.AddWithValue("@type", model.ObjectType);
                var value = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int.TryParse(value?.ToString(), out var result);
                return new BuyLimitInfoModel
                {
                    ModuleName = model.ModuleName,
                    ModuleProductId = model.ModuleProductId,
                    LimitObjectId = model.LimitObjectId,
                    ObjectType = model.ObjectType,
                    CurrentCount = result
                };
            }
        }
        public static async Task<List<BuyLimitModel>> RemoveBuyLimitInfo(string moduleName, string reference)
        {
            const string sqlStr = @"
update Activity..UserLimitBuyDetail with (rowlock)
set IsDelete = 1,
    LastUpdateDateTime = GETDATE()
output Inserted.ModuleName,
       Inserted.ModuleProductId,
       Inserted.LimitObjectId,
       Inserted.ObjectType
where ModuleName = @name
      and Reference = @reference
      and IsDelete = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@name", moduleName);
                cmd.Parameters.AddWithValue("@reference", reference);
                return (await DbHelper.ExecuteSelectAsync<BuyLimitModel>(false, cmd))?.ToList() ?? new List<BuyLimitModel>();
            }
        }
    }
}
