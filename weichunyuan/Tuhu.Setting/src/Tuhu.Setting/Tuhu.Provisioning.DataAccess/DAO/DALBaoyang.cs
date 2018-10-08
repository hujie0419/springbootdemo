using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BaoYang;
using static Tuhu.Provisioning.DataAccess.Entity.OilBrandPriorityModel;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALBaoyang
    {
        /// <summary>
        /// 获取保养页面保养项目
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectBaoYangActivityStyle()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("BaoYang.[dbo].[BaoYang_SelectBaoYangPageActivityStyleList]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        public static DataRow GetBaoYangModelByPKID(int pkid)
        {
            string sql = @"SELECT  BYAS.PKID ,
                            BYAS.byType ,
		                    BYAS.byValue, 
		                    BYAS.ImgCode,
		                    BYAS.StrUrl,
                            BYAS.sequence
                            FROM    BaoYang..BaoYangActivityStyle AS BYAS WITH ( NOLOCK )
                            WHERE   BYAS.PKID = @PKID;";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataRow(cmd);
                }
            }
        }

        public static int UpdateBaoYangModel(BaoYangModel model)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("BaoYang.[dbo].[BaoYang_UpertBaoYangActivityStyle]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@byType", model.byType);
                    cmd.Parameters.AddWithValue("@byValue", model.byValue);
                    cmd.Parameters.AddWithValue("@PageType", model.PageType);
                    cmd.Parameters.AddWithValue("@PKID", model.PKID);
                    return dbhelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static int UpdateBaoYangIndexModel(BaoYangModel model)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("BaoYang.[dbo].[BaoYang_UpertBaoYangIndexActivityStyle]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@byType", model.byType);
                    cmd.Parameters.AddWithValue("@byValue", model.byValue);
                    cmd.Parameters.AddWithValue("@PageType", model.PageType);
                    cmd.Parameters.AddWithValue("@ImgCode", model.ImgCode);
                    cmd.Parameters.AddWithValue("@StrUrl", model.StrUrl);
                    cmd.Parameters.AddWithValue("@sequence", model.sequence);

                    cmd.Parameters.AddWithValue("@PKID", model.PKID);
                    return dbhelper.ExecuteNonQuery(cmd);
                }
            }
        }


        /// <summary>
        /// 获取首页保养配置列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetBaoyangIndexConfigItemList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("BaoYang.[dbo].[BaoYang_SelectBaoYangIndexActivityStyleList]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }


        /// <summary>
        /// 添加活动项目安排
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertBaoyangList(BaoYangItemModel model)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("BaoYang.[dbo].[BaoYang_InsertBaoYangActivityStyleList]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BaoYangActivityStyleID", model.BaoYangActivityStyleID);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                    cmd.Parameters.AddWithValue("@isActivity", model.isActivity);
                    return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }


        /// <summary>
        /// 获取当前项目的活动集合
        /// </summary>
        /// <param name="BaoYangActivityStyleID"></param>
        /// <returns></returns>
        public static DataTable GetBaoyangListByBaoYangActivityStyleID(string BaoYangActivityStyleID)
        {

            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("BaoYang.[dbo].[BaoYang_GetBaoYangActivityList]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BaoYangActivityStyleID", BaoYangActivityStyleID);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        public static bool DeleteActivityItemByPkid(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("BaoYang.[dbo].[BaoYang_DeleteBaoyangActivityItemByPKID]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// 根据项目名称获取他所对应的优先级
        /// </summary>
        /// <param name="PartName"></param>
        /// <returns></returns>
        public static DataTable GetBaoYangPriorityByPartNameAndField(string PartName, string PriorityField)
        {
            if (!string.IsNullOrEmpty(PartName) && !string.IsNullOrEmpty(PriorityField))
            {
                string sql = "SELECT * FROM BaoYang.[dbo].tbl_ProductPrioritySetting WITH ( NOLOCK ) WHERE PartName=@PartName AND PriorityField=@PriorityField";
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@PartName", PartName);
                        cmd.Parameters.AddWithValue("@PriorityField", PriorityField);
                        return dbhelper.ExecuteDataTable(cmd);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据PriorityField获取整个，优先级推荐的数据
        /// </summary>
        /// <param name="PriorityField">优先级是根据什么来分的(品牌，或者其他)</param>
        /// <returns></returns>
        public static DataTable GetBaoYangPriority(string PriorityField)
        {
            string sql = "SELECT * FROM BaoYang.[dbo].tbl_ProductPrioritySetting WITH ( NOLOCK ) WHERE PriorityField=@PriorityField";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PriorityField", PriorityField);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 获取保养默认推荐表的所有信息通过type(采用新表，目前只有机油配置使用)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable GetRecommandProductsPriority(string type)
        {
            string sql = "SELECT * FROM BaoYang.[dbo].ProductRecommendPrioritySetting WITH ( NOLOCK ) WHERE Type=@Type; ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@Type", type);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 保存保养推荐页面的数据，根据名称和属性查找，如果存在此条数据，则修改，否则做添加操作
        /// </summary>
        /// <param name="projectList"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool UpsertRecommendPrioritySetting(List<BaoYangProductRecommendModel> projectList, string userName)
        {
            int count = 0;
            bool result = false;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = string.Empty;
                string updatesql = @"UPDATE  BaoYang..ProductRecommendPrioritySetting
                                    SET     Priority = @Priority ,
                                    UpdatedTime = GETDATE() ,
                                    UpdatedUser = @UserName
                                    WHERE   PartName = @PartName
                                    AND Property = @Property
                                    AND Type = @Type; ";
                string insertsql =
                    @"INSERT INTO BaoYang..ProductRecommendPrioritySetting(PartName,Property,Type,Priority,CreatedTime,CreatedUser )
                    VALUES  (@PartName,@Property,@Type,@Priority,GETDATE(),@UserName ); ";
                string existsql =
                    @"SELECT COUNT(*) FROM BaoYang.[dbo].ProductRecommendPrioritySetting WITH (NOLOCK) WHERE PartName=@PartName and Property=@Property;";
                try
                {
                    foreach (var item in projectList)
                    {
                        using (var selectcmd = new SqlCommand(existsql))
                        {
                            selectcmd.Parameters.AddWithValue("@PartName", item.PartName);
                            selectcmd.Parameters.AddWithValue("@Property", item.Property);
                            selectcmd.Parameters.AddWithValue("@UserName", userName);
                            count = (int)dbhelper.ExecuteScalar(selectcmd);
                        }
                        if (count > 0) //存在此条记录
                            sql = updatesql; //执行修改操作
                        else
                            sql = insertsql; //执行插入操作
                        using (var cmd = new SqlCommand(sql))
                        {
                            cmd.Parameters.AddWithValue("@PartName", item.PartName);
                            cmd.Parameters.AddWithValue("@Property", item.Property);
                            cmd.Parameters.AddWithValue("@Type", item.Type);
                            cmd.Parameters.AddWithValue("@Priority", item.Priority == "" ? null : item.Priority);
                            cmd.Parameters.AddWithValue("@UserName", userName);
                            dbhelper.ExecuteNonQuery(cmd);
                        }
                    }
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;

        }

        /// <summary>
        /// 获取保养默认推荐的信息
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable SelectRecommendPriorityByProjectName(string partName, string type)
        {
            if (!string.IsNullOrEmpty(partName) && !string.IsNullOrEmpty(type))
            {
                string sql = "SELECT * FROM BaoYang.[dbo].ProductRecommendPrioritySetting WITH ( NOLOCK ) WHERE PartName=@PartName AND Type=@Type; ";
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@PartName", partName);
                        cmd.Parameters.AddWithValue("@Type", type);
                        return dbhelper.ExecuteDataTable(cmd);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 关闭或启用模块通过partName
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="onOrof"></param>
        /// <returns></returns>
        public static bool setProductsRecommendIsEnable(string partName, int onOrof)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(partName))
            {
                string sql = @"UPDATE BaoYang.[dbo].ProductRecommendPrioritySetting SET Enabled=@Enabled WHERE PartName=@PartName;";
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@PartName", partName);
                        cmd.Parameters.AddWithValue("@Enabled", onOrof);
                        if (dbhelper.ExecuteNonQuery(cmd) > 0)
                            result = true;
                        else
                            result = false;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        //保存机油编辑页面的数据，根据名称和属性查找，如果存在此条数据，则修改，否则做添加操作
        public static bool SaveRecommendProducts(List<PrioritySettingModel> projectList, string userName)
        {
            int count = 0;
            bool result = false;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = string.Empty;
                string updatesql = @"update BaoYang.[dbo].tbl_ProductPrioritySetting set 
                          FirstPriority=@FirstPriority,
                          SecondPriority=@SecondPriority,
                          ThirdPriority=@ThirdPriority,
						  UpdatedTime=GETDATE(),
						  UpdatedUser=@UserName
                          where PartName=@PartName
                          and PropertyType=@PropertyType
                          and PriorityField=@PriorityField;";
                string insertsql =
                    @"INSERT INTO BaoYang.[dbo].tbl_ProductPrioritySetting(PartName,PropertyType,PriorityField,FirstPriority,SecondPriority,ThirdPriority,CreatedTime,CreatedUser)
                             VALUES(@PartName,@PropertyType,@PriorityField,@FirstPriority,@SecondPriority,@ThirdPriority,GETDATE(),@UserName);";
                string existsql =
                    @"SELECT COUNT(*) FROM BaoYang.[dbo].tbl_ProductPrioritySetting WHERE PartName=@PartName and PropertyType=@PropertyType;";
                try
                {
                    foreach (var item in projectList)
                    {
                        using (var selectcmd = new SqlCommand(existsql))
                        {
                            selectcmd.Parameters.AddWithValue("@PartName", item.PartName);
                            selectcmd.Parameters.AddWithValue("@PropertyType", item.PropertyType);
                            selectcmd.Parameters.AddWithValue("@UserName", userName);
                            count = (int)dbhelper.ExecuteScalar(selectcmd);
                        }
                        if (count > 0) //存在此条记录
                            sql = updatesql; //执行修改操作
                        else
                            sql = insertsql; //执行插入操作
                        using (var cmd = new SqlCommand(sql))
                        {
                            cmd.Parameters.AddWithValue("@PartName", item.PartName);
                            cmd.Parameters.AddWithValue("@PropertyType", item.PropertyType);
                            cmd.Parameters.AddWithValue("@PriorityField", item.PriorityField);
                            cmd.Parameters.AddWithValue("@FirstPriority", item.FirstPriority);
                            cmd.Parameters.AddWithValue("@SecondPriority", item.SecondPriority);
                            cmd.Parameters.AddWithValue("@ThirdPriority", item.ThirdPriority);
                            cmd.Parameters.AddWithValue("@UserName", userName);
                            dbhelper.ExecuteNonQuery(cmd);
                        }
                    }
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        //根据类目加载，品牌
        public static DataTable GetBaoYangCP_Brand(string PrimaryParentCategory)
        {
            string sql = @"SELECT [CP_Brand] FROM [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN] WITH ( NOLOCK ) WHERE cy_list_price>0 AND stockout = 0 AND OnSale = 1 AND PrimaryParentCategory=@PrimaryParentCategory  GROUP BY [CP_Brand];";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PrimaryParentCategory", PrimaryParentCategory);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        //根据车灯类目加载相应的属性
        public static DataTable GetDynamoCP_ShuXing(string PrimaryParentCategory)
        {
            string sql = @"SELECT DISTINCT [CP_ShuXing2] FROM [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN] WITH ( NOLOCK ) WHERE cy_list_price>0 AND stockout = 0 AND OnSale = 1 AND PrimaryParentCategory=@PrimaryParentCategory AND CP_ShuXing2 IS NOT NULL;";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PrimaryParentCategory", PrimaryParentCategory);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        #region 防冻液相关数据访问Method
        /// <summary>
        /// 加载所有省份或者直辖市
        /// </summary>
        /// <returns></returns>
        public static DataTable GetProviceList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
                conn = SecurityHelp.DecryptAES(conn);
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("[Gungnir].dbo.[Member_Region_SelectRegionDetail]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ParentID", 0); //ParentID为零则拉取的是省份
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 获取防冻液配置
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAntifreezeSetting()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
                conn = SecurityHelp.DecryptAES(conn);
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("SELECT * FROM BaoYang..AntifreezeSetting WITH(NOLOCK)"))
                {
                    var aa = dbhelper.ExecuteDataTable(cmd);
                    return aa;
                }
            }
        }
        /// <summary>
        /// 设置防冻液配置
        /// </summary>
        /// <param name="Code">0:添加 1：修改</param>
        /// <param name="AntifreezeItem">防冻液配置实体</param>
        /// <returns></returns>
        public static bool SaveAntifreezeSetting(int Code, AntifreezeSettingModel AntifreezeItem)
        {
            bool result = false;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
                conn = SecurityHelp.DecryptAES(conn);
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = string.Empty;
                if (Code == 0)
                    sql =
                        "INSERT INTO [BaoYang]..[AntifreezeSetting]([FreezingPoint],[Brand],[ProvinceIds], [ProvinceNames] ,[Status])VALUES(@FreezingPoint,@Brand, @ProvinceIds,@ProvinceNames,1)";
                else
                    sql =
                        "UPDATE [BaoYang]..[AntifreezeSetting] SET [FreezingPoint]=@FreezingPoint, [Brand]=@Brand,[ProvinceIds]=@ProvinceIds,[ProvinceNames]=@ProvinceNames WHERE PKID=@PKID";
                using (var cmd = new SqlCommand(sql))
                {
                    if (Code == 1) cmd.Parameters.AddWithValue("@PKID", AntifreezeItem.PKID);
                    cmd.Parameters.AddWithValue("@FreezingPoint", AntifreezeItem.FreezingPoint);
                    cmd.Parameters.AddWithValue("@Brand", AntifreezeItem.Brand);
                    cmd.Parameters.AddWithValue("@ProvinceIds", AntifreezeItem.ProvinceIds);
                    cmd.Parameters.AddWithValue("@ProvinceNames", AntifreezeItem.ProvinceNames);
                    result = dbhelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            return result;
        }
        /// <summary>
        /// 停止或启动防冻液模块
        /// </summary>
        /// <param name="PKID">主键ID</param>
        /// <param name="Status">状态：0：未启用 1：启用</param>
        /// <returns></returns>
        public static bool SetAntifreezeStatus(byte Status)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(@"UPDATE BaoYang..AntifreezeSetting SET Status=@Status"))
                {
                    //   cmd.Parameters.AddWithValue("@PKID", PKID);
                    cmd.Parameters.AddWithValue("@Status", Status);
                    return dbhelper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }
        #endregion
        public static DataTable GetBatteryCP_Brand(string partName)
        {
            string sql = @"SELECT Brand FROM BaoYang..Tuhu_BaoYangParts  WHERE PartName=@PartName AND Brand IS NOT NULL GROUP BY Brand;";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PartName", partName);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }
        //停止或启动此模块
        public static bool setIsEnable(string PartName, int onOrof)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(PartName))
            {
                string sql = @"UPDATE BaoYang.[dbo].tbl_ProductPrioritySetting SET Enabled=@Enabled WHERE PartName=@PartName;";
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@PartName", PartName);
                        cmd.Parameters.AddWithValue("@Enabled", onOrof);
                        if (dbhelper.ExecuteNonQuery(cmd) > 0)
                            result = true;
                        else
                            result = false;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static string SelectProductNameByPid(string pid, string category)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }

            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand("Product_SelectDisplayNameByPidAndCategory"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PID", pid);
                    cmd.Parameters.AddWithValue("@Category", category);

                    return dbhelper.ExecuteScalar(cmd) as string;
                }
            }

        }

        /// <summary>
        /// 获取所有车型的品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> SelectAllVehicleBrands(SqlConnection conn)
        {
            var result = new List<string>();

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "[Gungnir].[dbo].[VehicleType_SelectAllBrands]"))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        /// <summary>
        /// 根据选择的品牌该品牌的系列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public static IDictionary<string, string> SelectVehicleSeries(SqlConnection conn, string brand)
        {
            var result = new Dictionary<string, string>();

            var parameters = new[]
            {
                new SqlParameter("@Brand", brand)
            };

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "[Gungnir].[dbo].[VehicleType_SelectVehicleSeriesByBrand]", parameters))
            {
                while (reader.Read())
                {

                    var key = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    var value = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                }
            }

            return result;
        }

        public static IEnumerable<string> SelectVehiclePaiLiang(SqlConnection connection, string vid)
        {
            var result = new List<string>();

            var parameters = new[]
            {
                new SqlParameter("@VehicleID", vid)
            };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "VehicleType_SelectPaiLiangByVID", parameters))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        /// <summary>
        /// 根据productCategory获取产品表对应的系列名称
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        public static List<string> GetVehicleSeriesByProductCategory(SqlConnection conn, string productCategory)
        {
            var result = new List<string>();

            SqlParameter[] parameters =
            {
                new SqlParameter("@ProductCategory",productCategory)
            };

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "[BaoYang].[dbo].[VehicleSetting_GetVehicleSeriesByProductCategory]", parameters))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        /// <summary>
        /// 根据系列名称获取产品表机油对应的品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="priorityType"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<string> GetJYBrandByPriorityTypeAndSeries(SqlConnection conn, string priorityType, string series)
        {
            var result = new List<string>();
            SqlParameter[] parameters =
            {
                new SqlParameter("@PriorityType",priorityType),
                new SqlParameter("@Series",series)
            };
            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "[BaoYang].[dbo].[VehicleSetting_GetJYBrandByPriorityTypeAndSeries]", parameters))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        /// <summary>
        /// 根据partName(非机油)获取当前车型的配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="partName"></param>
        /// <param name="brand"></param>
        /// <param name="vehicleID"></param>
        /// <param name="staPrice"></param>
        /// <param name="endPrice"></param>
        /// <param name="isConfig"></param>
        /// <returns></returns>
        public static Tuple<List<BaoYangPriorityVehicleSettingModel>, int> SelectBaoYangVehicleSettingOther(SqlConnection conn,
            int pageIndex, int pageSize, string partName, string brand, string vehicleID, string staPrice,
            string endPrice, int isConfig, string firstPriority, string secondPriority)
        {
            List<BaoYangPriorityVehicleSettingModel> result = new List<BaoYangPriorityVehicleSettingModel>();
            int totalCount = 0;

            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PartName",partName),
                new SqlParameter("@Brand",brand),
                new SqlParameter("@VehicleID",vehicleID),
                new SqlParameter("@StaPrice",string.IsNullOrEmpty(staPrice)?0:Convert.ToInt32(staPrice)),
                new SqlParameter("@EndPrice",string.IsNullOrEmpty(endPrice)?0:Convert.ToInt32(endPrice)),
                new SqlParameter("@IsConfig",isConfig),
                new SqlParameter("@FirstPriority", firstPriority),
                new SqlParameter("@SecondPriority", secondPriority),
            };

            #region sql
            var sql = @"SELECT  U.VehicleID ,
        U.Brand ,
        U.Vehicle ,
        U.FirstPriority ,
        U.SecondPriority ,
        U.MinPrice ,
        COUNT(*) OVER ( ) AS Total
FROM    ( SELECT DISTINCT
                    vt.VehicleID ,
                    vt.Brand ,
                    vt.Vehicle ,
                    pvs.FirstPriority ,
                    pvs.SecondPriority ,
                    ( SELECT TOP 1
                                MinPrice
                      FROM      Gungnir..tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                      WHERE     vt.VehicleID = VehicleID
                                AND MinPrice IS NOT NULL
                      ORDER BY  MinPrice
                    ) AS MinPrice
          FROM      Gungnir..vw_Vehicle_Type vt WITH ( NOLOCK )
                    LEFT JOIN BaoYang..BaoYangPriorityVehicleSetting pvs WITH ( NOLOCK ) ON pvs.VehicleId  COLLATE Chinese_PRC_CI_AS = vt.VehicleID
                                                              AND pvs.PartName = @PartName
                                                              AND pvs.IsDeleted = 0
        ) AS U
WHERE   ( @VehicleID = ''
          OR ( @VehicleID <> ''
               AND U.VehicleID = N'' + @VehicleID + ''
             )
        )
        AND ( ( @Brand = '' )
              OR ( @Brand <> ''
                   AND U.Brand = N'' + @Brand + ''
                 )
            )
        AND ( @StaPrice = 0
              OR ( @StaPrice <> 0
                   AND U.MinPrice >= @StaPrice
                 )
            )
        AND ( @EndPrice = 0
              OR ( @EndPrice <> 0
                   AND U.MinPrice <= @EndPrice
                 )
            )
        AND ( @IsConfig = 0
              OR ( @IsConfig <> 0
                   AND ( U.FirstPriority IS NOT NULL
                         OR U.SecondPriority IS NOT NULL
                       )
                 )
            )
        AND ( @FirstPriority IS NULL
              OR @FirstPriority = N''
              OR @FirstPriority = U.FirstPriority
            )
        AND ( @SecondPriority IS NULL
              OR @SecondPriority = N''
              OR @SecondPriority = U.SecondPriority
            )
ORDER BY U.Brand ,
        U.Vehicle
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            #endregion

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.Text, sql, parameters))
            {
                while (reader.Read())
                {
                    BaoYangPriorityVehicleSettingModel item = new BaoYangPriorityVehicleSettingModel();

                    item.VehicleID = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    item.Brand = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    item.VehicleSeries = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    item.FirstPriority = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    item.SecondPriority = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                    item.MinPrice = reader.IsDBNull(5) ? 0 : reader.GetDouble(5);
                    totalCount = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    result.Add(item);
                }
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 根据机油获取当前车型的配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="partName"></param>
        /// <param name="brand"></param>
        /// <param name="vehicleID"></param>
        /// <param name="staPrice"></param>
        /// <param name="endPrice"></param>
        /// <param name="isConfig"></param>
        /// <returns></returns>
        public static Tuple<List<BaoYangPriorityVehicleSettingModel>, int> SelectBaoYangVehicleSettingOil(SqlConnection conn,
            int pageIndex, int pageSize, string brand, string vehicleID, string staPrice,
            string endPrice, int isConfig, string priorityType1, string firstPriority, string priorityType2,
            string secondPriority)
        {
            List<BaoYangPriorityVehicleSettingModel> result = new List<BaoYangPriorityVehicleSettingModel>();
            int totalCount = 0;

            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@Brand", brand),
                new SqlParameter("@VehicleID",vehicleID),
                new SqlParameter("@StaPrice",string.IsNullOrEmpty(staPrice)?0:Convert.ToInt32(staPrice)),
                new SqlParameter("@EndPrice",string.IsNullOrEmpty(endPrice)?0:Convert.ToInt32(endPrice)),
                new SqlParameter("@IsConfig",isConfig),
                new SqlParameter("@FirstPriority", firstPriority),
                new SqlParameter("@SecondPriority", secondPriority),
                new SqlParameter("@PriorityType1", priorityType1),
                new SqlParameter("@PriorityType2", priorityType2),
                new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output},
            };

            #region sql
            var sql = @"WITH    gvt
          AS ( SELECT DISTINCT
                        vt.VehicleID ,
                        vt.Brand ,
                        vt.Vehicle
               FROM     Gungnir..vw_Vehicle_Type vt WITH ( NOLOCK )
               WHERE    ( @Brand IS NULL
                          OR @Brand = N''
                          OR vt.Brand = @Brand
                        )
                        AND ( @VehicleID IS NULL
                              OR @VehicleID = N''
                              OR vt.VehicleID = @VehicleID
                            )
             ),
        pvs
          AS ( SELECT DISTINCT
                        pv.VehicleId ,
                        pv.PriorityType ,
                        pv.FirstPriority ,
                        pv.SecondPriority
               FROM     BaoYang..BaoYangPriorityVehicleSetting pv WITH ( NOLOCK )
               WHERE    pv.PartName = N'机油'
                        AND pv.IsDeleted = 0
             ),
        fs
          AS ( SELECT   f.VehicleId ,
                        f.PriorityType1 ,
                        f.FirstPriority ,
                        s.PriorityType2 ,
                        s.SecondPriority
               FROM     ( SELECT    pvs.VehicleId ,
                                    pvs.PriorityType AS PriorityType1 ,
                                    pvs.FirstPriority
                          FROM      pvs
                          WHERE     pvs.FirstPriority IS NOT NULL
                                    AND pvs.FirstPriority <> N''
                        ) AS f
                        LEFT JOIN ( SELECT  pvs.VehicleId ,
                                            pvs.PriorityType AS PriorityType2 ,
                                            pvs.SecondPriority
                                    FROM    pvs
                                    WHERE   pvs.SecondPriority IS NOT NULL
                                            AND pvs.SecondPriority <> N''
                                  ) AS s ON s.VehicleId = f.VehicleId
             )
    SELECT DISTINCT
            vt.VehicleID ,
            vt.Brand ,
            vt.Vehicle ,
            fs.PriorityType1 + '-' + fs.FirstPriority AS FirstPriority ,
            fs.PriorityType2 + '-' + fs.SecondPriority AS SecondPriority ,
            ti.MinPrice
    INTO    #temp
    FROM    gvt AS vt
            LEFT JOIN fs ON fs.VehicleId COLLATE Chinese_PRC_CI_AS = vt.VehicleID
            LEFT JOIN ( SELECT  VehicleID ,
                                MIN(MinPrice) AS MinPrice
                        FROM    Gungnir..tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                        WHERE   MinPrice IS NOT NULL
                        GROUP BY VehicleID
                      ) AS ti ON ti.VehicleID = vt.VehicleID
    WHERE   ( @IsConfig = 0
              OR ( @IsConfig = 1
                   AND ( ( fs.PriorityType1 IS NOT NULL
                           AND fs.FirstPriority IS NOT NULL
                           AND fs.PriorityType1 <> N''
                           AND fs.FirstPriority <> N''
                         )
                         OR ( fs.SecondPriority IS NOT NULL
                              AND fs.PriorityType2 IS NOT NULL
                              AND fs.PriorityType2 <> N''
                              AND fs.SecondPriority <> N''
                            )
                       )
                 )
            )
            AND ( @PriorityType1 IS NULL
                  OR @PriorityType1 = N''
                  OR ( fs.PriorityType1 = @PriorityType1
                       AND ( @FirstPriority IS NULL
                             OR @FirstPriority = N''
                             OR fs.FirstPriority = @FirstPriority
                           )
                     )
                )
            AND ( @PriorityType2 IS NULL
                  OR @PriorityType2 = N''
                  OR ( fs.PriorityType2 = @PriorityType2
                       AND ( @SecondPriority IS NULL
                             OR @SecondPriority = N''
                             OR fs.SecondPriority = @SecondPriority
                           )
                     )
                )
            AND ( @StaPrice = 0
                  OR ( @StaPrice <> 0
                       AND ti.MinPrice >= @StaPrice
                     )
                )
            AND ( @EndPrice = 0
                  OR ( @EndPrice <> 0
                       AND ti.MinPrice <= @EndPrice
                     )
                );
SELECT  t.VehicleID ,
        t.Brand ,
        t.Vehicle ,
        t.SecondPriority ,
        t.FirstPriority ,
        t.MinPrice ,
        pt.Viscosity
FROM    ( SELECT    *
          FROM      #temp
          ORDER BY  Brand ,
                    Vehicle
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
                    ROWS ONLY
        ) AS t
        LEFT JOIN ( SELECT  DISTINCT
                            ti.VehicleID ,
                            pa.Viscosity
                    FROM    Gungnir..tbl_Vehicle_Type_Timing AS ti WITH ( NOLOCK )
                            INNER JOIN BaoYang..tbl_PartAccessory AS pa WITH ( NOLOCK ) ON pa.TID = ti.TID
                                                              AND pa.IsDeleted = 0
                                                              AND pa.AccessoryName = N'发动机油'
                                                              AND pa.Viscosity IS NOT NULL
                                                              AND pa.Viscosity <> N''
                  ) AS pt ON pt.VehicleID = t.VehicleID;
SELECT  @TotalCount = COUNT(1)
FROM    #temp;
DROP TABLE #temp;";
            #endregion

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            result = (from DataRow row in dt.Rows
                      select new BaoYangPriorityVehicleSettingModel
                      {
                          VehicleID = row["VehicleID"].ToString(),
                          Brand = row["Brand"].ToString(),
                          VehicleSeries = row["Vehicle"].ToString(),
                          MinPrice = Equals(row["MinPrice"], DBNull.Value) ? 0 : (double)row["MinPrice"],
                          FirstPriority = row["FirstPriority"].ToString(),
                          SecondPriority = row["SecondPriority"].ToString(),
                          Viscosity = row["Viscosity"].ToString(),
                      }).ToList();
            result = result.GroupBy(x => x.VehicleID).Select(g =>
            {
                var item = g.FirstOrDefault();
                var temp = string.Join(",", g.Where(x => !string.IsNullOrEmpty(x.Viscosity)).Select(x => x.Viscosity));
                return new BaoYangPriorityVehicleSettingModel
                {
                    VehicleID = g.Key,
                    Brand = item.Brand,
                    VehicleSeries = item.VehicleSeries,
                    MinPrice = item.MinPrice,
                    FirstPriority = item.FirstPriority,
                    SecondPriority = item.SecondPriority,
                    Viscosity = temp
                };
            }).OrderBy(o => o.Brand).ThenBy(o => o.VehicleSeries).ToList();
            totalCount = Equals(parameters.Last().Value, DBNull.Value) ? 0 : (int)(parameters.Last().Value);
            return Tuple.Create(result, totalCount);
        }

        public static Tuple<List<BaoYangPriorityVehicleSettingModel>, int> SelectBaoYangVehicleSettingOil(SqlConnection conn,
            int pageIndex, int pageSize, string brand, string vehicleID, string staPrice,
            string endPrice, int isConfig, string priorityType1, string firstPriority, string priorityType2,
            string secondPriority, string viscosity)
        {
            List<BaoYangPriorityVehicleSettingModel> result = new List<BaoYangPriorityVehicleSettingModel>();
            int totalCount = 0;

            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@Brand", brand),
                new SqlParameter("@VehicleID",vehicleID),
                new SqlParameter("@StaPrice",string.IsNullOrEmpty(staPrice)?0:Convert.ToInt32(staPrice)),
                new SqlParameter("@EndPrice",string.IsNullOrEmpty(endPrice)?0:Convert.ToInt32(endPrice)),
                new SqlParameter("@IsConfig",isConfig),
                new SqlParameter("@FirstPriority", firstPriority),
                new SqlParameter("@SecondPriority", secondPriority),
                new SqlParameter("@Viscosity",viscosity),
                new SqlParameter("@PriorityType1", priorityType1),
                new SqlParameter("@PriorityType2", priorityType2),
                new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output},
            };

            #region sql
            var sql = @"WITH    gvt
          AS ( SELECT DISTINCT
                        vt.VehicleID ,
                        vt.Brand ,
                        vt.Vehicle
               FROM     Gungnir..vw_Vehicle_Type vt WITH ( NOLOCK )
               WHERE    ( @Brand IS NULL
                          OR @Brand = N''
                          OR vt.Brand = @Brand
                        )
                        AND ( @VehicleID IS NULL
                              OR @VehicleID = N''
                              OR vt.VehicleID = @VehicleID
                            )
             ),
        pvs
          AS ( SELECT DISTINCT
                        pv.VehicleId ,
                        pv.PriorityType ,
                        pv.FirstPriority ,
                        pv.SecondPriority
               FROM     BaoYang..BaoYangPriorityVehicleSetting pv WITH ( NOLOCK )
               WHERE    pv.PartName = N'机油'
                        AND pv.IsDeleted = 0
             ),
        fs
          AS ( SELECT   f.VehicleId ,
                        f.PriorityType1 ,
                        f.FirstPriority ,
                        s.PriorityType2 ,
                        s.SecondPriority
               FROM     ( SELECT    pvs.VehicleId ,
                                    pvs.PriorityType AS PriorityType1 ,
                                    pvs.FirstPriority
                          FROM      pvs
                          WHERE     pvs.FirstPriority IS NOT NULL
                                    AND pvs.FirstPriority <> N''
                        ) AS f
                        LEFT JOIN ( SELECT  pvs.VehicleId ,
                                            pvs.PriorityType AS PriorityType2 ,
                                            pvs.SecondPriority
                                    FROM    pvs
                                    WHERE   pvs.SecondPriority IS NOT NULL
                                            AND pvs.SecondPriority <> N''
                                  ) AS s ON s.VehicleId = f.VehicleId
             ),
        pt
          AS ( SELECT  DISTINCT
                        ti.VehicleID
               FROM     Gungnir..tbl_Vehicle_Type_Timing AS ti WITH ( NOLOCK )
                        INNER JOIN BaoYang..tbl_PartAccessory AS pa WITH ( NOLOCK ) ON pa.TID = ti.TID
                                                              AND pa.IsDeleted = 0
                                                              AND pa.AccessoryName = N'发动机油'
                                                              AND pa.Viscosity = @Viscosity
             )
    SELECT DISTINCT
            vt.VehicleID ,
            vt.Brand ,
            vt.Vehicle ,
            fs.PriorityType1 + '-' + fs.FirstPriority AS FirstPriority ,
            fs.PriorityType2 + '-' + fs.SecondPriority AS SecondPriority ,
            ti.MinPrice
    INTO    #temp
    FROM    gvt AS vt
            INNER JOIN pt ON pt.VehicleID = vt.VehicleID
            LEFT JOIN fs ON fs.VehicleId COLLATE Chinese_PRC_CI_AS = vt.VehicleID
            LEFT JOIN ( SELECT  VehicleID ,
                                MIN(MinPrice) AS MinPrice
                        FROM    Gungnir..tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                        WHERE   MinPrice IS NOT NULL
                        GROUP BY VehicleID
                      ) AS ti ON ti.VehicleID = vt.VehicleID
    WHERE   ( @IsConfig = 0
              OR ( @IsConfig = 1
                   AND ( ( fs.PriorityType1 IS NOT NULL
                           AND fs.FirstPriority IS NOT NULL
                           AND fs.PriorityType1 <> N''
                           AND fs.FirstPriority <> N''
                         )
                         OR ( fs.SecondPriority IS NOT NULL
                              AND fs.PriorityType2 IS NOT NULL
                              AND fs.PriorityType2 <> N''
                              AND fs.SecondPriority <> N''
                            )
                       )
                 )
            )
            AND ( @PriorityType1 IS NULL
                  OR @PriorityType1 = N''
                  OR ( fs.PriorityType1 = @PriorityType1
                       AND ( @FirstPriority IS NULL
                             OR @FirstPriority = N''
                             OR fs.FirstPriority = @FirstPriority
                           )
                     )
                )
            AND ( @PriorityType2 IS NULL
                  OR @PriorityType2 = N''
                  OR ( fs.PriorityType2 = @PriorityType2
                       AND ( @SecondPriority IS NULL
                             OR @SecondPriority = N''
                             OR fs.SecondPriority = @SecondPriority
                           )
                     )
                )
            AND ( @StaPrice = 0
                  OR ( @StaPrice <> 0
                       AND ti.MinPrice >= @StaPrice
                     )
                )
            AND ( @EndPrice = 0
                  OR ( @EndPrice <> 0
                       AND ti.MinPrice <= @EndPrice
                     )
                );
SELECT  t.VehicleID ,
        t.Brand ,
        t.Vehicle ,
        t.SecondPriority ,
        t.FirstPriority ,
        t.MinPrice ,
        pt.Viscosity
FROM    ( SELECT    *
          FROM      #temp
          ORDER BY  Brand ,
                    Vehicle
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
                    ROWS ONLY
        ) AS t
        INNER JOIN ( SELECT  DISTINCT
                            ti.VehicleID ,
                            pa.Viscosity
                     FROM   Gungnir..tbl_Vehicle_Type_Timing AS ti WITH ( NOLOCK )
                            INNER JOIN BaoYang..tbl_PartAccessory AS pa WITH ( NOLOCK ) ON pa.TID = ti.TID
                                                              AND pa.IsDeleted = 0
                                                              AND pa.AccessoryName = N'发动机油'
                                                              AND pa.Viscosity IS NOT NULL
                                                              AND pa.Viscosity <> N''
                   ) AS pt ON pt.VehicleID = t.VehicleID;
SELECT  @TotalCount = COUNT(1)
FROM    #temp;
DROP TABLE #temp;";
            #endregion

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            result = (from DataRow row in dt.Rows
                      select new BaoYangPriorityVehicleSettingModel
                      {
                          VehicleID = row["VehicleID"].ToString(),
                          Brand = row["Brand"].ToString(),
                          VehicleSeries = row["Vehicle"].ToString(),
                          MinPrice = Equals(row["MinPrice"], DBNull.Value) ? 0 : (double)row["MinPrice"],
                          FirstPriority = row["FirstPriority"].ToString(),
                          SecondPriority = row["SecondPriority"].ToString(),
                          Viscosity = row["Viscosity"].ToString(),
                      }).ToList();
            result = result.GroupBy(x => x.VehicleID).Select(g =>
            {
                var item = g.FirstOrDefault();
                var temp = string.Join(",", g.Where(x => !string.IsNullOrEmpty(x.Viscosity)).Select(x => x.Viscosity));
                return new BaoYangPriorityVehicleSettingModel
                {
                    VehicleID = g.Key,
                    Brand = item.Brand,
                    VehicleSeries = item.VehicleSeries,
                    MinPrice = item.MinPrice,
                    FirstPriority = item.FirstPriority,
                    SecondPriority = item.SecondPriority,
                    Viscosity = temp
                };
            }).OrderBy(o => o.Brand).ThenBy(o => o.VehicleSeries).ToList();
            totalCount = Equals(parameters.Last().Value, DBNull.Value) ? 0 : (int)(parameters.Last().Value);
            return Tuple.Create(result, totalCount);
        }

        public static Tuple<List<VehicleOilSetting>, int> SelectVehicleOilSetting(SqlConnection conn, int pageIndex,
            int pageSize,
            string brand, string vehicleID, string paiLiang, int isConfig)
        {
            var result = new List<VehicleOilSetting>();
            int totalCount = 0;

            #region sql

            var sql = @"IF ( @IsConfig = '0' )
    BEGIN
	  SELECT DISTINCT 
	  TT.Brand,TT.Vehicle,TT.VehicleID,TT.PaiLiang,TT.RecommendViscosity,TT.OriginalViscosity　 FROM (
	   SELECT  P.Brand ,
                P.Vehicle ,
				T.VehicleID,
                T.PaiLiang ,
                S.RecommendViscosity ,
                ( SELECT    Viscosity + ''
                  FROM      BaoYang.[dbo].[tbl_PartAccessory] tt
                  WHERE     tt.TID = T.TID
                FOR
                  XML PATH('')
                ) AS OriginalViscosity
        FROM    Gungnir..tbl_Vehicle_Type_Timing (NOLOCK) AS T
                JOIN Gungnir..tbl_Vehicle_Type (NOLOCK) AS P ON T.VehicleID = P.ProductID
                LEFT JOIN BaoYang..VehicleOilSetting (NOLOCK) AS S ON T.VehicleID = S.VehicleID COLLATE Chinese_PRC_CI_AS
                                                              AND S.PaiLiang = T.PaiLiang COLLATE Chinese_PRC_CI_AS
        WHERE   ( @VehicleID = ''
                  OR ( @VehicleID <> ''
                       AND T.VehicleID = N'' + @VehicleID + ''
                     )
                )
                AND ( ( @Brand = '' )
                      OR ( @Brand <> ''
                           AND P.Brand = N'' + @Brand + ''
                         )
                    )
                AND ( ( @PaiLiang = '' )
                      OR ( @PaiLiang <> ''
                           AND T.PaiLiang = N'' + @PaiLiang + ''
                         )
                    )
	  ) AS TT        WHERE TT.OriginalViscosity IS NOT NULL
        ORDER BY TT.Brand ,
                TT.Vehicle,
				TT.VehicleID,
				TT.PaiLiang,
				TT.RecommendViscosity,
				TT.OriginalViscosity
              OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
                ROWS ONLY;
    END
ELSE
    BEGIN
	SELECT DISTINCT TT.Brand,TT.Vehicle,TT.VehicleID,TT.PaiLiang,TT.RecommendViscosity,TT.OriginalViscosity　FROM (
	SELECT   P.Brand ,
                P.Vehicle ,
				T.VehicleID,
                T.PaiLiang ,
                S.RecommendViscosity ,
                ( SELECT    Viscosity + ''
                  FROM      BaoYang.[dbo].[tbl_PartAccessory] tt
                  WHERE     tt.TID = T.TID
                FOR
                  XML PATH('')
                ) AS OriginalViscosity
        FROM    Gungnir..tbl_Vehicle_Type_Timing (NOLOCK) AS T
                JOIN Gungnir..tbl_Vehicle_Type (NOLOCK) AS P ON T.VehicleID = P.ProductID
                JOIN BaoYang..VehicleOilSetting (NOLOCK) AS S ON T.VehicleID = S.VehicleID COLLATE Chinese_PRC_CI_AS
                                                              AND S.PaiLiang = T.PaiLiang COLLATE Chinese_PRC_CI_AS
        WHERE   ( @VehicleID = ''
                  OR ( @VehicleID <> ''
                       AND T.VehicleID = N'' + @VehicleID + ''
                     )
                )
                AND ( ( @Brand = '' )
                      OR ( @Brand <> ''
                           AND P.Brand = N'' + @Brand + ''
                         )
                    )
                AND ( ( @PaiLiang = '' )
                      OR ( @PaiLiang <> ''
                           AND T.PaiLiang = N'' + @PaiLiang + ''
                         )
                    )
	) AS TT        WHERE TT.OriginalViscosity IS NOT NULL
        ORDER BY TT.Brand ,
                TT.Vehicle,
				TT.VehicleID,
				TT.PaiLiang,
				TT.RecommendViscosity,
				TT.OriginalViscosity
                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
                ROWS ONLY;   
    END";

            #endregion

            #region 求总数

            var sqlTotalCount = @"IF ( @IsConfig = '0' )
    BEGIN  

SELECT  COUNT(1)
FROM    ( SELECT DISTINCT
                    TT.Brand ,
                    TT.Vehicle ,
                    TT.VehicleID ,
                    TT.PaiLiang ,
                    TT.RecommendViscosity ,
                    TT.OriginalViscosity
          FROM      ( SELECT    P.Brand ,
                                P.Vehicle ,
                                T.VehicleID ,
                                T.PaiLiang ,
                                S.RecommendViscosity ,
                                ( SELECT    Viscosity + ''
                                  FROM      BaoYang.[dbo].[tbl_PartAccessory] TT
                                  WHERE     TT.TID = T.TID
                                FOR
                                  XML PATH('')
                                ) AS OriginalViscosity
                      FROM      Gungnir..tbl_Vehicle_Type_Timing (NOLOCK) AS T
                                JOIN Gungnir..tbl_Vehicle_Type (NOLOCK) AS P ON T.VehicleID = P.ProductID
                                LEFT JOIN BaoYang..VehicleOilSetting (NOLOCK)
                                AS S ON T.VehicleID = S.VehicleId COLLATE Chinese_PRC_CI_AS
                                        AND S.PaiLiang = T.PaiLiang COLLATE Chinese_PRC_CI_AS
                      WHERE     ( @VehicleID = ''
                                  OR ( @VehicleID <> ''
                                       AND T.VehicleID = N'' + @VehicleID + ''
                                     )
                                )
                                AND ( ( @Brand = '' )
                                      OR ( @Brand <> ''
                                           AND P.Brand = N'' + @Brand + ''
                                         )
                                    )
                                AND ( ( @PaiLiang = '' )
                                      OR ( @PaiLiang <> ''
                                           AND T.PaiLiang = N'' + @PaiLiang
                                           + ''
                                         )
                                    )
                    ) AS TT  WHERE TT.OriginalViscosity IS NOT NULL
        ) AS TTT     
    END
ELSE
    BEGIN
      SELECT  COUNT(1)
FROM    ( SELECT DISTINCT
                    TT.Brand ,
                    TT.Vehicle ,
                    TT.VehicleID ,
                    TT.PaiLiang ,
                    TT.RecommendViscosity ,
                    TT.OriginalViscosity
          FROM      ( SELECT    P.Brand ,
                                P.Vehicle ,
                                T.VehicleID ,
                                T.PaiLiang ,
                                S.RecommendViscosity ,
                                ( SELECT    Viscosity + ''
                                  FROM      BaoYang.[dbo].[tbl_PartAccessory] TT
                                  WHERE     TT.TID = T.TID
                                FOR
                                  XML PATH('')
                                ) AS OriginalViscosity
                      FROM      Gungnir..tbl_Vehicle_Type_Timing (NOLOCK) AS T
                                JOIN Gungnir..tbl_Vehicle_Type (NOLOCK) AS P ON T.VehicleID = P.ProductID
                                JOIN BaoYang..VehicleOilSetting (NOLOCK) AS S ON T.VehicleID = S.VehicleId COLLATE Chinese_PRC_CI_AS
                                                              AND S.PaiLiang = T.PaiLiang COLLATE Chinese_PRC_CI_AS
                      WHERE     ( @VehicleID = ''
                                  OR ( @VehicleID <> ''
                                       AND T.VehicleID = N'' + @VehicleID + ''
                                     )
                                )
                                AND ( ( @Brand = '' )
                                      OR ( @Brand <> ''
                                           AND P.Brand = N'' + @Brand + ''
                                         )
                                    )
                                AND ( ( @PaiLiang = '' )
                                      OR ( @PaiLiang <> ''
                                           AND T.PaiLiang = N'' + @PaiLiang
                                           + ''
                                         )
                                    )
                    ) AS TT  WHERE TT.OriginalViscosity IS NOT NULL
        ) AS TTT 
    END";

            #endregion


            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Brand", brand),
                new SqlParameter("@VehicleID", vehicleID),
                new SqlParameter("@PaiLiang", paiLiang),
                new SqlParameter("@IsConfig", isConfig)
            };

            SqlParameter[] parametersCount =
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Brand", brand),
                new SqlParameter("@VehicleID", vehicleID),
                new SqlParameter("@PaiLiang", paiLiang),
                new SqlParameter("@IsConfig", isConfig)
            };

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.Text, sql, parameters))
            {
                while (reader.Read())
                {
                    var item = new VehicleOilSetting();

                    item.Brand = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    item.Vehicle = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    item.VehicleId = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    item.PaiLiang = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    item.RecommendViscosity = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                    item.Viscosity = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                    result.Add(item);
                }

            }

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sqlTotalCount, parametersCount).AsEnumerable();
            var itemArray = dt.FirstOrDefault()?.ItemArray;
            if (itemArray != null)
            {
                Int32.TryParse(itemArray[0].ToString(), out totalCount);
            }

            return Tuple.Create<List<VehicleOilSetting>, int>(result, totalCount);
        }

        public static bool UpsertVehicleOilViscosity(SqlConnection conn, string vehicleId, string paiLiang,
            string viscosity, string user)
        {
            string sql = @"IF ( EXISTS ( SELECT    1
              FROM      [BaoYang].[dbo].[VehicleOilSetting]
              WHERE     VehicleId = @VehicleId
                        AND PaiLiang = @PaiLiang ) )
    BEGIN
        UPDATE  [BaoYang].[dbo].[VehicleOilSetting]
        SET     RecommendViscosity = @RecommendViscosity ,
                UpdateTime = GETDATE() ,
                UpdateUser = @User
        WHERE   VehicleId = @VehicleId
                AND PaiLiang = @PaiLiang	
      
	 
    END
ELSE
    BEGIN
        INSERT  INTO [BaoYang].[dbo].[VehicleOilSetting]
                ( [VehicleId] ,
                  [PaiLiang] ,
                  [RecommendViscosity] ,
                  [CreateUser]
                )
        VALUES  ( @VehicleId , -- VehicleId - nvarchar(255)
                  @PaiLiang , -- PaiLiang - nvarchar(255)
                  @RecommendViscosity , -- RecommendViscosity - varchar(20)
                  @User  -- CreateUser - nvarchar(255)
                )
    END";
            SqlParameter[] parameters =
            {
                new SqlParameter("@VehicleId", vehicleId),
                new SqlParameter("@PaiLiang", paiLiang),
                new SqlParameter("@RecommendViscosity", viscosity),
                new SqlParameter("@User", user),
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool BatchUpsertVehicleOilViscosity(SqlConnection conn,
            List<VehicleOilSetting> batchVehicleModels, string viscosity, string user)
        {
            var result = false;
            SqlTransaction trans = conn.BeginTransaction();
            string sql = @"IF ( EXISTS ( SELECT    1
              FROM      [BaoYang].[dbo].[VehicleOilSetting]
              WHERE     VehicleId = @VehicleId
                        AND PaiLiang = @PaiLiang ) )
    BEGIN
        UPDATE  [BaoYang].[dbo].[VehicleOilSetting]
        SET     RecommendViscosity = @RecommendViscosity ,
                UpdateTime = GETDATE() ,
                UpdateUser = @User
        WHERE   VehicleId = @VehicleId
                AND PaiLiang = @PaiLiang	
      
	 
    END
ELSE
    BEGIN
        INSERT  INTO [BaoYang].[dbo].[VehicleOilSetting]
                ( [VehicleId] ,
                  [PaiLiang] ,
                  [RecommendViscosity] ,
                  [CreateUser]
                )
        VALUES  ( @VehicleId , -- VehicleId - nvarchar(255)
                  @PaiLiang , -- PaiLiang - nvarchar(255)
                  @RecommendViscosity , -- RecommendViscosity - varchar(20)
                  @User  -- CreateUser - nvarchar(255)
                )
    END";
            try
            {
                foreach (var vehicle in batchVehicleModels)
                {
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@VehicleId", vehicle.VehicleId),
                        new SqlParameter("@PaiLiang", vehicle.PaiLiang),
                        new SqlParameter("@RecommendViscosity", viscosity),
                        new SqlParameter("@User", user),
                    };
                    var count = SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sql, parameters);
                    if (count <= 0)
                    {
                        trans.Rollback();
                    }
                }
                trans.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                result = false;
            }
            conn.Close();
            return result;
        }


        public static bool DeleteVehicleOilViscosity(SqlConnection conn, string vehicleId, string paiLiang)
        {
            string sql =
                @"DELETE FROM [BaoYang].[dbo].[VehicleOilSetting] WHERE VehicleId=@VehicleId AND PaiLiang=@PaiLiang";
            SqlParameter[] parameters =
            {
                new SqlParameter("@VehicleId", vehicleId),
                new SqlParameter("@PaiLiang", paiLiang),
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// 根据partName、PriorityType、vehicleID获取特殊车型配置表养护类和品牌类的配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="partName"></param>
        /// <param name="PriorityType"></param>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public static List<BaoYangPriorityVehicleSettingModel> GetPriorityVehicleRulesByPartNameAndType(SqlConnection conn, string partName, string PriorityType, string vehicleID)
        {
            List<BaoYangPriorityVehicleSettingModel> result = new List<BaoYangPriorityVehicleSettingModel>();
            SqlParameter[] parameters =
            {
                new SqlParameter("@PartName",partName),
                new SqlParameter("@PriorityType",PriorityType),
                new SqlParameter("@VehicleID",vehicleID)
            };

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "[BaoYang].[dbo].[VehicleSetting_GetPriorityVehicleRulesByPartNameAndType]", parameters))
            {
                while (reader.Read())
                {
                    BaoYangPriorityVehicleSettingModel item = new BaoYangPriorityVehicleSettingModel();

                    item.VehicleID = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    item.PartName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    item.PriorityType = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    item.FirstPriority = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                    item.SecondPriority = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);

                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// 根据VehicleID获取机油的特殊车型的配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public static BaoYangPriorityVehicleSettingModel GetJYPriorityVehicleSetting(SqlConnection conn, string vehicleID)
        {
            BaoYangPriorityVehicleSettingModel result = new BaoYangPriorityVehicleSettingModel();
            SqlParameter[] parameters =
            {
                new SqlParameter("@VehicleID",vehicleID)
            };

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "[BaoYang].[dbo].[VehicleSetting_GetJYPriorityVehicleSetting]", parameters))
            {
                while (reader.Read())
                {
                    result.VehicleID = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    result.PartName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    result.FirstPriority = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    result.SecondPriority = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                }
            }

            return result;
        }

        /// <summary>
        /// 插入机油保养特殊车型的配置信息
        /// </summary>
        /// <param name="projectList"></param>
        /// <param name="vehicleIDList"></param>
        /// <param name="partName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static int InsertBaoYangJYVehicleSeriesSetting(List<BaoYangPriorityVehicleSettingModel> projectList, List<string> vehicleIDList, string partName, string userName)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var result = 0;

            using (var dbhelper = new SqlDbHelper(conn))
            {
                try
                {
                    dbhelper.BeginTransaction();
                    foreach (var vehicleID in vehicleIDList)
                    {
                        var r = DALBaoyang.DeletedBaoYangVehicleSeriesSetting(dbhelper, vehicleID, partName, userName);

                        foreach (var item in projectList)
                        {
                            var row = DALBaoyang.InsertBaoyangVehicleSetting(dbhelper, vehicleID, item, userName);

                            if (row <= 0)
                            {
                                dbhelper.Rollback();
                                return -1;
                            }
                        }
                    }

                    dbhelper.Commit();
                    result = 1;
                }
                catch (Exception ex)
                {
                    dbhelper.Rollback();
                    return -1;
                }
            }

            return result;
        }

        /// <summary>
        /// 保存保养的特殊车型的配置信息
        /// </summary>
        /// <param name="projectList"></param>
        /// <param name="vehicleIDList"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static int InsertOrUpdateVehicleSetting(List<BaoYangPriorityVehicleSettingModel> projectList, List<string> vehicleIDList, string userName)
        {
            var result = 0;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                try
                {
                    dbhelper.BeginTransaction();
                    foreach (var vehicleID in vehicleIDList)
                    {
                        foreach (var item in projectList)
                        {
                            result = DALBaoyang.IsExistVehicleSetting(dbhelper, vehicleID, item.PartName, item.PriorityType);
                            if (result > 0)
                            {
                                var r = DALBaoyang.UpdateBaoyangVehicleSetting(dbhelper, vehicleID, item, userName);
                                if (r <= 0)
                                {
                                    dbhelper.Rollback();
                                    return -1;
                                }
                            }
                            else
                            {
                                var r = DALBaoyang.InsertBaoyangVehicleSetting(dbhelper, vehicleID, item, userName);
                                if (r <= 0)
                                {
                                    dbhelper.Rollback();
                                    return -1;
                                }
                            }
                        }
                    }

                    dbhelper.Commit();
                    result = 1;
                }
                catch (Exception ex)
                {
                    dbhelper.Rollback();
                    return -1;
                }

                return result;
            }
        }

        /// <summary>
        /// 插入信息的信息
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="vehicleID"></param>
        /// <param name="vehicleSettingModel"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static int InsertBaoyangVehicleSetting(SqlDbHelper dbhelper, string vehicleID, BaoYangPriorityVehicleSettingModel vehicleSettingModel, string userName)
        {
            using (var cmd = new SqlCommand("[BaoYang].[dbo].[VehicleSetting_InsertBaoyangVehicleSetting]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VehicleID", vehicleID);
                cmd.Parameters.AddWithValue("@PartName", vehicleSettingModel.PartName);
                cmd.Parameters.AddWithValue("@PriorityType", vehicleSettingModel.PriorityType == "" ? null : vehicleSettingModel.PriorityType);
                cmd.Parameters.AddWithValue("@FirstPriority", vehicleSettingModel.FirstPriority == "" ? null : vehicleSettingModel.FirstPriority);
                cmd.Parameters.AddWithValue("@SecondPriority", vehicleSettingModel.SecondPriority == "" ? null : vehicleSettingModel.SecondPriority);
                cmd.Parameters.AddWithValue("@UserName", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 更新存在的信息
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="vehicleID"></param>
        /// <param name="vehicleSettingModel"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static int UpdateBaoyangVehicleSetting(SqlDbHelper dbhelper, string vehicleID, BaoYangPriorityVehicleSettingModel vehicleSettingModel, string userName)
        {
            using (var cmd = new SqlCommand("[BaoYang].[dbo].[VehicleSetting_UpdateBaoyangVehicleSetting]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VehicleID", vehicleID);
                cmd.Parameters.AddWithValue("@PartName", vehicleSettingModel.PartName);
                cmd.Parameters.AddWithValue("@PriorityType", vehicleSettingModel.PriorityType);
                cmd.Parameters.AddWithValue("@FirstPriority", vehicleSettingModel.FirstPriority == "" ? null : vehicleSettingModel.FirstPriority);
                cmd.Parameters.AddWithValue("@SecondPriority", vehicleSettingModel.SecondPriority == "" ? null : vehicleSettingModel.SecondPriority);
                cmd.Parameters.AddWithValue("@UserName", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 判断特殊车型配置表是否包含养护类和品牌类的信息，存在Update,不存在Insert
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="vehicleID"></param>
        /// <param name="partName"></param>
        /// <param name="priorityType"></param>
        /// <returns></returns>
        public static int IsExistVehicleSetting(SqlDbHelper dbhelper, string vehicleID, string partName, string priorityType)
        {
            using (var cmd = new SqlCommand("[BaoYang].[dbo].[VehicleSetting_IsExistVehicleSetting]"))
            {
                var result = 0;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VehicleID", vehicleID);
                cmd.Parameters.AddWithValue("@PartName", partName);
                cmd.Parameters.AddWithValue("@PriorityType", priorityType);
                cmd.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output,
                    ParameterName = "@result",
                });

                dbhelper.ExecuteNonQuery(cmd);

                result = Convert.ToInt32(cmd.Parameters["@result"].Value);

                return result;
            }
        }


        public static int DeletedBaoYangPriorityVehicleSettingByVehicleIDAndPartName(SqlConnection conn, string vehicleID, string partName, string userName)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@VehicleID",vehicleID),
                new SqlParameter("@PartName",partName),
                new SqlParameter("UserName",userName)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "[BaoYang].[dbo].[VehicleSetting_DeletedBaoYangPriorityVehicleSettingByVehicleIDAndPartName]", parameters);
        }

        public static int BatchDeletedBaoYangVehicleConfig(SqlConnection conn, string vehicleIdStr, string partName, string userName)
        {
            var sql = @"  WITH  vehicleIdList AS ( SELECT   *  FROM     Gungnir..SplitString(@VehicleIdStr,',', 1)  )
                        UPDATE  BaoYang..BaoYangPriorityVehicleSetting
                        SET     IsDeleted = 1 ,
                                DeletedUser = @UserName ,
                                DeletedTime = GETDATE()
                        WHERE   PartName = @PartName
                                AND IsDeleted = 0
                                AND EXISTS ( SELECT 1
                                             FROM   vehicleIdList WITH ( NOLOCK )
                                             WHERE  VehicleId  COLLATE Chinese_PRC_CI_AS = Item );";
            SqlParameter[] parameters =
            {
                new SqlParameter("@VehicleIdStr",vehicleIdStr),
                new SqlParameter("@PartName",partName),
                new SqlParameter("UserName",userName)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 删除车型系列的特殊车型的配置信息通过partName和VehicleID
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleID"></param>
        /// <param name="partName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static int DeletedBaoYangVehicleSeriesSetting(SqlDbHelper dbhelper, string vehicleID, string partName, string userName)
        {
            using (var cmd = new SqlCommand("[BaoYang].[dbo].[VehicleSetting_DeletedBaoYangPriorityVehicleSettingByVehicleIDAndPartName]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VehicleID", vehicleID);
                cmd.Parameters.AddWithValue("@PartName", partName);
                cmd.Parameters.AddWithValue("@UserName", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static DataTable SelectBaoYangPriorityDetailsByPartName(string partName)
        {
            string sql = "SELECT * FROM BaoYang.[dbo].tbl_ProductPrioritySetting WITH ( NOLOCK ) WHERE PartName=@PartName ORDER BY ID DESC";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PartName", partName);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        public static DataTable SelectJYPriorityDetailsByPartName(string partName)
        {
            string sql = "SELECT * FROM BaoYang.[dbo].ProductRecommendPrioritySetting WITH ( NOLOCK ) WHERE PartName=@PartName ORDER BY PKID DESC";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PartName", partName);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        #region 机油品牌推荐优先级--指定手机号
        public static Tuple<List<OilBrandPhonePriorityModel>, int> SelectOilBrandPhonePriority(string phoneNumber,
            string brand,int pageIndex, int pageSize)
        {
            List<OilBrandPhonePriorityModel> result = new List<OilBrandPhonePriorityModel>();
            int totalCount = 0;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            #region sql
            var sql = @"SELECT  @Total = COUNT(1)
FROM BaoYang.dbo.BrandPriorityConfig AS s WITH (NOLOCK ) 
WHERE (@PhoneNumber IS NULL OR @PhoneNumber =N'' OR s.PhoneNumber LIKE '%'+@PhoneNumber+'%') 
AND (@Brand IS NULL OR @Brand =N'' OR s.Brand=@Brand) AND s.IsEnabled=1 AND s.PhoneNumber IS NOT NULL;
SELECT s.PKID,s.PhoneNumber, s.Brand,s.CreateDateTime,s.LastUpdateDateTime
FROM BaoYang.dbo.BrandPriorityConfig AS s WITH (NOLOCK )  
WHERE  (@PhoneNumber IS NULL OR @PhoneNumber =N'' OR  s.PhoneNumber LIKE '%'+@PhoneNumber+'%') 
AND (@Brand IS NULL OR @Brand =N'' OR s.Brand=@Brand) AND s.IsEnabled=1 
AND s.PhoneNumber IS NOT NULL AND s.ProductType='Oil'
 ORDER BY  s.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            #endregion
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@Brand", brand);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.Add(new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    var dt = dbhelper.ExecuteDataTable(cmd);
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                    result = dt.Rows.Cast<DataRow>().Select(row => new OilBrandPhonePriorityModel(row)).ToList();
                    return Tuple.Create(result, totalCount);
                }
            }    
        }


        public static bool IsRepeatOilBrandPhonePriority(OilBrandPhonePriorityModel model)
        {
            var sql = @"SELECT  COUNT(1)
FROM    BaoYang.dbo.BrandPriorityConfig as S with(nolock)
WHERE   (s.PhoneNumber=@PhoneNumber AND s.PKID<>@PKID AND s.ProductType='Oil');";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);             
                    cmd.Parameters.AddWithValue("@PKID", model.PKID);
                    var count = Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
                    return count > 0;
                }
            }
        }


        public static OilBrandPhonePriorityModel GetOilBrandPhonePriorityByPKID(int pkid)
        {
            var sql = @"SELECT s.PKID,s.PhoneNumber, s.Brand,s.CreateDateTime,s.LastUpdateDateTime
FROM BaoYang.dbo.BrandPriorityConfig AS s WITH (NOLOCK )
WHERE  s.PKID=@PKID AND s.ProductType='Oil';";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {          
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    var dr = dbhelper.ExecuteDataRow(cmd);
                    var result = new OilBrandPhonePriorityModel(dr);
                    return result ;
                }
            }
        }

        public static int AddOilBrandPhonePriority(SqlDbHelper dbhelper,OilBrandPhonePriorityModel model)
        {
            var sql = @"INSERT  INTO BaoYang.dbo.BrandPriorityConfig
					        ( PhoneNumber ,					         
					          Brand,
                              ProductType
					        )
					VALUES  (@PhoneNumber,
					@Brand,
                    'Oil')		
		           SELECT  SCOPE_IDENTITY();";         
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Brand", model.Brand);
                    int pkid = Convert.ToInt32(dbhelper.ExecuteScalar(cmd));                   
                    return pkid;
                }            
        }

        public static bool DeleteOilBrandPriorityByPKID(int pkid)
        {
            var sql = @"	DELETE FROM BaoYang.dbo.BrandPriorityConfig 
						WHERE PKID= @PKID;";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);    
                    var count = dbhelper.ExecuteNonQuery(cmd);
                    return count>0;
                }
            }
        }


        public static bool EditOilBrandPhonePriority(OilBrandPhonePriorityModel model)
        {
            var sql = @"UPDATE  BaoYang.dbo.BrandPriorityConfig
                        SET     Brand = @Brand ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@Brand", model.Brand);
                    cmd.Parameters.AddWithValue("@PKID", model.PKID);
                    var count = dbhelper.ExecuteNonQuery(cmd);
                    return count > 0;
                }
            }
        }
        #endregion


        #region 机油品牌推荐优先级配置--指定城市
        public static Tuple<List<OilBrandRegionPriorityModel>, int> SelectOilBrandRegionPriority(int provinceId,
         int regionId,string brand, int pageIndex, int pageSize)
        {
            List<OilBrandRegionPriorityModel> result = new List<OilBrandRegionPriorityModel>();
            int totalCount = 0;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            #region sql
            var sql = @"SELECT  @Total = COUNT(1)
FROM    BaoYang..BrandPriorityConfig AS t WITH(NOLOCK)
LEFT JOIN Gungnir..tbl_region AS r
ON r.PKID=t.RegionId
WHERE   (t.RegionId = @RegionId OR @RegionId=-1)
         AND (@Brand IS NULL OR @Brand =N'' OR t.Brand=@Brand) AND t.RegionId IS NOT NULL AND t.ProductType='Oil';
SELECT v.PKID,( CASE WHEN (s.ParentID=0) THEN  v.RegionId   ELSE s.ParentID END) AS ProvinceId,
		 ISNULL(( SELECT    tr.RegionName
          FROM      Gungnir..tbl_region AS tr WITH (NOLOCK)
          WHERE     s.ParentID = tr.PKID
        ),s.RegionName) AS ProvinceName,
		v.RegionId,s.RegionName AS CityName,v.Brand,v.CreateDateTime,v.LastUpdateDateTime FROM Gungnir..tbl_region AS s 
		LEFT  JOIN BaoYang..BrandPriorityConfig AS v
		ON s.PKID=v.RegionId
		WHERE   (v.RegionId = @RegionId OR @RegionId=-1)
		AND (s.ProvinceID = @ProvinceId OR @ProvinceId=-1)
         AND (@Brand IS NULL OR @Brand =N'' OR v.Brand=@Brand)
         AND v.PKID IS NOT NULL AND v.ProductType='Oil'
		 ORDER BY  v.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            #endregion
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@Brand", brand);
                    cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                    cmd.Parameters.AddWithValue("@RegionId", regionId);
                    cmd.Parameters.Add(new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    var dt = dbhelper.ExecuteDataTable(cmd);
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                    result = dt.Rows.Cast<DataRow>().Select(row => new OilBrandRegionPriorityModel(row)).ToList();
                    return Tuple.Create(result, totalCount);
                }
            }
        }

        public static bool IsRepeatOilBrandRegionPriority(OilBrandRegionPriorityModel model)
        {
            var sql = @"SELECT  COUNT(1)
FROM    BaoYang.dbo.BrandPriorityConfig as S with(nolock)
WHERE   (s.RegionId=@RegionId AND s.PKID<>@PKID AND s.ProductType='Oil');";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {                   
                    cmd.Parameters.AddWithValue("@RegionId", model.RegionId);
                    cmd.Parameters.AddWithValue("@PKID", model.PKID);
                    var count = Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
                    return count > 0;
                }
            }
        }


        public static OilBrandRegionPriorityModel GetOilBrandRegionPriorityByPKID(int pkid)
        {
            var sql = @"SELECT s.PKID,(CASE WHEN r.ParentID=0 THEN r.PKID ELSE r.ParentID END) AS ProvinceId ,ISNULL(( SELECT    tr.RegionName
          FROM      Gungnir..tbl_region AS tr WITH (NOLOCK)
          WHERE     r.ParentID = tr.PKID
        ),r.RegionName) AS ProvinceName ,s.RegionId,r.RegionName AS CityName, s.Brand,s.CreateDateTime,s.LastUpdateDateTime	 
FROM BaoYang.dbo.BrandPriorityConfig AS s WITH (NOLOCK )
LEFT JOIN Gungnir..tbl_region AS r
ON r.PKID=s.RegionId
WHERE  s.PKID=@PKID AND s.ProductType='Oil';";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    var dr = dbhelper.ExecuteDataRow(cmd);
                    var result = new OilBrandRegionPriorityModel(dr);
                    return result;
                }
            }
        }

        public static int AddOilBrandRegionPriority(OilBrandRegionPriorityModel model)
        {
            var sql = @"INSERT  INTO BaoYang.dbo.BrandPriorityConfig
					        ( RegionId ,					         
					          Brand,
                              ProductType
					        )
					VALUES  (@RegionId,
					@Brand,
                    'Oil')		
		           SELECT  SCOPE_IDENTITY();";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@RegionId", model.RegionId);
                    cmd.Parameters.AddWithValue("@Brand", model.Brand);
                    int pkid = Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
                    return pkid;
                }
            }
        }
        public static bool EditOilBrandRegionPriority(OilBrandRegionPriorityModel model)
        {
            var sql = @"UPDATE  BaoYang.dbo.BrandPriorityConfig
                        SET     Brand = @Brand ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@Brand", model.Brand);
                    cmd.Parameters.AddWithValue("@PKID", model.PKID);
                    var count = dbhelper.ExecuteNonQuery(cmd);
                    return count > 0;
                }
            }
        }
        #endregion

        #region 机油品牌优先级配置--用户购买过的品牌
        /// <summary>
        /// 获取用户购买过的机油
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Tuple<List<OilBrandUserOrderViewModel>, int> SelectOilBrandUserOrder
            (SqlConnection conn, Guid userId, int pageIndex, int pageSize)
        {
            var result = null as List<OilBrandUserOrderViewModel>;
            #region sql
            var sql = @"SELECT  @Total = COUNT(DISTINCT s.orderid)
                        FROM    Tuhu_bi..dm_oil_order_user_daily AS s WITH ( NOLOCK )
                        WHERE   s.userid = @UserId;
                        SELECT  s.orderid AS OrderId ,
                                STUFF(( SELECT  DISTINCT ',' + v.pid
                                        FROM    Tuhu_bi..dm_oil_order_user_daily AS v WITH ( NOLOCK )
                                        WHERE   v.userid = s.userid
                                                AND v.orderid = s.orderid
                                                AND v.VehicleID = s.VehicleID
                                                AND v.PaiLiang = s.PaiLiang
                                                AND v.Nian = s.Nian
                                      FOR
                                        XML PATH('')
                                      ), 1, 1, '') AS Pid ,
                                CONVERT(UNIQUEIDENTIFIER, s.userid) AS UserId ,
                                s.VehicleID ,
                                s.PaiLiang ,
                                s.Nian
                        FROM    Tuhu_bi..dm_oil_order_user_daily AS s WITH ( NOLOCK )
                        WHERE   s.userid = @UserId
                        GROUP BY s.userid ,
                                s.orderid ,
                                s.VehicleID ,
                                s.PaiLiang ,
                                s.Nian
                        ORDER BY s.orderid DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new SqlParameter[] {
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter("@UserId",userId.ToString()),
                    new SqlParameter("@Total", SqlDbType.Int, 4) { Direction = ParameterDirection.Output }
            };
            result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<OilBrandUserOrderViewModel>().ToList();
            var totalCount = Convert.ToInt32(parameters.LastOrDefault().Value);
            return Tuple.Create(result, totalCount);
        }
        #endregion

        #region 查看日志
        public static List<BaoYangOprLog> SelectBaoYangOprLog(string logType, string identityID, string operateUser, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int totalCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            #region sql
            var sql = @"SELECT  @TotalCount = COUNT(1)
FROM    Tuhu_Log..BaoYangOprLog AS log WITH ( NOLOCK )
WHERE   log.LogType = @LogType
        AND ( @IdentityID IS NULL
              OR @IdentityID = N''
              OR log.IdentityID = @IdentityID
            )
        AND ( @OperateUser IS NULL
              OR @OperateUser = ''
              OR log.OperateUser = @OperateUser
            )
        AND ( @StartTime IS NULL
              OR @StartTime = N''
              OR log.CreateTime >= @StartTime
            )
        AND ( @EndTime IS NULL
              OR @EndTime = N''
              OR log.CreateTime <= @EndTime
            );
SELECT  log.PKID ,
        log.LogType ,
        log.OldValue ,
        log.NewValue ,
        log.OperateUser ,
        log.Remarks ,
        log.IdentityID ,
        log.CreateTime
FROM    Tuhu_Log..BaoYangOprLog AS log WITH ( NOLOCK )
WHERE   log.LogType = @LogType
        AND ( @IdentityID IS NULL
              OR @IdentityID = N''
              OR log.IdentityID = @IdentityID
            )
        AND ( @OperateUser IS NULL
              OR @OperateUser = ''
              OR log.OperateUser = @OperateUser
            )
        AND ( @StartTime IS NULL
              OR @StartTime = N''
              OR log.CreateTime >= @StartTime
            )
        AND ( @EndTime IS NULL
              OR @EndTime = N''
              OR log.CreateTime <= @EndTime
            )
ORDER BY log.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            #endregion
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@LogType", logType);
                    cmd.Parameters.AddWithValue("@IdentityID", identityID);
                    cmd.Parameters.AddWithValue("@OperateUser", operateUser);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.Add(new SqlParameter("@TotalCount", SqlDbType.Int, 4) { Direction = ParameterDirection.Output });
                    var dt = dbhelper.ExecuteDataTable(cmd);
                    totalCount = (int)cmd.Parameters["@TotalCount"].Value;
                    var result = (from DataRow row in dt.Rows
                                  select new BaoYangOprLog
                                  {
                                      PKID = (long)row["PKID"],
                                      LogType = row["LogType"].ToString(),
                                      OldValue = row["OldValue"].ToString(),
                                      NewValue = row["NewValue"].ToString(),
                                      Remarks = row["Remarks"].ToString(),
                                      OperateUser = row["OperateUser"].ToString(),
                                      CreateTime = (DateTime)row["CreateTime"],
                                      IdentityID = row["IdentityID"].ToString(),
                                  }).ToList();
                    return result;
                };
            }
        }
        public static BaoYangOprLog GetBaoYangOprLogByPKID(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            #region sql
            var sql = @"SELECT 
		log.PKID ,
        log.LogType ,
        log.OldValue ,
        log.NewValue ,
        log.OperateUser ,
        log.Remarks ,
        log.IdentityID ,
        log.CreateTime
FROM    Tuhu_Log..BaoYangOprLog AS log WITH ( NOLOCK )
WHERE   log.PKID=@PKID;";
            #endregion
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    var row = dbhelper.ExecuteDataRow(cmd);
                    return new BaoYangOprLog
                    {
                        PKID = (long)row["PKID"],
                        LogType = row["LogType"].ToString(),
                        OldValue = row["OldValue"].ToString(),
                        NewValue = row["NewValue"].ToString(),
                        Remarks = row["Remarks"].ToString(),
                        OperateUser = row["OperateUser"].ToString(),
                        CreateTime = (DateTime)row["CreateTime"],
                        IdentityID = row["IdentityID"].ToString(),
                    };
                }
            }

        }
        #endregion

        #region Vehicle settingNew

        public static Tuple<int, IEnumerable<FullPriorityVehicleSettingNew>> SelectBaoYangVehicleSettingNewOil(SqlConnection conn,
            VehicleSettingNewSearchRequest request)
        {
            const string sqlBase = @"SELECT  vtt.VehicleID ,
        vt.Brand ,
        vt.Vehicle ,
        MIN(vtt.GuidePrice) AS Price ,
        COUNT(vtt.VehicleID) OVER ( ) AS Total
FROM    Gungnir.dbo.tbl_Vehicle_Type AS vt WITH ( NOLOCK )
        JOIN Gungnir.dbo.tbl_Vehicle_Type_Timing AS vtt WITH ( NOLOCK ) ON vt.ProductID = vtt.VehicleID
WHERE   ISNULL(vtt.GuidePrice, 0) >= @MinPrice
        AND ISNULL(vtt.GuidePrice, 0) <= @MaxPrice
        {0}
GROUP BY vtt.VehicleID ,
        vt.Brand ,
        vt.Vehicle
ORDER BY vt.Brand ,
        vt.Vehicle
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

            var condition = string.Empty;
            var skip = (request.PageIndex - 1) * request.PageSize;
            var take = request.PageSize;
            var parameterList = new List<SqlParameter>
            {
                new SqlParameter("@Skip", skip),
                new SqlParameter("@Take", take),
                new SqlParameter("@MaxPrice", (double)(request.MaxPrice ?? int.MaxValue)),
                new SqlParameter("@MinPrice", (double)(request.MinPrice ?? -1)),
            };
            if (!string.IsNullOrEmpty(request.VehicleId))
            {
                condition = $"{condition} AND @VehicleId = vtt.VehicleID";
                parameterList.Add(new SqlParameter("@VehicleId", request.VehicleId));
            }

            if (!string.IsNullOrEmpty(request.Brand))
            {
                condition = $"{condition} AND vt.Brand = @Brand";
                parameterList.Add(new SqlParameter("@Brand", request.Brand));
            }

            if (!string.IsNullOrEmpty(request.Viscosity) || !string.IsNullOrEmpty(request.Grade))
            {
                var innerSql = @"SELECT  1
FROM    BaoYang..tbl_PartAccessory AS pt WITH ( NOLOCK )
WHERE   pt.AccessoryName = N'发动机油'
        AND pt.IsDeleted = 0
        AND pt.TID = CAST(vtt.TID AS INT)";
                if (!string.IsNullOrEmpty(request.Viscosity))
                {
                    innerSql = $"{innerSql} AND pt.Viscosity = @Viscosity";
                    parameterList.Add(new SqlParameter("@Viscosity", request.Viscosity));
                }
                if (!string.IsNullOrEmpty(request.Grade))
                {
                    innerSql = $"{innerSql} AND pt.Grade = @Grade";
                    parameterList.Add(new SqlParameter("@Grade", request.Grade));
                }
                condition = $@"{condition} AND EXISTS ( {innerSql} )";
            }

            if (request.IsConfig)
            {
                condition = $@"{condition} AND EXISTS ( SELECT 1
                     FROM   BaoYang..BaoYangPriorityVehicleSettingNew AS pv
                            WITH ( NOLOCK )
                     WHERE  pv.VehicleId = vtt.VehicleID COLLATE Chinese_PRC_CI_AS )";
            }

            if (request.IsEnabled)
            {
                condition = $@"{condition} AND NOT EXISTS ( SELECT 1
                         FROM   BaoYang..BaoYangPriorityVehicleSettingNew AS pv
                                WITH ( NOLOCK )
                         WHERE  pv.IsEnabled = 0
                                AND pv.VehicleId = vtt.VehicleID COLLATE Chinese_PRC_CI_AS )";
            }

            if (!string.IsNullOrEmpty(request.PriorityBrand) ||
                !string.IsNullOrEmpty(request.PrioritySeries) ||
                !string.IsNullOrEmpty(request.PriorityType) ||
                request.Priority > 0)
            {
                var innerSql = @"SELECT 1
                     FROM   BaoYang..BaoYangPriorityVehicleSettingNew AS pv
                            WITH ( NOLOCK )
                     WHERE  pv.VehicleId = vtt.VehicleID COLLATE Chinese_PRC_CI_AS";
                if (request.Priority > 0)
                {
                    innerSql = $"{innerSql} AND pv.Priority = @Priority";
                    parameterList.Add(new SqlParameter("@Priority", request.Priority));
                }

                if (!string.IsNullOrEmpty(request.PriorityBrand))
                {
                    innerSql = $"{innerSql} AND pv.Brand = @PriorityBrand";
                    parameterList.Add(new SqlParameter("@PriorityBrand", request.PriorityBrand));
                }

                if (!string.IsNullOrEmpty(request.PrioritySeries))
                {
                    innerSql = $"{innerSql} AND pv.Series = @PrioritySeries";
                    parameterList.Add(new SqlParameter("@PrioritySeries", request.PrioritySeries));
                }

                if (!string.IsNullOrEmpty(request.PriorityType))
                {
                    innerSql = $"{innerSql} AND pv.PriorityType = @PriorityType";
                    parameterList.Add(new SqlParameter("@PriorityType", request.PriorityType));
                }

                condition = $"{condition} AND EXISTS ({innerSql})";
            }

            var sql = string.Format(sqlBase, condition);
            var parameters = parameterList.ToArray();
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var list = dt.Rows.OfType<DataRow>().Select(x => new
            {
                VehicleId = x["VehicleID"].ToString(),
                Brand = x["Brand"].ToString(),
                Vehicle = x["Vehicle"].ToString(),
                Price = DBNull.Value.Equals(x["Price"]) ? 0M : (decimal)Convert.ToDouble(x["Price"]),
                Total = Convert.ToInt32(x["Total"]),
            });
            var total = list.FirstOrDefault()?.Total ?? 0;
            var result = list.Select(x => new FullPriorityVehicleSettingNew
            {
                Brand = x.Brand,
                Price = x.Price,
                Vehicle = x.Vehicle,
                VehicleId = x.VehicleId,
            });
            return Tuple.Create(total, result);
        }

        public static List<PriorityVehicleSettingNew> SelectBaoYangVehicleSettingNew(SqlConnection conn, List<string> vehicleIds, List<string> partNames)
        {
            const string sql = @"SELECT  pv.PKID ,
        pv.VehicleId ,
        pv.PartName ,
        pv.PriorityType ,
        pv.Brand ,
        pv.Series ,
        pv.PID ,
        pv.Priority ,
        pv.IsEnabled ,
        pv.CreateDateTime ,
        pv.LastUpdateDateTime
FROM    BaoYang..BaoYangPriorityVehicleSettingNew AS pv WITH ( NOLOCK )
WHERE   EXISTS ( SELECT 1
                 FROM   Gungnir.dbo.SplitString(@PartNames, N',', 1)
                 WHERE  pv.PartName = Item COLLATE Chinese_PRC_CI_AS )
        AND EXISTS ( SELECT 1
                     FROM   Gungnir.dbo.SplitString(@VehicleIds, N',', 1)
                     WHERE  pv.VehicleId = Item COLLATE Chinese_PRC_CI_AS );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PartNames", string.Join(",", partNames?? new List<string>())),
                new SqlParameter("@VehicleIds", string.Join(",", vehicleIds ?? new List<string>())),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.ConvertTo<PriorityVehicleSettingNew>().ToList();
            return result;
        }

        /// <summary>
        /// 根据vid查询粘度
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleIds"></param>
        /// <returns></returns>
        public static List<PriorityVehicleOilModel> SelectBaoYangVehicleOilModel(SqlConnection conn, List<string> vehicleIds)
        {
            const string sql = @"SELECT DISTINCT
        vtt.VehicleID ,
        pt.Viscosity ,
        pt.Grade
FROM    Gungnir..tbl_Vehicle_Type_Timing AS vtt WITH ( NOLOCK )
        JOIN Gungnir.dbo.SplitString(@VehicleIds, N',', 1) ON vtt.VehicleID = Item
        JOIN BaoYang..tbl_PartAccessory AS pt WITH ( NOLOCK ) ON pt.TID = CAST(vtt.TID AS INT)
WHERE   pt.IsDeleted = 0
        AND pt.AccessoryName = N'发动机油'
        AND pt.Viscosity IS NOT NULL
        AND pt.Viscosity <> N'';";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@VehicleIds", string.Join(",", vehicleIds ?? new List<string>())),
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<PriorityVehicleOilModel>().ToList();
            return result;
        }

        public static bool DelBaoYangVehicleSettingNewByPkids(SqlConnection conn, List<int> pkids)
        {
            const string sql = @"DELETE  FROM BaoYang..BaoYangPriorityVehicleSettingNew
WHERE   EXISTS ( SELECT 1
                 FROM   Gungnir..SplitString(@Ids, N',', 1)
                 WHERE  PKID = CAST(Item AS INT) );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Ids", string.Join(",", pkids ?? new List<int>())),
            };
            var rows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        public static bool DelBaoYangVehicleSettingNew(SqlConnection conn, List<string> vehicleIds, string partName)
        {
            const string sql = @"DELETE  FROM BaoYang..BaoYangPriorityVehicleSettingNew
WHERE   PartName = @PartName
        AND EXISTS ( SELECT 1
                     FROM   Gungnir..SplitString(@VehicleIds, N',', 1)
                     WHERE  VehicleId = Item COLLATE Chinese_PRC_CI_AS );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@VehicleIds", string.Join(",", vehicleIds ?? new List<string>())),
                new SqlParameter("@PartName", partName),
            };
            var rows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        public static bool AddPriorityVehicleSettingNew(SqlConnection conn, PriorityVehicleSettingNew setting)
        {
            const string sql = @"INSERT  INTO BaoYang..BaoYangPriorityVehicleSettingNew
        ( VehicleId ,
          PartName ,
          PriorityType ,
          Brand ,
          Series ,
          PID ,
          Priority ,
          IsEnabled ,
          CreateDateTime ,
          LastUpdateDateTime
        )
VALUES  ( @VehicleId ,
          @PartName ,
          @PriorityType ,
          @Brand ,
          @Series ,
          @PID ,
          @Priority ,
          @IsEnabled ,
          GETDATE() ,
          GETDATE()
        );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@VehicleId", setting.VehicleId),
                new SqlParameter("@PartName", setting.PartName),
                new SqlParameter("@PriorityType", setting.PriorityType),
                new SqlParameter("@Brand", setting.Brand),
                new SqlParameter("@Series", setting.Series),
                new SqlParameter("@PID", setting.PID),
                new SqlParameter("@Priority", setting.Priority),
                new SqlParameter("@IsEnabled", setting.IsEnabled),
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool EnableOrDisableVehicleSettingNew(SqlConnection conn, List<string> vehicleIds, string partName, bool isEnabled)
        {
            const string sql = @"UPDATE  BaoYang..BaoYangPriorityVehicleSettingNew
SET     IsEnabled = @IsEnabled ,
        LastUpdateDateTime = GETDATE()
WHERE   PartName = @PartName
        AND EXISTS ( SELECT 1
                     FROM   Gungnir..SplitString(@VehicleIds, N',', 1)
                     WHERE  VehicleId = Item COLLATE Chinese_PRC_CI_AS );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@VehicleIds", string.Join(",", vehicleIds ?? new List<string>())),
                new SqlParameter("@PartName", partName),
                new SqlParameter("@IsEnabled", isEnabled),
            };
            var rows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        public static Dictionary<string, List<string>> SelectOilBrandAndSeries(SqlConnection conn)
        {
            const string sql = @"SELECT DISTINCT
        p.CP_Brand ,
        p.CP_Remark
FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS p WITH ( NOLOCK )
WHERE   p.PrimaryParentCategory = N'Oil'
        AND p.IsUsedInAdaptation = 1
        AND p.stockout = 0
        AND p.OnSale = 1
        AND p.i_ClassType IN ( 2, 4 )
        AND p.CP_Brand IS NOT NULL
        AND p.CP_Brand <> N''
        AND p.CP_Remark IS NOT NULL
        AND p.CP_Remark <> N'';";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            var result = dt.Rows.Cast<DataRow>().Select(row => new
            {
                Brand = row["CP_Brand"].ToString(),
                Series = row["CP_Remark"].ToString(),
            }).GroupBy(x => x.Brand).ToDictionary(x => x.Key, list => list.Select(x => x.Series).ToList());
            return result;
        }

        #endregion

        #region Product SettingNew

        /// <summary>
        /// 根据PartName获取配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public static IEnumerable<ProductPrioritySettingNew> SelectProductPrioritySettingsNew(SqlConnection conn, IEnumerable<string> partNames)
        {
            const string sql = @"SELECT  t.PKID ,
        t.PartName ,
        t.PriorityType ,
        t.Brand ,
        t.Series ,
        t.PID ,
        t.Priority ,
        t.IsEnabled ,
        t.CreateDateTime ,
        t.LastUpdateDateTime
FROM    BaoYang..ProductPrioritySettingNew AS t WITH ( NOLOCK )
WHERE   EXISTS ( SELECT 1
                 FROM   Gungnir..SplitString(@PartNames, N',', 1)
                 WHERE  t.PartName = Item COLLATE Chinese_PRC_CI_AS );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter(@"PartNames", string.Join(",", partNames ?? new List<string>())),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            return dt.ConvertTo<ProductPrioritySettingNew>();
        }
        
        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public static bool DeleteProductPrioritySettingsNew(SqlConnection conn, IEnumerable<int> pkids)
        {
            const string sql = @"DELETE  FROM BaoYang..ProductPrioritySettingNew
WHERE   EXISTS ( SELECT 1
                 FROM   Gungnir.dbo.SplitString(@PKIDs, N',', 1)
                 WHERE  PKID = CAST(Item AS INT) );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKIDs", string.Join(",", pkids ?? new List<int>())),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static int AddProductPrioritySettingsNew(SqlConnection conn, ProductPrioritySettingNew setting)
        {
            const string sql = @"INSERT  INTO BaoYang..ProductPrioritySettingNew
        ( PartName ,
          PriorityType ,
          Brand ,
          Series ,
          PID ,
          Priority ,
          IsEnabled ,
          CreateDateTime ,
          LastUpdateDateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @PartName ,
          @PriorityType ,
          @Brand ,
          @Series ,
          @PID ,
          @Priority ,
          @IsEnabled ,
          GETDATE() ,
          GETDATE()
        );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PartName", setting.PartName),
                new SqlParameter("@PriorityType", setting.PriorityType),
                new SqlParameter("@Brand", setting.Brand),
                new SqlParameter("@Series", setting.Series),
                new SqlParameter("@PID", setting.PID),
                new SqlParameter("@Priority", setting.Priority),
                new SqlParameter("@IsEnabled", setting.IsEnabled),
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        public static bool UpdateProductPrioritySettingNew(SqlConnection conn, ProductPrioritySettingNew setting)
        {
            const string sql = @"UPDATE  BaoYang.dbo.ProductPrioritySettingNew
SET     Brand = @Brand ,
        Series = @Series ,
        PID = @PID ,
        IsEnabled = @IsEnabled ,
        Priority=@Priority,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKID", setting.PKID),
                new SqlParameter("@Brand", setting.Brand),
                new SqlParameter("@Series", setting.Series),
                new SqlParameter("@PID", setting.PID),
                new SqlParameter("@Priority", setting.Priority),
                new SqlParameter("@IsEnabled", setting.IsEnabled),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        #endregion

        /// <summary>
        /// 更新或插入保养升级购图标配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static bool UpsertBaoYangLevelUpIcon(SqlConnection conn, string icon)
        {
            const string sql = @"IF exists(
                                    SELECT 1
                                    FROM BaoYang..BaoYangConfig
                                    WHERE ConfigName = N'BaoYangLevelUpIcon')
                                  BEGIN
                                    UPDATE BaoYang..BaoYangConfig
                                    SET Config = @Config
                                    WHERE ConfigName = N'BaoYangLevelUpIcon';
                                  END
                                ELSE
                                  BEGIN
                                    INSERT BaoYang..BaoYangConfig
                                    (ConfigName, Config, CreatedTime, UpdatedTime)
                                    VALUES
                                      ('BaoYangLevelUpIcon', @Config, getdate(), getdate())
                                  END";
            var parameters = new SqlParameter[]
           {
                new SqlParameter("@Config", $"<BaoYangLevelUpIcon>{icon}</BaoYangLevelUpIcon>")
           };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
    }
}
