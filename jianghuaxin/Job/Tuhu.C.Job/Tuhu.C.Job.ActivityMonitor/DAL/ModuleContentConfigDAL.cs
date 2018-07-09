using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tuhu.C.Job.ActivityMonitor.Model;

namespace Tuhu.C.Job.ActivityMonitor.DAL
{
    public class ModuleContentConfigDAL
    {
        public static IEnumerable<ModuleContentConfig> GetActivityModuleContentConfigList()
        {
            IEnumerable<ModuleContentConfig> moduleContentConfigList;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = @"SELECT ID,ParentID,HomePageName,ModuleName,HelperModuleName,MoreUri,StartVersion,EndVersion,Title,LinkUrl FROM
                            (
                           SELECT HPC.HomePageName,HPMC.ModuleName,'' AS HelperModuleName, HPMC.MoreUri, HPMCC.ID, HPMCC.FKHomePageModuleID AS ParentID, HPMCC.StartVersion, HPMCC.EndVersion, HPMCC.Title, HPMCC.LinkUrl FROM Configuration..SE_HomePageConfig HPC WITH(NOLOCK)
                            INNER JOIN Configuration..SE_HomePageModuleConfig HPMC WITH(NOLOCK)
                            ON HPC.ID = HPMC.FKHomePage AND HPC.IsEnabled = 1 AND HPMC.IsEnabled=1
                            INNER JOIN Configuration..SE_HomePageModuleContentConfig HPMCC WITH(NOLOCK)
                            ON HPMCC.FKHomePageModuleID = HPMC.ID AND HPMCC.IsEnabled=1 
                            UNION
                            SELECT HPC.HomePageName,HPMC.ModuleName,HPMHC.ModuleName AS HelperModuleName, HPMHC.MoreUri, HPMCC.ID, HPMCC.FKHomePageModuleHelperID AS ParentID, HPMCC.StartVersion, HPMCC.EndVersion, HPMCC.Title, HPMCC.LinkUrl FROM Configuration..SE_HomePageConfig HPC WITH(NOLOCK)
                            INNER JOIN Configuration..SE_HomePageModuleConfig HPMC WITH(NOLOCK)
                            ON HPC.ID = HPMC.FKHomePage AND HPC.IsEnabled = 1 AND HPMC.IsEnabled=1 AND HPMC.IsChildModule=1
                            INNER JOIN Configuration..SE_HomePageModuleHelperConfig HPMHC WITH(NOLOCK)
                            ON HPMHC.FKHomePageModuleID = HPMC.ID AND HPMHC.IsEnabled = 1
                            INNER JOIN Configuration..SE_HomePageModuleContentConfig HPMCC WITH(NOLOCK)
                            ON HPMCC.FKHomePageModuleHelperID = HPMHC.ID AND HPMCC.IsEnabled=1
                            ) ModuleContentConfig
                          ";
                using (var cmd = new SqlCommand(sql))
                {
                    moduleContentConfigList = helper.ExecuteQuery(cmd, ConvertToModuleContentConfig);
                }
            }
            return moduleContentConfigList;
        }

        public static IEnumerable<ModuleContentConfig> GetPromotionModuleContentConfigList()
        {
            IEnumerable<ModuleContentConfig> moduleContentConfigList;
            using (var helper = DbHelper.CreateDbHelper(true))
            {

                string sql = @"SELECT ID,ParentID,HomePageName,ModuleName,HelperModuleName,MoreUri,StartVersion,EndVersion,Title,LinkUrl FROM
                            (
                           SELECT HPC.HomePageName,HPMC.ModuleName,'' AS HelperModuleName, HPMC.MoreUri, HPMCC.ID, HPMCC.FKHomePageModuleID AS ParentID, HPMCC.StartVersion, HPMCC.EndVersion, HPMCC.Title, HPMCC.LinkUrl FROM Configuration..SE_HomePageConfig HPC WITH(NOLOCK)
                            INNER JOIN Configuration..SE_HomePageModuleConfig HPMC WITH(NOLOCK)
                            ON HPC.ID = HPMC.FKHomePage AND HPC.IsEnabled = 1 AND HPMC.IsEnabled=1
                            INNER JOIN Configuration..SE_HomePageModuleContentConfig HPMCC WITH(NOLOCK)
                            ON HPMCC.FKHomePageModuleID = HPMC.ID AND HPMCC.IsEnabled=1 
                            UNION
                            SELECT HPC.HomePageName,HPMC.ModuleName,HPMHC.ModuleName AS HelperModuleName, HPMHC.MoreUri, HPMCC.ID, HPMCC.FKHomePageModuleHelperID AS ParentID, HPMCC.StartVersion, HPMCC.EndVersion, HPMCC.Title, HPMCC.LinkUrl FROM Configuration..SE_HomePageConfig HPC WITH(NOLOCK)
                            INNER JOIN Configuration..SE_HomePageModuleConfig HPMC WITH(NOLOCK)
                            ON HPC.ID = HPMC.FKHomePage AND HPC.IsEnabled = 1 AND HPMC.IsEnabled=1 AND HPMC.IsChildModule=1
                            INNER JOIN Configuration..SE_HomePageModuleHelperConfig HPMHC WITH(NOLOCK)
                            ON HPMHC.FKHomePageModuleID = HPMC.ID AND HPMHC.IsEnabled = 1
                            INNER JOIN Configuration..SE_HomePageModuleContentConfig HPMCC WITH(NOLOCK)
                            ON HPMCC.FKHomePageModuleHelperID = HPMHC.ID AND HPMCC.IsEnabled=1
                            ) ModuleContentConfig
                           ";
                using (var cmd = new SqlCommand(sql))
                {
                    moduleContentConfigList = helper.ExecuteQuery(cmd, ConvertToModuleContentConfig);
                }
            }
            return moduleContentConfigList;
        }

        public static IEnumerable<FlowConfig> GetFlowConfigList()
        {
            IEnumerable<FlowConfig> flowConfigList;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = "SELECT ID,Title,LinkUrl,StartVersion,EndVersion,StartDateTime,EndDateTime  FROM Configuration..SE_HomePageFlowConfig WITH ( NOLOCK) WHERE IsEnabled=1";
                using (var cmd = new SqlCommand(sql))
                {
                    flowConfigList = helper.ExecuteQuery(cmd, ConvertToFlowConfig);
                }
            }
            return flowConfigList;
        }

        public static IEnumerable<FlowConfig> GetFlowConfigActivityList()
        {
            IEnumerable<FlowConfig> flowConfigActivityList;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = "SELECT  ID,Title,LinkUrl,StartVersion,EndVersion,StartDateTime,EndDateTime FROM Configuration..SE_HomePageFlowConfig WITH ( NOLOCK) WHERE IsEnabled=1 and Type=1";
                using (var cmd = new SqlCommand(sql))
                {

                    flowConfigActivityList = helper.ExecuteQuery(cmd, ConvertToFlowConfig);
                }
            }
            return flowConfigActivityList;
        }

        public static IEnumerable<FlowConfig> GetFlowConfigPromotionList()
        {
            IEnumerable<FlowConfig> flowConfigPromotionList;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = "SELECT  ID,Title,LinkUrl,StartVersion,EndVersion,StartDateTime,EndDateTime FROM Configuration..SE_HomePageFlowConfig WITH ( NOLOCK) WHERE IsEnabled=1 and Type=1";
                using (var cmd = new SqlCommand(sql))
                {

                    flowConfigPromotionList = helper.ExecuteQuery(cmd, ConvertToFlowConfig);
                }
            }
            return flowConfigPromotionList;
        }

        public static List<ModuleContentConfig> ConvertToModuleContentConfig(DataTable dt)
        {
            List<ModuleContentConfig> list = new List<ModuleContentConfig>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var item = new ModuleContentConfig()
                    {
                        ID = Convert.ToInt32(row["ID"] == DBNull.Value ? 0 : row["ID"]),
                        ParentID = Convert.ToInt32(row["ParentID"] == DBNull.Value ? 0 : row["ParentID"]),
                        StartVersion = row["StartVersion"]?.ToString(),
                        EndVersion = row["EndVersion"]?.ToString(),
                        Title = row["Title"]?.ToString(),
                        LinkUrl = HttpUtility.UrlDecode(row["LinkUrl"] == DBNull.Value ? "" : row["LinkUrl"].ToString()),
                        ModuleName = row["ModuleName"]?.ToString(),
                        HelperModuleName = row["HelperModuleName"]?.ToString(),
                        HomePageName = row["HomePageName"]?.ToString(),
                        MoreUri = HttpUtility.UrlDecode(row["MoreUri"] == DBNull.Value ? "" : row["MoreUri"].ToString()),
                    };
                    list.Add(item);
                }
            }
            return list;
        }


        public static List<FlowConfig> ConvertToFlowConfig(DataTable dt)
        {
            List<FlowConfig> list = new List<FlowConfig>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var item = new FlowConfig()
                    {
                        ID = Convert.ToInt32(row["ID"] == DBNull.Value ? 0 : row["ID"]),
                        StartVersion = row["StartVersion"]?.ToString(),
                        EndVersion = row["EndVersion"]?.ToString(),
                        Title = row["Title"]?.ToString(),
                        StartDateTime = Convert.ToDateTime(row["StartDateTime"] == DBNull.Value ? DateTime.MinValue : row["StartDateTime"]),
                        EndDateTime = Convert.ToDateTime(row["EndDateTime"] == DBNull.Value ? DateTime.MaxValue : row["EndDateTime"]),
                        LinkUrl = HttpUtility.UrlDecode(row["LinkUrl"] == DBNull.Value ? "" : row["LinkUrl"].ToString()),
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public static List<ActivityBuild> GetActivityBuildById(int id)
        {
            List<ActivityBuild> activityBuildList = new List<ActivityBuild>();
            try
            {
                using (var helper = DbHelper.CreateDbHelper(true))
                {
                    string sql = "SELECT id,Title,Content,StartDT,EndDate FROM Activity..ActivityBuild WITH ( NOLOCK) where id=@id";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);
                        activityBuildList = helper.ExecuteQuery(cmd, ConvertToActivityBuild);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return activityBuildList;
        }


        public static List<ActivePage> GetActivePageByHashKey(string hashkey)
        {
            List<ActivePage> activePageList;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = "SELECT PKID,Title,HashKey,RuleDesc,StartDate,EndDate FROM Configuration..ActivePageList WITH ( NOLOCK) where hashkey=@hashkey";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@hashkey", hashkey);
                    activePageList = helper.ExecuteQuery(cmd, ConvertToActivePage);
                }
            }
            return activePageList;
        }

        public static List<ActivityBuild> GetSpecificDaysExpireOldActivities(int days)
        {
            List<ActivityBuild> activityBuildList = new List<ActivityBuild>();
            try
            {
                using (var helper = DbHelper.CreateDbHelper(true))
                {
                    string sql = "SELECT id,Title,StartDT,EndDate FROM Activity..ActivityBuild WITH ( NOLOCK) where datediff(dd,getdate(),EndDate)>0 and datediff(dd,getdate(),EndDate)<=@days";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@days", days);
                        activityBuildList = helper.ExecuteQuery(cmd, ConvertToActivityBuild);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return activityBuildList;
        }

        public static List<ActivePage> GetSpecificDaysExpireNewActivities(int days)
        {
            // SELECT PKID, Title, EndDate FROM Configuration..ActivePageList WITH(NOLOCK) where EndDate is not null and IsEnabled = 1 and datediff(dd, getdate(), EndDate)> 0 and datediff(dd, getdate(), EndDate)<= 4
            List<ActivePage> activePageList;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = "SELECT PKID, Title,RuleDesc,HashKey,StartDate, EndDate FROM Configuration..ActivePageList WITH(NOLOCK) where EndDate is not null and IsEnabled = 1 and datediff(dd, getdate(), EndDate)> 0 and datediff(dd, getdate(), EndDate)<= @days";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@days", days);
                    activePageList = helper.ExecuteQuery(cmd, ConvertToActivePage);
                }
            }
            return activePageList;
        }

        public static List<ActivityBuild> ConvertToActivityBuild(DataTable dt)
        {


            List<ActivityBuild> list = new List<ActivityBuild>();
            try
            {
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new ActivityBuild()
                        {
                            id = Convert.ToInt32(row["id"] == DBNull.Value ? 0 : row["id"]),
                            Title = row["Title"]?.ToString(),
                            Content = row["Content"]?.ToString(),
                            StartDT = Convert.ToDateTime(row["StartDT"] == DBNull.Value ? DateTime.MinValue : row["StartDT"]),
                            EndDate = Convert.ToDateTime(row["EndDate"] == DBNull.Value ? DateTime.MaxValue : row["EndDate"])
                        };
                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return list;
        }




        public static List<ActivePage> ConvertToActivePage(DataTable dt)
        {


            List<ActivePage> list = new List<ActivePage>();
            try
            {
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new ActivePage()
                        {
                            PKID = Convert.ToInt32(row["PKID"] == DBNull.Value ? 0 : row["PKID"]),
                            Title = row["Title"]?.ToString(),
                            HashKey = row["HashKey"]?.ToString(),
                            RuleDesc = row["RuleDesc"]?.ToString(),
                            StartDate = Convert.ToDateTime(row["StartDate"] == DBNull.Value ? DateTime.MinValue : row["StartDate"]),
                            EndDate = Convert.ToDateTime(row["EndDate"] == DBNull.Value ? DateTime.MaxValue : row["EndDate"])
                        };
                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return list;
        }
    }
}
