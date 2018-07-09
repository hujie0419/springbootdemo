using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    public class ArticleFavoriteDal : BaseDal
    {
        private static readonly DiscoveryDbContext dbContext = new DiscoveryDbContext();
        #region 收藏或取消收藏文章

        /// <summary>
        /// 收藏或取消收藏文章
        /// </summary>
        /// <param name="articleFavorite">文章收藏模型</param>
        /// <returns></returns>
        public async Task<int> AddOrUpdateArticleFavorite(ArticleFavorite articleFavorite)
        {
            var oldArticleFavorite =await dbContext.ArticleFavorite.SingleOrDefaultAsync(af => af.PKID == articleFavorite.PKID && af.UserId == articleFavorite.UserId);
            try
            {
                //取消收藏文章
                if (oldArticleFavorite != null)
                {
                    dbContext.ArticleFavorite.Remove(oldArticleFavorite);
                    await dbContext.SaveChangesAsync();
                    return Success;
                }
                //收藏文章
                else
                {
                    dbContext.ArticleFavorite.Add(articleFavorite);
                    await dbContext.SaveChangesAsync();
                    return Success;
                }
            }
            catch (Exception ex)
            {

                //记录日志

                return Error;
            }
        }
        /// <summary>
        /// 是否被收藏
        /// </summary>
        /// <param name="articleFavorite"></param>
        /// <returns></returns>
        public async Task<bool> HasFavorite(UserOperation articleFavorite)
        {
            var oldArticleFavorite = await dbContext.ArticleFavorite.SingleOrDefaultAsync(af => af.PKID == articleFavorite.ArticleId && af.UserId == articleFavorite.UserId.ToString("B"));
            try
            {
                //未收藏文章
                if (oldArticleFavorite == null)
                {
                    return false;
                }
                //已收藏文章
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
