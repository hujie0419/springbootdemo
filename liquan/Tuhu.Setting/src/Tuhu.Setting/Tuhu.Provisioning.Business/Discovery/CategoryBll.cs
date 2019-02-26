using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.Discovery;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.Business.Discovery
{
    public class CategoryBll : BaseBll
    {
        private static readonly CategoryDal CategoryDal = new CategoryDal();
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ArticleBll));
        /// <summary>
        /// 新增Category
        /// </summary>
        /// <param name="Category"></param>
        /// <returns></returns>
        public static async Task<Tuhu.Provisioning.DataAccess.Entity.Discovery.Category> AddCategory(Tuhu.Provisioning.DataAccess.Entity.Discovery.Category Category)
        {
            return await CategoryDal.AddCategory(Category);
        }



        /// <summary>
        /// 修改Category标签
        /// </summary>
        /// <param name="Category">要更新的标签对象</param>
        /// <returns></returns>
        public static async Task<Tuhu.Provisioning.DataAccess.Entity.Discovery.Category> UpdateCategory(Tuhu.Provisioning.DataAccess.Entity.Discovery.Category Category)
        {
            return await CategoryDal.UpdateCategory(Category);
        }

        /// <summary>
        /// 修改标签状态
        /// </summary>
        /// <param name="categoryId">标签ID</param>
        /// <param name="status">标签状态</param>
        public static async Task<int> UpdateCategoryStatus(int categoryId, bool status)
        {
            //删除标签
            return await CategoryDal.UpdateCategoryStatus(categoryId, status);
        }

        /// <summary>
        /// 根据标签Id查询标签详情
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        /// <returns></returns>
        public static async Task<Tuhu.Provisioning.DataAccess.Entity.Discovery.Category> GetCategoryDetailById(int CategoryId)
        {
            return await CategoryDal.GetCategoryById(CategoryId);
        }

        /// <summary>
        /// 查询所有标签
        /// </summary>
        /// <param name="isLoadChildren">是否加载子标签</param>
        /// <param name="status">过滤标签的状态</param>
        /// <returns></returns>
        public static async Task<List<Tuhu.Provisioning.DataAccess.Entity.Discovery.Category>> GetAllCategory(bool? isDisable, bool isLoadChildren = false)
        {
            return await CategoryDal.GetAllCategory(isDisable, isLoadChildren);
        }

        /// <summary>
        /// 根据标签Id查询标签的子标签
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        /// <returns></returns>
        public static async Task<List<Tuhu.Provisioning.DataAccess.Entity.Discovery.Category>> GetChildrenCategoryByCategoryId(int CategoryId)
        {
            return await CategoryDal.GetChildrenCategoryByCategoryId(CategoryId);
        }

        #region  优选标签

        public static List<DataAccess.Entity.Discovery.Category> GetYouXuanCategoryList()
        {
            List<DataAccess.Entity.Discovery.Category> result = new List<DataAccess.Entity.Discovery.Category>();
            try
            {
                result = CategoryDal.GetALLYouXuanCategoryList(ProcessConnection.OpenMarketingReadOnly);
                if (result != null && result.Any())
                {
                    result = result.Where(x => x.ParentId == 0).Select(t => new DataAccess.Entity.Discovery.Category
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Image = t.Image,
                        Disable = t.Disable,
                        Describe = t.Describe,
                        ChildrenCategory = result.Where(ct => ct.ParentId == t.Id).ToList()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static DataAccess.Entity.Discovery.Category GetYouXuanCategoryById(int categoryId)
        {
            DataAccess.Entity.Discovery.Category result = null;
            try
            {
                var data = CategoryDal.GetALLYouXuanCategoryList(ProcessConnection.OpenMarketingReadOnly, categoryId);
                if (data != null && data.Any())
                {
                    result = data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result ?? new DataAccess.Entity.Discovery.Category();
        }

        public static List<DataAccess.Entity.Discovery.Category> GetYouXuanChildCategoryById(int parentId)
        {
            List<DataAccess.Entity.Discovery.Category> result = new List<DataAccess.Entity.Discovery.Category>();
            try
            {
                result = CategoryDal.GetALLYouXuanCategoryList(ProcessConnection.OpenMarketingReadOnly, parentId: parentId);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static bool AddYouXuanCategory(DataAccess.Entity.Discovery.Category category)
        {
            var result = false;
            try
            {
                result = CategoryDal.AddYouXuanCategory(ProcessConnection.OpenMarketing, category) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }


        public static bool UpdateYouXuanCategory(DataAccess.Entity.Discovery.Category category)
        {
            var result = false;
            try
            {
                result = CategoryDal.UpdateYouXuanCategory(ProcessConnection.OpenMarketing, category) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static bool UpdateYouXuanCategoryStatus(int categoryId, bool isDisable)
        {
            var result = false;
            try
            {
                result = CategoryDal.UpdateYouXuanCategoryStatus(ProcessConnection.OpenMarketing, categoryId, isDisable) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        #endregion
    }
}
