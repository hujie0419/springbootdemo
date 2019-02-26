using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.WeiXinCard
{
    public class WeiXinCardManager
    {

        public static List<WeiXinColor> Colors = new List<WeiXinColor> {
                        new WeiXinColor()
                    {
                        ColorIndex = 1,
                        ColorValue = "Color010"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 2,
                        ColorValue = "Color020"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 3,
                        ColorValue = "Color030"
                    },
                            new WeiXinColor()
                    {
                        ColorIndex = 4,
                        ColorValue = "Color040"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 5,
                        ColorValue = "Color050"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 6,
                        ColorValue = "Color060"
                    },
                            new WeiXinColor()
                    {
                        ColorIndex = 7,
                        ColorValue = "Color070"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 8,
                        ColorValue = "Color080"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 9,
                        ColorValue = "Color081"
                    },
                             new WeiXinColor()
                    {
                        ColorIndex = 10,
                        ColorValue = "Color082"
                    },
                            new WeiXinColor()
                    {
                        ColorIndex = 11,
                        ColorValue = "Color090"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 12,
                        ColorValue = "Color100"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 13,
                        ColorValue = "Color101"
                    },
                             new WeiXinColor()
                    {
                        ColorIndex = 13,
                        ColorValue = "Color102"
                    }
                    };
        public static int SaveWeiXinCard(WeixinCardModel model)
        {
            return DALWeiXinCard.SaveWeiXinCard(model);
        }

        public static int UpdateWeiXinCardPushCount(string cardid, int count)
        {
            return DALWeiXinCard.UpdateWeiXinCardPushCount(cardid, count);
        }
        public static int UpdateWeiXinCard(WeixinCardModel model)
        {
            return DALWeiXinCard.UpdateWeiXinCard(model);
        }

        public static int SaveWeiXinCardSupplier(SupplierInfo model)
        {
            return DALWeiXinCard.SaveWeiXinCardSupplier(model);
        }

        public static int DeleteWeiXinCardSupplier(int pkid)
        {
            return DALWeiXinCard.DeleteWeiXinCardSupplier(pkid);
        }

        public static int DeleteWeiXinCard(int pkid)
        {
            return DALWeiXinCard.DeleteWeiXinCard(pkid);
        }
        public static int UpdateWeiXinCardSupplier(SupplierInfo model)
        {
            return DALWeiXinCard.UpdateSupplierInfo(model);
        }

        public static List<SupplierInfo> GetSupplierInfo(int pkid)
        {
            return DALWeiXinCard.GetSupplierInfo(pkid);
        }

        public static List<WeixinCardModel> GetWeixinCardModelList(int pkid = -1)
        {
            List<WeixinCardModel> list = new List<WeixinCardModel>();
            var dt = DALWeiXinCard.GetWeiXinCardList(pkid);
            try
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var dateinfo = new DateInfo()
                        {
                            begin_time = Convert.IsDBNull(row["begin_time"]) ? new DateTime(1970, 1, 1) : Convert.ToDateTime(row["begin_time"]),
                            end_time = Convert.IsDBNull(row["end_time"]) ? new DateTime(1970, 1, 1) : Convert.ToDateTime(row["end_time"]),
                            begin_timestamp = Convert.IsDBNull(row["begin_timestamp"]) ? null : (ulong?)Convert.ToUInt64(row["begin_timestamp"]),
                            end_timestamp = Convert.IsDBNull(row["end_timestamp"]) ? null : (ulong?)Convert.ToUInt64(row["end_timestamp"]),
                            fixed_begin_term = Convert.IsDBNull(row["fixed_begin_term"]) ? null : (Int32?)Convert.ToInt32(row["fixed_begin_term"]),
                            fixed_term = Convert.IsDBNull(row["fixed_term"]) ? null : (Int32?)Convert.ToInt32(row["fixed_term"]),
                            // type=(TimeType)Enum.Parse(typeof(TimeType),row["type"].ToString())
                            type = Convert.IsDBNull(row["type"]) ? null : row["type"].ToString()
                        };
                        var skuobj = new SKUQuantity()
                        {
                            quantity = Convert.IsDBNull(row["quantity"]) ? null : (Int32?)Convert.ToInt32(row["quantity"])
                        };
                        var baseinfo = new WeixinCardBaseInfo()
                        {
                            title = Convert.IsDBNull(row["title"]) ? null : row["title"].ToString(),
                            colorid = Convert.IsDBNull(row["color"]) ? 1 : Colors.Where(q=>q.ColorValue== row["color"].ToString()).FirstOrDefault()!=null ? Colors.Where(q => q.ColorValue == row["color"].ToString()).First().ColorIndex:2,
                            notice = Convert.IsDBNull(row["notice"]) ? null : row["notice"].ToString(),
                            description = Convert.IsDBNull(row["description"]) ? null : row["description"].ToString(),
                            supplierId = Convert.IsDBNull(row["supplierId"]) ? 1 : (Int32?)Convert.ToInt32(row["supplierId"]),
                            center_title = Convert.IsDBNull(row["center_title"]) ? null : row["center_title"].ToString(),
                            center_sub_title = Convert.IsDBNull(row["center_sub_title"]) ? null : row["center_sub_title"].ToString(),
                            center_url = Convert.IsDBNull(row["center_url"]) ? null : row["center_url"].ToString(),
                            custom_url = Convert.IsDBNull(row["custom_url"]) ? null : row["custom_url"].ToString(),
                            custom_url_name = Convert.IsDBNull(row["custom_url_name"]) ? null : row["custom_url_name"].ToString(),
                            custom_url_sub_title = Convert.IsDBNull(row["custom_url_sub_title"]) ? null : row["custom_url_sub_title"].ToString(),
                            promotion_url = Convert.IsDBNull(row["promotion_url"]) ? null : row["promotion_url"].ToString(),
                            promotion_url_name = Convert.IsDBNull(row["promotion_url_name"]) ? null : row["promotion_url_name"].ToString(),
                            promotion_url_sub_title = Convert.IsDBNull(row["promotion_url_sub_title"]) ? null : row["promotion_url_sub_title"].ToString(),
                            service_phone = Convert.IsDBNull(row["service_phone"]) ? null : row["service_phone"].ToString(),
                            get_limit = Convert.IsDBNull(row["get_limit"]) ? null : (Int32?)Convert.ToInt32(row["get_limit"]),
                            use_limit = Convert.IsDBNull(row["use_limit"]) ? null : (Int32?)Convert.ToInt32(row["use_limit"]),
                            can_share = Convert.IsDBNull(row["can_share"]) ? false : Convert.ToBoolean(row["can_share"]),
                            can_give_friend = Convert.IsDBNull(row["can_give_friend"]) ? false : Convert.ToBoolean(row["can_give_friend"]),
                            use_all_locations = Convert.IsDBNull(row["use_all_locations"]) ? false : Convert.ToBoolean(row["use_all_locations"]),
                            // location_id_list= row["location_id_list"].ToString(),
                            source = Convert.IsDBNull(row["source"]) ? null : row["source"].ToString(),
                            use_custom_code = Convert.IsDBNull(row["use_custom_code"]) ? false : Convert.ToBoolean(row["use_custom_code"]),
                            get_custom_code_mode = Convert.IsDBNull(row["get_custom_code_mode"]) ? null : row["get_custom_code_mode"].ToString(),
                            bind_openid = Convert.IsDBNull(row["bind_openid"]) ? false : Convert.ToBoolean(row["bind_openid"]),
                            sku = skuobj,
                            date_info = dateinfo
                        };
                        var usecondition = new ConditionalUse()
                        {
                            accept_category = Convert.IsDBNull(row["accept_category"]) ? null : row["accept_category"].ToString(),
                            reject_category = Convert.IsDBNull(row["reject_category"]) ? null : row["accept_category"].ToString(),
                            least_cost = Convert.IsDBNull(row["use_condition_least_cost"]) ? null : (Int32?)Convert.ToInt32(row["use_condition_least_cost"]),
                            object_use_for = Convert.IsDBNull(row["object_use_for"]) ? null : row["object_use_for"].ToString(),
                            can_use_with_other_discount = Convert.IsDBNull(row["can_use_with_other_discount"]) ? false : Convert.ToBoolean(row["can_use_with_other_discount"])
                        };
                        var icon_url_list=Convert.IsDBNull(row["icon_url_list"]) ? string.Empty : row["icon_url_list"].ToString();
                        List<string> icons = icon_url_list.Split(new char[] { ';' }).Where(q=>q!=null).ToList();
                        var imagetextstr = Convert.IsDBNull(row["text_image_list"]) ? string.Empty : row["text_image_list"].ToString();
                        List<ImageText> imageTextList = JsonConvert.DeserializeObject<List<ImageText>>(imagetextstr);


                        var abstractinfo = new AbstractInfo();

                        abstractinfo.abstractstr = Convert.IsDBNull(row["abstract"]) ? null : row["abstract"].ToString();
                        if (icons != null && icons.Any())
                        {
                            icons = icons.Where(q => !string.IsNullOrWhiteSpace(q)).ToList();
                            if (icons.Count() == 5)
                            {
                                abstractinfo.icon1 = icons[0];
                                abstractinfo.icon2 = icons[1];
                                abstractinfo.icon3 = icons[2];
                                abstractinfo.icon4 = icons[3];
                                abstractinfo.icon5 = icons[4];
                            }
                            else if (icons.Count() == 4)
                            {
                                abstractinfo.icon1 = icons[0];
                                abstractinfo.icon2 = icons[1];
                                abstractinfo.icon3 = icons[2];
                                abstractinfo.icon4 = icons[3];
                            }
                            else if (icons.Count() == 3)
                            {
                                abstractinfo.icon1 = icons[0];
                                abstractinfo.icon2 = icons[1];
                                abstractinfo.icon3 = icons[2];
                            }
                            else if (icons.Count() == 2)
                            {
                                abstractinfo.icon1 = icons[0];
                                abstractinfo.icon2 = icons[1];
                            }
                            else if (icons.Count() == 1)
                            {
                                abstractinfo.icon1 = icons[0];
                            }                           
                        }
                        if (imageTextList != null && imageTextList.Any())
                        {
                            imageTextList = imageTextList.Where(q => q != null).ToList();
                            if (imageTextList.Count() == 5)
                            {
                                abstractinfo.imageText1 = imageTextList[0];
                                abstractinfo.imageText2 = imageTextList[1];
                                abstractinfo.imageText3 = imageTextList[2];
                                abstractinfo.imageText4 = imageTextList[3];
                                abstractinfo.imageText5 = imageTextList[4];
                            }
                            else if (imageTextList.Count() == 4)
                            {
                                abstractinfo.imageText1 = imageTextList[0];
                                abstractinfo.imageText2 = imageTextList[1];
                                abstractinfo.imageText3 = imageTextList[2];
                                abstractinfo.imageText4 = imageTextList[3];
                            }
                            else if (imageTextList.Count() == 3)
                            {
                                abstractinfo.imageText1 = imageTextList[0];
                                abstractinfo.imageText2 = imageTextList[1];
                                abstractinfo.imageText3 = imageTextList[2];
                            }
                            else if (imageTextList.Count() == 2)
                            {
                                abstractinfo.imageText1 = imageTextList[0];
                                abstractinfo.imageText2 = imageTextList[1];
                            }
                            else if (imageTextList.Count() == 1)
                            {
                                abstractinfo.imageText1 = imageTextList[0];                                
                            }                         
                        }


                        var advancedinfo = new WeixinCardAdvancedInfo()
                        {
                            abstractinfo = abstractinfo,
                            use_condition = usecondition
                        };
                        var totalinfo = new WeixinCardTotalModel()
                        {
                            deal_detail = Convert.IsDBNull(row["deal_detail"]) ? null : row["deal_detail"].ToString(),
                            least_cost = Convert.IsDBNull(row["least_cost"]) ? null : (Int32?)Convert.ToInt32(row["least_cost"]),
                            reduce_cost = Convert.IsDBNull(row["reduce_cost"]) ? null : (Int32?)Convert.ToInt32(row["reduce_cost"]),
                            discount = Convert.IsDBNull(row["discount"]) ? null : (Int32?)Convert.ToInt32(row["discount"]),
                            gift = Convert.IsDBNull(row["gift"]) ? null : row["gift"].ToString(),
                            default_detail = Convert.IsDBNull(row["default_detail"]) ? null : row["default_detail"].ToString(),
                            base_info = baseinfo,
                            advanced_info=advancedinfo
                        };
                        var model = new WeixinCardModel()
                        {
                            total_info = totalinfo,
                            card_id = Convert.IsDBNull(row["card_id"]) ? null : row["card_id"].ToString(),
                            PKID = Convert.IsDBNull(row["PKID"]) ? null : (Int32?)Convert.ToInt32(row["PKID"]),
                            PushedCount = Convert.IsDBNull(row["PushedCount"]) ? null : (Int32?)Convert.ToInt32(row["PushedCount"]),                         
                            card_type = (CardTypeEnum)Enum.Parse(typeof(CardTypeEnum), row["card_type"].ToString())
                        };
                        list.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return list;
        }


        public static int InsertPromotionCodeToWeixinCard(string cardid, int count = 100)
        {
            return DALWeiXinCard.InsertPromotionCodeToWeixinCard(cardid, count);
        }

        public static int GetPKIDByWeiXinCard(string cardid)
        {
            return DALWeiXinCard.GetTopPkidByCardId(cardid);
        }

        public static List<string> GetWeixinCardCode(string cardid, int pkid,int count = 100)
        {
            List<string> cardcodeList = new List<string>();
            var dt = DALWeiXinCard.GetWeixinCardCode(cardid, pkid, count);
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    cardcodeList.Add(row["code"].ToString());
                }
            }
            return cardcodeList;
        }
    }
}