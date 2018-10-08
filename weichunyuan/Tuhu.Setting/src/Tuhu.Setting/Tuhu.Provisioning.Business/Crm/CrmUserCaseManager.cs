using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Crm
{
    public class CrmUserCaseManager : ConnectionBase
    {
        #region 单例
        private static CrmUserCaseManager _crmUserCase = null;
        public static CrmUserCaseManager CreateCrmUserCase
        {
            get
            {
                if (_crmUserCase == null)
                    return _crmUserCase = new CrmUserCaseManager();
                else
                    return _crmUserCase;
            }
        }
        public CrmUserCaseManager() { }
        #endregion

        #region  select
        /// <summary>
        /// 分页获取userCase数据集
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="orderBy">排序字段, a desc</param>
        /// <param name="totalRecord">返回数据总数</param>
        /// <param name="TotalPage">返回总页数</param>
        /// <param name="sqlWhere">条件:a=1 and b =2 ...</param>
        /// <param name="fields">显示字段a,b,c...</param>
        /// <returns>返回DataTable</returns>
        public DataTable SelectCrmUserCase(
            string tableName,
            int pageSize,
            int pageIndex,
            string orderBy,
            out int totalRecord,
            out int totalPage,
            string sqlWhere = null,
            string fields = "*")
        {
            int _totalRecord = -1;
            int _totalPage = -1;
            string errerMsg = null;
            sqlWhere = QueryParamsSerialize(sqlWhere);

            try
            {
                DataTable dataTable = DalCrmUserCase.CreateDalCrmUserCase.SelectCrmUserCase(connectionStr, tableName, pageSize, pageIndex, orderBy, out _totalRecord, out _totalPage, out errerMsg, sqlWhere, fields);
                totalRecord = _totalRecord;
                totalPage = _totalPage;
                return dataTable;
            }
            catch
            {
                totalRecord = _totalRecord;
                totalPage = _totalPage;
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 分页获取userCase数据集
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="orderBy">排序字段, a desc</param>
        /// <param name="totalRecord">返回数据总数</param>
        /// <param name="TotalPage">返回总页数</param>
        /// <param name="sqlWhere">条件:a=1 and b =2 ...</param>
        /// <param name="fields">显示字段a,b,c...</param>
        /// <returns>返回DataTable</returns>
        public DataTable VenderApiSelectCrmUserCase(
            string tableName,
            int pageSize,
            int pageIndex,
            string orderBy,
            List<SqlParameter> sqlParams,
            string sqlWhere = null,
            string fields = "*")
        {
            //int _totalRecord = -1;
            //int _totalPage = -1;
            string errerMsg = null;
            //sqlWhere = QueryParamsSerialize(sqlWhere);

            try
            {
                string connStr = SecurityHelp.IsBase64Formatted(connectionStr) ? SecurityHelp.DecryptAES(connectionStr) : connectionStr;
                DataTable dataTable = DalCrmUserCase.CreateDalCrmUserCase.VenderApiSelectCrmUserCase(connStr, tableName, pageSize, pageIndex, orderBy, out errerMsg, sqlParams, sqlWhere, fields);
                //totalRecord = _totalRecord;
                //totalPage = _totalPage;
                return dataTable;
            }
            catch
            {
                //totalRecord = _totalRecord;
                //totalPage = _totalPage;
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// dataTable 转化为list
        /// </summary>
        /// <param name="dtSources"></param>
        /// <returns></returns>
        public List<CrmUserCaseModel> SelectCrmUserCaseList(DataTable dtSources)
        {
            try
            {
                List<CrmUserCaseModel> crmUserCaseList = new List<CrmUserCaseModel>();
                if (dtSources != null && dtSources.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSources.Rows.Count; i++)
                    {
                        var dataRows = dtSources.Rows[i];
                        CrmUserCaseModel _crmUserCaseModel = new CrmUserCaseModel();

                        //NextContactTime
                        DateTime NextContactTime = DateTime.MinValue;
                        DateTime.TryParse(dataRows["NextContactTime"].ToString(), out NextContactTime);
                        _crmUserCaseModel.NextContactTime = NextContactTime;

                        //ForecastDate
                        DateTime ForecastDate = DateTime.MinValue;
                        DateTime.TryParse(dataRows["ForecastDate"].ToString(), out ForecastDate);
                        _crmUserCaseModel.ForecastDate = ForecastDate;

                        //Nickname
                        _crmUserCaseModel.Nickname = dataRows["Nickname"].ToString();

                        //Type
                        _crmUserCaseModel.Type = getType(dataRows["Type"].ToString());

                        //District
                        _crmUserCaseModel.District = dataRows["District"].ToString();

                        //first_name
                        _crmUserCaseModel.first_name = dataRows["first_name"].ToString();

                        //last_name
                        _crmUserCaseModel.last_name = dataRows["last_name"].ToString();

                        //carno
                        _crmUserCaseModel.carno = dataRows["carno"].ToString();

                        //cartype_description
                        _crmUserCaseModel.cartype_description = dataRows["cartype_description"].ToString();

                        //Products
                        _crmUserCaseModel.Products = dataRows["Products"].ToString();

                        //email
                        _crmUserCaseModel.email = dataRows["email"].ToString();

                        //telephone
                        _crmUserCaseModel.telephone = dataRows["telephone"].ToString();

                        //TaobaoId
                        _crmUserCaseModel.TaobaoId = dataRows["TaobaoId"].ToString();

                        //Q_Name
                        _crmUserCaseModel.Q_Name = dataRows["Q_Name"].ToString();

                        //Q_CarNo
                        _crmUserCaseModel.Q_CarNo = dataRows["Q_CarNo"].ToString();

                        //Q_Telephone
                        _crmUserCaseModel.Q_Telephone = dataRows["Q_Telephone"].ToString();

                        //CaseInterest
                        _crmUserCaseModel.CaseInterest = dataRows["CaseInterest"].ToString();

                        //StatusID
                        _crmUserCaseModel.StatusID = getCaseStatus(dataRows["StatusID"].ToString());

                        //Lastlog
                        _crmUserCaseModel.Lastlog = dataRows["Lastlog"].ToString();

                        //CaseLastModifiedTime
                        DateTime CaseLastModifiedTime = DateTime.MinValue;
                        DateTime.TryParse(dataRows["CaseLastModifiedTime"].ToString(), out CaseLastModifiedTime);
                        _crmUserCaseModel.CaseLastModifiedTime = CaseLastModifiedTime;

                        //CreateTime
                        DateTime CreateTime = DateTime.MinValue;
                        DateTime.TryParse(dataRows["CreateTime"].ToString(), out CreateTime);
                        _crmUserCaseModel.CreateTime = CreateTime;

                        //CaseGuid
                        _crmUserCaseModel.CaseGuid = dataRows["CaseGuid"].ToString();

                        //EndUserGuid
                        _crmUserCaseModel.EndUserGuid = dataRows["EndUserGuid"].ToString();

                        //isClosed
                        bool IsClosed = false;
                        bool.TryParse(dataRows["isClosed"].ToString(), out IsClosed);
                        _crmUserCaseModel.IsClosed = IsClosed;

                        //OwnerGuid
                        _crmUserCaseModel.OwnerGuid = dataRows["OwnerGuid"].ToString();

                        crmUserCaseList.Add(_crmUserCaseModel);
                    }
                    return crmUserCaseList;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 查询参数序列化为SQL条件
        /// </summary>
        /// <param name="paramsJson"></param>
        /// <returns></returns>
        public string QueryParamsSerialize(string queryParamsJson)
        {
            try
            {
                if (!string.IsNullOrEmpty(queryParamsJson))
                {
                    if (queryParamsJson.IndexOf("[") > 0 || queryParamsJson.IndexOf("]") > 0)
                        queryParamsJson = queryParamsJson.Replace("[", "{").Replace("]", "}");

                    string queryParamsSql = string.Empty;
                    QueryParamsModel pm = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryParamsModel>(queryParamsJson);

                    //input_IsInbound为True筛选所有信息
                    if (!pm.input_IsInbound)
                    {
                        if (!string.IsNullOrEmpty(pm.input_Status))
                            queryParamsSql += string.Format(" [StatusID] = '{0}' and ", pm.input_Status.Trim());

                        if (!string.IsNullOrEmpty(pm.input_Tasks))
                            queryParamsSql += string.Format(" [Tasks] = N'{0}' and ", pm.input_Tasks.Trim());

                        if (!string.IsNullOrEmpty(pm.input_Owner))
                            queryParamsSql += string.Format(" [OwnerGuid] = '{0}' and ", pm.input_Owner.Trim());
                    }

                    if (!pm.input_IsClosed)
                        queryParamsSql += " [isClosed] = 0 and ";
                    //判断Email是否存在
                    if (!string.IsNullOrEmpty(pm.input_Email))
                    {
                        queryParamsSql += string.Format(" [email] like N'%{0}%' and ", pm.input_Email.Trim());
                    }
                    //判断跟踪时间
                    if (!string.IsNullOrEmpty(pm.input_Start_Time) && !string.IsNullOrEmpty(pm.input_End_Time))
                    {
                        var _input_Start_Time = DateTime.Parse(pm.input_Start_Time).ToString("yyyy-MM-dd HH:mm:ss");
                        var _input_End_Time = DateTime.Parse(pm.input_End_Time).ToString("yyyy-MM-dd HH:mm:ss");
                        if (_input_Start_Time.CompareTo(_input_End_Time) <= 0)
                        {
                            queryParamsSql += string.Format("[NextContactTime] between '{0}' and '{1}' and", _input_Start_Time, _input_End_Time);
                            //queryParamsSql += string.Format("[CreateTime] between '{0}' and '{1}' and", _input_Start_Time, _input_End_Time);
                        }
                    }

                    if (!pm.input_IsInbound)
                        queryParamsSql += " [type] in('InBound','OutBound','Taobao') and ";

                    if (!string.IsNullOrEmpty(pm.input_Mobile))
                        queryParamsSql += string.Format(" [Mobile] = '{0}' and ", pm.input_Mobile.Trim());

                    if (!string.IsNullOrEmpty(pm.input_Tabo))
                        queryParamsSql += string.Format(" [TaoBaoId] = N'{0}' and ", pm.input_Tabo.Trim());

                    if (!string.IsNullOrEmpty(pm.input_Telphone))
                        queryParamsSql += string.Format(" [Telephone] = '{0}' and ", pm.input_Telphone.Trim());

                    if (!string.IsNullOrEmpty(pm.input_Carno))
                        queryParamsSql += string.Format(" [carno] like N'%{0}%' and ", pm.input_Carno.Trim());
                    if (!string.IsNullOrEmpty(pm.input_firstName))
                        queryParamsSql += string.Format(" (first_name LIKE N'%{0}%' OR last_name LIKE N'%{0}%' ) and ", pm.input_firstName.Trim());

                    if (queryParamsSql.LastIndexOf("and") > 0)
                        return queryParamsSql.Substring(0, queryParamsSql.LastIndexOf("and"));
                    else
                        return queryParamsSql;
                }
                else
                {
                    return "1=1";
                }
            }
            catch
            {
                return "1=1";
            }
        }

        /// <summary>
        /// 判断用户是否有未关闭case
        /// </summary>
        /// <param name="endUserGuid"></param>
        /// <returns></returns>
        public bool CheckUserHasOpenCase(string endUserGuid)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(endUserGuid))
                    return DalCrmUserCase.CreateDalCrmUserCase.CheckUserHasOpenCase(connectionStr, endUserGuid, out errerMsg);
                else
                    return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public string getType(string Type)
        {
            switch (Type)
            {
                case "InBound":
                    return "呼入";
                    break;
                case "OutBound":
                    return "呼出";
                    break;
                case "Taobao":
                    return "淘宝";
                    break;
                case "Survey-Bak":
                    return "问卷";
                    break;
                case "Survey-Error":
                    return "问卷错";
                    break;
                default:
                    return Type;
                    break;
            }
        }

        /// <summary>
        /// 状态转换
        /// </summary>
        /// <param name="strStatusID"></param>
        /// <returns></returns>
        public string getCaseStatus(string strStatusID)
        {
            int statusID = -1;
            int.TryParse(strStatusID, out statusID);
            switch (statusID)
            {
                case 1:
                    return "待拨打";
                    break;
                case 2:
                    return "待跟踪";
                    break;
                case 3:
                    return "已出单";
                    break;
                case 4:
                    return "信息错误";
                    break;
                case 5:
                    return "任务失败";
                    break;
                case 6:
                    return "完成安装";
                    break;
                case 7:
                    return "订单成功完成";
                    break;
                case 8:
                    return "订单取消";
                    break;
                default:
                    return strStatusID;
                    break;
            }
        }

        #endregion

        #region update

        /// <summary>
        /// 更新任务记录
        /// </summary>       
        /// <param name="endUserCase"></param>
        /// <returns></returns>
        public bool UpdateCrmUserCase(tbl_EndUserCaseModel endUserCase)
        {
            string errerMsg = null;
            try
            {
                if (endUserCase != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.UpdateCrmUserCase(connectionStr, endUserCase, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 更新 case tasks
        /// </summary>
        /// <param name="endUserCase"></param>
        /// <returns></returns>
        public bool UpdateCrmUserCaseTasks(tbl_EndUserCaseModel endUserCase)
        {
            string errerMsg = null;
            try
            {
                if (endUserCase != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.UpdateCrmUserCaseTasks(connectionStr, endUserCase, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="coconnectionStr"></param>
        /// <param name="userObject"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool UpdateUserObject(tbl_UserObjectModel userObject)
        {
            string errerMsg = null;
            try
            {
                if (userObject != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.UpdateUserObject(connectionStr, userObject, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        public bool UpdateUserLastName(tbl_UserObjectModel userObject)
        {
            string errerMsg = "";
            try
            {
                if (userObject != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.UpdateUserLastName(connectionStr, userObject, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }
        #endregion

        #region 判断用户淘宝ID和手机号是否唯一
        /// <summary>
        /// 判断用户淘宝ID和手机号是否唯一
        /// </summary>
        /// <param name="taobaoID"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool GetUserByTaoBaoIDAndMobileNumber(string taobaoID, string mobileNumber, string userID)
        {
            string errorMsg = "";
            try
            {
                if ((!string.IsNullOrEmpty(taobaoID) || !string.IsNullOrEmpty(mobileNumber)) && !string.IsNullOrEmpty(userID))
                    return DalCrmUserCase.CreateDalCrmUserCase.GetUserByTaoBaoIDAndMobileNumber(connectionStr, taobaoID, mobileNumber, userID, out errorMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errorMsg))
                    logger.Log(Level.Error, errorMsg);
            }
        }
        #endregion

        #region  获取Case信息
        /// <summary>
        /// 获取Case信息
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <returns></returns>
        public tbl_EndUserCaseModel GetCaseByCaseGuid(string caseGuid)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(caseGuid))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetCaseByCaseGuid(connectionStr, caseGuid, out errerMsg);
                    if (tableSource != null)
                    {
                        return (tbl_EndUserCaseModel)ModelConvertHelper<tbl_EndUserCaseModel>.ConvertToModel(tableSource)[0] ?? null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }
        #endregion

        #region 其他任务
        /// <summary>
        /// 其他任务
        /// </summary>
        /// <param name="endUserGuid"></param>
        /// <param name="caseGuid"></param>       
        /// <param name="day"></param>
        /// <returns></returns>
        public List<tbl_EndUserCaseModel> GetCaseByEndUserGuid(string endUserGuid, string caseGuid, int day = -30)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(endUserGuid))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetCaseByEndUserGuid(connectionStr, endUserGuid, caseGuid, out errerMsg, day);
                    if (tableSource != null)
                    {
                        return (List<tbl_EndUserCaseModel>)ModelConvertHelper<tbl_EndUserCaseModel>.ConvertToModel(tableSource);
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        //
        public List<tbl_EndUserCaseModel> GetCaseByEndUserGuid(string endUserGuid, string caseGuid, int statusid, int day = -30)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(endUserGuid))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetCaseByEndUserGuid(connectionStr, endUserGuid, caseGuid, statusid, out errerMsg, day);
                    if (tableSource != null)
                    {
                        return (List<tbl_EndUserCaseModel>)ModelConvertHelper<tbl_EndUserCaseModel>.ConvertToModel(tableSource);
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }


        #endregion

        #region 相关订单
        /// <summary>
        /// 相关订单
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<tbl_OrderModel> GetOrderByUserID(string userID, int day = -30)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(userID))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetOrderByUserID(connectionStr, userID, out errerMsg, day);
                    if (tableSource != null)
                    {
                        return (List<tbl_OrderModel>)ModelConvertHelper<tbl_OrderModel>.ConvertToModel(tableSource) ?? null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }
        #endregion

        #region 用户信息
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public tbl_UserObjectModel GetUserObjectByUserID(string userID)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(userID))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetUserObjectByUserID(connectionStr, userID, out errerMsg);
                    if (tableSource != null)
                    {
                        return (tbl_UserObjectModel)ModelConvertHelper<tbl_UserObjectModel>.ConvertToModel(tableSource)[0] ?? null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 通过用户email 获取用户信息
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public tbl_UserObjectModel GetUserObjectByEmail(string email)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetUserObjectByEmail(connectionStr, email, out errerMsg);
                    if (tableSource != null)
                    {
                        return (tbl_UserObjectModel)ModelConvertHelper<tbl_UserObjectModel>.ConvertToModel(tableSource)[0] ?? null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 查询所有信息
        /// </summary>
        public List<tbl_UserObjectModel> GetUserObjectByList()
        {
            string errerMsg = null;
            try
            {
                var tableList = DalCrmUserCase.CreateDalCrmUserCase.GetUserObjectByList(connectionStr, out errerMsg);
                if (tableList != null)
                {
                    return (List<tbl_UserObjectModel>)ModelConvertHelper<tbl_UserObjectModel>.ConvertToModel(tableList);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 通过手机号获取用户
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        public List<tbl_UserObjectModel> ProcGetUserProfileByPhoneNumber(string PhoneNumber)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    var dataSource = DalCrmUserCase.CreateDalCrmUserCase.ProcGetUserProfileByPhoneNumber(connectionStr, PhoneNumber, out errerMsg);
                    if (dataSource != null && dataSource.Rows.Count > 0)
                        return (List<tbl_UserObjectModel>)ModelConvertHelper<tbl_UserObjectModel>.ConvertToModel(dataSource);
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);

                return null;
            }
        }

        /// <summary>
        /// 通过手机号获取用户
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        public DataTable ProcGetUserProfileByPhoneNumberToTable(string PhoneNumber)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    var dataSource = DalCrmUserCase.CreateDalCrmUserCase.ProcGetUserProfileByPhoneNumber(connectionStr, PhoneNumber, out errerMsg);
                    if (dataSource != null && dataSource.Rows.Count > 0)
                        return dataSource;
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);

                return null;
            }
        }
        #endregion

        #region 日志信息
        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public List<tbl_LogModel> GetLogs(string caseGuid, string userGuid)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(caseGuid) && !string.IsNullOrEmpty(userGuid))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetLogs(connectionStr, caseGuid, userGuid, out errerMsg);
                    if (tableSource != null)
                    {
                        return (List<tbl_LogModel>)ModelConvertHelper<tbl_LogModel>.ConvertToModel(tableSource) ?? null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 创建日志记录(事物创建日志记录,修改tbl_EndUserCase表时间)
        /// </summary>
        /// <param name="logModel"></param>
        /// <returns></returns>
        public bool CreateLogs(tbl_LogModel logModel)
        {
            string errerMsg = null;
            try
            {
                if (logModel != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.CreateLogs(connectionStr, logModel, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 判断是否为Cases所有者,修改tbl_EndUserCase表的NextContactTime时间
        /// </summary>
        /// <param name="logModel"></param>
        /// <returns></returns>
        public bool CannotBeChanged_NextContactTime(tbl_LogModel logModel)
        {
            string errerMsg = null;
            try
            {
                if (logModel != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.CannotBeChanged_NextContactTime(connectionStr, logModel, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="logModel"></param>
        /// <returns></returns>
        public bool AddLogAsync(tbl_LogModel logModel)
        {
            string errerMsg = null;
            try
            {
                if (logModel != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.AddLogAsync(connectionStr, logModel, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 获取数据库 History 中的日志数据
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public List<tbl_LogModel> GetHistoryLogs(string caseGuid, string userGuid)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(caseGuid) && !string.IsNullOrEmpty(userGuid))
                {
                    var tableSource = DalCrmUserCase.CreateDalCrmUserCase.GetHistoryLogs(connectionStr, caseGuid, userGuid, out errerMsg);
                    if (tableSource != null)
                    {
                        return (List<tbl_LogModel>)ModelConvertHelper<tbl_LogModel>.ConvertToModel(tableSource) ?? null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }


        #endregion

        #region 创建任务
        /// <summary>
        /// 事物创建任务
        /// </summary>
        /// <param name="tbl_PotentialForm"></param>
        /// <param name="tbl_EndUserCase"></param>
        /// <returns></returns>
        public tbl_EndUserCaseModel CreateNewEndUserCase(tbl_PotentialFormModel tbl_PotentialForm, tbl_EndUserCaseModel tbl_EndUserCase)
        {
            string errerMsg = null;
            try
            {
                if (tbl_PotentialForm != null && tbl_EndUserCase != null)
                {
                    var isCreateNewEndUserCase = DalCrmUserCase.CreateDalCrmUserCase.CreateNewEndUserCase(connectionStr, tbl_PotentialForm, tbl_EndUserCase, out errerMsg);
                    if (isCreateNewEndUserCase)
                    {
                        return tbl_EndUserCase;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }
        #endregion

        #region  信息确认

        /// <summary>
        /// 检测case是否存在
        /// </summary>
        /// <param name="CaseId"></param>
        /// <param name="PotentialFormId"></param>
        /// <returns></returns>
        public bool CheckEndUserCaseExisted(Guid CaseId, Guid PotentialFormId)
        {
            string errerMsg = null;
            try
            {
                if (CaseId != null && PotentialFormId != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.CheckEndUserCaseExisted(connectionStr, CaseId, PotentialFormId, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 淘宝ID获取用户信息
        /// </summary>      
        /// <param name="TaobaoId"></param>      
        /// <returns></returns>
        public bool ProcGetUserProfileByTaobaoId(string TaobaoId, out DataTable UserCaseTable)
        {
            UserCaseTable = null;
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(TaobaoId))
                {
                    var dataSource = DalCrmUserCase.CreateDalCrmUserCase.ProcGetUserProfileByTaobaoId(connectionStr, TaobaoId, out errerMsg);
                    if (dataSource != null && dataSource.Rows.Count > 0)
                    {
                        UserCaseTable = dataSource;
                        if (dataSource.Rows[0]["u_user_id"] != null && !string.IsNullOrEmpty(dataSource.Rows[0]["u_user_id"].ToString()))
                        {
                            return CheckUserHasOpenCase(dataSource.Rows[0]["u_user_id"].ToString().Trim());
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 电话号码获取用户信息
        /// </summary>              
        /// <param name="PhoneNumber"></param>   
        /// <returns></returns>
        public bool ProcGetUserProfileByPhoneNumber(string PhoneNumber, out DataTable UserCaseTable)
        {
            UserCaseTable = null;
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    var dataSource = DalCrmUserCase.CreateDalCrmUserCase.ProcGetUserProfileByPhoneNumber(connectionStr, PhoneNumber, out errerMsg);
                    if (dataSource != null && dataSource.Rows.Count > 0)
                    {
                        UserCaseTable = dataSource;
                        if (dataSource.Rows[0]["u_user_id"] != null && !string.IsNullOrEmpty(dataSource.Rows[0]["u_user_id"].ToString()))
                        {
                            return CheckUserHasOpenCase(dataSource.Rows[0]["u_user_id"].ToString().Trim());
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// Creates CRM user and associates this user with one case.
        /// </summary>
        /// <param name="UserObjectModel"></param>
        /// <returns></returns>
        public bool AddUserProfileAndBindCase(tbl_UserObjectModel UserObjectModel, string CaseId)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(CaseId) && UserObjectModel != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.AddUserProfileAndBindCase(connectionStr, UserObjectModel, CaseId, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 修改case状态
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="endUserId"></param>
        public bool AddEndUserForCase(Guid caseId, Guid endUserId, string knowChannel, int type)
        {
            string errerMsg = null;
            try
            {
                if (caseId != null && endUserId != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.AddEndUserForCase(connectionStr, caseId, endUserId, knowChannel, type, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 关闭EndUserCase
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="reason"></param>
        /// <param name="caseStatus"></param>
        /// <returns></returns>
        public bool CloseEndUserCase(Guid caseId, string reason, int caseStatus)
        {
            string errerMsg = null;
            try
            {
                if (caseId != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.CloseEndUserCase(connectionStr, caseId, reason, caseStatus, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 关闭PotentialForm
        /// </summary>
        /// <param name="potentialFormId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool ClosePotentialForm(Guid potentialFormId, string reason)
        {
            string errerMsg = null;
            try
            {
                if (potentialFormId != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.ClosePotentialForm(connectionStr, potentialFormId, reason, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 执行事务，关闭EndUserCase 与 PotentialForm
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="potentialFormId"></param>
        /// <param name="reason"></param>
        /// <param name="caseStatus"></param>
        /// <returns></returns>
        public bool TranCloseFormAsWrong(Guid caseId, Guid potentialFormId, string reason, int caseStatus)
        {
            string errerMsg = null;
            try
            {
                if (potentialFormId != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.TranCloseFormAsWrong(connectionStr, caseId, potentialFormId, reason, caseStatus, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        /// <summary>
        /// 下次联系(修改case状态)
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="reason"></param>
        /// <param name="caseStatus"></param>
        /// <returns></returns>
        public bool ChangeEndUserCaseStatus(Guid caseId, string reason, int caseStatus)
        {
            string errerMsg = null;
            try
            {
                if (caseId != null)
                    return DalCrmUserCase.CreateDalCrmUserCase.ChangeEndUserCaseStatus(connectionStr, caseId, reason, caseStatus, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }
        #endregion

        #region 修改任务状态
        /// <summary>
        /// 修改任务状态
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <param name="caseStatus"></param>
        /// <param name="isClosed"></param>       
        /// <returns></returns>
        public bool UpdateEndUserCaseStatusID(string caseGuid, int caseStatus, int isClosed)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(caseGuid))
                    return DalCrmUserCase.CreateDalCrmUserCase.UpdateEndUserCaseStatusID(connectionStr, caseGuid, caseStatus, isClosed, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }
        #endregion

        #region 分配任务
        /// <summary>
        /// 分配任务
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <returns></returns>
        public bool UpdateEndUserCaseOwnerGuid(string caseGuid, string ownerGuid)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(caseGuid))
                {
                    return DalCrmUserCase.CreateDalCrmUserCase.UpdateEndUserCaseOwnerGuid(connectionStr, caseGuid, ownerGuid, out errerMsg);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }
        #endregion


        #region 优惠券
        public List<CrmPromotionCodeModel> GetUserCouponImportant(string userID)
        {
            string errerMsg = null;
            try
            {
                if (!string.IsNullOrEmpty(userID))
                {
                    var dataSource = DalCrmUserCase.CreateDalCrmUserCase.GetUserCouponImportant(connectionStr, userID, out errerMsg);
                    if (dataSource != null && dataSource.Rows.Count > 0)
                        return (List<CrmPromotionCodeModel>)ModelConvertHelper<CrmPromotionCodeModel>.ConvertToModel(dataSource);
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);

                return null;
            }
        }
        #endregion

        public string GetUserKnowChannelByTphone(string tphone)
        {
            try
            {
                return DalCrmUserCase.CreateDalCrmUserCase.GetUserKnowChannelByTphone(connectionStr, tphone);

            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public void UpdateKnowChannelByUserID(string guid, string channel)
        {
            try
            {
                DalCrmUserCase.CreateDalCrmUserCase.UpdateKnowChannelByUserID(connectionStr, guid, channel);

            }
            catch (Exception ex)
            {

            }
        }

        public bool UpdateUserKnowChannelByUserId(string EndUserGuid, string KnowChannel)
        {
            string errerMsg = null;
            try
            {
                if (EndUserGuid != null && KnowChannel != "")
                    return DalCrmUserCase.CreateDalCrmUserCase.UpdateUserKnowChannelByUserId(connectionStr, EndUserGuid, KnowChannel, out errerMsg);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errerMsg))
                    logger.Log(Level.Error, errerMsg);
            }
        }

        public DataTable GetUserObjectInfoByPhone(string phone)
        {
            try
            {
                return DalCrmAddress.CreateDalCrmAddress.GetUserObjectInfoByPhone(connectionStr, phone);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 查询条件序列化
    /// </summary>
    public class QueryParamsModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string input_Status { get; set; }
        /// <summary>
        /// 明细
        /// </summary>
        public string input_Tasks { get; set; }
        /// <summary>
        /// 所有者
        /// </summary>
        public string input_Owner { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string input_Mobile { get; set; }
        /// <summary>
        /// 淘宝ID
        /// </summary>
        public string input_Tabo { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string input_Telphone { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        public string input_Carno { get; set; }
        /// <summary>
        /// 是否呼入
        /// </summary>
        public bool input_IsInbound { get; set; }
        /// <summary>
        /// 是否关闭
        /// </summary>
        public bool input_IsClosed { get; set; }
        /// <summary>
        /// 创建开始时间
        /// </summary>
        public string input_Start_Time { get; set; }

        /// <summary>
        /// 创建结束时间
        /// </summary>
        public string input_End_Time { get; set; }

        /// <summary>
        /// Email地址
        /// </summary>
        public string input_Email { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string input_firstName { get; set; }
    }
}