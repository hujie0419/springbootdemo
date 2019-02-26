using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public class DalTaskType
    {

        /// <summary>
        /// 获取所有任务(福利)类型
        /// </summary>
        public static List<TaskTypeModel> SearchAllTaskType()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
 SELECT [PKID] ,
        [TaskTypeCode] ,
        [TaskTypeName] ,
        [IsDisabled] ,
        [SortIndex] ,
        [CreateBy] ,
        [CreateDateTime] ,
        [LastUpdateBy] ,
        [LastUpdateDateTime] ,
        [IsDeleted]
 FROM   [Configuration].[dbo].[TaskType] WITH(NOLOCK)
 WHERE  IsDeleted = 0 order by SortIndex
");
            using (var dataCmd = new SqlCommand(strSql.ToString()))
            {
                return DbHelper.ExecuteDataTable(dataCmd)?.ConvertTo<TaskTypeModel>().ToList();
            }
        }
    }
}
