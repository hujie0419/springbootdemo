using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class CategoryBrandRankManager
    {
        public IEnumerable<CategoryBrandRankModel> SelectCategoryBrand(long pkid, DateTime? date)
        {
            using (var db = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                return DalCategoryBrandRank.SelectCategoryBrand(pkid, date,db);
            }
        }
        public CategoryBrandRankModel FetchCategoryBrand(long pkid)
        {
            using (var db = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                return DalCategoryBrandRank.FetchCategoryBrand(pkid,db);
            }
        }

        public int UpdateCategoryBrand(CategoryBrandRankModel model)
        {
            using (var db = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                return DalCategoryBrandRank.UpdateCategoryBrand(model,db);
            }
        }

        public int DeleteCategoryBrand(long id)
        {
            using (var db = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                var children = DalCategoryBrandRank.SelectCategoryBrand(id, null, db);
                foreach (var c in children)
                {
                    DalCategoryBrandRank.DeleteCategoryBrand(c.PKID, db);
                }
                return DalCategoryBrandRank.DeleteCategoryBrand(id, db);
            }
        }

        public long InsertCategoryBrand(CategoryBrandRankModel model)
        {
            using (var db = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                return DalCategoryBrandRank.InsertCategoryBrand(model, db);
            }
        }

        public int UpdateCategoryBrands(IEnumerable<CategoryBrandRankModel> models)
        {
            using (var db = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var model in models)
                    {
                        if (model.PKID > 0)
                        {
                            DalCategoryBrandRank.UpdateCategoryBrand(model, db);
                        }
                        else
                        {
                            DalCategoryBrandRank.InsertCategoryBrand(model, db);
                        }
                    }
                    db.Commit();
                    return models.Count();
                }
                catch (Exception e)
                {
                    db.Rollback();
                    return 0;
                }
                
            }
        }
    }
}