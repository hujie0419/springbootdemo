using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity.AllSort;

namespace Tuhu.Provisioning.DataAccess.DAO.AllSort
{
    public class DalSort
    {
        /// <summary>
        ///     获取所有全品类分类
        /// </summary>
        public IEnumerable<AllSortConfig> GetList(SqlConnection connection)
        {
            var sql = "SELECT *  FROM[Configuration].[dbo].[AllSortConfig] where IsAbled='1'  ORDER BY Priority ASC ";
            return connection.Query<AllSortConfig>(sql);
        }

        /// <summary>
        ///     逻辑删除选中的全品类分类
        /// </summary>
        public bool DeleteEntity(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection.Execute("UPDATE Configuration.dbo.AllSortConfig SET IsAbled=0,State=0 WHERE PKID=@PKID",
                       dp) > 0;
        }

        /// <summary>
        ///     逻辑删除选中的全品类分类标签
        /// </summary>
        public bool DeleteTagEntity(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection.Execute(
                       "UPDATE Configuration.dbo.AllSortTagConfig SET IsAbled=0,State=0 WHERE PKID=@PKID", dp) > 0;
        }

        /// <summary>
        ///     逻辑删除选中的全品类分类内容
        /// </summary>
        public bool DeleteListEntity(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection.Execute(
                       "UPDATE Configuration.dbo.AllSortListConfig SET IsAbled=0,State=0 WHERE PKID=@PKID", dp) > 0;
        }

        /// <summary>
        ///     获取选中的全品类分类详情
        /// </summary>
        public AllSortConfig GetEntity(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            dp.Add("@ParentId", id);
            var allSortConfig = connection
                .Query<AllSortConfig>(
                    "SELECT * FROM Configuration.dbo.AllSortConfig  (NOLOCK) WHERE PKID=@PKID and IsAbled='1'", dp)
                .FirstOrDefault();
            if (allSortConfig != null)
                allSortConfig.Tag = connection
                    .Query<AllSortTagConfig>(
                        "SELECT * FROM Configuration.dbo.AllSortTagConfig  (NOLOCK) WHERE ParentId=@ParentId and IsAbled='1' ",
                        dp)
                    .FirstOrDefault();
            if (allSortConfig != null)
                allSortConfig.ListConfig = connection
                    .Query<AllSortListConfig>(
                        "SELECT * FROM Configuration.dbo.AllSortListConfig  (NOLOCK) WHERE ParentId=@ParentId and IsAbled='1' ORDER BY Priority ASC",
                        dp);
            if (allSortConfig != null)
                allSortConfig.AllConfig =
                    connection.Query<AllSortConfig>(
                        "SELECT *  FROM[Configuration].[dbo].[AllSortConfig] where IsAbled='1'  ORDER BY Priority ASC ");
            return allSortConfig;
        }

        /// <summary>
        ///     获取选中的全品类分类标签详情
        /// </summary>
        public AllSortTagConfig GetTagEntity(SqlConnection connection, int id, string title)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            var allSortTagConfig = connection
                .Query<AllSortTagConfig>(
                    "SELECT * FROM Configuration.dbo.AllSortTagConfig  (NOLOCK) WHERE PKID=@PKID and IsAbled='1'",
                    dp)
                .FirstOrDefault();
            if (allSortTagConfig != null)
                allSortTagConfig.ParentMaintitle = title;
            return allSortTagConfig;
        }

        /// <summary>
        ///     获取选中的全品类内容详情
        /// </summary>
        public AllSortListConfig GetListEntity(SqlConnection connection, int id, string title)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            var allSortListConfig = connection
                .Query<AllSortListConfig>(
                    "SELECT * FROM Configuration.dbo.AllSortListConfig  (NOLOCK) WHERE PKID=@PKID and IsAbled='1' ORDER BY Priority ASC",
                    dp)
                .FirstOrDefault();
            if (allSortListConfig != null)
                allSortListConfig.ParentMaintitle = title;
            return allSortListConfig;
        }

        /// <summary>
        ///     新建全品类分类
        /// </summary>
        public bool AddConfig(SqlConnection connection, AllSortConfig model)
        {
            var sql = @"INSERT INTO Configuration.dbo.AllSortConfig
        ( CreateDateTime ,
          UpdateDateTime ,
          Maintitle ,
          StartVersion ,
          EndVersion ,
          Priority ,
          State ,
          MaintitleColor ,
          [Statistics] ,
          StartTime ,
          EndTime
         )
VALUES  ( GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @Maintitle, -- Maintitle - varchar(100)
          @StartVersion , -- StartVersion - varchar(50)
          @EndVersion , -- EndVersion - varchar(50)
          @Priority , -- Priority - int
          @State , -- State - bit
          @MaintitleColor , -- MaintitleColor - varchar(50)
          @Statistics , -- [Statistics] - varchar(100)
          @StartTime , -- StartTime - datetime
          @EndTime  -- EndTime - datetime
         )";
            return connection.Execute(sql, model) > 0;
        }

        /// <summary>
        ///     新建全品类标签
        /// </summary>
        public bool AddTagConfig(SqlConnection connection, AllSortTagConfig model)
        {
            var sql = @"INSERT INTO Configuration.dbo.AllSortTagConfig
        ( CreateDateTime ,
          UpdateDateTime ,
          Maintitle,
          Subtitle , 
          SubtitleColor , 
          ChannelJumpUrl ,
          XiaoChannelJumpUrl,
          ChannelState ,
          [Statistics] , 
          State , 
          ParentId ,
          ChannelDescription 
         )
VALUES  ( GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @Maintitle, -- Maintitle - varchar(100)
          @Subtitle , -- Subtitle - varchar(100)
          @SubtitleColor , -- SubtitleColor - varchar(50)
          @ChannelJumpUrl , -- ChannelJumpUrl - varchar(1000)
          @XiaoChannelJumpUrl , -- XiaoChannelJumpUrl - varchar(1000)
          @ChannelState , -- ChannelState - int
          @Statistics , -- [Statistics] - varchar(50)
          @State , -- State - int
          @ParentId , -- ParentId - int
          @ChannelDescription  -- ChannelDescription - varchar(1000)
         )";
            return connection.Execute(sql, model) > 0;
        }

        /// <summary>
        ///     新建全品类内容
        /// </summary>
        public bool AddListConfig(SqlConnection connection, AllSortListConfig model)
        {
            var sql = @"INSERT INTO Configuration.dbo.AllSortListConfig
        ( CreateDateTime ,
          UpdateDateTime ,
          Title,
          State ,
          TitleColor ,
          TitleBgColor , 
          StartVersion , 
          EndVersion , 
          ButtonImage ,
          BannerImage ,
          JumpUrl , 
          XiaoJumpUrl,
          Priority, 
          [Statistics],
          StartTime ,
          EndTime,  
          ParentId  

         )
VALUES  ( GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @Title, -- Title - varchar(100)
          @State , -- State - int
          @TitleColor , -- TitleColor - varchar(50)
          @TitleBgColor , -- TitleBgColor - varchar(50)
          @StartVersion , -- StartVersion - varchar(50)
          @EndVersion , -- EndVersion - varchar(50)
          @ButtonImage , -- ButtonImage - varchar(1000)
          @BannerImage , -- BannerImage - varchar(1000)
          @JumpUrl , -- JumpUrl - varchar(1000)
          @XiaoJumpUrl , -- XiaoJumpUrl - varchar(1000)
          @Priority, -- Priority - int
          @Statistics , -- [Statistics] - varchar(100)
          @StartTime , -- StartTime - datetime
          @EndTime,  -- EndTime - datetime
          @ParentId  -- ParentId - int


         )";
            return connection.Execute(sql, model) > 0;
        }

        /// <summary>
        ///     修改全品类分类
        /// </summary>
        public bool UpdateEntity(SqlConnection connection, AllSortConfig model)
        {
            var sql = @"UPDATE Configuration.dbo.AllSortConfig SET 
          
          Maintitle=@Maintitle ,
          StartVersion=@StartVersion ,
          EndVersion=@EndVersion ,
          Priority=@Priority ,
          State=@State ,
          MaintitleColor=@MaintitleColor ,
          [Statistics]=@Statistics ,
          StartTime=@StartTime ,
          EndTime=@EndTime,
          UpdateDateTime=GetDate()  

          WHERE PKID=@PKID";
            return connection.Execute(sql, model) > 0;
        }

        /// <summary>
        ///     修改全品类标签
        /// </summary>
        public bool UpdateTagEntity(SqlConnection connection, AllSortTagConfig model)
        {
            var sql = @"UPDATE Configuration.dbo.AllSortTagConfig SET 
          Maintitle=@Maintitle,
          Subtitle=@Subtitle , 
          SubtitleColor=@SubtitleColor , 
          ChannelJumpUrl=@ChannelJumpUrl  ,
          XiaoChannelJumpUrl=@XiaoChannelJumpUrl  ,
          ChannelState=@ChannelState  ,
         [Statistics]=@Statistics , 
          State=@State  , 
          ParentId=@ParentId  ,
          ChannelDescription=@ChannelDescription ,
          UpdateDateTime=GetDate()  
          WHERE PKID=@PKID";
            return connection.Execute(sql, model) > 0;
        }

        /// <summary>
        ///     修改全品类内容
        /// </summary>
        public bool UpdateListEntity(SqlConnection connection, AllSortListConfig model)
        {
            var sql = @"UPDATE Configuration.dbo.AllSortListConfig SET 
          Title=@Title,
          State=@State ,
          TitleColor=@TitleColor ,
          TitleBgColor=@TitleBgColor , 
          StartVersion=@StartVersion , 
          EndVersion=@EndVersion , 
          ButtonImage=@ButtonImage ,
          BannerImage=@BannerImage ,
          JumpUrl=@JumpUrl , 
          XiaoJumpUrl=@XiaoJumpUrl ,
          Priority=@Priority, 
          [Statistics]=@Statistics ,
          StartTime=@StartTime ,
          EndTime=@EndTime,  
          ParentId=@ParentId ,
          UpdateDateTime=GetDate()  WHERE PKID=@PKID";
            return connection.Execute(sql, model) > 0;
        }
    }
}