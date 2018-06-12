using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class SE_MDBeautyCategoryProductConfigDAL
    {
        public static IEnumerable<SE_MDBeautyCategoryProductConfigModel> Select(SqlConnection connection, int pageIndex = 1, int pageSize = 20, string categoryIds = "")
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT *
                               ,(SELECT ParentsName FROM SE_MDBeautyCategoryConfigForParents(tab1.CategoryIds)) AS ParentsName FROM (
                               SELECT ROW_NUMBER() OVER(ORDER BY PId desc) AS 'RowNumber'
                               ,'TotalCount' = (SELECT COUNT(1) FROM SE_MDBeautyCategoryProductConfig WITH(NOLOCK) {0})
                               ,* FROM SE_MDBeautyCategoryProductConfig WITH(NOLOCK) {0}
                               ) AS tab1 
                               WHERE tab1.rowNumber between ((@pageIndex - 1)* @pageSize) + 1 AND @pageIndex * @pageSize ";

                if (!string.IsNullOrWhiteSpace(categoryIds))
                    sql = string.Format(sql, string.Format("Where CategoryIds in ({0})", categoryIds));
                else
                    sql = string.Format(sql, "", "");

                return conn.Query<SE_MDBeautyCategoryProductConfigModel>(sql, new { pageIndex = pageIndex, pageSize = pageSize });
            }
        }

        public static SE_MDBeautyCategoryProductConfigModel Select(SqlConnection connection, int id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM SE_MDBeautyCategoryProductConfig WITH(NOLOCK) WHERE PId = @id ";
                return conn.Query<SE_MDBeautyCategoryProductConfigModel>(sql, new { id = id })?.FirstOrDefault();
            }
        }

        public static bool Insert(SqlConnection connection, SE_MDBeautyCategoryProductConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" 
                                INSERT INTO [SE_MDBeautyCategoryProductConfig]
                                           ([ProdcutName]
                                           ,[CategoryIds]
                                           ,[Describe]
                                           ,[BeginPrice]
                                           ,[EndPrice]
                                           ,[BeginPromotionPrice]
                                           ,[EndPromotionPrice]
                                           ,[Commission]
                                           ,[EveryDayNum]
                                           ,[Brands]
                                           ,[RecommendCar]
                                           ,[AdaptiveCar]
                                           ,[IsDisable]
                                           ,[IsNotShow]
                                           ,[CreateTime])
                                     VALUES
                                           (@ProdcutName
                                           ,@CategoryIds
                                           ,@DESCRIBE
                                           ,@BeginPrice
                                           ,@EndPrice
                                           ,@BeginPromotionPrice
                                           ,@EndPromotionPrice
                                           ,@Commission
                                           ,@EveryDayNum
                                           ,@Brands
                                           ,@RecommendCar
                                           ,@AdaptiveCar
                                           ,@IsDisable
                                           ,@IsNotShow
                                           ,@CreateTime)";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Update(SqlConnection connection, SE_MDBeautyCategoryProductConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" UPDATE [SE_MDBeautyCategoryProductConfig]
                               SET [ProdcutName] = @ProdcutName
                                  ,[CategoryIds] = @CategoryIds
                                  ,[Describe] = @Describe
                                  ,[Commission] = @Commission
                                  ,[BeginPrice] = @BeginPrice
                                  ,[EndPrice] = @EndPrice
                                  ,[BeginPromotionPrice] = @BeginPromotionPrice
                                  ,[EndPromotionPrice] = @EndPromotionPrice
                                  ,[EveryDayNum] = @EveryDayNum
                                  ,[Brands] = @Brands
                                  ,[RecommendCar] = @RecommendCar
                                  ,[AdaptiveCar] = @AdaptiveCar
                                  ,[IsDisable] = @IsDisable
                                  ,[IsNotShow]=@IsNotShow
                                  ,[CreateTime] = @CreateTime
                             WHERE PId = @PId ";
                return conn.Execute(sql, model) > 0;
            }
        }

        /// <summary>
        /// 批量生成&批量修改产品
        /// </summary>
        public static bool BatchInsertOrUpdate(SqlConnection connection, SE_MDBeautyCategoryProductConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                List<BatchTreeModel> dataTreeItems = JsonConvert.DeserializeObject<List<BatchTreeModel>>(model?.Brands);

                #region 检测产品
                var _AdaptiveCarCheckBox = model.AdaptiveCarCheckBox?.Split(',');
                List<string> sqlWhere = new List<string>();
                if (_AdaptiveCarCheckBox != null && _AdaptiveCarCheckBox.Any())
                {
                    if (dataTreeItems != null && dataTreeItems.Any())
                    {
                        foreach (var a in dataTreeItems)
                        {
                            if (a.Childs != null && a.Childs.Any())
                            {
                                foreach (var b in a.Childs)
                                {
                                    foreach (var c in _AdaptiveCarCheckBox?.ToList())
                                    {
                                        sqlWhere.Add(string.Format("{0}|{1}|{2}", model.CategoryIds, a.ParentId + "," + b.Id, c));
                                    }
                                }
                            }
                            else
                            {
                                foreach (var c in _AdaptiveCarCheckBox?.ToList())
                                {
                                    sqlWhere.Add(string.Format("{0}|{1}|{2}", model.CategoryIds, a.ParentId, c));
                                }
                            }
                        }
                    }
                }
                string sql = @"SELECT * FROM 
                               (
	                                SELECT ISNULL(CategoryIds,'') + '|' + ISNULL(Brands,'') + '|'+ ISNULL(CONVERT(nvarchar,AdaptiveCar),'') as 'TreeItems',* FROM SE_MDBeautyCategoryProductConfig WITH(NOLOCK)
                               ) AS tab1
                               WHERE tab1.TreeItems IN(" + "'" + string.Join("','", sqlWhere) + "'" + ")";

                IEnumerable<SE_MDBeautyCategoryProductConfigModel> dataList = conn.Query<SE_MDBeautyCategoryProductConfigModel>(sql);
                #endregion

                #region 批量插入或更新

                string sqlInsert = @" INSERT [SE_MDBeautyCategoryProductConfig] ([ProdcutName],[CategoryIds],[Describe],[Commission],[BeginPrice],[EndPrice],[BeginPromotionPrice],[EndPromotionPrice],[EveryDayNum],[Brands],[RecommendCar],[AdaptiveCar],[IsDisable],[CreateTime]) VALUES ";
                string sqlUpdate = @" UPDATE [SE_MDBeautyCategoryProductConfig]
                                           SET [ProdcutName] = {1}
                                              ,[Describe] = {2}
                                              ,[Commission] = @Commission
                                              ,[BeginPrice] = @BeginPrice
                                              ,[EndPrice] = @EndPrice
                                              ,[BeginPromotionPrice] = @BeginPromotionPrice
                                              ,[EndPromotionPrice] = @EndPromotionPrice
                                              ,[EveryDayNum] = @EveryDayNum
                                              ,[IsDisable] = @IsDisable
                                          WHERE PId IN({0}) ";

                List<string> sqlInsertWhere = new List<string>();
                List<SE_MDBeautyCategoryProductConfigModel> sqlUpdateWhere = new List<SE_MDBeautyCategoryProductConfigModel>();

                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    #region 遍历品牌 
                    foreach (var item in dataTreeItems)
                    {
                        if (item.Childs != null && item.Childs.Any())  //判断是否遍历子系列
                        {
                            #region 遍历系列
                            foreach (var itemChilds in item.Childs)
                            {
                                #region 遍历车型
                                foreach (var itemCar in _AdaptiveCarCheckBox?.ToList())
                                {
                                    string itemCarName = (itemCar == "1" ? "五座轿车"
                                                        : itemCar == "2" ? "SUV/MPV"
                                                        : itemCar == "3" ? "SUV"
                                                        : "MPV");

                                    string _TreeItems = string.Format("{0}|{1}|{2}", model.CategoryIds, item.ParentId + "," + itemChilds.Id, itemCar);
                                    var _compareData = dataList?.Where(_ => _.TreeItems == _TreeItems)?.FirstOrDefault();
                                    if (_compareData == null)
                                    {
                                        sqlInsertWhere.Add(string.Format(" ( N'{0}',N'{1}',N'{2}',{3},{4},{5},{6},{7},{8},N'{9}',{10},{11},{12},N'{13}') \r\n",
                                            item.Name + itemChilds.Name + itemCarName + item.CategorysName,
                                            model.CategoryIds,
                                            model.Describe?.Replace("$1", item.CategorysName).Replace("$2", itemChilds.Name),
                                            model.Commission,
                                            model.BeginPrice,
                                            model.EndPrice,
                                            model.BeginPromotionPrice,
                                            model.EndPromotionPrice,
                                            model.EveryDayNum,
                                            item.ParentId + "," + itemChilds.Id,
                                            model.RecommendCar,
                                            itemCar,
                                            model.IsDisable.GetHashCode(),
                                            model.IsNotShow.GetHashCode(),
                                            model.CreateTime));
                                    }
                                    else
                                    {
                                        sqlUpdateWhere.Add(new SE_MDBeautyCategoryProductConfigModel()
                                        {
                                            PId = _compareData.PId,
                                            ProdcutName = item.Name + itemCarName + item.CategorysName,
                                            Describe = model.Describe?.Replace("$1", item.CategorysName).Replace("$2", itemChilds.Name),
                                        });
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region 遍历车型
                            foreach (var itemCar in _AdaptiveCarCheckBox?.ToList())
                            {
                                string itemCarName = (itemCar == "1" ? "五座轿车"
                                                    : itemCar == "2" ? "SUV/MPV"
                                                    : itemCar == "3" ? "SUV"
                                                    : "MPV");

                                string _TreeItems = string.Format("{0}|{1}|{2}", model.CategoryIds, item.ParentId, itemCar);
                                var _compareData = dataList?.Where(_ => _.TreeItems == _TreeItems)?.FirstOrDefault();
                                if (_compareData == null)
                                {
                                    sqlInsertWhere.Add(string.Format(" ( N'{0}',N'{1}',N'{2}',{3},{4},{5},{6},{7},{8},N'{9}',{10},{11},{12},N'{13}') \r\n",
                                    item.Name + itemCarName + item.CategorysName,
                                    model.CategoryIds,
                                    model.Describe?.Replace("$1", item.CategorysName).Replace("$2", ""),
                                    model.Commission,
                                    model.BeginPrice,
                                    model.EndPrice,
                                    model.BeginPromotionPrice,
                                    model.EndPromotionPrice,
                                    model.EveryDayNum,
                                    item.ParentId,
                                    model.RecommendCar,
                                    itemCar,
                                    model.IsDisable.GetHashCode(),
                                    model.IsNotShow.GetHashCode(),
                                    model.CreateTime));
                                }
                                else
                                {
                                    sqlUpdateWhere.Add(new SE_MDBeautyCategoryProductConfigModel()
                                    {
                                        PId = _compareData.PId,
                                        ProdcutName = item.Name + itemCarName + item.CategorysName,
                                        Describe = model.Describe?.Replace("$1", item.CategorysName).Replace("$2", ""),
                                    });
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region 组装SQL
                    if (sqlInsertWhere != null && sqlInsertWhere.Any())
                        conn.Execute(sqlInsert + string.Join(",", sqlInsertWhere), null, transaction);

                    if (sqlUpdateWhere != null && sqlUpdateWhere.Any())
                    {
                        string _ProdcutName = " CASE PId ", _Describe = " CASE PId ";

                        foreach (var item in sqlUpdateWhere)
                        {
                            _ProdcutName += string.Format("WHEN {0} THEN N'{1}'", item.PId, item.ProdcutName);
                            _Describe += string.Format("WHEN {0} THEN N'{1}'", item.PId, item.Describe);
                        }
                        _ProdcutName += " END ";
                        _Describe += " END ";

                        conn.Execute(string.Format(sqlUpdate, string.Join(",", sqlUpdateWhere.Select(s => s.PId)), _ProdcutName, _Describe),
                            new
                            {
                                @Commission = model.Commission,
                                @BeginPrice = model.BeginPrice,
                                @EndPrice = model.EndPrice,
                                @BeginPromotionPrice = model.BeginPromotionPrice,
                                @EndPromotionPrice = model.EndPromotionPrice,
                                @EveryDayNum = model.EveryDayNum,
                                @IsDisable = model.IsDisable,
                                @IsNotShow=model.IsNotShow,
                            }, transaction);
                    }
                    #endregion

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 批量插入或修改产品，并同步产品库
        /// </summary>
        public static bool BatchInsertOrUpdateSyncProdcutLibrary(SqlConnection connection, Dictionary<string, string> dicSQL, SE_MDBeautyCategoryProductConfigModel model)
        {
            if (dicSQL == null || !dicSQL.Any())
                return false;

            using (IDbConnection conn = connection)
            {
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    if (dicSQL.ContainsKey("INSERT"))
                    {
                        conn.Execute(dicSQL["INSERT"], null, transaction);
                    }

                    if (dicSQL.ContainsKey("UPDATE"))
                    {
                        conn.Execute(dicSQL["UPDATE"],
                         new
                         {
                             @Commission = model.Commission,
                             @BeginPrice = model.BeginPrice,
                             @EndPrice = model.EndPrice,
                             @BeginPromotionPrice = model.BeginPromotionPrice,
                             @EndPromotionPrice = model.EndPromotionPrice,
                             @EveryDayNum = model.EveryDayNum,
                             @IsDisable = model.IsDisable,
                             @IsNotShow=model.IsNotShow,
                         }, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return true;
        }

        /// <summary>
        /// 自定义SQL查询
        /// </summary>
        public static IEnumerable<T> CustomQuery<T>(SqlConnection connection, string sql)
        {
            using (IDbConnection conn = connection)
            {
                return conn.Query<T>(sql, null);
            }
        }

        private class BatchTreeModel
        {
            /// <summary>
            /// 类目名
            /// </summary>
            public string CategorysName {get;set;}
            /// <summary>
            /// 品牌ID
            /// </summary>
            public int? ParentId { get; set; }
            /// <summary>
            /// 品牌名
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 品牌子系列
            /// </summary>
            public IEnumerable<BatchTreeChildsModel> Childs { get; set; }
        }

        private class BatchTreeChildsModel {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
