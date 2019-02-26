using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    public class CommentDal : BaseDal
    {
        private DiscoveryDbContext discoveryDbContext = new DBContextFactory().GetDbContext() as DiscoveryDbContext;

        public async Task<List<Comment>> GetCommentListBy<TKey>(Expression<Func<Comment, bool>> whereLambda, Expression<Func<Comment, TKey>> orderLambda, PagerModel pager)
        {
            var result = discoveryDbContext.Comment.Where(whereLambda).OrderByDescending(orderLambda);

            pager.TotalItem = result.Count();

            var comments = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize);

            await comments.ForEachAsync(u =>
            {
                u.CommentArticle = discoveryDbContext.Article.FirstOrDefault(s => s.PKID == u.PKID);

            });
            return await comments.ToListAsync();
        }

        public async Task<List<Comment>> GetCommentListSearch<TKey>(Expression<Func<Comment, bool>> whereLambda, Expression<Func<Comment, TKey>> orderLambda, PagerModel pager,string articleTitle)
        {
            var result = discoveryDbContext.Comment.Where(whereLambda).OrderByDescending(orderLambda);

            await result.ForEachAsync(u =>
            {
                u.CommentArticle = discoveryDbContext.Article.FirstOrDefault(s => s.PKID == u.PKID);
            });
            var query=result.ToList<Comment>().Where(s=>s.CommentArticle.SmallTitle.Contains(articleTitle));
            //result = result.Where(s => s.ArticleTitle!="").OrderByDescending(orderLambda);

            pager.TotalItem = query.Count();

            var comments = query.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
 
            return comments;
        }


        public async Task<Comment> GetCommentById(int cid)
        {
            return await discoveryDbContext.Comment.FirstOrDefaultAsync(s => s.Id == cid);
        }

        public async Task<int> Modify(Comment model, params string[] proNames)
        {

            discoveryDbContext.Set<Comment>().Attach(model);
            //将对象添加到EF中
            DbEntityEntry entry = discoveryDbContext.Entry<Comment>(model);

            //先设置对象的包装 状态为Unchanged
            entry.State = EntityState.Unchanged;
            //将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
            foreach (string proName in proNames)
            {
                //将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                entry.Property(proName).IsModified = true;
            }
            return await discoveryDbContext.SaveChangesAsync();
        }

        public DataRow GetCommentUser(SqlConnection connectionStr,string userId)
        {
            string strSql = @"
                                    SELECT TOP 1 UserID,
                                    u_Pref5,
                                    u_last_name,
                                    u_mobile_number,
                                    u_Imagefile 
                                    FROM Tuhu_profiles.dbo.UserObject(NOLOCK)   
                                    WHERE UserID=@UserID ";

            var sqlParams = new[] {
                new SqlParameter("@UserID",userId)
            };

            return SqlHelper.ExecuteDataRow(connectionStr, CommandType.Text, strSql, sqlParams);
        }

        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<Comment> AddComment(Comment comment)
        {
            using (var addDBContext = new DiscoveryDbContext())
            {
                if (comment.ParentID>0)
                {
                    Comment reply = await addDBContext.Comment.FirstOrDefaultAsync(m => m.Id == comment.ParentID);
                    if (reply == null)
                        return comment;
                    comment.ReplyComment = reply;
                    comment.ParentID = reply.Id;
                    //await discoveryDbContext.SaveChangesAsync();
                }
                addDBContext.Comment.Add(comment);
                await addDBContext.SaveChangesAsync();
                return comment;
            }
        }
    }
}
