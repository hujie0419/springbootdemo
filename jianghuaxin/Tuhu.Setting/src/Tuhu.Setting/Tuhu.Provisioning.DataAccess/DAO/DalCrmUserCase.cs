using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Collections.Generic;
using Microsoft.ApplicationBlocks.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalCrmUserCase
    {
        #region 单例
        private static DalCrmUserCase _dalCrmUserCase = null;
        public static DalCrmUserCase CreateDalCrmUserCase
        {
            get
            {
                if (_dalCrmUserCase == null)
                    return _dalCrmUserCase = new DalCrmUserCase();
                else
                    return _dalCrmUserCase;
            }
        }
        private DalCrmUserCase() { }
        #endregion

        #region  select
        /// <summary>
        /// 分页获取userCase数据集
        /// </summary>
        /// <param name="connectionStr">数据库链接对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="orderBy">排序字段, a desc</param>
        /// <param name="totalRecord">返回数据总数</param>
        /// <param name="TotalPage">返回总页数</param>
        /// <param name="errorMsg">返回报错信息</param>        
        /// <param name="sqlWhere">条件:a=1 and b =2 ...</param>
        /// <param name="fields">显示字段a,b,c...</param>
        /// <returns>返回DataTable</returns>
        public DataTable SelectCrmUserCase(
            string connectionStr,
            string tableName,
            int pageSize,
            int pageIndex,
            string orderBy,
            out int totalRecord,
            out int totalPage,
            out string errorMsg,
            string sqlWhere = "1=1",
            string fields = "*")
        {
            errorMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@TableName",SqlDbType.NVarChar),
                    new SqlParameter("@pageSize",SqlDbType.Int),
                    new SqlParameter("@pageIndex",SqlDbType.Int),
                    new SqlParameter("@OrderField",SqlDbType.NVarChar),
                    new SqlParameter("@sqlWhere",SqlDbType.NVarChar),
                    new SqlParameter("@Fields",SqlDbType.NVarChar),
                    new SqlParameter("@return_value",SqlDbType.Int),
                    new SqlParameter("@totalRecord",SqlDbType.Int),
                    new SqlParameter("@TotalPage",SqlDbType.Int)
                };

                sqlParamters[0].Value = tableName;
                sqlParamters[1].Value = pageSize;
                sqlParamters[2].Value = pageIndex;
                sqlParamters[3].Value = orderBy;
                sqlParamters[4].Value = sqlWhere;
                sqlParamters[5].Value = fields;
                sqlParamters[6].Direction = ParameterDirection.ReturnValue;
                sqlParamters[7].Direction = ParameterDirection.Output;
                sqlParamters[8].Direction = ParameterDirection.Output;

                DataTable dataTable = new DataTable();
                using (SqlConnection sqlCon = new SqlConnection(connectionStr))
                {
                    try
                    {
                        sqlCon.Open();
                        using (var dataReader = SqlHelper.ProcOutputExecuteReader(sqlCon, CommandType.StoredProcedure, "csp_CrmUserCasePage", sqlParamters))
                        {
                            dataTable.Load(dataReader);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = string.Format("方法名称:SelectCrmUserCase => (dataReader),存储过程:csp_CrmUserCasePage=>参数:sqlWhere={1},tableName={2},fields={3},orderBy={4},错误信息:{0}", ex.ToString(), sqlWhere, tableName, fields, orderBy);
                    }
                    finally
                    {
                        //int returnvalue = (int)sqlParamters[6].Value;
                        totalRecord = (int)sqlParamters[7].Value;
                        totalPage = (int)sqlParamters[8].Value;

                        sqlCon.Close();
                    }
                }

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:SelectCrmUserCase,存储过程:csp_CrmUserCasePage=>参数:sqlWhere={1},tableName={2},fields={3},orderBy={4},错误信息:{0}", ex.ToString(), sqlWhere, tableName, fields, orderBy);
                totalRecord = -1;
                totalPage = -1;
                return null;
            }
        }

        /// <summary>
        /// 分页获取userCase数据集
        /// </summary>
        /// <param name="connectionStr">数据库链接对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="orderBy">排序字段, a desc</param>
        /// <param name="totalRecord">返回数据总数</param>
        /// <param name="TotalPage">返回总页数</param>
        /// <param name="errorMsg">返回报错信息</param>        
        /// <param name="sqlWhere">条件:a=1 and b =2 ...</param>
        /// <param name="fields">显示字段a,b,c...</param>
        /// <returns>返回DataTable</returns>
        public DataTable VenderApiSelectCrmUserCase(
            string connectionStr,
            string tableName,
            int pageSize,
            int pageIndex,
            string orderBy,
            out string errorMsg,
            List<SqlParameter> sqlParams,
            string sqlWhere = "1=1",
            string fields = "*"
            )
        {
            errorMsg = string.Empty;
            try
            {
                DataTable dataTable = new DataTable();
                using (SqlConnection sqlCon = new SqlConnection(connectionStr))
                {
                    try
                    {
                        string sql = @"SELECT  OwnerGuid ,
        CaseGuid ,
        EndUserGuid ,
        isClosed ,
        NextContactTime ,
        ForecastDate ,
        nickname ,
        Type ,
        district ,
        first_name ,
        last_name ,
        carno ,
        cartype_description ,
        Products ,
        email ,
        telephone ,
        TaobaoId ,
        Q_Name ,
        Q_CarNo ,
        Q_TelePhone ,
        CaseInterest ,
        StatusID ,
        lastlog ,
        CaseLastModifiedTime ,
        CreateTime
FROM    vw_EndUserCase_Ext_V2015 WITH(NOLOCK)"+sqlWhere;

                        sqlCon.Open();
                        using (var dataReader = SqlHelper.ExecuteReaderV2(sqlCon, CommandType.Text, sql, sqlParams.ToArray()))
                        {
                            dataTable.Load(dataReader);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = string.Format("方法名称:SelectCrmUserCase => (dataReader),存储过程:csp_CrmUserCasePage=>参数:sqlWhere={1},tableName={2},fields={3},orderBy={4},错误信息:{0}", ex.ToString(), sqlWhere, tableName, fields, orderBy);
                    }
                    finally
                    {
                        //int returnvalue = (int)sqlParamters[6].Value;
                        //totalRecord = (int)sqlParamters[7].Value;
                        //totalPage = (int)sqlParamters[8].Value;

                        sqlCon.Close();
                    }
                }

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:SelectCrmUserCase,存储过程:csp_CrmUserCasePage=>参数:sqlWhere={1},tableName={2},fields={3},orderBy={4},错误信息:{0}", ex.ToString(), sqlWhere, tableName, fields, orderBy);
                //totalRecord = -1;
                //totalPage = -1;
                return null;
            }
        }

        /// <summary>
        /// 判断用户是否有未关闭case
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="endUserId"></param>
        /// <returns></returns>
        public bool CheckUserHasOpenCase(string connectionStr, string endUserGuid, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                Guid _endUserGuid = new Guid(endUserGuid);

                var userIdParameter = new SqlParameter("@UserId", _endUserGuid.ToString("D"));
                var returnValuParameter = new SqlParameter("@ReturnValue", SqlDbType.Bit);
                returnValuParameter.Direction = ParameterDirection.ReturnValue;

                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.StoredProcedure, "procCheckUserHasNoOpenCase", new[] { userIdParameter, returnValuParameter });

                return (int)returnValuParameter.Value == 1;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:CheckUserHasOpenCase,存储过程:procCheckUserHasNoOpenCase=>参数:endUserId={1},错误信息:{0}", ex.ToString(), endUserGuid);
                return false;
            }
        }
        #endregion

        #region update
        /// <summary>
        /// 更新任务记录
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="endUserCase"></param>
        /// <param name="errerMsg">返回错误信息</param>
        /// <returns></returns>
        public bool UpdateCrmUserCase(string connectionStr, tbl_EndUserCaseModel endUserCase, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                string updateSQL = " Update Gungnir.dbo.tbl_EndUserCase set ForecastDate=@ForecastDate,CaseInterest=@CaseInterest,Products=@Products,Way=@Way,Source=@Source Where CaseGuid = @CaseGuid ";

                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@ForecastDate",endUserCase.ForecastDate),
                    new SqlParameter("@CaseInterest",endUserCase.CaseInterest),
                    new SqlParameter("@Products",endUserCase.Products),
                    new SqlParameter("@Way",endUserCase.Way),
                    new SqlParameter("@Source",endUserCase.Source),
                    new SqlParameter("@CaseGuid",endUserCase.CaseGuid)
                };

                var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:UpdateCrmUserCase,错误信息:{0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 更新 case,tasks
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="endUserCase"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool UpdateCrmUserCaseTasks(string connectionStr, tbl_EndUserCaseModel endUserCase, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                string updateSQL = " Update Gungnir.dbo.tbl_EndUserCase set Tasks=@Tasks,LastUpdateTime=@LastUpdateTime Where CaseGuid = @CaseGuid ";

                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@Tasks",endUserCase.Tasks),
                    new SqlParameter("@LastUpdateTime",endUserCase.LastUpdateTime),
                    new SqlParameter("@CaseGuid",endUserCase.CaseGuid)
                };

                var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:UpdateCrmUserCaseTasks,错误信息:{0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="coconnectionStr"></param>
        /// <param name="userObject"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool UpdateUserObject(string coconnectionStr, tbl_UserObjectModel userObject, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                string updateSQL = "";
                updateSQL += @" update Tuhu_profiles..UserObject  WITH(rowlock)
                                      set u_last_name=@u_last_name,
	                                    u_first_name=@u_first_name,
	                                    u_pref5=@u_pref5,
	                                    i_gender=@i_gender,
	                                    u_email_address=@u_email_address,
	                                    u_Pref4=@u_Pref4,
	                                    u_mobile_number=@u_mobile_number,
	                                    u_tel_number=@u_tel_number,
	                                    u_tel_extension=@u_tel_extension,
	                                    u_fax_number=@u_fax_number,
	                                    u_fax_extension=@u_fax_extension,
	                                    u_msn=@u_msn,
	                                    u_qq=@u_qq,
	                                    u_gmail=@u_gmail,
	                                    u_yahoo=@u_yahoo,
	                                    u_Pref3=@u_Pref3,
                                        u_addresses=@u_addresses,
                                        u_know_channel=@u_know_channel";

                if (!string.IsNullOrEmpty(userObject.u_date_BlackListEndTime))
                {
                    updateSQL += ",u_date_BlackListEndTime=@u_date_BlackListEndTime";
                }
                updateSQL += " where u_user_id = @u_user_id";

                SqlParameter[] sqlParamtersField = new SqlParameter[]
                {
                    new SqlParameter("@u_last_name",userObject.u_last_name),
                    new SqlParameter("@u_first_name",userObject.u_first_name),
                    new SqlParameter("@u_pref5",userObject.u_Pref5),
                    new SqlParameter("@i_gender",userObject.i_gender),
                    new SqlParameter("@u_email_address",userObject.u_email_address),
                    new SqlParameter("@u_Pref4",userObject.u_Pref4),
                    new SqlParameter("@u_mobile_number",userObject.u_mobile_number),
                    new SqlParameter("@u_tel_number",userObject.u_tel_number),
                    new SqlParameter("@u_tel_extension",userObject.u_tel_extension),
                    new SqlParameter("@u_fax_number",userObject.u_fax_number),
                    new SqlParameter("@u_fax_extension",userObject.u_fax_extension),
                    new SqlParameter("@u_msn",userObject.u_msn),
                    new SqlParameter("@u_qq",userObject.u_qq),
                    new SqlParameter("@u_gmail",userObject.u_gmail),
                    new SqlParameter("@u_yahoo",userObject.u_yahoo),
                    new SqlParameter("@u_Pref3",userObject.u_Pref3),
                    new SqlParameter("@u_user_id",userObject.u_user_id),
                    new SqlParameter("@u_addresses",userObject.u_addresses),
                    new SqlParameter("@u_know_channel",userObject.u_know_channel), 
                };

                SqlParameter[] sqlParamtersBlack = null;
                if (!string.IsNullOrEmpty(userObject.u_date_BlackListEndTime))
                {
                    sqlParamtersBlack = new SqlParameter[]
                    {
                        new SqlParameter("@u_date_BlackListEndTime",userObject.u_date_BlackListEndTime)    
                    };
                }

                int paramNum = sqlParamtersField.Count() + (sqlParamtersBlack == null ? 0 : sqlParamtersBlack.Count());
                SqlParameter[] sqlParamters = new SqlParameter[paramNum];

                for (int i = 0; i < sqlParamtersField.Count(); i++)
                {
                    sqlParamters[i] = new SqlParameter(sqlParamtersField[i].ParameterName, sqlParamtersField[i].SqlValue);
                }

                if (sqlParamtersBlack != null && !string.IsNullOrEmpty(userObject.u_date_BlackListEndTime))
                {
                    for (int i = 0; i < sqlParamtersBlack.Count(); i++)
                    {
                        sqlParamters[paramNum - 1] = new SqlParameter(sqlParamtersBlack[i].ParameterName, sqlParamtersBlack[i].SqlValue);
                    }
                }

                var result = SqlHelper.ExecuteNonQuery(coconnectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:UpdateCrmUserCase,错误信息:{0}", ex.ToString());
                return false;
            }
        }

        public bool UpdateUserLastName(string coconnectionStr, tbl_UserObjectModel userObject, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                string updateSQL = @" update Tuhu_profiles..UserObject  WITH(rowlock)
                                      set u_last_name=@u_last_name
                                     where u_user_id = @u_user_id";

                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@u_last_name",userObject.u_last_name),
                    new SqlParameter("@u_user_id",userObject.u_user_id)
                };

                var result = SqlHelper.ExecuteNonQuery(coconnectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:UpdateCrmUserCase,错误信息:{0}", ex.ToString());
                return false;
            }
        }
        #endregion

        #region 判断用户淘宝ID和手机号是否唯一
        /// <summary>
        /// 判断用户淘宝ID和手机号是否唯一
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="taobaoID">淘宝ID</param>
        /// <param name="mobileNumber">手机号</param>
        /// <param name="userID"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool GetUserByTaoBaoIDAndMobileNumber(string connectionStr, string taobaoID, string mobileNumber, string userID, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@TaoBaoID", taobaoID),
                    new SqlParameter("@MobileNumber", mobileNumber),
                    new SqlParameter("@UserID", userID)
                };
                var result = SqlHelper.ExecuteDataset(connectionStr, CommandType.StoredProcedure, "procCheckUserTaoBaoAndMobile", sqlParamters).Tables[0];
                if (result != null && result.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:GetUserByTaoBaoIDAndMobileNumber,存储过程:procCheckUserTaoBaoAndMobile=>参数:userID={3},mobileNumber={2},taobaoID={1},错误信息:{0}", ex.ToString(), taobaoID, mobileNumber, userID);
                return false;
            }
        }
        #endregion

        #region 获取Case信息
        /// <summary>
        /// 获取Case信息
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseGuid"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public DataTable GetCaseByCaseGuid(string connectionStr, string caseGuid, out string errerMsg)
        {
            errerMsg = string.Empty;
            string sql = "select * from Gungnir.dbo.tbl_EndUserCase with(nolock) where CaseGuid = @CaseGuid";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseGuid",SqlDbType.VarChar),
                };
                sqlParamters[0].Value = caseGuid;
                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetCaseByCaseGuid,sqlText:{2}=>参数:caseGuid={1},错误信息:{0}", ex.ToString(), caseGuid, sql);
                return null;
            }
        }
        #endregion

        #region 其他任务
        /// <summary>
        /// 其他任务
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="endUserGuid"></param>
        /// <param name="caseGuid"></param>       
        /// <param name="day"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public DataTable GetCaseByEndUserGuid(string connectionStr, string endUserGuid, string caseGuid, out string errerMsg, int day = -30)
        {
            errerMsg = string.Empty;
            string sql = @"select * from Gungnir.dbo.tbl_EndUserCase with(nolock) 
                            where EndUserGuid = @endUserGuid 
                            and CreateTime >= dateadd(day,@day,getdate())
                            and CaseGuid != @caseGuid and LEN(CaseGuid) > 0";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@endUserGuid",SqlDbType.VarChar),
                    new SqlParameter("@caseGuid",SqlDbType.VarChar),
                    new SqlParameter("@day",SqlDbType.Int)
                };
                sqlParamters[0].Value = endUserGuid;
                sqlParamters[1].Value = caseGuid;
                sqlParamters[2].Value = day;

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetCaseByEndUserGuid,sqlText:{4}=>参数:endUserGuid={1},day={2},CaseGuid={3},错误信息:{0}", ex.ToString(), endUserGuid, day, caseGuid, sql);
                return null;
            }
        }

        //方法重载
        public DataTable GetCaseByEndUserGuid(string connectionStr, string endUserGuid, string caseGuid, int statusid, out string errerMsg, int day = -30)
        {
            errerMsg = string.Empty;
            string sql = @" select * from Gungnir.dbo.tbl_EndUserCase with(nolock) 
                            where EndUserGuid = @endUserGuid 
                            and CreateTime >= dateadd(day,@day,getdate())
                            and CaseGuid != @caseGuid 
                            and LEN(CaseGuid) > 0 
                            and StatusID <> @statusid 
                            and EndUserGuid != @GuidEmpty ";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@endUserGuid",SqlDbType.VarChar),
                    new SqlParameter("@caseGuid",SqlDbType.VarChar),
                    new SqlParameter("@day",SqlDbType.Int),
                    new SqlParameter("@statusid",SqlDbType.Int),
                    new SqlParameter("@GuidEmpty",SqlDbType.VarChar)
                };
                sqlParamters[0].Value = endUserGuid;
                sqlParamters[1].Value = caseGuid;
                sqlParamters[2].Value = day;
                sqlParamters[3].Value = statusid;
                sqlParamters[4].Value = Guid.Empty.ToString();

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetCaseByEndUserGuid,sqlText:{5}=>参数:endUserGuid={1},day={2},CaseGuid={3},statusid={4},错误信息:{0}", ex.ToString(), endUserGuid, day, caseGuid, statusid, sql);
                return null;
            }
        }

        #endregion

        #region 相关订单
        /// <summary>
        /// 相关订单
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataTable GetOrderByUserID(string connectionStr, string userID, out string errerMsg, int day = -30)
        {

            errerMsg = string.Empty;
            string sql = @" SELECT (select top 1 DicValue from Gungnir.dbo.tbl_Dictionaries WITH(NOLOCK) where Dickey = Status)as 'StatusToCN',* FROM Gungnir.dbo.tbl_Order as tab1 with (nolock) 
left join Tuhu_profiles..vw_AssociateUser as tab2 with (nolock) on tab1.UserID = tab2.AssociateUserID WHERE (tab2.UserID = @UserID OR tab2.UserID = @EmptyUserID)";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@UserID",SqlDbType.VarChar),
                    new SqlParameter("@EmptyUserID",SqlDbType.VarChar)
                };
                sqlParamters[0].Value = userID;
                sqlParamters[1].Value = new Guid(userID).ToString("B");

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetOrderByUserID,sqlText:{3}=>参数:userID={1},day={2},错误信息:{0}", ex.ToString(), userID, day, sql);
                return null;
            }
        }
        #endregion

        #region 用户信息
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="userID"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public DataTable GetUserObjectByUserID(string connectionStr, string userID, out string errerMsg)
        {
            errerMsg = string.Empty;
            string sql = @" SELECT * FROM Tuhu_profiles.dbo.UserObject WITH (NOLOCK) WHERE u_user_id = @UserID";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@UserID",SqlDbType.VarChar)
                };
                sqlParamters[0].Value = userID;

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetUserObjectByUserID,sqlText:{2}=>参数:userID={1},错误信息:{0}", ex.ToString(), userID, sql);
                return null;
            }
        }

        /// <summary>
        /// 通过用户email 获取用户信息
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public DataTable GetUserObjectByEmail(string connectionStr, string email, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@u_email_address",SqlDbType.VarChar),
                };
                sqlParamters[0].Value = email;
                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.StoredProcedure, "csp_CrmUserObject", sqlParamters).Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetCrmUserInfo,存储过程:csp_CrmUserObject=>参数:email={1},错误信息:{0}", ex.ToString(), email);
                return null;
            }
        }


        /// <summary>
        ///  获取用户信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserObjectByList(string connectionStr, out string errerMsg)
        {
            errerMsg = string.Empty;
            string sql = @" SELECT u_user_id, u_last_name,u_email_address FROM Tuhu_profiles.dbo.UserObject with(nolock) WHERE u_user_type = 10 ";
            try
            {
                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, null).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetUserObjectByList,sqlText:{1},错误信息:{0}", ex.ToString(), sql);
                return null;
            }
        }
        #endregion

        #region 日志信息
        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseGuid"></param>
        /// <param name="userGuid"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public DataTable GetLogs(string connectionStr, string caseGuid, string userGuid, out string errerMsg)
        {
            errerMsg = string.Empty;
            string sql = @"
            select 
	            [CreateTime],
	            [LastUpdate],
	            [OwnerGuid],
	            [Content],
	            [Action],
	            [ActionText1],
	            [ActionText2],
	            tab2.u_Pref5 as 'ActionText3',
	            [CaseGuid],
	            [FormGuid],
	            [EndUserGuid],
	            [OrgGuid],
	            [IsActive],
	            [LastUpdateTime],
	            [NextContactTime],
	            [CreateDate]
             from Gungnir.dbo.tbl_Log as tab1 with(nolock)
             left join Tuhu_profiles.dbo.UserObject  as tab2 with(nolock) on '{'+CONVERT(nvarchar(50),tab1.OwnerGuid)+'}' = tab2.u_user_id  
             where tab1.CaseGuid = @CaseGuid or (tab1.EndUserGuid = @UserGuid and tab1.CaseGuid = @EmptyGuid) 
             order by tab1.LastUpdate desc ";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseGuid",SqlDbType.VarChar),
                    new SqlParameter("@UserGuid",SqlDbType.VarChar),
                    new SqlParameter("@EmptyGuid",SqlDbType.VarChar)
                };
                sqlParamters[0].Value = caseGuid;
                sqlParamters[1].Value = userGuid;
                sqlParamters[2].Value = Guid.Empty.ToString();

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetLogs,sqlText:{3}=>参数:caseGuid={1},userGuid={2},错误信息:{0}", ex.ToString(), caseGuid, userGuid, sql);
                return null;
            }
        }

        /// <summary>
        /// 事物创建日志记录,修改tbl_EndUserCase表时间
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="logModel"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool CreateLogs(string connectionStr, tbl_LogModel logModel, out string errerMsg)
        {
            errerMsg = string.Empty;
            string createLogsSql = @"INSERT INTO Gungnir.dbo.tbl_Log(Action,CreateTime,LastUpdate,NextContactTime,OwnerGuid,CaseGuid,Content) VALUES(@Action,@CreateTime,@LastUpdate,@NextContactTime,@OwnerGuid,@CaseGuid,@Content)";
            string updateEndUserCaseSql = @"UPDATE Gungnir.dbo.tbl_EndUserCase set NextContactTime = @NextContactTime where CaseGuid = @CaseGuid";

            SqlConnection conn = new SqlConnection(connectionStr);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                #region logs

                SqlParameter[] sqlParamLogs = new SqlParameter[]
                {
                    new SqlParameter("@Action",SqlDbType.VarChar),
                    new SqlParameter("@CreateTime",SqlDbType.DateTime),
                    new SqlParameter("@LastUpdate",SqlDbType.DateTime),
                    new SqlParameter("@NextContactTime",SqlDbType.DateTime),
                    new SqlParameter("@OwnerGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@CaseGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@Content",SqlDbType.NText)
                };
                sqlParamLogs[0].Value = logModel.Action;
                sqlParamLogs[1].Value = logModel.CreateTime;
                sqlParamLogs[2].Value = logModel.LastUpdate;
                sqlParamLogs[3].Value = logModel.NextContactTime;
                sqlParamLogs[4].Value = logModel.OwnerGuid;
                sqlParamLogs[5].Value = logModel.CaseGuid;
                sqlParamLogs[6].Value = logModel.Content;
                #endregion

                #region EndUserCase

                SqlParameter[] sqlParamEndUserCase = new SqlParameter[]
                {
                    new SqlParameter("@CaseGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@NextContactTime",SqlDbType.DateTime)
                };
                sqlParamEndUserCase[0].Value = logModel.CaseGuid;
                sqlParamEndUserCase[1].Value = logModel.NextContactTime;

                #endregion

                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, createLogsSql, sqlParamLogs);
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateEndUserCaseSql, sqlParamEndUserCase);
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                errerMsg = string.Format("方法名称:CreateLogs,错误信息:{0}", ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                trans.Dispose();
            }
        }

        /// <summary>
        /// 判断是否为Cases所有者,修改tbl_EndUserCase表的NextContactTime时间
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="logModel"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool CannotBeChanged_NextContactTime(string connectionStr, tbl_LogModel logModel, out string errerMsg)
        {
            errerMsg = string.Empty;
            string createLogsSql = @"INSERT INTO Gungnir.dbo.tbl_Log(Action,CreateTime,LastUpdate,NextContactTime,OwnerGuid,CaseGuid,Content) VALUES(@Action,@CreateTime,@LastUpdate,@NextContactTime,@OwnerGuid,@CaseGuid,@Content)";
            SqlConnection conn = new SqlConnection(connectionStr);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                #region logs

                SqlParameter[] sqlParamLogs = new SqlParameter[]
                {
                    new SqlParameter("@Action",SqlDbType.VarChar),
                    new SqlParameter("@CreateTime",SqlDbType.DateTime),
                    new SqlParameter("@LastUpdate",SqlDbType.DateTime),
                    new SqlParameter("@NextContactTime",SqlDbType.DateTime),
                    new SqlParameter("@OwnerGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@CaseGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@Content",SqlDbType.NText)
                };
                sqlParamLogs[0].Value = logModel.Action;
                sqlParamLogs[1].Value = logModel.CreateTime;
                sqlParamLogs[2].Value = logModel.LastUpdate;
                sqlParamLogs[3].Value = logModel.NextContactTime;
                sqlParamLogs[4].Value = logModel.OwnerGuid;
                sqlParamLogs[5].Value = logModel.CaseGuid;
                sqlParamLogs[6].Value = logModel.Content;
                #endregion

                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, createLogsSql, sqlParamLogs);
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                errerMsg = string.Format("方法名称:CannotBeChanged_NextContactTime,错误信息:{0}", ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                trans.Dispose();
            }
        }


        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="logModel"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool AddLogAsync(string connectionStr, tbl_LogModel logModel, out string errerMsg)
        {
            errerMsg = string.Empty;
            string createLogsSql = @"insert into Gungnir.dbo.tbl_Log(CaseGuid,FormGuid,OwnerGuid,Content,[Action],ActionText1,IsActive,CreateTime,LastUpdate,NextContactTime)
                                     values(@CaseGuid,@FormGuid,@OwnerGuid,@Content,@Action,@ActionText1,@IsActive,@CreateTime,@LastUpdate,@NextContactTime)";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseGuid", logModel.CaseGuid),
                    new SqlParameter("@FormGuid", logModel.FormGuid),
                    new SqlParameter("@OwnerGuid", logModel.OwnerGuid),
                    new SqlParameter("@Content", logModel.Content),
                    new SqlParameter("@Action", logModel.Action),
                    new SqlParameter("@ActionText1", logModel.ActionText1),
                    new SqlParameter("@IsActive", logModel.IsActive),
                    new SqlParameter("@CreateTime", logModel.CreateTime),
                    new SqlParameter("@LastUpdate", logModel.LastUpdate),
                    new SqlParameter("@NextContactTime", logModel.NextContactTime)
                };
                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, createLogsSql, sqlParamters);
                return true;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:AddLogAsync,错误信息:{0}", ex.ToString());
                return false;
            }
        }


        /// <summary>
        /// 获取数据库 Gungnir_History 中的日志信息
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseGuid"></param>
        /// <param name="userGuid"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public DataTable GetHistoryLogs(string connectionStr, string caseGuid, string userGuid, out string errerMsg)
        {
            errerMsg = string.Empty;
            string sql = @"
            select 
	            [CreateTime],
	            [LastUpdate],
	            [OwnerGuid],
	            [Content],
	            [Action],
	            [ActionText1],
	            [ActionText2],
	            tab2.u_Pref5 as 'ActionText3',
	            [CaseGuid],
	            [FormGuid],
	            [EndUserGuid],
	            [OrgGuid],
	            [IsActive],
	            [LastUpdateTime],
	            [NextContactTime],
	            [CreateDate]
             from Gungnir_History.dbo.tbl_Log as tab1 with(nolock)
             left join Tuhu_profiles.dbo.UserObject  as tab2 with(nolock) on '{'+CONVERT(nvarchar(50),tab1.OwnerGuid)+'}' = tab2.u_user_id  
             where tab1.CaseGuid = @CaseGuid or (tab1.EndUserGuid = @UserGuid and tab1.CaseGuid = @EmptyGuid) 
             order by tab1.LastUpdate desc ";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseGuid",SqlDbType.VarChar),
                    new SqlParameter("@UserGuid",SqlDbType.VarChar),
                    new SqlParameter("@EmptyGuid",SqlDbType.VarChar)
                };
                sqlParamters[0].Value = caseGuid;
                sqlParamters[1].Value = userGuid;
                sqlParamters[2].Value = Guid.Empty.ToString();

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetHistoryLogs,sqlText:{3}=>参数:caseGuid={1},userGuid={2},错误信息:{0}", ex.ToString(), caseGuid, userGuid, sql);
                return null;
            }
        }



        #endregion

        #region 创建任务

        /// <summary>
        /// 事物创建任务
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="tbl_PotentialForm"></param>
        /// <param name="tbl_EndUserCase"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool CreateNewEndUserCase(string connectionStr, tbl_PotentialFormModel tbl_PotentialForm, tbl_EndUserCaseModel tbl_EndUserCase, out string errerMsg)
        {
            errerMsg = string.Empty;

            string tbl_PotentialFormSql = @"INSERT INTO Gungnir.dbo.tbl_PotentialForm (CreateTime,CreatorGuid,DataID,FormGuid,isClosed,Type,LastModifiedTime) 
                                            VALUES(@CreateTime,@CreatorGuid,@DataID,@FormGuid,@isClosed,@Type,@LastModifiedTime)";

            string tbl_EndUserCaseSql = @"INSERT INTO Gungnir.dbo.tbl_EndUserCase (CaseGuid,CreateTime,isClosed,OwnerGuid,PotentialFormGuid,Type,StatusID,Creator,NextContactTime,LastModifiedTime)
                                          VALUES(@CaseGuid,@CreateTime,@isClosed,@OwnerGuid,@PotentialFormGuid,@Type,@StatusID,@Creator,@NextContactTime,@LastModifiedTime)";
            if (tbl_EndUserCase.EndUserGuid != null)
            {
                tbl_EndUserCaseSql = @"INSERT INTO Gungnir.dbo.tbl_EndUserCase (CaseGuid,CreateTime,isClosed,OwnerGuid,PotentialFormGuid,Type,StatusID,Creator,NextContactTime,LastModifiedTime,EndUserGuid)
                                          VALUES(@CaseGuid,@CreateTime,@isClosed,@OwnerGuid,@PotentialFormGuid,@Type,@StatusID,@Creator,@NextContactTime,@LastModifiedTime,@EndUserGuid)";
            }
            SqlConnection conn = new SqlConnection(connectionStr);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                #region  tbl_PotentialForm

                SqlParameter[] p_params = new SqlParameter[]
                {
                    new SqlParameter("@CreateTime",SqlDbType.DateTime),
                    new SqlParameter("@CreatorGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@DataID",SqlDbType.Int),
                    new SqlParameter("@FormGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@isClosed",SqlDbType.Bit),
                    new SqlParameter("@Type",SqlDbType.VarChar),
                    new SqlParameter("@LastModifiedTime",SqlDbType.DateTime)
                };
                p_params[0].Value = tbl_PotentialForm.CreateTime;
                p_params[1].Value = tbl_PotentialForm.CreatorGuid;
                p_params[2].Value = tbl_PotentialForm.DataID;
                p_params[3].Value = tbl_PotentialForm.FormGuid;
                p_params[4].Value = tbl_PotentialForm.isClosed;
                p_params[5].Value = tbl_PotentialForm.Type;
                p_params[6].Value = tbl_PotentialForm.LastModifiedTime;

                #endregion

                #region tbl_EndUserCase

                SqlParameter[] e_params = new SqlParameter[]
                {
                    new SqlParameter("@CaseGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@CreateTime",SqlDbType.DateTime),
                    new SqlParameter("@isClosed",SqlDbType.Bit),
                    new SqlParameter("@OwnerGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@PotentialFormGuid",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@Type",SqlDbType.VarChar),
                    new SqlParameter("@StatusID",SqlDbType.Int),
                    new SqlParameter("@Creator",SqlDbType.VarChar),
                    new SqlParameter("@NextContactTime",SqlDbType.DateTime),
                    new SqlParameter("@LastModifiedTime",SqlDbType.DateTime),
                    new SqlParameter("@EndUserGuid",SqlDbType.UniqueIdentifier)
                };
                e_params[0].Value = tbl_EndUserCase.CaseGuid;
                e_params[1].Value = tbl_EndUserCase.CreateTime;
                e_params[2].Value = tbl_EndUserCase.isClosed;
                e_params[3].Value = tbl_EndUserCase.OwnerGuid;
                e_params[4].Value = tbl_EndUserCase.PotentialFormGuid;
                e_params[5].Value = tbl_EndUserCase.Type;
                e_params[6].Value = tbl_EndUserCase.StatusID;
                e_params[7].Value = tbl_EndUserCase.Creator;
                e_params[8].Value = tbl_EndUserCase.NextContactTime;
                e_params[9].Value = tbl_EndUserCase.LastModifiedTime;
                e_params[10].Value = tbl_EndUserCase.EndUserGuid;

                #endregion

                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, tbl_PotentialFormSql, p_params);
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, tbl_EndUserCaseSql, e_params);
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                errerMsg = string.Format("方法名称:CreateNewEndUserCase,错误信息:{0}", ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                trans.Dispose();
            }
        }
        #endregion

        #region 修改任务状态
        /// <summary>
        /// 修改任务状态
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseGuid"></param>
        /// <param name="caseStatus"></param>
        /// <param name="isClosed"></param>   
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool UpdateEndUserCaseStatusID(string connectionStr, string caseGuid, int caseStatus, int isClosed, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                string updateSQL = " Update Gungnir.dbo.tbl_EndUserCase set StatusID = @StatusID,Tasks=@Tasks,isClosed =@isClosed Where CaseGuid = @CaseGuid ";
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@StatusID",caseStatus),
                    new SqlParameter("@CaseGuid",caseGuid),
                    new SqlParameter("@Tasks",string.Empty),
                    new SqlParameter("@isClosed",isClosed)                    
                };
                var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:UpdateEndUserCaseStatusID,错误信息:{0}", ex.ToString());
                return false;
            }
        }
        #endregion

        #region 信息确认

        /// <summary>
        /// 检测case是否存在
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="CaseId"></param>
        /// <param name="PotentialFormId"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool CheckEndUserCaseExisted(string connectionStr, Guid CaseId, Guid PotentialFormId, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseId",SqlDbType.UniqueIdentifier),
                    new SqlParameter("@PotentialFormId",SqlDbType.UniqueIdentifier)
                };
                sqlParamters[0].Value = CaseId;
                sqlParamters[1].Value = PotentialFormId;

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.StoredProcedure, "procGetCaseByCaseIdAndPotentialFormId", sqlParamters).Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:CheckEndUserCaseExisted,存储过程:procGetCaseByCaseIdAndPotentialFormId=>参数:CaseId={1},PotentialFormId={2},错误信息:{0}", ex.ToString(), CaseId, PotentialFormId);
                return false;
            }
        }

        /// <summary>
        /// 淘宝ID获取用户信息
        /// </summary>
        /// <param name="connectionStr"></param>        
        /// <param name="TaobaoId"></param>
        /// <param name="errerMsg"></param>        
        /// <returns></returns>
        public DataTable ProcGetUserProfileByTaobaoId(string connectionStr, string TaobaoId, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@TaobaoId",SqlDbType.NVarChar)
                };
                sqlParamters[0].Value = TaobaoId;

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.StoredProcedure, "procGetUserProfileByTaobaoId", sqlParamters).Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:ProcGetUserProfileByTaobaoId,存储过程:procGetUserProfileByTaobaoId=>参数:TaobaoId={1},错误信息:{0}", ex.ToString(), TaobaoId);
                return null;
            }
        }

        /// <summary>
        /// 电话号码获取用户信息
        /// </summary>
        /// <param name="connectionStr"></param>                
        /// <param name="PhoneNumber"></param>
        /// <param name="errerMsg"></param>        
        /// <returns></returns>
        public DataTable ProcGetUserProfileByPhoneNumber(string connectionStr, string PhoneNumber, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@PhoneNumber",SqlDbType.NVarChar)
                };
                sqlParamters[0].Value = PhoneNumber;

                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.StoredProcedure, "procGetUserProfileByPhoneNumber", sqlParamters).Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:ProcGetUserProfileByPhoneNumber,存储过程:procGetUserProfileByPhoneNumber=>参数:PhoneNumber={1},错误信息:{0}", ex.ToString(), PhoneNumber);
                return null;
            }
        }

        /// <summary>
        /// Creates CRM user and associates this user with one case.
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="UserObjectModel"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool AddUserProfileAndBindCase(string connectionStr, tbl_UserObjectModel UserObjectModel, string CaseId, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                        new SqlParameter("@Id", UserObjectModel.u_user_id),
                        new SqlParameter("@AccountStatus",UserObjectModel.u_account_status),
                        new SqlParameter("@DataId", UserObjectModel.i_dataid),
                        new SqlParameter("@Email",  UserObjectModel.u_email_address),
                        new SqlParameter("@Gender", UserObjectModel.i_gender),
                        new SqlParameter("@MobileNumber", UserObjectModel.u_mobile_number),
                        new SqlParameter("@TelephoneNumber",UserObjectModel.u_tel_number),
                        new SqlParameter("@UserType",UserObjectModel.u_user_type),
                        new SqlParameter("@Yahoo", UserObjectModel.u_yahoo?? string.Empty),
                        new SqlParameter("@CaseId", CaseId), 
                        new SqlParameter("@KnowChannel",  UserObjectModel.u_know_channel)
                };

                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.StoredProcedure, "procAddUserProfileAndAssociateCase", sqlParamters);
                return true;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:AddUserProfileAndBindCase,存储过程:procAddUserProfileAndAssociateCase,错误信息:{0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 修改case状态
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseId"></param>
        /// <param name="endUserId"></param>
        public bool AddEndUserForCase(string connectionStr, Guid caseId, Guid endUserId, string knowChannel,int type, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseId", caseId),
                    new SqlParameter("@EndUserId", endUserId),
                    new SqlParameter("@KnowChannel", knowChannel),
                    new SqlParameter("@Type", type)
                };

                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.StoredProcedure, "procAddEndUserForCase", sqlParamters);
                return true;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:AddEndUserForCase,存储过程:procAddEndUserForCase,参数:caseId={1},endUserId={2},错误信息:{0}", ex.ToString(), caseId, endUserId);
                return false;
            }
        }

        /// <summary>
        /// 关闭EndUserCase
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseId"></param>
        /// <param name="reason"></param>
        /// <param name="caseStatus"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool CloseEndUserCase(string connectionStr, Guid caseId, string reason, int caseStatus, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                     new SqlParameter("@CaseId", caseId),
                     new SqlParameter("@Reason", reason),
                     new SqlParameter("@StatusId", caseStatus)
                };

                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.StoredProcedure, "procCloseEndUserCase", sqlParamters);
                return true;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:CloseEndUserCase,存储过程:procCloseEndUserCase,参数:caseId={1},reason={2},caseStatus={3},错误信息:{0}", ex.ToString(), caseId, reason, caseStatus);
                return false;
            }
        }

        /// <summary>
        /// 关闭PotentialForm
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="potentialFormId"></param>
        /// <param name="reason"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool ClosePotentialForm(string connectionStr, Guid potentialFormId, string reason, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                       new SqlParameter("@PotentialFormId", potentialFormId),
                       new SqlParameter("@Reason", reason)
                };

                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.StoredProcedure, "procClosePotentialForm", sqlParamters);
                return true;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:ClosePotentialForm,存储过程:procClosePotentialForm,参数:potentialFormId={1},reason={2},错误信息:{0}", ex.ToString(), potentialFormId, reason);
                return false;
            }
        }

        /// <summary>
        /// 执行事务，关闭EndUserCase 与 PotentialForm
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseId"></param>
        /// <param name="potentialFormId"></param>
        /// <param name="reason"></param>
        /// <param name="caseStatus"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool TranCloseFormAsWrong(string connectionStr, Guid caseId, Guid potentialFormId, string reason, int caseStatus, out string errerMsg)
        {
            errerMsg = string.Empty;

            SqlConnection conn = new SqlConnection(connectionStr);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                #region EndUserCase

                SqlParameter[] sqlParamters_End = new SqlParameter[]
                {
                     new SqlParameter("@CaseId", caseId),
                     new SqlParameter("@Reason", reason),
                     new SqlParameter("@StatusId", caseStatus)
                };

                SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "procCloseEndUserCase", sqlParamters_End);

                #endregion

                #region PotentialForm

                SqlParameter[] sqlParamters_Pote = new SqlParameter[]
                {
                       new SqlParameter("@PotentialFormId", potentialFormId),
                       new SqlParameter("@Reason", reason)
                };

                SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "procClosePotentialForm", sqlParamters_Pote);

                #endregion

                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                errerMsg = string.Format("方法名称:TranCloseFormAsWrong,错误信息:{0}", ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                trans.Dispose();
            }
        }

        /// <summary>
        /// 下次联系(修改case状态)
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseId"></param>
        /// <param name="reason"></param>
        /// <param name="caseStatus"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool ChangeEndUserCaseStatus(string connectionStr, Guid caseId, string reason, int caseStatus, out string errerMsg)
        {
            errerMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseId", caseId),
                    new SqlParameter("@Reason", reason),
                    new SqlParameter("@StatusId", caseStatus)
                };

                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.StoredProcedure, "procChangeEndUserCaseStatus", sqlParamters);
                return true;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:ChangeEndUserCaseStatus,存储过程:procChangeEndUserCaseStatus,参数:caseId={1},reason={2},caseStatus={3},错误信息:{0}", ex.ToString(), caseId, reason, caseStatus);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="statusID"></param>
        /// <param name="start_time"></param>
        /// <param name="end_time"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public DataTable GetUserCaseInfo(string connectionStr, int statusID, string start_time, string end_time, out string errerMsg)
        {
            errerMsg = string.Empty;
            string sql = "select * from Gungnir.dbo.vw_EndUserCase_Ext_V2 with(nolock) where statusID = @statusID and LastUpdateTime>=@start_time and LastUpdateTime<=@end_time";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@statusID",SqlDbType.VarChar),
                    new SqlParameter("@start_time",start_time),
                    new SqlParameter("@end_time",end_time)
                };
                sqlParamters[0].Value = statusID;
                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetUserCaseInfo,sqlText:{4}=>参数:end_time[3],start_time[2],statusID={1},错误信息:{0}", ex.ToString(), statusID, start_time, end_time, sql);
                return null;
            }
        }

        #region 分配任务
        /// <summary>
        /// 分配任务
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="caseGuid"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public bool UpdateEndUserCaseOwnerGuid(string connectionStr, string caseGuid, string ownerGuid, out string errerMsg)
        {
            errerMsg = string.Empty;
            string updateSQL = " Update Gungnir.dbo.tbl_EndUserCase set OwnerGuid = @OwnerGuid Where CaseGuid = @CaseGuid ";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@CaseGuid",caseGuid),
                    new SqlParameter("@OwnerGuid",ownerGuid)                     
                };
                var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:UpdateEndUserCaseOwnerGuid,sqlText:{3}=>参数:OwnerGuid={2},CaseGuid={1},错误信息:{0}", ex.ToString(), caseGuid, ownerGuid, updateSQL);
                return false;
            }
        }
        #endregion

        #region 优惠券
        /// <summary>
        /// 获取用户优惠券信息
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="userID"></param>
        /// <param name="errerMsg"></param>
        /// <returns></returns>
        public DataTable GetUserCouponImportant(string connectionStr, string userID, out string errerMsg)
        {
            errerMsg = string.Empty;
            string sql = @" SELECT * FROM Gungnir.dbo.tbl_PromotionCode as tab1 WITH (NOLOCK) 
left join Tuhu_profiles..vw_AssociateUser as tab2 WITH (NOLOCK) on tab1.UserID = tab2.AssociateUserID WHERE ((tab2.UserID = @UserID OR tab2.UserID = @EmptyUserID) and tab2.UserID! = @Guid)";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                   new SqlParameter("@UserID",SqlDbType.VarChar),
                   new SqlParameter("@EmptyUserID",SqlDbType.VarChar),
                   new SqlParameter("@Guid",SqlDbType.VarChar)
                };
                sqlParamters[0].Value = userID;
                sqlParamters[1].Value = new Guid(userID).ToString("D");
                sqlParamters[2].Value = Guid.Empty.ToString();
                var dataTable = SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, sql, sqlParamters).Tables[0];

                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:GetUserCouponImportant,sqlText:{2}=>参数:userID={1},错误信息:{0}", ex.ToString(), userID, sql);
                return null;
            }
        }
        #endregion



        public string GetUserKnowChannelByTphone(string connectionStr, string tphone)
        {
            string sSql = @"SELECT  u_know_channel
                            FROM    Tuhu_profiles.dbo.UserObject WITH ( NOLOCK )
                            WHERE   u_tel_number = @PhoneNumber
                                    OR u_mobile_number = @PhoneNumber";
            SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@PhoneNumber", tphone) 
                };

            return "" + SqlHelper.ExecuteScalar(connectionStr, CommandType.Text, sSql, sqlParamters);
        }

        public void UpdateKnowChannelByUserID(string connectionStr, string guid,string channel)
        {
            string sSql = @"Update Tuhu_profiles.dbo.UserObject WITH ( rowLOCK )
                            set u_know_channel = @knowChannel
                            WHERE   u_user_id  = @userId";
            SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@knowChannel", channel) ,
                    new SqlParameter("@userId", guid) 
                };

            SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, sSql, sqlParamters);
        }

        public bool UpdateUserKnowChannelByUserId(string connectionStr, string EndUserGuid, string KnowChannel, out string errerMsg)
        {
            errerMsg = string.Empty; 
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                { 
                    new SqlParameter("@EndUserId", EndUserGuid),
                    new SqlParameter("@KnowChannel", KnowChannel) 
                };

                SqlHelper.ExecuteNonQuery(connectionStr, CommandType.StoredProcedure, "crm_UpdateUserKnowChannelByUserId", sqlParamters);
                return true;
            }
            catch (Exception ex)
            {
                errerMsg = string.Format("方法名称:UpdateUserKnowChannelByUserId,存储过程:crm_UpdateUserKnowChannelByUserId,参数:EndUserGuid={1},KnowChannel={2},错误信息:{0}", ex.ToString(), EndUserGuid, KnowChannel);
                return false;
            }
        }
    }
}