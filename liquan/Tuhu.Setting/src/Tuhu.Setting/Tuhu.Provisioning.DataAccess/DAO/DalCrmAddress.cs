using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// CRM 地址信息
    /// </summary>
    public class DalCrmAddress
    {
        #region 单例
        private static DalCrmAddress _DalCrmAddress = null;
        public static DalCrmAddress CreateDalCrmAddress
        {
            get
            {
                if (_DalCrmAddress == null)
                    return _DalCrmAddress = new DalCrmAddress();
                else
                    return _DalCrmAddress;
            }
        }
        private DalCrmAddress() { }
        #endregion

        #region  BasicMethod

        /// <summary>
        /// 增加一条数据mo
        /// </summary>
        public bool Add(AddressesModel model, string connectionStr, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into Tuhu_profiles.dbo.Addresses(");
                strSql.Append("u_address_id,i_address_type,u_address_name,u_first_name,u_last_name,u_description,u_address_line1,u_address_line2,u_city,u_region_code,u_region_name,u_postal_code,u_country_code,u_country_name,u_tel_number,u_tel_extension,i_locale,u_user_id_changed_by,dt_date_last_changed,dt_csadapter_date_last_changed,dt_date_created)");
                strSql.Append(" values (");
                strSql.Append("@u_address_id,@i_address_type,@u_address_name,@u_first_name,@u_last_name,@u_description,@u_address_line1,@u_address_line2,@u_city,@u_region_code,@u_region_name,@u_postal_code,@u_country_code,@u_country_name,@u_tel_number,@u_tel_extension,@i_locale,@u_user_id_changed_by,@dt_date_last_changed,@dt_csadapter_date_last_changed,@dt_date_created)");
                SqlParameter[] parameters = {
                    new SqlParameter("@u_address_id", SqlDbType.NVarChar,50),
                    new SqlParameter("@i_address_type", SqlDbType.Int,4),
                    new SqlParameter("@u_address_name", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_first_name", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_last_name", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_description", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_address_line1", SqlDbType.NVarChar,80),
                    new SqlParameter("@u_address_line2", SqlDbType.NVarChar,80),
                    new SqlParameter("@u_city", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_region_code", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_region_name", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_postal_code", SqlDbType.NVarChar,20),
                    new SqlParameter("@u_country_code", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_country_name", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_tel_number", SqlDbType.NVarChar,32),
                    new SqlParameter("@u_tel_extension", SqlDbType.NVarChar,50),
                    new SqlParameter("@i_locale", SqlDbType.Int,4),
                    new SqlParameter("@u_user_id_changed_by", SqlDbType.NVarChar,50),
                    new SqlParameter("@dt_date_last_changed", SqlDbType.DateTime),
                    new SqlParameter("@dt_csadapter_date_last_changed", SqlDbType.DateTime),
                    new SqlParameter("@dt_date_created", SqlDbType.DateTime)};
                parameters[0].Value = model.u_address_id;
                parameters[1].Value = model.i_address_type;
                parameters[2].Value = model.u_address_name;
                parameters[3].Value = model.u_first_name;
                parameters[4].Value = model.u_last_name;
                parameters[5].Value = model.u_description;
                parameters[6].Value = model.u_address_line1;
                parameters[7].Value = model.u_address_line2;
                parameters[8].Value = model.u_city;
                parameters[9].Value = model.u_region_code;
                parameters[10].Value = model.u_region_name;
                parameters[11].Value = model.u_postal_code;
                parameters[12].Value = model.u_country_code;
                parameters[13].Value = model.u_country_name;
                parameters[14].Value = model.u_tel_number;
                parameters[15].Value = model.u_tel_extension;
                parameters[16].Value = model.i_locale;
                parameters[17].Value = model.u_user_id_changed_by;
                parameters[18].Value = model.dt_date_last_changed;
                parameters[19].Value = model.dt_csadapter_date_last_changed;
                parameters[20].Value = model.dt_date_created;

                int rows = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, strSql.ToString(), parameters);
                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("类:DalCrmAddress,方法：Add,错误信息:{0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(AddressesModel model, string connectionStr, string strWhere, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update Tuhu_profiles.dbo.Addresses  WITH(rowlock) set ");
                //strSql.Append("u_address_id=@u_address_id,");
                strSql.Append("i_address_type=@i_address_type,");
                strSql.Append("u_address_name=@u_address_name,");
                strSql.Append("u_first_name=@u_first_name,");
                strSql.Append("u_last_name=@u_last_name,");
                strSql.Append("u_description=@u_description,");
                strSql.Append("u_address_line1=@u_address_line1,");
                strSql.Append("u_address_line2=@u_address_line2,");
                strSql.Append("u_city=@u_city,");
                strSql.Append("u_region_code=@u_region_code,");
                strSql.Append("u_region_name=@u_region_name,");
                strSql.Append("u_postal_code=@u_postal_code,");
                strSql.Append("u_country_code=@u_country_code,");
                strSql.Append("u_country_name=@u_country_name,");
                strSql.Append("u_tel_number=@u_tel_number,");
                strSql.Append("u_tel_extension=@u_tel_extension,");
                strSql.Append("i_locale=@i_locale,");
                //strSql.Append("u_user_id_changed_by=@u_user_id_changed_by,");
                strSql.Append("dt_date_last_changed=@dt_date_last_changed,");
                strSql.Append("dt_csadapter_date_last_changed=@dt_csadapter_date_last_changed,");
                strSql.Append("dt_date_created=@dt_date_created");

                strSql.Append(" where u_address_id=@u_address_id ");

                if (!string.IsNullOrEmpty(strWhere))
                {
                    strSql.Append(" and " + strWhere);
                }

                SqlParameter[] parameters = {
                    new SqlParameter("@u_address_id", SqlDbType.NVarChar,50),
                    new SqlParameter("@i_address_type", SqlDbType.Int,4),
                    new SqlParameter("@u_address_name", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_first_name", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_last_name", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_description", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_address_line1", SqlDbType.NVarChar,80),
                    new SqlParameter("@u_address_line2", SqlDbType.NVarChar,80),
                    new SqlParameter("@u_city", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_region_code", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_region_name", SqlDbType.NVarChar,64),
                    new SqlParameter("@u_postal_code", SqlDbType.NVarChar,20),
                    new SqlParameter("@u_country_code", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_country_name", SqlDbType.NVarChar,50),
                    new SqlParameter("@u_tel_number", SqlDbType.NVarChar,32),
                    new SqlParameter("@u_tel_extension", SqlDbType.NVarChar,50),
                    new SqlParameter("@i_locale", SqlDbType.Int,4),
					//new SqlParameter("@u_user_id_changed_by", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_date_last_changed", SqlDbType.DateTime),
                    new SqlParameter("@dt_csadapter_date_last_changed", SqlDbType.DateTime),
                    new SqlParameter("@dt_date_created", SqlDbType.DateTime)};
                parameters[0].Value = model.u_address_id;
                parameters[1].Value = model.i_address_type;
                parameters[2].Value = model.u_address_name;
                parameters[3].Value = model.u_first_name;
                parameters[4].Value = model.u_last_name;
                parameters[5].Value = model.u_description;
                parameters[6].Value = model.u_address_line1;
                parameters[7].Value = model.u_address_line2;
                parameters[8].Value = model.u_city;
                parameters[9].Value = model.u_region_code;
                parameters[10].Value = model.u_region_name;
                parameters[11].Value = model.u_postal_code;
                parameters[12].Value = model.u_country_code;
                parameters[13].Value = model.u_country_name;
                parameters[14].Value = model.u_tel_number;
                parameters[15].Value = model.u_tel_extension;
                parameters[16].Value = model.i_locale;
                //parameters[17].Value = model.u_user_id_changed_by;
                parameters[17].Value = model.dt_date_last_changed;
                parameters[18].Value = model.dt_csadapter_date_last_changed;
                parameters[19].Value = model.dt_date_created;

                int rows = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, strSql.ToString(), parameters);

                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("类:DalCrmAddress,方法：Update,错误信息:{0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string connectionStr, string strWhere, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                //该表无主键信息，请自定义主键/条件字段
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete from Tuhu_profiles.dbo.Addresses ");
                SqlParameter[] parameters = { };

                if (!string.IsNullOrEmpty(strWhere))
                {
                    strSql.Append(" where " + strWhere);
                }

                int rows = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, strSql.ToString(), parameters);

                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("类:DalCrmAddress,方法：Delete,错误信息:{0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string connectionStr, string strWhere, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select u_address_id,i_address_type,u_address_name,u_first_name,u_last_name,u_description,u_address_line1,u_address_line2,u_city,u_region_code,u_region_name,u_postal_code,u_country_code,u_country_name,u_tel_number,u_tel_extension,i_locale,u_user_id_changed_by,dt_date_last_changed,dt_csadapter_date_last_changed,dt_date_created ");
                strSql.Append(" FROM Tuhu_profiles.dbo.Addresses WITH(NOLOCK)");
                SqlParameter[] parameters = { };

                if (!string.IsNullOrEmpty(strWhere))
                {
                    strSql.Append(" where " + strWhere);
                }
                return SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("类:DalCrmAddress,方法：Delete,错误信息:{0}", ex.ToString());
                return null;
            }
        }
        public List<AddressesModel> GetAddressListByAddressIDsAndPreferAddress(string addressIDs, string preferAddress, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                string _SqlStr = @"select u_address_id,i_address_type,u_address_name,u_first_name,u_last_name,u_description,u_address_line1,u_address_line2,u_city,u_region_code,u_region_name,u_postal_code,u_country_code,u_country_name,u_tel_number,u_tel_extension,i_locale,u_user_id_changed_by,dt_date_last_changed,dt_csadapter_date_last_changed,dt_date_created
				FROM Tuhu_profiles.dbo.Addresses WITH(NOLOCK) WHERE u_address_id in(" + addressIDs + ")" + (string.IsNullOrEmpty(preferAddress) ? "" : "ORDER BY (CASE WHEN u_address_id =@preaddr THEN 0 ELSE 1 END)");
                SqlParameter[] parameters = {
                    new SqlParameter("@preaddr", SqlDbType.NVarChar,50)};
                parameters[0].Value = preferAddress ?? "";
                return DbHelper.ExecuteDataTable(_SqlStr, CommandType.Text, parameters).ConvertTo<AddressesModel>().ToList();
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("类:DalCrmAddress,方法：Delete,错误信息:{0}", ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 修改默认地址
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="userId"></param>
        /// <param name="userPreferredAddress"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool UpdateField(string connectionStr, string userId, string userPreferredAddress, out string errorMsg)
        {
            errorMsg = string.Empty;
            string updateSQL = " Update Tuhu_profiles.dbo.UserObject  WITH(rowlock) set u_preferred_address = @UserPreferredAddress Where u_user_id = @UserId ";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@UserId",userId),
                    new SqlParameter("@UserPreferredAddress",userPreferredAddress)
                };
                var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:UpdateField,sqlText:{3}=>参数:userPreferredAddress={2},userId={1},错误信息:{0}", ex.ToString(), userId, userPreferredAddress, updateSQL);
                return false;
            }
        }

        /// <summary>
        /// 修改新地址（添加）
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="userId"></param>
        /// <param name="userNewAddress">新地址</param>
        /// <param name="erroeMsg"></param>
        /// <returns></returns>
        public bool UpdateAddress(string connectionStr, string userId, string userNewAddress, out string errorMsg)
        {
            errorMsg = string.Empty;
            string updateSQL = "Update Tuhu_profiles.dbo.UserObject WITH(rowlock) set u_addresses = @UserNewAddress Where u_user_id = @UserId";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@UserId",userId),
                    new SqlParameter("@UserNewAddress",userNewAddress)
                };
                var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, updateSQL, sqlParamters);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:UpdateField,sqlText:{3}=>参数:userPreferredAddress={2},userId={1},错误信息:{0}", ex.ToString(), userId, userNewAddress, updateSQL);
                return false;
            }
        }

        #endregion  BasicMethod

        public DataTable GetUserObjectInfoByPhone(string connectionStr, string phone)
        {
            string strSql = @"
            SELECT 
            tab2.UserGrade,
            tab2.Name,
            tab1.u_user_id,
            tab1.u_last_name,
            tab1.u_Pref5,
            tab1.u_mobile_number,
            tab1.u_Imagefile,
            'i_gender' = CASE WHEN tab1.i_gender > 1 THEN N'女' ELSE N'男' END
            FROM Tuhu_profiles.dbo.UserObject AS tab1
            LEFT JOIN Tuhu_profiles.dbo.UserObjectInfo AS tab2 ON tab1.u_user_id = tab2.UserID
            WHERE tab1.u_mobile_number = @phone";

            var sqlParams = new[] {
                new SqlParameter("@phone",phone)
            };

            return SqlHelper.ExecuteDataset(connectionStr, CommandType.Text, strSql, sqlParams).Tables[0];
        }

        public void AddUserAddress(string connectionStr, string addressId, string lastName, string addressLine1, string city, string regionCode, string regionName, string telNumber, string postalCode, int type)
        {
            string sSql = @"INSERT  INTO Tuhu_profiles..Addresses
                                    ( u_address_id ,
                                      u_last_name ,
                                      u_address_line1 ,
                                      u_city ,
                                      u_region_code ,
                                      u_region_name ,
                                      u_tel_number ,
                                      u_postal_code,u_address_name
                                    )
                            VALUES  ( @u_address_id ,
                                      @u_last_name ,
                                      @u_address_line1 ,
                                      @u_city ,
                                      @u_region_code ,
                                      @u_region_name ,
                                      @u_tel_number ,
                                      @u_postal_code,@u_address_name
                                    )";
            if (type == 1) //修改
            {
                sSql = @"UPDATE  Tuhu_profiles..Addresses WITH ( ROWLOCK )
                                    SET     u_last_name = @u_last_name ,
                                            u_address_line1 = @u_address_line1 ,
                                            u_city = @u_city ,
                                            u_region_code = @u_region_code ,
                                            u_region_name = @u_region_name ,
                                            u_postal_code = @u_postal_code ,
                                            u_tel_number = @u_tel_number,
                                            u_address_name = @u_address_name
                                    WHERE   u_address_id = @u_address_id";
            }
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@u_last_name",lastName)   ,
                    new SqlParameter("@u_address_line1",addressLine1)  ,
                    new SqlParameter("@u_city",city)  ,
                    new SqlParameter("@u_region_code",regionCode)  ,
                    new SqlParameter("@u_region_name",regionName)  ,
                    new SqlParameter("@u_tel_number",telNumber)  ,
                    new SqlParameter("@u_postal_code",postalCode)  ,
                    new SqlParameter("@u_address_name",lastName)  ,
                    new SqlParameter("@u_address_id",addressId)
                };
                var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, sSql, sqlParamters);

            }
            catch (Exception ex)
            {
            }
        }

        public void UpdateUserObject(string connectionStr, string userId, string userAddress)
        {
            string updateSQL = "Update Tuhu_profiles.dbo.UserObject WITH(rowlock) set u_addresses = @UserNewAddress Where u_user_id = @UserId";
            SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@UserId","{"+userId+"}"),
                    new SqlParameter("@UserNewAddress",userAddress)
                };
            var result = SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, updateSQL, sqlParamters);

        }
    }
}
