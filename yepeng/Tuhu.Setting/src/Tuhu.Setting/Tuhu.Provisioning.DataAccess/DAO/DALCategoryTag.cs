using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;
using Tuhu.Component.Common;
using Microsoft.ApplicationBlocks.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALCategoryTag
    {
        public static IEnumerable<CategoryTagModel> SelectById(SqlConnection sqlconnection, int? id, int? parentId)
        {
            if (id == null && parentId == null)
            {
                string sql = "SELECT * FROM [Marketing].[dbo].[HD_CategoryTag] WITH(NOLOCK) ORDER BY sort";
                return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, null).ConvertTo<CategoryTagModel>().ToList();
            }
            else if (id == null && parentId != null)
            {
                string sql = "SELECT * FROM [Marketing].[dbo].[HD_CategoryTag] WITH(NOLOCK) WHERE parentId = @parentId ORDER BY sort";
                return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@parentId", Value = parentId }).ConvertTo<CategoryTagModel>().ToList();
            }
            else
            {
                string sql = "SELECT * FROM [Marketing].[dbo].[HD_CategoryTag] WITH(NOLOCK) WHERE id = @id ORDER BY sort";
                return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@id", Value = id }).ConvertTo<CategoryTagModel>().ToList();
            }
        }

        public static IEnumerable<ZTreeModel> SelectByTree(SqlConnection sqlconnection)
        {
            string sql = @"SELECT id AS 'id',parentId AS 'pId',name AS 'name',0 AS 'open',disable AS 'chkDisabled' 
                           FROM [Marketing].[dbo].[HD_CategoryTag] WITH(NOLOCK) WHERE ISNULL(type,0) <> 1 ORDER BY sort";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, null).ConvertTo<ZTreeModel>().ToList();
        }

        public static bool Insert(SqlConnection sqlconnection, CategoryTagModel model)
        {
            const string sql = @"INSERT  INTO Marketing.dbo.HD_CategoryTag
                                    ( parentId ,
                                      isParent ,
                                      name ,
                                      code ,
                                      describe ,
                                      image ,
                                      disable ,
                                      sort ,
                                      type ,
                                      topicsId ,
                                      isHomePage ,
                                      createTime ,
                                      nickName ,
                                      showNum ,
                                      isShowList
                                    )
                            VALUES  ( @parentId ,
                                      @isParent ,
                                      @name ,
                                      @code ,
                                      @describe ,
                                      @image ,
                                      @disable ,
                                      @sort ,
                                      @type ,
                                      @topicsId ,
                                      @isHomePage ,
                                      @createTime ,
                                      @nickName ,
                                      @showNum ,
                                      @isShowList
                                    )";

            var sqlpara = new SqlParameter[]{
                    new SqlParameter("@parentId",model.parentId),
                    new SqlParameter("@isParent",model.isParent),
                    new SqlParameter("@name",model.name),
                    new SqlParameter("@code",model.code),
                    new SqlParameter("@describe",model.describe),
                    new SqlParameter("@image",model.image),
                    new SqlParameter("@disable",model.disable),
                    new SqlParameter("@sort",model.sort),
                    new SqlParameter("@type",model.type),
                    new SqlParameter("@topicsId",model.topicsId),
                    new SqlParameter("@isHomePage",model.isHomePage),
                    new SqlParameter("@createTime",model.createTime),
                    new SqlParameter("@nickName",model.nickName??string.Empty),
                    new SqlParameter("@showNum",model.showNum),
                    new SqlParameter("@isShowList",model.isShowList)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0 ? true : false;
        }

        public static bool UpdateById(SqlConnection sqlconnection, CategoryTagModel model)
        {
            string sql = @"update Marketing.dbo.HD_CategoryTag
                        set parentId=@parentId,
                        isParent=@isParent,
                        name=@name,
                        code=@code,
                        describe=@describe,
                        image=@image,
                        disable=@disable,
                        sort=@sort,
                        type=@type,
                        topicsId=@topicsId,
                        isHomePage=@isHomePage,
                        createTime=@createTime,
                        nickName=@nickName,
                        showNum=@showNum,
                        isShowList=@isShowList
                        WHERE id = @id";

            var sqlpara = new SqlParameter[]{
                    new SqlParameter("@parentId",model.parentId),
                    new SqlParameter("@isParent",model.isParent),
                    new SqlParameter("@name",model.name),
                    new SqlParameter("@code",model.code),
                    new SqlParameter("@describe",model.describe),
                    new SqlParameter("@image",model.image),
                    new SqlParameter("@disable",model.disable),
                    new SqlParameter("@sort",model.sort),
                    new SqlParameter("@type",model.type),
                    new SqlParameter("@topicsId",model.topicsId),
                    new SqlParameter("@isHomePage",model.isHomePage),
                    new SqlParameter("@createTime",model.createTime),
                    new SqlParameter("@id",model.id),
                    new SqlParameter("@nickName",model.nickName??string.Empty),
                    new SqlParameter("@showNum",model.showNum),
                    new SqlParameter("@isShowList",model.isShowList)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0 ? true : false;
        }

        public static IEnumerable<CategoryTagModel> SelectByPID(SqlConnection sqlcon, int parentId, int isParent, string orderBy)
        {
            string sql = " SELECT * FROM Marketing..HD_CategoryTag WITH(NOLOCK) WHERE parentId = @parentId AND isParent = @isParent ORDER BY ";

            if (!string.IsNullOrWhiteSpace(orderBy))
                sql += orderBy;

            var sqlparam = new SqlParameter[]{
                new SqlParameter("@parentId", parentId),
                new SqlParameter("@isParent", isParent)
            };

            return SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, sql, sqlparam).ConvertTo<CategoryTagModel>();
        }

        /// <summary>
        /// 检测顶级节点是否车型，且为可修改子级
        /// </summary>
        public static bool IsVehicle(SqlConnection sqlcon, int id)
        {
            string sql = @"
                WITH ParentAll AS
                (
	                SELECT * FROM Marketing..HD_CategoryTag WITH(NOLOCK) WHERE id  = @id
	                UNION ALL
	                SELECT a.* FROM Marketing..HD_CategoryTag AS a WITH(NOLOCK)
	                INNER JOIN
	                ParentAll AS b 
	                ON 
	                a.id = b.parentId
                )
                SELECT TOP 1 ParentAll.id + (SELECT COUNT(1) FROM ParentAll) AS [count] FROM ParentAll WHERE id = 2
            ";

            var sqlparam = new SqlParameter[]{
                new SqlParameter("@id", id)
            };

            return (int)SqlHelper.ExecuteScalar(sqlcon, CommandType.Text, sql, sqlparam) == 5;
        }
    }
}