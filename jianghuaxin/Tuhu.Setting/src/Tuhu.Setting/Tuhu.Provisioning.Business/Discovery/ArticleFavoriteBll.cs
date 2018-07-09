using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.Discovery;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.Business.Discovery
{
    public class ArticleFavoriteBll:BaseBll
    {
        private static readonly ArticleFavoriteDal articleFavoriteDal = new ArticleFavoriteDal();
        private static readonly ArticleDal articleDal = new ArticleDal();
        /// <summary>
        /// 新增或更新文章收藏记录
        /// </summary>
        /// <param name="articleFavorite"></param>
        /// <returns></returns>
        public static async Task<int> SaveArticleFavorite(ArticleFavorite articleFavorite)
        {
            var article =await articleDal.SelectArticleDetailById(articleFavorite.PKID);
            if (article == null)
                throw new Exception("文章不存在");
            return await articleFavoriteDal.AddOrUpdateArticleFavorite(articleFavorite);
        }
    }
}
