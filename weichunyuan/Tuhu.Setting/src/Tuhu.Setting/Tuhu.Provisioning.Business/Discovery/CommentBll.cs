using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.Crm;
using Tuhu.Provisioning.DataAccess.DAO.Discovery;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.Business.Discovery
{
    public class CommentBll: ConnectionBase
    {
        private static CommentDal commentDal = new CommentDal();

        public static async Task<List<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment>> GetCommentListBy<TKey>(Expression<Func<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment, bool>> whereLambda, Expression<Func<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment, TKey>> orderLambda, PagerModel pager)
        {
            return await commentDal.GetCommentListBy(whereLambda, orderLambda, pager);
        }

        public static async Task<List<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment>> GetCommentListSearch<TKey>(Expression<Func<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment, bool>> whereLambda, Expression<Func<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment, TKey>> orderLambda, PagerModel pager, string articleTitle)
        {
            return await commentDal.GetCommentListSearch(whereLambda, orderLambda, pager, articleTitle);
        }

        public static async Task<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment> GetCommentById(int cid)
        {
            return await commentDal.GetCommentById(cid);
        }

        public static async Task<int> Modify(Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment model, params string[] proNames)
        {
            return await commentDal.Modify(model, proNames);
        }

        public CommentUser GetCommentUser(string UserID)
        {
            var row = commentDal.GetCommentUser(SqlConnectionOpen, UserID);

            if (row == null)
                return new CommentUser();
            else
            {
                string uName = !string.IsNullOrEmpty(row["u_Pref5"].ToString()) ? row["u_Pref5"].ToString() : row["u_last_name"].ToString();
                if (string.IsNullOrEmpty(uName))
                {
                    string phone = row["u_mobile_number"].ToString();
                    uName= phone.Replace(phone.Substring(3, 4), "****");
                }
                return new CommentUser()
                {
                    UserID = row["UserID"].ToString(),
                    UserName = uName,
                    UserHead = !string.IsNullOrEmpty(row["u_Imagefile"].ToString()) ? System.Configuration.ConfigurationManager.AppSettings["DoMain_image"] + row["u_Imagefile"].ToString() : "http://resource.tuhu.cn/Image/Product/zhilaohu.png",
                    UserPhone = row["u_mobile_number"].ToString()
                };
            }
               
        }

        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static async Task<Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment> AddComment(Tuhu.Provisioning.DataAccess.Entity.Discovery.Comment comment)
        {
            return await commentDal.AddComment(comment);
        }
    }
}
