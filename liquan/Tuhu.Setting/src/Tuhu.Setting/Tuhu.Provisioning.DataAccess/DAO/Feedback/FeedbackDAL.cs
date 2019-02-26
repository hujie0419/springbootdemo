using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.Feedback;

namespace Tuhu.Provisioning.DataAccess.DAO.Feedback
{
    public class FeedbackDAL
    {

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public List<string> GetVersionNumList()
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr = "SELECT DISTINCT VersionNum FROM Tuhu_profiles..Feedback WITH(NOLOCK)";
                return sqlHelper.ExecuteDataTable(sqlStr, CommandType.Text).ConvertToStringList();
            }
        }

        /// <summary>
        /// 获取手机型号
        /// </summary>
        /// <returns></returns>
        public List<string> GetPhoneModelList()
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr = "SELECT DISTINCT PhoneModels FROM Tuhu_profiles..Feedback WITH(NOLOCK)";
                return sqlHelper.ExecuteDataTable(sqlStr, CommandType.Text).ConvertToStringList();
            }
        }

        /// <summary>
        /// 获取网络环境
        /// </summary>
        /// <returns></returns>
        public List<string> GetNetworkEnvironmentList()
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr = "SELECT DISTINCT NetworkEnvironment FROM Tuhu_profiles..Feedback WITH(NOLOCK)";
                return sqlHelper.ExecuteDataTable(sqlStr, CommandType.Text).ConvertToStringList();
            }
        }

        /// <summary>
        /// 获取问题反馈数据集合（包括搜索）
        /// </summary>
        /// <param name="typeIds">问题类型Id,用逗号隔开</param>
        /// <param name="flag">时间标识，0全部，1一周内，2一个月内，3具体时间段</param>
        /// <param name="versionNum">版本号</param>
        /// <param name="phoneModel">手机型号</param>
        /// <param name="networkEnvir">网络环境</param>
        /// <param name="startTime">日期：开始时间</param>
        /// <param name="endTime">日期：结束时间</param>
        /// <param name="isDown">是否是下载数据：1为是，0为查询</param>
        /// <returns></returns>
        public DataTable GetFeedbackListByCondition(int pageIndex, int pageSize, string typeIds, int flag, string versionNum, string phoneModel, string networkEnvir, DateTime? startTime, DateTime? endTime, out int count, int isDown = 0)
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr =
                    @"SELECT  ROW_NUMBER() over(order by f.Id desc) as rowId,f.Id,q.TypeName,UserPhone,CreateTime,FeedbackContent, PhoneModels,VersionNum,NetworkEnvironment,IsCustomerServer,
                           ImgUrl=(select distinct stuff((select ',' + ImgUrl from Tuhu_profiles..FeedbackImg where FeedbackId = f.Id  for xml path('')),1,1,'') from Tuhu_profiles..FeedbackImg) 
                           FROM Tuhu_profiles..Feedback f WITH(NOLOCK) LEFT JOIN Tuhu_profiles..QuestionType q ON f.TypeId=q.Id";
                string condition = string.Empty;

                switch (flag)
                {
                    case 1:
                        condition += " DATEDIFF(week,CreateTime,getdate())=0";
                        break;
                    case 2:
                        condition += " DATEDIFF(month,CreateTime,getdate())=0";
                        break;
                    case 3:
                        if (startTime.HasValue)
                            condition += " CreateTime>='" + startTime + "'";
                        if (endTime.HasValue)
                            condition += string.IsNullOrEmpty(condition) ? (" CreateTime<='" + endTime.Value.AddDays(1) + "'") : ("  AND CreateTime<='" + endTime.Value.AddDays(1) + "'");
                        break;

                }

                if (!string.IsNullOrEmpty(typeIds))
                {
                    var typeIdArray = typeIds.Split(',');
                    var typeIdStr = string.Empty;
                    for (int i = 0; i < typeIdArray.Length; i++)
                    {
                        if (i == typeIdArray.Length - 1)
                            typeIdStr = typeIdStr + "'" + typeIdArray[i] + "'";
                        else
                            typeIdStr = typeIdStr + "'" + typeIdArray[i] + "',";
                    }
                    condition += string.IsNullOrEmpty(condition) ? " TypeId IN (" + typeIdStr + ")" : " AND TypeId IN (" + typeIdStr + ")";
                }


                if (!string.IsNullOrEmpty(versionNum))
                    condition += string.IsNullOrEmpty(condition) ? " VersionNum='" + versionNum + "'" : " AND VersionNum='" + versionNum + "'";
                if (!string.IsNullOrEmpty(phoneModel))
                    condition += string.IsNullOrEmpty(condition) ? " PhoneModels='" + phoneModel + "'" : " AND PhoneModels='" + phoneModel + "'";
                if (!string.IsNullOrEmpty(networkEnvir))
                    condition += string.IsNullOrEmpty(condition) ? " NetworkEnvironment='" + networkEnvir + "'" : " AND NetworkEnvironment='" + networkEnvir + "'";

                //sqlStr = sqlStr + " WHERE " + (string.IsNullOrEmpty(condition) ? "q.IsDelete=0" : condition + " AND q.IsDelete=0");
                if (!string.IsNullOrEmpty(condition))
                {
                    sqlStr = sqlStr + " WHERE " + condition;
                }
                count = 0;
                if (isDown == 0)
                {
                    count = Convert.ToInt32(sqlHelper.ExecuteScalar("SELECT COUNT(*) AS COUNT FROM (" + sqlStr + ") t", CommandType.Text));
                    sqlStr = "SELECT Top " + pageSize + " T.Id,TypeName,UserPhone,CreateTime,FeedbackContent, PhoneModels,VersionNum,NetworkEnvironment,IsCustomerServer,ImgUrl FROM (" + sqlStr + ") T WHERE T.rowId>" +
                             (pageIndex - 1) * pageSize + "  ORDER BY T.CreateTime DESC";
                }
                return sqlHelper.ExecuteDataTable(sqlStr, CommandType.Text);
            }
        }

        /// <summary>
        /// 添加建议反馈信息
        /// </summary>
        /// <param name="typeId">问题类型Id</param>
        /// <param name="userPhone">用户手机号</param>
        /// <param name="feedbackContent">反馈内容</param>
        /// <param name="versionNum">版本号</param>
        /// <param name="phoneModels">手机型号</param>
        /// <param name="networkEnvironment">网络环境</param>
        /// <param name="images">图片，>=0张</param>
        /// <returns></returns>
        public async Task<int> AddFeedbackInfo(int typeId, string userPhone, string feedbackContent, string versionNum, string phoneModels, string networkEnvironment, List<string> images)
        {
            int result = 0;
            using (var sqlHelper = new SqlDbHelper())
            {
                sqlHelper.BeginTransaction();
                try
                {
                    var sql = @"INSERT INTO Tuhu_profiles..Feedback
                        (TypeId
                        ,CreateTime
                        ,UserPhone
                        ,FeedbackContent
                        ,VersionNum
                        ,PhoneModels
                        ,NetworkEnvironment)
                  VALUES
                        (@TypeId
                        ,GETDATE()
                        ,@UserPhone
                        ,@FeedbackContent 
                        ,@VersionNum
                        ,@PhoneModels 
                        ,@NetworkEnvironment)";
                    SqlCommand cmd = new SqlCommand(sql + ";SELECT @@identity as Id");
                    cmd.Parameters.AddWithValue("@TypeId", typeId);
                    cmd.Parameters.AddWithValue("@UserPhone", userPhone);
                    cmd.Parameters.AddWithValue("@FeedbackContent", feedbackContent);
                    cmd.Parameters.AddWithValue("@VersionNum", versionNum);
                    cmd.Parameters.AddWithValue("@PhoneModels", phoneModels);
                    cmd.Parameters.AddWithValue("@NetworkEnvironment", networkEnvironment);
                    result = Convert.ToInt32(await sqlHelper.ExecuteScalarAsync(cmd));
                    if (images != null && images.Any())
                    {
                        foreach (var imgurl in images)
                        {
                            var imgsql = @"INSERT INTO Tuhu_profiles..FeedbackImg
                                      (ImgUrl
                                      ,FeedbackId)
                                    VALUES
                                       (@ImgUrl
                                       ,@FeedbackId)";
                            SqlCommand cmd1 = new SqlCommand(imgsql);
                            cmd1.Parameters.AddWithValue("@ImgUrl", imgurl);
                            cmd1.Parameters.AddWithValue("@FeedbackId", result);
                            await sqlHelper.ExecuteNonQueryAsync(cmd1);
                        }
                    }
                    sqlHelper.Commit();
                    return result;
                }
                catch (Exception e)
                {
                    sqlHelper.Rollback();
                    throw new Exception(e.Message);
                }

            }
        }

        /// <summary>
        /// 按问题类型和用户手机号查询问题反馈信息
        /// </summary>
        /// <param name="typeId">问题类型</param>
        /// <param name="userPhone">手机号</param>
        /// <returns></returns>
        public DataTable GetFeedbackEntityByTypeIdAndUser(int typeId, string userPhone)
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr = @"SELECT f.Id
                        , f.TypeId
                        ,f.CreateTime
                        ,f.UserPhone
                        ,f.FeedbackContent
                        ,f.VersionNum
                        ,f.PhoneModels
                        ,f.NetworkEnvironment
                        ,i.ImgUrl
                        FROM Tuhu_profiles..Feedback f WITH(NOLOCK) LEFT JOIN Tuhu_profiles..FeedbackImg i ON f.Id = i.FeedbackId 
                       WHERE f.TypeId = @typeId AND f.UserPhone = @userPhone ORDER BY f.CreateTime DESC";
                SqlCommand cmd = new SqlCommand(sqlStr);
                cmd.Parameters.AddWithValue("@typeId", typeId);
                cmd.Parameters.AddWithValue("@userPhone", userPhone);
                var rows = sqlHelper.ExecuteDataRow(cmd);
                return rows?.Table;
            }
        }


        public async Task<int> UpdateIsCustomerServer(int id)
        {
            int result = 0;
            using (var sqlHelper = new SqlDbHelper())
            {
                var sql = @"UPDATE Tuhu_profiles..Feedback  SET IsCustomerServer=1 WHERE Id=@id";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@id", id);

                result = await sqlHelper.ExecuteNonQueryAsync(cmd);

                return result;
            }
        }

    }
}
