using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalEmployee
    {
        public static void OnDutyEmployee(SqlConnection connection, string employeeEmail)
        {
            var parameters = new[]
            {
                new SqlParameter("@EmployeeEmail", employeeEmail)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "procOnDutyEmployeeByEmployeeEmail",
                parameters);
        }

        public static void OffDutyEmployee(SqlConnection connection, string employeeEmail)
        {
            var parameters = new[]
            {
                new SqlParameter("@EmployeeEmail", employeeEmail)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "procOffDutyEmployeeByEmployeeEmail",
                parameters);
        }

        public static string getMobileByEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return "";
            }
            var sql = string.Format("select Mobile from HrEmployee where EmailAddress='{0}'", emailAddress);
            return Tuhu.Component.Common.DbHelper.ExecuteScalar(sql).ToString();
        }

        public static List<BizHrEmployee> SeleBizHrEmployees(SqlConnection connection)
        {
            var sql = "SELECT HE.PKID,HE.EmployeeName,HE.Department,HE.DepartmentName, HE.EmailAddress FROM dbo.HrEmployee AS HE WITH (NOLOCK)";

            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<BizHrEmployee>().ToList();
        }
        public static List<BizHrEmployee> FilterColumnOfHrEmployee(SqlConnection connection, string col, string filter)
        {
            string sql = @"SELECT " + col + " FROM dbo.HrEmployee WITH (NOLOCK)  where 1=1 " + filter;
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<BizHrEmployee>().ToList();
        }

        /// <summary>
        /// 获取组
        /// </summary>
        /// <param name="EmailAddress"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static List<BizHrEmployeeGroup> SelectBizHrEmployeeGroupByEP(string EmailAddress, int? ParentID)
        {
            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", string.IsNullOrEmpty(EmailAddress)?"":EmailAddress),
                new SqlParameter("@ParentID", ParentID.GetValueOrDefault(-100)),
            };

            var sql = @"  SELECT	EmailAddress,
			                        ParentID
                          FROM		HrEmployeeGroup WITH(NOLOCK)
                          WHERE		EmailAddress = @EmailAddress
			                        AND ParentID = @ParentID";

            return DbHelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BizHrEmployeeGroup>().ToList();
        }

        /// <summary>
        /// 查询仓库所在的人员
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static List<BizHrEmployee> SelectWareHouseEmployeesByShopId(SqlConnection connection, int shopId)
        {
            var employees = new List<BizHrEmployee>();

            var parameters = new[]
            {
                new SqlParameter("@ShopID",shopId)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "WMSUserObject_SelectUserListByShopID", parameters))
            {
                while (dataReader.Read())
                {
                    var employee = new BizHrEmployee();
                    employee.EmailAddress = dataReader.GetTuhuString(0);
                    employee.EmployeeName = dataReader.GetTuhuString(1);
                    employees.Add(employee);
                }
            }

            return employees;
        }

        public static void AddWMSUserObject(SqlConnection con, WMSUserObject wmsUser)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@EmailAddress",wmsUser.EmailAddress),
                new SqlParameter("@ShopID",wmsUser.ShopID)
            };
            SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "WMSUserObject_InsertWMSUserObject", sqlParamters);
        }

        public static WMSUserObject SelectWMSUserObjectByEmailAddressAndShopID(SqlConnection connection, string emailAddress, int shopID)
        {
            WMSUserObject wmsUser = null;

            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", emailAddress),
                new SqlParameter("@ShopID", shopID)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "WMSUserObject_SelectWMSUserObjectByEmailAddressAndShopID", parameters))
            {
                if (dataReader.Read())
                {
                    wmsUser = new WMSUserObject()
                    {
                        PKID = dataReader.GetTuhuValue<int>(0),
                        EmailAddress = dataReader.GetTuhuString(1),
                        ShopID = dataReader.GetTuhuValue<int>(2),
                        IsActive = dataReader.GetTuhuValue<bool>(3),
                        LastUpdate = dataReader.GetTuhuValue<DateTime>(4)
                    };

                }
            }

            return wmsUser;
        }

        public static void UpdateWMSUserObjectByShopId(SqlConnection con, int shopId)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@ShopId",shopId)
            };
            SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "WMSUserObject_UpdateWMSUserObjectByShopId",
                sqlParamters);
        }

        public static void UpdateWMSUserObjectByEmailAddressAndShopId(SqlConnection con, string emailAddress, int shopId)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@EmailAddress",emailAddress),
                new SqlParameter("@ShopId",shopId),
            };
            SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure,
                "WMSUserObject_UpdateWMSUserObjectByEmailAddressAndShopId", sqlParamters);
        }

        public static List<WMSUserObject> SelectShopsEmployeeByEmailAddress(SqlConnection connection, string emailAddress)
        {
            var wmsUsers = new List<WMSUserObject>();

            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", emailAddress)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "WMSUserObject_SelectShopsEmployeeByEmailAddress", parameters))
            {
                while (dataReader.Read())
                {
                    var employee = new WMSUserObject()
                    {
                        ShopID = dataReader.GetTuhuValue<int>(0),
                        ShopsNo = dataReader.GetTuhuValue<int>(1),
                        ShopsName = dataReader.GetTuhuString(2)
                    };
                    wmsUsers.Add(employee);
                }
            }

            return wmsUsers;
        }
        /// <summary>
        /// 员工信息表
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="conditions"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static List<HrEmploryee> SelectHrEmployeeList(SqlConnection connection, int pageIndex, string conditions, List<SqlParameter> paramList)
        {
            string startPage = Convert.ToString(20 * (pageIndex - 1));
            string endPage = Convert.ToString(20 * pageIndex + 1);

            List<SqlParameter> sqlParams = new List<SqlParameter>();

            var sqlParameters = new[] { new SqlParameter("@StartPage", startPage), new SqlParameter("@EndPage", endPage) };
            var list = new List<HrEmploryee>();

            sqlParams.AddRange(sqlParameters);
            sqlParams.AddRange(paramList);

            //  using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "HrEmployee_SelectList", sqlParameters))
            //{

            string sql = @"SELECT	Tb.PKID,
		Tb.EmployeeID,
		Tb.EmployeeName,
		Tb.Department,
		Tb.DepartmentName,
		Tb.Mobile,
		Tb.Tel,
		Tb.EmailAddress,
		Tb.Remark,
		Tb.IsActive,
		Tb.Roles,
		Tb.RolesName
FROM	( SELECT	HE.PKID,
					HE.EmployeeID,
					HE.EmployeeName,
					HE.Department,
					HE.DepartmentName,
					HE.Mobile,
					HE.Tel,
					HE.EmailAddress,
					HE.Remark,
					HE.IsActive,
					HE.Roles,
					HE.RolesName,
					ROW_NUMBER() OVER ( ORDER BY HE.PKID ) AS RowNum
		  FROM		dbo.HrEmployee AS HE WITH ( NOLOCK )
		  WHERE		1 = 1 " + conditions +
        ") AS Tb WHERE	Tb.RowNum < @EndPage AND Tb.RowNum > @StartPage";

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParams.ToArray()))
            {
                while (reader.Read())
                {
                    var model = new HrEmploryee
                    {
                        PKID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                        EmployeeID = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        EmployeeName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Department = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        DepartmentName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        Mobile = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        Tel = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                        EmailAddress = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                        Remark = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                        IsActive = reader.IsDBNull(9) ? false : reader.GetBoolean(9),
                        Roles = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                        RolesName = reader.IsDBNull(11) ? string.Empty : reader.GetString(11)
                    };
                    list.Add(model);
                }
            }
            return list;
        }
        /// <summary>
        /// 员工信息总行数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public static int SelectHrEmployeeListCount(SqlConnection connection, string conditions, List<SqlParameter> sqlParams)
        {
            // var sqlPameters = new SqlParameter("@Conditions", conditions);

            string sql = @"SELECT COUNT(1) FROM Gungnir.dbo.HrEmployee AS HE WITH ( NOLOCK ) WHERE	1 = 1 " + conditions;

            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, sqlParams.ToArray()));

            // return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, "HrEmployee_SelectListCount", sqlParams.ToArray()));
        }
        /// <summary>
        /// 得到所有的员工类型（部门）
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetAllOfTheDepartment(SqlConnection connection)
        {
            const string sql = "SELECT HG.PKID,HG.Name FROM dbo.HrGroup AS HG WITH ( NOLOCK )";
            var dicDepartment = new Dictionary<int, string> { { -1, "请选择" } };
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    dicDepartment.Add(reader.IsDBNull(0) ? 0 : reader.GetInt32(0), reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                }
            }
            return dicDepartment;
        }
        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int DelTheHrEmployee(SqlConnection connection, int id)
        {
            const string sql = "DELETE FROM dbo.HrEmployee WHERE PKID = @id";
            var sqlParameters = new SqlParameter("@id", id);
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParameters);
        }
        /// <summary>
        /// 根据部门ID得到部门信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="hrGroupID"></param>
        /// <returns></returns>
        public static HrDepartment SelectHrGroupByHrGroupID(SqlConnection connection, int hrGroupID)
        {
            var model = new HrDepartment();
            const string sql = "SELECT HG.Name,HG.Parent,HG.PKID FROM dbo.HrGroup AS HG WITH(NOLOCK) WHERE PKID = @hrGroupID";
            var sqlParameter = new SqlParameter("@hrGroupID", hrGroupID);
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameter))
            {
                if (reader.Read())
                {
                    model.Name = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    model.Parent = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                    model.PKID = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                }
            }
            return model;
        }
        /// <summary>
        /// 员工如果存在角色，则显示角色名称
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="groupID"></param>
        /// <param name="rolesID"></param>
        /// <returns></returns>
        public static string GetHrRolesName(SqlConnection connection, int groupID, int rolesID)
        {
            const string sql = "SELECT HR.Name FROM dbo.HrRoles AS HR WITH(NOLOCK) WHERE PKID = @PKID AND GroupID = @GroupID";
            var slqParameters = new[]
            {
                new SqlParameter("@PKID",rolesID),
                new SqlParameter("@GroupID",groupID)
            };
            return Convert.ToString(SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, slqParameters));
        }
        /// <summary>
        /// 根据角色ID得到角色名称
        /// </summary>
        /// <param name="connetion"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public static string GetRoleNameByRoleID(SqlConnection connection, int roleID)
        {
            const string sql = "SELECT HR.Name FROM dbo.HrRoles AS HR WITH ( NOLOCK ) WHERE HR.PKID = @PKID";
            var sqlParameter = new SqlParameter("@PKID", roleID);
            return Convert.ToString(SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, sqlParameter));
        }
        /// <summary>
        /// 根据父部门ID得到所有的子部门名称
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetGroupNameByGroupID(SqlConnection connection, int groupID)
        {
            const string sql = "SELECT HG.PKID,HG.Name FROM dbo.HrGroup AS HG WITH(NOLOCK) WHERE HG.Parent = @Parent";
            var sqlParameters = new SqlParameter("@Parent", groupID);
            var dic = new Dictionary<int, string>() { { -1, "请选择" } };
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameters))
            {
                while (reader.Read())
                {
                    dic.Add(reader.IsDBNull(0) ? 0 : reader.GetInt32(0), reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                }
            }
            return dic;
        }
        /// <summary>
        /// 根据部门ID得到角色名称
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetRoleNameByGoupID(SqlConnection connection, int groupID)
        {
            const string sql = "SELECT HR.PKID,HR.Name FROM dbo.HrRoles AS HR WITH(NOLOCK) WHERE HR.GroupID = @GroupID";
            var sqlParameter = new SqlParameter("@GroupID", groupID);
            var dic = new Dictionary<int, string>() { { -1, "请选择" } };
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameter))
            {
                while (reader.Read())
                {
                    dic.Add(reader.IsDBNull(0) ? 0 : reader.GetInt32(0), reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                }
            }
            return dic;
        }
        /// <summary>
        /// 新增员工信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="he"></param>
        public static void AddTheHrEmployee(SqlConnection connection, HrEmploryee he)
        {
            const string sql = @"
INSERT INTO dbo.HrEmployee
		( EmployeeID,
		  EmployeeName,
		  Department,
		  DepartmentName,
		  Mobile,
		  Tel,
		  EmailAddress,
		  Remark,
		  IsActive,
		  Roles,
		  RolesName
		)
VALUES	( @EmployeeID, -- EmployeeID - nvarchar(20)
		  @EmployeeName, -- EmployeeName - nvarchar(50)
		  @Department, -- Department - nvarchar(500)
		  @DepartmentName, -- DepartmentName - nvarchar(50)
		  @Mobile, -- Mobile - varchar(50)
		  @Tel, -- Tel - varchar(50)
		  @EmailAddress, -- EmailAddress - varchar(50)
		  @Remark, -- Remark - nvarchar(50)
		  @IsActive, -- IsActive - bit
		  @Roles, -- Roles - nvarchar(500)
		  @RolesName  -- RolesName - nvarchar(500)
		)";
            var sqlParameters = new[]{
                new SqlParameter("@EmployeeID",he.EmployeeID),
                new SqlParameter("@EmployeeName",he.EmployeeName),
                new SqlParameter("@Department",he.Department),
                new SqlParameter("@DepartmentName",he.DepartmentName),
                new SqlParameter("@Mobile",he.Mobile),
                new SqlParameter("@Tel",he.Tel),
                new SqlParameter("@EmailAddress",he.EmailAddress),
                new SqlParameter("@Remark",he.Remark),
                new SqlParameter("@IsActive",he.IsActive),
                new SqlParameter("@Roles",he.Roles),
                new SqlParameter("@RolesName",he.RolesName)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParameters);
        }
        /// <summary>
        /// 编辑员工信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="he"></param>
        public static void EditTheHrEmployee(SqlConnection connection, HrEmploryee he)
        {
            const string sql = @"   UPDATE	dbo.HrEmployee
                                    SET		EmployeeID = @EmployeeID,
		                                    EmployeeName = @EmployeeName,
		                                    Department = @Department,
		                                    DepartmentName = @DepartmentName,
		                                    Mobile = @Mobile,
		                                    Tel = @Tel,
		                                    EmailAddress = @EmailAddress,
		                                    Remark = @Remark,
		                                    IsActive = @IsActive,
		                                    Roles = @Roles,
		                                    RolesName = @RolesName
                                    WHERE	PKID = @PKID";
            var sqlParameters = new[]
            {
                new SqlParameter("@EmployeeID",he.EmployeeID),
                new SqlParameter("@EmployeeName",he.EmployeeName),
                new SqlParameter("@Department",he.Department),
                new SqlParameter("@DepartmentName",he.DepartmentName),
                new SqlParameter("@Mobile",he.Mobile),
                new SqlParameter("@Tel",he.Tel),
                new SqlParameter("@EmailAddress",he.EmailAddress),
                new SqlParameter("@Remark",he.Remark),
                new SqlParameter("@IsActive",he.IsActive),
                new SqlParameter("@Roles",he.Roles),
                new SqlParameter("@RolesName",he.RolesName),
                new SqlParameter("@PKID",he.PKID)
            };
            var result = SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParameters);
            if (result > 0)
            {
                var editSql =
                    @"UPDATE Tuhu_profiles..UserObject SET u_tel_number=@Tel,u_mobile_number=@Mobile WHERE u_email_address=@EmailAddress";
                SqlHelper.ExecuteNonQuery(connection, CommandType.Text, editSql, sqlParameters);
            }
        }
        /// <summary>
        /// 根据邮箱地址查询姓名
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static string GetEmployeeNameByEmailAddress(SqlConnection connection, string emailAddress)
        {
            var employeeName = "";
            string sql = "SELECT EmployeeName FROM dbo.HrEmployee WITH(NOLOCK) WHERE EmailAddress=@EmailAddress";
            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", emailAddress)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, parameters))
            {
                while (dataReader.Read())
                {
                    employeeName = dataReader.GetTuhuString(0);
                }
            }
            return employeeName;
        }
        public static string GetEmployeeNameByMobile(SqlConnection connection, string Mobile, int Department)
        {
            var employeeName = "";
            string sql = "SELECT top 1 EmployeeName FROM dbo.HrEmployee WHERE Mobile=@Mobile ";
            if (Department > 0)
            {
                sql += " AND Department like @Department ";
            }
            var parameters = new[]
            {
                new SqlParameter("@Mobile", Mobile),
                new SqlParameter("@Department", "%" + Department.ToString() + "%")
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, parameters))
            {
                while (dataReader.Read())
                {
                    employeeName = dataReader.GetTuhuString(0);
                }
            }
            return employeeName;
        }

        public static bool IsActiveByGroups(SqlConnection con, string groups, string userName)
        {
            var sp = new[] { new SqlParameter("@HrGroups", groups), new SqlParameter("@userName", userName) };
            DataRow dr = SqlHelper.ExecuteDataRow(con, CommandType.StoredProcedure, "[HrEmployee_IsActiveByGroups]", sp);
            int i = 0;
            int.TryParse(dr["ok"].ToString(), out i);
            return i > 0;
        }

        /// <summary>
        /// 判断Hremployee表里是否已经存在了该邮箱
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="emailAddree"></param>
        /// <returns></returns>
        public static bool IsEmailAddressExist(SqlConnection connection, string emailAddree)
        {
            string sql = string.Format(@"
SELECT	1
FROM	dbo.HrEmployee AS HE WITH ( NOLOCK )
WHERE	HE.EmailAddress = '{0}'", emailAddree);
            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.Text, sql)) > 0;
        }

        /// <summary>
        /// 判断Hremployee表里是否存在该手机号
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobileExist(SqlConnection connection, string mobile)
        {
            string sql = string.Format(@"
SELECT	1
FROM	dbo.HrEmployee AS HE WITH ( NOLOCK )
WHERE	HE.Mobile = '{0}'", mobile);
            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.Text, sql)) > 0;
        }


        public static List<HrEmploryee> GetAllEmploryee()
        {
            string sSql = @" SELECT	 
					            HE.EmployeeName,
					            HE.Department
		                     FROM dbo.HrEmployee AS HE WITH ( NOLOCK )";
            return DbHelper.ExecuteDataTable(sSql, CommandType.Text).ConvertTo<HrEmploryee>().ToList();
        }
        public static List<UserGroups> GetAllUserGroups(SqlConnection con)
        {
            string sql = "  SELECT a.PKID,a.EmployeeID,a.EmployeeName,a.EmailAddress,b.Name GroupName FROM dbo.HrEmployee a WITH(NOLOCK) LEFT JOIN dbo.HrGroup b WITH(NOLOCK) ON a.DepartmentName LIKE N'%'+b.Name+'%'";
            return SqlHelper.ExecuteDataTable(con, CommandType.Text, sql).ConvertTo<UserGroups>().ToList();
        }
        public static List<GroupsInfo> GetAllGroupsByGroupNames(SqlConnection con, string groupNames)
        {
            string sql = "SELECT a.PKID ParentId,a.Name ParentName,b.PKID ChildId,b.Name ChildName FROM dbo.HrGroup a JOIN dbo.HrGroup b ON a.PKID=b.Parent WHERE  a.Name IN (SELECT * FROM dbo.Split(@groupsname,','))";
            var sp = new[] { new SqlParameter("@groupsname", groupNames) };
            return SqlHelper.ExecuteDataTable(con, CommandType.Text, sql, sp).ConvertTo<GroupsInfo>().ToList();
        }
        public static List<HrEmploryee> GetAllUserByGroupId(SqlConnection con, int groupId)
        {
            string sql = "HrEmployee_SelectUsersByAllGroup";
            var sp = new[] { new SqlParameter("@groupId", groupId) };
            return SqlHelper.ExecuteDataTable(con, CommandType.StoredProcedure, sql, sp).ConvertTo<HrEmploryee>().ToList();
        }
        public static List<HrEmploryee> GetEmailByDepartmentName(SqlConnection conn)
        {
            string sql = @"SELECT	
		Tb.EmailAddress
	
FROM	( SELECT	HE.PKID,
					HE.EmployeeID,
					HE.EmployeeName,
					HE.Department,
					HE.DepartmentName,
					HE.Mobile,
					HE.Tel,
					HE.EmailAddress,
					HE.Remark,
					HE.IsActive,
					HE.Roles,
					HE.RolesName,
					ROW_NUMBER() OVER ( ORDER BY HE.PKID ) AS RowNum
		  FROM		dbo.HrEmployee AS HE WITH ( NOLOCK )
		  WHERE		1 = 1 
		) AS Tb

		WHERE  CHARINDEX(N'电销管理组',Tb.DepartmentName)>0";

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<HrEmploryee>().ToList();
        }
        /// <summary>
        /// 根据部门名称获取员工
        /// </summary>
        public static List<HrEmploryee> GetEmployeesByDeptName(SqlConnection conn, string deptName)
        {
            const string commandText = @"SELECT  he.PKID ,
                                                                        he.EmailAddress ,
                                                                        he.EmployeeName ,
                                                                        he.Department ,
                                                                        he.DepartmentName
                                                                FROM    dbo.HrEmployee AS he WITH ( NOLOCK )
                                                                WHERE   he.IsActive = 1
                                                                        AND ( EXISTS ( SELECT   1
                                                                                       FROM     dbo.HrGroup AS hg WITH ( NOLOCK )
                                                                                       WHERE    hg.Name = @DeptName
                                                                                                AND CHARINDEX(',' + CAST(hg.PKID AS NVARCHAR(20)) + ',', ',' + he.Department + ',') > 0 ) ); ";
            return
                SqlHelper.ExecuteDataTable2(conn, CommandType.Text, commandText, new SqlParameter("@DeptName", deptName))
                    .ConvertTo<HrEmploryee>()
                    .ToList();
        }

        /// <summary>
        /// 根据账号更新TOKEN
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="token">pwd</param>
        /// <param name="email">账号</param>
        public static void UpdateEmployeeTokenPwd(SqlConnection sqlconnection, string token, string email)
        {
            string sql = "update HrEmployee with(rowlock) set TokenPwd=@TokenPwd where IsActive=1 and EmailAddress=@EmailAddress";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TokenPwd", token));
            parameters.Add(new SqlParameter("@EmailAddress", email));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 根据邮箱地址查询员工
        /// </summary>
        public static HrEmploryee GetEmployeeByEmailAddress(SqlConnection connection, string emailAddress)
        {
            var employee = new HrEmploryee();
            string sql = "SELECT PKID,EmployeeName,Mobile,EmailAddress FROM dbo.HrEmployee WITH(NOLOCK) WHERE EmailAddress=@EmailAddress";
            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", emailAddress)
            };

            using (var reader = SqlHelper.ExecuteReaderV2(connection, CommandType.Text, sql, parameters))
            {
                if (reader.Read())
                {
                    employee.PKID = reader.GetTuhuValue<int>(0);
                    employee.EmployeeName = reader.GetTuhuString(1);
                    employee.Mobile = reader.GetTuhuString(2);
                    employee.EmailAddress = reader.GetTuhuString(3);
                }
            }
            return employee;
        }
        public static List<string> GetEmailAddressByGroups(SqlConnection con, string groups)
        {
            var sp = new[] { new SqlParameter("@HrGroups", groups) };
            List<string> ls = new List<string>();
            using (var dataReader = SqlHelper.ExecuteReader(con, CommandType.StoredProcedure, "[Order_Permissions_SelectEmailAddress]", sp))
            {
                while (dataReader.Read())
                {
                    ls.Add(dataReader.GetTuhuString(0));
                }
            }
            return ls;
        }

        public static List<HrEmploryee> SelectEmployeeNameAndDepartmentNameByEmailAddress(SqlConnection conn, List<string> emailAddressList)
        {
            var hrEmploryeeList = new List<HrEmploryee>();
            var emailAddressTable = new DataTable();
            emailAddressTable.Columns.Add("EmailAddress", Type.GetType("System.String"));
            emailAddressList.ForEach(item =>
            {
                emailAddressTable.Rows.Add(emailAddressTable.NewRow()[0] = item);
            });
            var parm = new[]
            {
                new SqlParameter("@EmailAddressList", emailAddressTable)
            };

            using (var dataReader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "HrEmploryee_SelectEmployeeNameAndDepartmentNameByEmailAddress", parm))
            {
                while (dataReader.Read())
                {
                    var hrEmploryee = new HrEmploryee();
                    hrEmploryee.EmployeeName = dataReader.GetTuhuString(0);
                    hrEmploryee.DepartmentName = dataReader.GetTuhuString(1);
                    hrEmploryee.EmailAddress = dataReader.GetTuhuString(2);
                    hrEmploryeeList.Add(hrEmploryee);
                }
            }
            return hrEmploryeeList;
        }
    }
}
