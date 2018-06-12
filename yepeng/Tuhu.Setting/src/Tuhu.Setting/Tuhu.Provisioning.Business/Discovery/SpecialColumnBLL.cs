using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.Discovery;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.Business.Discovery
{
    public class SpecialColumnBLL
    {
        public static bool AddSpecialColumn(SpecialColumn model)
        {
            try
            {
                return SpecialColumnDAL.AddSpecialColumn(ProcessConnection.OpenMarketing, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateSpecialColumn(SpecialColumn model)
        {
            try
            {
                return SpecialColumnDAL.UpdateSpecialColumn(ProcessConnection.OpenMarketing, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateIsShow(int id, bool isShow)
        {
            try
            {
                return SpecialColumnDAL.UpdateIsShow(ProcessConnection.OpenMarketing, id, isShow);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<SpecialColumn> SelectColumnList(PagerModel pager, string strWhere)
        {
            try
            {
                DataTable dt = SpecialColumnDAL.SelectColumnList(ProcessConnection.OpenMarketing, pager, strWhere);
                if (dt.Rows.Count > 0)
                {
                    return DTConvertSpecialColumn(dt);
                }
                else
                    return new List<SpecialColumn>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SpecialColumn SelectSpecialColumnByID(int ID)
        {
            var row = SpecialColumnDAL.SelectSpecialColumnByID(ProcessConnection.OpenMarketing, ID);
            if (row == null)
                return new SpecialColumn();
            int id = row.Field<int>("ID");
            string columnName = row.Field<string>("ColumnName");
            string columnDesc = row.Field<string>("ColumnDesc");
            string columnImage = row.Field<string>("ColumnImage");
            int isTop = row.Field<int>("IsTop");
            bool isShow = row.Field<bool>("IsShow");
            DateTime createTime = row.Field<DateTime>("CreateTime");
            DateTime? publishTime = row.Field<DateTime?>("PublishTime");
            string creator = row.Field<string>("Creator");
            return new SpecialColumn
            {
                ID = id,
                ColumnName = columnName,
                ColumnDesc = columnDesc,
                ColumnImage = columnImage,
                IsTop = isTop,
                IsShow = isShow,
                CreateTime = createTime,
                Creator = creator,
                PublishTime=publishTime
            };
        }

        public static List<ColumnArticle> SelectArticleBySCID(int scid)
        {
            try
            {
                DataTable dt = SpecialColumnDAL.SelectColumnArticleBySql(ProcessConnection.OpenMarketing,string.Format("AND SCID={0}", scid.ToString()));
                if (dt.Rows.Count > 0)
                {
                    return DTConvertColumnArticle(dt);
                }
                else
                    return new List<ColumnArticle>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<SpecialColumn> DTConvertSpecialColumn(DataTable dt)
        {
            List<SpecialColumn> modelList = new List<SpecialColumn>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int i = 0; i < rowsCount; i++)
                {
                    #region MyRegion
                    DataRow row = dt.Rows[i];
                    int id = row.Field<int>("ID");
                    string columnName = row.Field<string>("ColumnName");
                    string columnDesc = row.Field<string>("ColumnDesc");
                    string columnImage = row.Field<string>("ColumnImage");
                    int isTop = row.Field<int>("IsTop");
                    bool isShow = row.Field<bool>("IsShow");
                    DateTime createTime = row.Field<DateTime>("CreateTime");
                    var publishTime = row.Field<DateTime?>("PublishTime");
                    string creator = row.Field<string>("Creator");
                    modelList.Add(new SpecialColumn()
                    {
                        ID = id,
                        ColumnName = columnName,
                        ColumnDesc = columnDesc,
                        ColumnImage = columnImage,
                        IsTop = isTop,
                        IsShow = isShow,
                        CreateTime = createTime,
                        Creator = creator,
                        PublishTime=publishTime
                    });
                    #endregion
                }
            }
            return modelList;
        }

        private static List<ColumnArticle> DTConvertColumnArticle(DataTable dt)
        {
            List<ColumnArticle> modelList = new List<ColumnArticle>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int i = 0; i < rowsCount; i++)
                {
                    #region MyRegion
                    DataRow row = dt.Rows[i];
                    int id = row.Field<int>("ID");
                    int pkid = row.Field<int>("PKID");
                    int scid = row.Field<int>("SCID");
                    DateTime createTime = row.Field<DateTime>("CreateTime");
                    modelList.Add(new ColumnArticle()
                    {
                        ID = id,
                        PKID = pkid,
                        SCID = scid,
                        CreateTime = createTime
                    });
                    #endregion
                }
            }
            return modelList;
        }

    }
}
