using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Framework;
using Tuhu.Component.Common;
using System.Data.Common;
using System.Linq;
using Tuhu.Provisioning.DataAccess.Entity.Push;

namespace Tuhu.Provisioning.DataAccess.DAO.Push
{
    public class DalPush
    {
        private static readonly string PushSourceName = "yewu.tuhu.cn";
        public static IList<string> GetPushMsgPersonConfig(SqlConnection connection)
        {
            IList<string> list = new List<string>();

            using (var dataReader = SqlHelper.ExecuteReader(connection,
                CommandType.Text, "SELECT PhoneNumber FROM Gungnir..PushMsgInternalUser(NOLOCK)"))
            {
                if (dataReader.Read())
                {
                    list.Add(dataReader.GetTuhuString(0));
                }
            }

            return list;
        }

        /// <summary>
        /// 查询推送日志
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<PushMessageViewModel> SearchPushHis(int pageIndex, int pageSize, out int count)
        {
            count = 0;
            DbParameter[] parameters =
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@SourceName", PushSourceName),
                new SqlParameter("@Count", SqlDbType.Int) {Direction = ParameterDirection.Output}
            };
            var promotionList = DbHelper.ExecuteDataTable("Push_SearchPushHis", CommandType.StoredProcedure
                , parameters).ConvertTo<PushMessageViewModel>().ToList();
            var lastOrDefault = parameters.LastOrDefault();
            if (lastOrDefault != null)
            {
                int.TryParse(lastOrDefault.Value.ToString(), out count);
            }
            return promotionList;
        }

        /// <summary>
        /// 查询标签配置
        /// </summary>
        /// <returns></returns>
        public static List<TagConfigModel> GetAllPushTag(SqlConnection connection)
        {
            var list = new List<TagConfigModel>();

            using (
                   var reader = SqlHelper.ExecuteReader(connection, CommandType.Text,
                       @"SELECT PKID, [FirstLevel], [SecondLevel], TagName, [Description], [Frequency] 
                            FROM Tuhu_bi.dbo.dw_UMeng_Tag_Config(NOLOCK) WHERE EnableTime < GETDATE()"))
            {
                while (reader.Read())
                {
                    var config = new TagConfigModel();
                    config.TagName = CommonUtil.ConvertObjectToString(reader["TagName"]);
                    config.PKID = CommonUtil.ConvertObjectToInt32(reader["PKID"]);
                    config.FirstLevel = CommonUtil.ConvertObjectToString(reader["FirstLevel"]);
                    config.SecondLevel = CommonUtil.ConvertObjectToString(reader["SecondLevel"]);
                    config.Description = CommonUtil.ConvertObjectToString(reader["Description"]);

                    list.Add(config);
                }
            }

            return list;
        }
    }
}
