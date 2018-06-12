using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;
using Tuhu.Provisioning.DataAccess;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    public class CategoryDal : BaseDal
    {
        private static readonly DiscoveryDbContext discoveryDbContext = new DiscoveryDbContext();
        #region Insert
        /// <summary>
        /// 新增Category
        /// </summary>
        /// <param name="Category"></param>
        /// <returns></returns>
        public async Task<Category> AddCategory(Category Category)
        {
            //if (Category.ParentId > 0)
            //{
            //    var parentCategory = await discoveryDbContext.Category.SingleOrDefaultAsync(t => t.Id == Category.ParentId);
            //    //var parentCategory = discoveryDbContext.Category.SingleOrDefault(t => t.Id == Category.ParentId);
            //    //parentCategory.ChildrenCategory.Add(Category);
            //    await discoveryDbContext.SaveChangesAsync();
            //    //discoveryDbContext.SaveChanges();
            //}
            //else
            //{
            await DbManager.InsertAsync<Category>(Category);
            //}
            //DbManager.Insert<Category>(Category);
            return Category;
        }

        #endregion

        #region Update
        /// <summary>
        /// 修改Category标签
        /// </summary>
        /// <param name="Category">要更新的标签对象</param>
        /// <returns></returns>
        public async Task<Category> UpdateCategory(Category Category)
        {
            var currentCategory = await discoveryDbContext.Category.FirstOrDefaultAsync(t => t.Id == Category.Id);
            //var currentCategory = discoveryDbContext.Category.SingleOrDefault(t => t.Id == Category.Id);
            if (currentCategory == null)
                return null;

            currentCategory.Name = Category.Name;
            currentCategory.Image = Category.Image;
            currentCategory.Describe = Category.Describe;
            //currentCategory.ParentId = Category.ParentId;

            await discoveryDbContext.SaveChangesAsync();

            return Category;
        }


        /// <summary>
        /// 修改标签状态
        /// </summary>
        /// <param name="CategoryId">标签ID</param>
        /// <param name="isDisable">是否禁用</param>
        public async Task<int> UpdateCategoryStatus(int CategoryId, bool isDisable)
        {
            var currentCategory = await discoveryDbContext.Category.FirstOrDefaultAsync<Category>(t => t.Id == CategoryId);
            if (currentCategory == null)
                throw new Exception("该标签不存在");
            currentCategory.Disable = isDisable;
            await discoveryDbContext.SaveChangesAsync();
            return Success;
        }


        /// <summary>
        /// 删除文章标签关系
        /// </summary>
        public async Task<int> DeleteArticleCategory(params ArticleCategory[] articleCategorys)
        {
            discoveryDbContext.ArticleCategory.RemoveRange(articleCategorys);
            await discoveryDbContext.SaveChangesAsync();
            return Success;
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据标签Id查询标签详情
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        /// <returns></returns>
        public async Task<Category> GetCategoryById(int CategoryId)
        {
            var Category = await DbManager.ReadOnly.SingleQueryAsync<Category>(a => a.Id == CategoryId);
            return Category;
        }

        /// <summary>
        /// 查询所有标签
        /// </summary>
        /// <param name="isLoadChildren">是否加载子标签</param>
        /// <returns></returns>
        public async Task<List<Category>> GetAllCategory(bool? isDisable, bool isLoadChildren = false)
        {
            if (isLoadChildren)
            {
                //查询父级标签
                var Categorys = await discoveryDbContext.Category.Where(t => t.ParentId == 0 && (isDisable.HasValue == false || t.Disable == isDisable.Value)).ToListAsync();
                Categorys = Categorys.Select(t => new Category
                {
                    Id = t.Id,
                    Name = t.Name,
                    Image = t.Image,
                    Disable = t.Disable,
                    Describe = t.Describe,
                    ChildrenCategory = discoveryDbContext.Category.Where(ct => ct.ParentId == t.Id && (isDisable.HasValue == false || ct.Disable == isDisable.Value)).ToList()
                }).ToList();
                return Categorys;
            }
            else
            {
                //查询父级标签
                var Categorys = await DbManager.QueryAsync<Category>(t => t.ParentId == 0 && (isDisable.HasValue == false || t.Disable == isDisable.Value));
                return Categorys;
            }
        }

        /// <summary>
        /// 根据标签Id查询标签的子标签
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        /// <returns></returns>
        public async Task<List<Category>> GetChildrenCategoryByCategoryId(int CategoryId)
        {
            var childrenCategory = await discoveryDbContext.Category.Where(t => t.ParentId == CategoryId).ToListAsync();
            return childrenCategory;
        }
        #endregion

        #region  优选标签
        /// <summary>
        /// 获取优选所有标签
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<Category> GetALLYouXuanCategoryList(SqlConnection conn, int categoryId = 0,int parentId=0)
        {
            const string sql = @"
            SELECT  PKID AS ID ,
                    Name ,
                    Describe ,
                    Image ,
                    IsDisable AS Disable ,
                    ParentId
            FROM    Marketing..YXCategoryTag WITH ( NOLOCK )
            WHERE   ( @PKID = 0
                      OR PKID = @PKID
                    )
                    AND ( @ParentId = 0
                          OR ParentId = @ParentId
                        )";
            using (conn)
            {
                return conn.Query<Category>(sql, new
                {
                    PKID = categoryId,
                    ParentId = parentId
                }, commandType: CommandType.Text).ToList();
            };
        }
    

        /// <summary>
        /// 更新优选标签状态
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="categoryId"></param>
        /// <param name="isDisable"></param>
        /// <returns></returns>
        public int UpdateYouXuanCategoryStatus(SqlConnection conn, int categoryId, bool isDisable)
        {
            const string sql = @"
              UPDATE    Marketing..YXCategoryTag
              SET       IsDisable = @IsDisable ,
                        UpdateTime = GETDATE()
              WHERE     PKID = @PKID";
            using (conn)
            {
                return conn.Execute(sql, new
                {
                    PKID = categoryId,
                    IsDisable = isDisable
                }, commandType: CommandType.Text);
            };
        }

        /// <summary>
        /// 更新优选标签信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public int UpdateYouXuanCategory(SqlConnection conn, Category category)
        {
            const string sql = @"
              UPDATE    Marketing..YXCategoryTag
              SET       Name = @Name ,
                        Describe = @Describe ,
                        Image = @Image ,
                        UpdateTime = GETDATE()
              WHERE     PKID = @PKID";
            using (conn)
            {
                return conn.Execute(sql, new
                {
                    Name = category.Name,
                    Describe = category.Describe,
                    Image = category.Image,
                    PKID = category.Id
                }, commandType: CommandType.Text);
            };
        }

        /// <summary>
        /// 添加优选标签信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public int AddYouXuanCategory(SqlConnection conn, Category category)
        {
            const string sql = @"   
            INSERT  INTO Marketing..YXCategoryTag
                    ( ParentId ,
                      Name ,
                      Describe ,
                      IsDisable ,
                      Image ,
                      CreateTime ,
                      UpdateTime
	                )
            VALUES  ( @ParentId ,
                      @Name ,
                      @Describe ,
                      @IsDisable ,
                      @Image ,
                      GETDATE() ,
                      GETDATE()
                    )";
            using (conn)
            {
                return conn.Execute(sql, new
                {
                    ParentId = category.ParentId,
                    Name = category.Name,
                    Describe = category.Describe,
                    Image = category.Image,
                    IsDisable = false
                }, commandType: CommandType.Text);
            };
        }
        #endregion
    }
}
