using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;
using Tuhu.Service.Tousu;

namespace Tuhu.Provisioning.Business.Tousu
{
    public  class TousuBusiness
    {
        public  TousuBusiness()
        {
            //  SetThirdLevel();
            tousuList = GetALLTousuModelList();
           // orderTypeList = OrderTypeList();
        }

        public static IEnumerable<TousuModel> tousuList = GetALLTousuModelList();





        public static IEnumerable<TousuModel> GetALLTousuModelList()
        {           
            List<TousuModel> tousumodelList = new List<TousuModel>();
            using (var client = new TousuTypeClient())
            {
                var result = client.SelectTousuType();               
                if (result != null && result.Result != null)
                {
                    tousumodelList = result.Result.Select(p => new TousuModel()
                    {
                        DicText = p.YeWuDisplayName?.Trim(),
                        DicType = p.DicType?.Trim(),
                        DicValue = p.DicValue?.Trim(),
                        AppDisplayName = p.AppDisplayName?.Trim(),
                        TypeLevel=p.TypeLevel
                    }).ToList();
                }
                tousumodelList.Add(new TousuModel()
                {
                    DicText="其他",
                    DicType="other",
                    DicValue="other/other",
                    AppDisplayName= "其他"
                });
            }
            if (tousumodelList != null && tousumodelList.Any())
            {
                var TopLevelTousuList = tousumodelList.Where(p => string.Equals(p.DicType, "tousuType", StringComparison.OrdinalIgnoreCase)).ToList();
                var topLevelDicValueList = TopLevelTousuList.Select(q => q.DicValue).ToList();
                var secondLevelTousuList = tousumodelList.Where(q => topLevelDicValueList.Contains(q.DicType)).ToList();
                var secondLevelDicValueList = secondLevelTousuList.Select(q => q.DicValue).ToList();
                var thirdLevelTousuList = tousumodelList.Where(q => secondLevelDicValueList.Contains(q.DicType)).ToList();
                var thirdLevelDicValueList = thirdLevelTousuList.Select(q => q.DicValue).ToList();
                var fourthLevelTousuList = tousumodelList.Where(q => thirdLevelDicValueList.Contains(q.DicType)).ToList();
                var fourthLevelDicValueList = fourthLevelTousuList.Select(q => q.DicValue).ToList();
                var allLevelDicTypeList = tousumodelList.Select(q => q.DicType).ToList();
                secondLevelTousuList.ForEach(p =>
                {
                    if (allLevelDicTypeList.Contains(p.DicValue))
                    {
                        p.HasThirdLevel = true;
                    }
                });
                thirdLevelTousuList.ForEach(p =>
                {
                    if (allLevelDicTypeList.Contains(p.DicValue))
                    {
                        p.HasFourthLevel = true;
                    }
                });
                fourthLevelTousuList.ForEach(p =>
                {
                    if (allLevelDicTypeList.Contains(p.DicValue))
                    {
                        p.HasFifthLevel = true;
                    }
                });
            }
            return tousumodelList;
        }
        public static IEnumerable<TousuModel> GetTopLevelTousuModelList()
        {
            IEnumerable<TousuModel> tousumodelList = null;
           
                    tousumodelList = tousuList.Where(p=> string.Equals(p.DicType, "tousuType", StringComparison.OrdinalIgnoreCase)).Select(p => new TousuModel()
                    {
                        DicText = p.DicText,
                        DicType = p.DicType,
                        DicValue = p.DicValue,
                        AppDisplayName = p.AppDisplayName,
                        HasFourthLevel = p.HasFourthLevel
                    });
         
            return tousumodelList;
        }

        public static IEnumerable<TousuModel> GetTouModelListByDicValue(string dicValue)
        {
            //SetThirdLevel();
            IEnumerable<TousuModel> tousumodelList = null;
          
                        tousumodelList = tousuList.Where(p => string.Equals(p.DicType, dicValue, StringComparison.OrdinalIgnoreCase)).Select(p => new TousuModel()
                        {
                            DicText = p.DicText,
                            DicType = p.DicType,
                            DicValue = p.DicValue,
                            AppDisplayName = p.AppDisplayName,
                            HasThirdLevel=p.HasThirdLevel,
                            HasFourthLevel = p.HasFourthLevel,
                            HasFifthLevel=p.HasFifthLevel
                        });
                 

            return tousumodelList;
        }

        public static bool SetAppDisplayName(TousuModel model,string operatorName)
        {
            using (var client = new TousuTypeClient())
            {
                var result = client.UpdateTousuTypeAppDisplayName(model.DicValue, model.AppDisplayName, operatorName);
                // Assert.IsNotNull(result.Result);
                return ((result != null) ? true : false);
               
            }
        }

        public static void SetThirdLevel()
        {
            var TopLevelTousuList = GetTopLevelTousuModelList().ToList();
            var topLevelDicValueList = TopLevelTousuList.Select(q => q.DicValue).ToList();
            var secondLevelTousuList = tousuList.Where(q => topLevelDicValueList.Contains(q.DicType)).ToList();
            var secondLevelDicValueList = secondLevelTousuList.Select(q => q.DicValue).ToList();
            var thirdLevelTousuList = tousuList.Where(q => secondLevelDicValueList.Contains(q.DicType)).ToList();
            var thirdLevelDicValueList = thirdLevelTousuList.Select(q => q.DicValue).ToList();
            var allLevelDicTypeList = tousuList.Select(q => q.DicType).ToList();
            thirdLevelTousuList.ForEach(p =>
            {
                if (allLevelDicTypeList.Contains(p.DicValue))
                {
                    p.HasFourthLevel = true;
                }
            });
            var hasFourthLevelDicTypeValueList = thirdLevelTousuList.Where(p => p.HasFourthLevel == true).Select(p => p.DicValue).ToList();
            tousuList.ForEach(p =>
            {
                if (hasFourthLevelDicTypeValueList.Contains(p.DicValue))
                {
                    p.HasFourthLevel = true;
                }
            }
                );
        }


        public static IEnumerable<TousuModel> SelectList(string dicValue,int showLevel, PagerModel pager,string orderType="")
        {
            List<TousuModel> temptousulist;
            pager.TotalItem = GetTousuItemCount(dicValue, showLevel);
            IEnumerable<TousuModel> tousulist;
            if (string.IsNullOrWhiteSpace(dicValue))
            {
                tousulist = tousuList;
            }
            else
            {
                tousulist = tousuList.Where(q => string.Equals(q.DicType, dicValue, StringComparison.OrdinalIgnoreCase));
            }
          
            if (string.IsNullOrWhiteSpace(orderType))
            {
                temptousulist= tousulist.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            }
            else
            {
               var ordertypetousuConfigList= DalTousuConfig.GetOrderTypeTousuConfig(orderType,dicValue);
                var checkedlasttousuNameList = ordertypetousuConfigList.Where(s => s.IsChecked == true).Select(p=>p.LastLevelTousu).ToList();
                var uncheckedtousuItemList = tousulist.Where(s => !checkedlasttousuNameList.Contains(s.DicValue)).ToList();
                uncheckedtousuItemList.ForEach(p => { p.IsChecked = false; p.IsNeedPhoto = false; p.CautionText = string.Empty;p.GroupName = string.Empty; });
                var checkedtousuItemList = tousulist.Where(s => checkedlasttousuNameList.Contains(s.DicValue)).ToList();
                foreach (var checkedtousuItem in checkedtousuItemList)
                {
                    var checkeditem = ordertypetousuConfigList.FirstOrDefault(q => string.Equals(q.LastLevelTousu, checkedtousuItem.DicValue, StringComparison.OrdinalIgnoreCase));
                    if (checkeditem != null)
                    {
                        checkedtousuItem.IsChecked = true;
                        checkedtousuItem.CautionText = checkeditem.CautionText;
                        checkedtousuItem.IsNeedPhoto = checkeditem.IsNeedPhoto;
                        checkedtousuItem.GroupName = checkeditem.GroupName;
                    }
                }
                //uncheckedtousuItemList.AddRange(checkedtousuItemList);
                checkedtousuItemList.AddRange(uncheckedtousuItemList);
                temptousulist= checkedtousuItemList.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            }
            switch (showLevel)
            {
                case 2:
                    temptousulist = temptousulist.Where(p => p.HasThirdLevel == false).ToList();
                    break;
                case 3:
                    temptousulist = temptousulist.Where(p => p.HasFourthLevel == false).ToList();
                    break;
                case 4:
                    temptousulist = temptousulist.Where(p => p.HasFifthLevel == false).ToList();
                    break;
               
            }
            return temptousulist;
        }

        public static int GetTousuItemCount(string dicValue,int showLevel)
        {
            if (string.IsNullOrWhiteSpace(dicValue))
            {
                return tousuList.Count();
            }
            var tousulist = tousuList.Where(q => string.Equals(q.DicType, dicValue, StringComparison.OrdinalIgnoreCase)).ToList();
            switch (showLevel)
            {
                case 2:
                    tousulist = tousulist.Where(p => p.HasThirdLevel == false).ToList();
                    break;
                case 3:
                    tousulist = tousulist.Where(p => p.HasFourthLevel == false).ToList();
                    break;
                case 4:
                    tousulist = tousulist.Where(p => p.HasFifthLevel == false).ToList();
                    break;
                
            }
            return tousulist != null ? tousulist.Count() : 0;
        }

        public static IEnumerable<string> GetOrderTypeList()
        {
            List<string> orderTypeList = new List<string>();
            using (var client = new ConfigClient())
            {
                var dicKeys = new List<string> { "OrderType" };
                var result = client.GetDictionariesByType(dicKeys);
                if (result != null && result.Result != null)
                {
                    var dictList = result.Result.Select(s => s.Value);
                    foreach (var dict in dictList)
                    {
                        foreach (var item in dict)
                        {
                            if (!orderTypeList.Contains(item.Key))
                            {
                                orderTypeList.Add(item.Key);
                            }
                          
                        }
                    }
                }
            }
            return orderTypeList;
        }

        public static bool UpdateOrInsertOrderTypeTousuConfig(OrderTypeTousuConfig model)
        {
            return DalTousuConfig.UpdateOrInsertOrderTypeTousuConfig(model);
        }
    }
}
