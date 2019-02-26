using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity.Router;

namespace Tuhu.Provisioning.DataAccess.DAO.Router
{
    public class DalRouter
    {
        /// <summary>
        ///     获取所有全品类分类
        /// </summary>
        public IEnumerable<RouterMainLink> GetMainList(SqlConnection connection,int linkKind)
        {
            var dp = new DynamicParameters();
            dp.Add("@LinkKind", linkKind);
            var sql = "SELECT PKID, Content,Discription FROM[Configuration].[dbo].[RouterMainLink] WHERE LinkKind = @LinkKind;";
            return connection.Query<RouterMainLink>(sql,dp);
        }

        /// <summary>
        ///     获取所有主链接和参数
        /// </summary>
        public RouterList GetListForApp(SqlConnection connection)
        {
            var list = new RouterList
            {
                RouterMainLinkList =
                    connection.Query<RouterMainLink>("select PKID,UpdateDateTime,Content,Discription,LinkKind from  [Configuration].[dbo].[RouterMainLink] where LinkKind=1;"),
                RouterParameterList =
                    connection.Query<RouterParameter>("select PKID,UpdateDateTime,Content,Discription,LinkKind,Kind from  [Configuration].[dbo].[RouterParameter] where LinkKind=1;")
            };
            return list;
        }

        /// <summary>
        ///     获取所有主链接和参数
        /// </summary>
        public RouterList GetListForWeixin(SqlConnection connection)
        {
            var list = new RouterList
            {
                RouterMainLinkList =
                    connection.Query<RouterMainLink>("select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from  [Configuration].[dbo].[RouterMainLink] where LinkKind=2;"),
                RouterParameterList =
                    connection.Query<RouterParameter>("select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind,Kind from  [Configuration].[dbo].[RouterParameter] where LinkKind=2;")
            };
            return list;
        }

        /// <summary>
        ///     获取所有主链接和参数
        /// </summary>
        public RouterList GetList(SqlConnection connection)
        {
            var list = new RouterList
            {
                RouterMainLinkList =
                    connection.Query<RouterMainLink>("select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from  [Configuration].[dbo].[RouterMainLink] order by LinkKind;"),
                RouterParameterList =
                    connection.Query<RouterParameter>("select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind,Kind from  [Configuration].[dbo].[RouterParameter] order by LinkKind;")
            };
            return list;
        }

        /// <summary>
        ///     根据i描述获取主链接
        /// </summary>
        public RouterMainLink GetMain(SqlConnection connection,String routerMainLinkDiscription,int linkKind)
        {
            var dp = new DynamicParameters();
            dp.Add("@Discription", routerMainLinkDiscription);
            dp.Add("@linkKind", linkKind);
            var sqlMainLink =
                "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterMainLink where RouterMainLink.Discription=@Discription and LinkKind=@linkKind;";
            return connection.Query<RouterMainLink>(sqlMainLink, dp).FirstOrDefault();
        }

        /// <summary>
        ///     根据id获取参数
        /// </summary>
        public RouterParameter GetParameter(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection
                .Query<RouterParameter>(
                    "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterParameter where RouterParameter.PKID=@PKID;", dp)
                .FirstOrDefault();
        }

        /// <summary>
        ///     获取所有参数
        /// </summary>
        public IEnumerable<RouterParameter> GetParameterList(SqlConnection connection,int mId)
        {
            var dp = new DynamicParameters();
            dp.Add("@MId", mId);
            RouterMainLink mainLink = connection
                .Query<RouterMainLink>(
                    "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterMainLink where RouterMainLink.PKID=@MId;", dp)
                .FirstOrDefault();
            if (mainLink != null)
            { dp.Add("@LinkKind", mainLink.LinkKind);
            return  connection
                .Query<RouterParameter>(
                    "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterParameter where LinkKind=@LinkKind;", dp);
            }
            return null;
        }

        /// <summary>
        ///     根据id获取主链接
        /// </summary>
        public RouterMainLink GetMainLink(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection
                .Query<RouterMainLink>(
                    "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterMainLink where RouterMainLink.PKID=@PKID;", dp)
                .FirstOrDefault();
        }

        /// <summary>
        ///     根据id获取关联的详情，以便编辑
        /// </summary>
        public RouterLink  GetLink(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();

            dp.Add("@PKID", id);
            var routerLink = connection
                .Query<RouterLink>(
                    "select PKID,UpdateDateTime,MainLinkId,ParameterId from [Configuration].[dbo].RouterLink where PKID=@PKID;", dp)
                .FirstOrDefault();

            if (routerLink != null)
            {
                dp.Add("@MId", routerLink.MainLinkId);
                routerLink.MainLink = connection
                    .Query<RouterMainLink>(
                        "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterMainLink where RouterMainLink.PKID=@MId;", dp)
                    .FirstOrDefault();
                dp.Add("@PId", routerLink.ParameterId);
                routerLink.Parameter = connection
                    .Query<RouterParameter>(
                        "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterParameter where RouterParameter.PKID=@PId;",
                        dp).FirstOrDefault();
                if (routerLink.MainLink != null)
                { dp.Add("@LinkKind", routerLink.MainLink.LinkKind);
                routerLink.RouterParameterList= connection
                    .Query<RouterParameter>(
                        "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterParameter where LinkKind=@LinkKind;", dp);
                }
                else
                    routerLink.RouterParameterList = connection
                        .Query<RouterParameter>(
                            "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterParameter;");
                return routerLink;
            }
            return null;
        }

        /// <summary>
        ///     根据id获取对应主链接关联的（多个）参数
        /// </summary>
        public IEnumerable<RouterLink> GetLinkList(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();

            dp.Add("@MainLinkId", id);
            var routerLinkList = connection
                .Query<RouterLink>(
                    "select PKID,UpdateDateTime,MainLinkId,ParameterId from [Configuration].[dbo].RouterLink where RouterLink.MainLinkId=@MainLinkId;", dp).ToList();
            if (routerLinkList.Count!=0)
            {
                //var routerLinks = routerLinkList as RouterLink[] ?? routerLinkList.ToArray();
                foreach (var item in routerLinkList)
                {
                    dp.Add("@MId", item.MainLinkId);
                    item.MainLink = connection
                        .Query<RouterMainLink>(
                            "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterMainLink where RouterMainLink.PKID=@MId;", dp)
                        .FirstOrDefault();
                    dp.Add("@PId", item.ParameterId);
                    item.Parameter = connection
                        .Query<RouterParameter>(
                            "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterParameter where RouterParameter.PKID=@PId;",
                            dp).FirstOrDefault();
                }
                //return routerLinkList;
            }
            else
            {
                RouterLink rl = new RouterLink();
                rl.MainLink = connection
                        .Query<RouterMainLink>(
                            "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterMainLink where RouterMainLink.PKID=@MainLinkId;", dp)
                        .FirstOrDefault();

                routerLinkList.Add(rl);
            }
            return routerLinkList;
        }

        /// <summary>
        ///     修改参数
        /// </summary>
        public bool DeleteParameter(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection.Execute(
                       "delete from [Configuration].[dbo].RouterParameter where RouterParameter.PKID=@PKID;", dp) > 0;
        }

        /// <summary>
        ///     删除主链接
        /// </summary>
        public bool DeleteMainLink(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection.Execute(
                       "delete from [Configuration].[dbo].RouterMainLink where RouterMainLink.PKID=@PKID;", dp) > 0;
        }

        /// <summary>
        ///     删除链接关联
        /// </summary>
        public bool DeleteLink(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return connection.Execute(
                       "delete from [Configuration].[dbo].RouterLink where PKID=@PKID;", dp) > 0;
        }

        /// <summary>
        ///     根据参数类型和描述获取 关联参数
        /// </summary>
        public RouterLink GetParameterList(SqlConnection connection, string routerMainLinkDiscription,int linkKind)
        {
            var routerLink = new RouterLink();

            var dp = new DynamicParameters();
            dp.Add("@Discription", routerMainLinkDiscription);
            dp.Add("@LinkKind", linkKind);
            var sqlMainLink =
                "select PKID,CreateDateTime,UpdateDateTime,Content,Discription,LinkKind from [Configuration].[dbo].RouterMainLink where RouterMainLink.Discription=@Discription and LinkKind=@linkKind;";
            routerLink.MainLink = connection.Query<RouterMainLink>(sqlMainLink, dp).FirstOrDefault();

            var sqlParameter =
                "select RouterParameter.PKID,RouterParameter.UpdateDateTime,RouterParameter.Content,RouterParameter.Discription,RouterParameter.Kind from [Configuration].[dbo].RouterParameter,[Configuration].[dbo].RouterMainLink,[Configuration].[dbo].RouterLink where RouterLink.MainLinkId=RouterMainLink.PKID and RouterLink.ParameterId=RouterParameter.PKID and RouterMainLink.Discription=@Discription and  RouterMainLink.LinkKind=@LinkKind;";
            routerLink.RouterParameterList = connection.Query<RouterParameter>(sqlParameter, dp);

            return routerLink;
        }


        /// <summary>
        ///     修改主链接
        /// </summary>
        public bool UpdateMainLink(SqlConnection connection, RouterMainLink model)
        {
            var sql = @"UPDATE Configuration.dbo.RouterMainLink SET 
          
          Content=@Content ,
          Discription=@Discription ,
          UpdateDateTime=GetDate() ,
          LinkKind=@LinkKind

          WHERE PKID=@PKID";
            return connection.Execute(sql, model) > 0;
        }


        /// <summary>
        ///     修改参数
        /// </summary>
        public bool UpdateParameter(SqlConnection connection, RouterParameter model)
        {
            var sql = @"UPDATE Configuration.dbo.RouterParameter SET 
          
          Content=@Content ,
          Discription=@Discription ,
          UpdateDateTime=GetDate(),
          Kind=@Kind,
          LinkKind=@LinkKind

          WHERE PKID=@PKID";
            return connection.Execute(sql, model) > 0;
        }


        /// <summary>
        ///     修改完整链接
        /// </summary>
        public bool UpdateLink(SqlConnection connection, RouterLink model)
        {
            var sql = @"UPDATE Configuration.dbo.RouterLink SET 
          
          MainLinkId=@MainLinkId ,
          ParameterId=@ParameterId ,
          UpdateDateTime=GetDate()  

          WHERE PKID=@PKID";
            return connection.Execute(sql, model) > 0;
        }


        /// <summary>
        ///     新建主链接
        /// </summary>
        public bool AddMainLink(SqlConnection connection, RouterMainLink model)
        {
            var sql = @"INSERT INTO Configuration.dbo.RouterMainLink
        ( CreateDateTime ,
          UpdateDateTime ,
          [Content],
          Discription,
          LinkKind
         )
VALUES  ( GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @Content, -- Content - varchar(1000)
          @Discription,  -- Discription - varchar(100)
          @LinkKind  -- LinkKind - int
         )";
            return connection.Execute(sql, model) > 0;
        }


        /// <summary>
        ///     新建参数
        /// </summary>
        public bool AddParameter(SqlConnection connection, RouterParameter model)
        {
            var sql = @"INSERT INTO Configuration.dbo.RouterParameter
        ( CreateDateTime ,
          UpdateDateTime ,
          [Content],
          Discription,
          Kind,
          LinkKind
         )
VALUES  ( GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @Content, -- Content - varchar(1000)
          @Discription,  -- Discription - varchar(100)
          @Kind,   -- Kind - int
          @LinkKind  -- LinkKind - int
         )";
            return connection.Execute(sql, model) > 0;
        }

        /// <summary>
        ///     新建完整链接
        /// </summary>
        public bool AddLink(SqlConnection connection, RouterLink model)
        {
            var sql = @"INSERT INTO Configuration.dbo.RouterLink
        ( CreateDateTime ,
          UpdateDateTime ,
          MainLinkId ,
          ParameterId 
         )
VALUES  ( GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @MainLinkId, -- MainLinkId - int
          @ParameterId  -- ParameterId - int
         )";
            try
            {
                return connection.Execute(sql, model) > 0;
            }
            catch (Exception)
            {
                return false;//记录重复时返回添加失败
            }
        }
    }
}