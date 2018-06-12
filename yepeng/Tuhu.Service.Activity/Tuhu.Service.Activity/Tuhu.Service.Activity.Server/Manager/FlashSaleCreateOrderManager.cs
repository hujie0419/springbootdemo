using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Utils;
using Tuhu.Service.Order;
using Tuhu.Service.Activity.Server.Manager.FlashSaleSystem;
using Tuhu.Service.Activity.Server.FlashSaleSystem;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Models.Order;
using Newtonsoft.Json;
using Tuhu.Service.Activity.Models;
using CreateOrderResult = Tuhu.Service.Activity.Models.CreateOrderResult;
using Tuhu.Service.PinTuan;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class FlashSaleCreateOrderManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FlashSaleCreateOrderManager));

        public static List<Order.Request.OrderItem> ModelConvert(List<OrderItem> items, ref FlashSaleOrderRequest flash)
        {
            var orderItems = new List<Order.Request.OrderItem>();
            foreach (var item in items)
            {
                var orderItem = new Order.Request.OrderItem();
                {
                    orderItem.Category = item.Category;
                    orderItem.Cost = item.Cost;
                    orderItem.Fupid = item.Fupid;
                    orderItem.MarkedPrice = item.MarkedPrice;
                    orderItem.Name = item.Name;
                    orderItem.Num = item.Num;
                    orderItem.Pid = item.Pid;
                    orderItem.Price = item.Price;
                    switch (item.ProductType)
                    {
                        case OrderProductTypes.None:
                            orderItem.ProductType = Order.Request.OrderProductTypes.None;
                            break;
                        case OrderProductTypes.Tire:
                            orderItem.ProductType = Order.Request.OrderProductTypes.Tire;
                            break;
                        case OrderProductTypes.BaoYang:
                            orderItem.ProductType = Order.Request.OrderProductTypes.BaoYang;
                            break;
                        case OrderProductTypes.AutoProduct:
                            orderItem.ProductType = Order.Request.OrderProductTypes.AutoProduct;
                            break;
                        case OrderProductTypes.Beauty:
                            orderItem.ProductType = Order.Request.OrderProductTypes.Beauty;
                            break;
                        case OrderProductTypes.Gifts:
                            orderItem.ProductType = Order.Request.OrderProductTypes.Gifts;
                            break;
                        case OrderProductTypes.Package:
                            orderItem.ProductType = Order.Request.OrderProductTypes.Package;
                            break;
                        case OrderProductTypes.Promotion:
                            orderItem.ProductType = Order.Request.OrderProductTypes.Promotion;
                            break;
                    }
                    orderItem.Remark = item.Remark;
                    orderItem.Size = item.Size;
                    orderItem.UsePromotionCode = item.UsePromotionCode;
                    orderItem.ActivityId = item.ActivityId;
                    if (item.ActivityId.HasValue)
                    {
                        var activityTypeModel = ActivityManager.SelectActivityTypeByActivityIds(new List<Guid>
                        {
                            item.ActivityId.Value
                        }).FirstOrDefault();
                        if (activityTypeModel != null)
                        {
                            var temp = flash.Products.ToList();
                            temp.Add(
                                    new OrderItems
                                    {
                                        ActivityId = item.ActivityId,
                                        Num = item.Num,
                                        PID = item.Pid,
                                        Type = activityTypeModel.Type
                                    });
                            flash.Products = temp;
                            if (activityTypeModel.Type == 1)
                            {
                                //用来控制下单是否走老逻辑
                                orderItem.IsVerifyActivity = GlobalConstant.VerifyActivityNew == 0;
                            }
                            if (activityTypeModel.Type == 5)
                            {
                                orderItem.IsVerifyActivity = false;
                            }
                        }

                    }

                    if (item.ExtInfo != null)
                    {
                        orderItem.ExtInfo = new Order.Models.OrderListExtModel
                        {
                            InstallShopId = item.ExtInfo.InstallShopId,
                            InstallShop = item.ExtInfo.InstallShop
                        };
                        if (item.ExtInfo.Car != null)
                        {
                            orderItem.ExtInfo.Car = new Order.Models.OrderCarModel
                            {
                                VehicleId = item.ExtInfo.Car.VehicleId,
                                Vehicle = item.ExtInfo.Car.Vehicle,
                                Brand = item.ExtInfo.Car.Brand,
                                PaiLiang = item.ExtInfo.Car.PaiLiang,
                                Nian = item.ExtInfo.Car.Nian,
                                SalesName = item.ExtInfo.Car.SalesName,
                                LiYangId = item.ExtInfo.Car.LiYangId,
                                Tid = item.ExtInfo.Car.Tid,
                                VinCode = item.ExtInfo.Car.VinCode,
                                PlateNumber = item.ExtInfo.Car.PlateNumber,
                                ExtCol = item.ExtInfo.Car.ExtCol,
                                Distance = item.ExtInfo.Car.Distance,
                                OnRoadMonth = item.ExtInfo.Car.OnRoadMonth,
                                OnRoadYear = item.ExtInfo.Car.OnRoadYear,
                                Remark = item.ExtInfo.Car.Remark,
                                CarTypeDescription = item.ExtInfo.Car.CarTypeDescription,
                            };
                        };
                    }
                    orderItem.ServiceGroupId = item.ServiceGroupId;
                    orderItem.PackageItems = item.PackageItems != null ? ModelConvert(item.PackageItems.ToList(), ref flash) : null;
                }

                orderItems.Add(orderItem);
            }
            return orderItems;
        }

        public static async Task<CreateOrderResult> FlashSaleCreateOrder(Order.Request.CreateOrderRequest request)
        {
            var flashrequest = new FlashSaleOrderRequest()
            {
                Products = new List<OrderItems>()
            };
            var pintuanFlag = false;
            var pintuanProductGroupId = "";
            var pintuanPid = "";
            var pintuanCount = 0;
            try
            {
                if (request.Items != null && request.Items.Any())
                {
                    foreach (var item in request.Items)
                    {
                        if (item.ActivityId.HasValue)
                        {
                            var activityTypeModel = new ActivityTypeModel();
                            // 增加拼团校验逻辑
                            if (item.ActivityId != null &&
                                await GroupBuyingManager.CheckProductGroupId(item.ActivityId.Value))
                            {
                                activityTypeModel =
                                    new ActivityTypeModel { ActivityId = item.ActivityId.Value, Type = 7 };
                            }
                            else
                            {
                                // 不是拼团ActivityId
                                activityTypeModel = ActivityManager.SelectActivityTypeByActivityIds(new List<Guid>
                                {
                                    item.ActivityId.Value
                                }).FirstOrDefault();
                            }

                            if (activityTypeModel != null)
                            {
                                var temp = flashrequest.Products.ToList();
                                temp.Add(
                                    new OrderItems
                                    {
                                        ActivityId = item.ActivityId,
                                        Num = item.Num,
                                        PID = item.Pid,
                                        Type = activityTypeModel.Type
                                    });
                                flashrequest.Products = temp;
                                if (activityTypeModel.Type == 1)
                                {
                                    //用来控制下单是否走老逻辑
                                    item.IsVerifyActivity = false;
                                }

                                if (activityTypeModel.Type == 5)
                                {
                                    item.IsVerifyActivity = false;
                                }

                                //砍价
                                if (activityTypeModel.Type == 9)
                                {
                                    item.IsVerifyActivity = false;
                                }

                                //拼团
                                if (activityTypeModel.Type == 7)
                                {
                                    request.Status = "0NewPingTuan";
                                    item.IsVerifyActivity = false;
                                    var buyLimitInfo =
                                        await GroupBuyingManager.GetBuyLimitInfo(item.ActivityId.Value, item.Pid,
                                            request.Customer.UserId);
                                    if (string.IsNullOrWhiteSpace(buyLimitInfo?.PID))
                                    {
                                        return new CreateOrderResult
                                        {
                                            ErrorCode = -1000,
                                            ErrorMessage = "未找到该拼团产品!"
                                        };
                                    }

                                    if (buyLimitInfo.BuyLimitCount > 0 &&
                                        buyLimitInfo.BuyLimitCount <= buyLimitInfo.CurrentOrderCount)
                                    {
                                        return new CreateOrderResult
                                        {
                                            ErrorCode = -1000,
                                            ErrorMessage = "已达到限购单数!"
                                        };
                                    }

                                    if (item.Num > buyLimitInfo.UpperLimitPerOrder && buyLimitInfo.UpperLimitPerOrder != 0)
                                    {
                                        return new CreateOrderResult
                                        {
                                            ErrorCode = -1000,
                                            ErrorMessage = "购买数量不符合要求!"
                                        };
                                    }
                                    using (var client = new PinTuanClient())
                                    {
                                        var result = await client.IncreaseSoldCountAsync(buyLimitInfo.ProductGroupId, buyLimitInfo.PID, item.Num);
                                        if (!(result.Success && result.Result.Code == 1))
                                        {
                                            return new CreateOrderResult
                                            {
                                                ErrorCode = -1000,
                                                ErrorMessage = result.Result?.Info ?? "出现异常"
                                            };
                                        }
                                        pintuanFlag = true;
                                        pintuanPid = buyLimitInfo.PID;
                                        pintuanProductGroupId = buyLimitInfo.ProductGroupId;
                                        pintuanCount = item.Num;
                                    }
                                }
                            }

                        }
                    }
                }

                flashrequest.DeviceId = request.DeviceID;
                flashrequest.UseTel = request.Customer.UserTel;
                flashrequest.UserId = request.Customer.UserId;

                ICounter counter = null;

                #region 限时抢购校验

                var fsRequest = new FlashSaleOrderRequest();
                var isCheckFlash = false;
                var flashSaleResult = new List<CheckFlashSaleResponseModel>();
                if (flashrequest.Products != null && flashrequest.Products.Any())
                {
                    var fsItems = flashrequest.Products.Where(r => r.Type == 1);
                    var orderItemses = fsItems as OrderItems[] ?? fsItems.ToArray();
                    if (orderItemses.Any())
                    {
                        isCheckFlash = true;
                        fsRequest.DeviceId = flashrequest.DeviceId;
                        fsRequest.UseTel = flashrequest.UseTel;
                        fsRequest.UserId = flashrequest.UserId;
                        fsRequest.Products = orderItemses;

                        flashSaleResult = await ActivityValidator.CheckFlashSaleAsync(fsRequest);
                        if (flashSaleResult.Any(r => r.Code != Model.CheckFlashSaleStatus.Succeed))
                        {
                            return new CreateOrderResult
                            {
                                ErrorCode = (int)flashSaleResult.Where(r => r.Code != Model.CheckFlashSaleStatus.Succeed).Select(c => c.Code).FirstOrDefault(),
                                ErrorMessage = flashSaleResult.Where(r => r.Code != Model.CheckFlashSaleStatus.Succeed).Select(c => c.Code).FirstOrDefault().GetRemark()
                            };
                        }
                        flashrequest.Products = orderItemses.Select(r =>
                        {
                            r.AllPlaceLimitId =
                                flashSaleResult.Where(p => p.PID == r.PID)
                                    .Select(_ => _.AllPlaceLimitId)
                                    .FirstOrDefault();
                            return r;
                        });
                    }
                }


                #endregion
                #region 保养校验

                if (flashrequest.Products != null && flashrequest.Products.Any())
                {
                    var byRequest = flashrequest.Products.Where(r => r.Type == 5);
                    var activityId = byRequest.FirstOrDefault(o => o.ActivityId != Guid.Empty)?.ActivityId;
                    if (activityId != null)
                    {
                        List<BaoYang.Models.BaoYangVehicleFivePropertyModel> propertiesList = null;
                        if (request.Car != null && request.Car.ExtCol != null &&
                            request.Car.ExtCol.ContainsKey("Properties")
                            && request.Car.ExtCol["Properties"] != null)
                        {
                            string properties = request.Car.ExtCol["Properties"].ToString();
                            List<dynamic> list = JsonConvert.DeserializeObject<List<dynamic>>(properties);
                            propertiesList = list.Select(o => new BaoYang.Models.BaoYangVehicleFivePropertyModel()
                            {
                                Property = o.propertyKey,
                                PropertyValue = o.propertyValue
                            }).ToList();
                        }
                        ValidateOrderRequest validateRequest = new ValidateOrderRequest()
                        {
                            ActivityId = activityId.Value,
                            UserId = flashrequest.UserId,
                            Products =
                                request.Items.Where(
                                        o => o.ActivityId != null && o.ActivityId.HasValue && !o.Pid.StartsWith("FU-") && !o.Pid.StartsWith("TR-"))
                                    .Select(o => new OrderProduct()
                                    {
                                        ProductId = o.Pid,
                                        Count = o.Num,
                                        ActivityId = o.ActivityId.Value,
                                        Price = o.Price,
                                        ProductType = "Product"
                                    }).ToList(),
                            ShopId = request.Delivery.InstallShopId ?? 0,
                            Channel = request.OrderChannel,
                            InstallType = request.Delivery.InstallType,
                            Vehicle = new BaoYang.Models.VehicleRequestModel()
                            {
                                VehicleId = request.Car.VehicleId,
                                PaiLiang = request.Car.PaiLiang,
                                Nian = request.Car.Nian,
                                Tid = request.Car.Tid,
                                Properties = propertiesList
                            },
                            RegionId = 1
                        };
                        using (var client = new BaoYangClient())
                        {
                            var baoyangResult = await client.ValidateFixedPriceActivityOrderAsync(validateRequest);
                            if (!baoyangResult.Success || !baoyangResult.Result)
                            {
                                return new CreateOrderResult()
                                {
                                    ErrorCode = (int)CreateOrderErrorCode.ProductValidateFailed,
                                    ErrorMessage =
                                        CreateOrderMessageDic.GetMessage(CreateOrderErrorCode.ProductValidateFailed)
                                };
                            }
                        }
                        // 验证活动状态
                        var validateResult = await ActivityValidator.ValidateBaoyang(activityId.Value);
                        if (validateResult.Item1 == Model.CreateOrderErrorCode.ActivitySatisfied)
                        {
                            // 验证限购数量
                            counter = new BaoYangCounter(activityId.Value, validateResult.Item2, validateResult.Item3,
                                validateResult.Item4);
                            var countResult = await counter.CanPurchaseAndIncreaseCount(flashrequest.UserId,
                                flashrequest.DeviceId, flashrequest.UseTel);

                            if (countResult.Code != Model.CreateOrderErrorCode.ActivitySatisfied)
                            {
                                return new CreateOrderResult()
                                {
                                    ErrorCode = (int)countResult.Code,
                                    ErrorMessage = CreateOrderMessageDic.GetMessage(countResult.Code)
                                };
                            }
                        }
                        else
                        {
                            return new CreateOrderResult()
                            {
                                ErrorCode = (int)validateResult.Item1,
                                ErrorMessage = CreateOrderMessageDic.GetMessage(validateResult.Item1)
                            };
                        }
                    }
                }

                #endregion
                #region 分享砍价活动
                bool bargainflag = false;
                Guid ownerId = new Guid();
                string pid = "";
                var parameters = new List<BuyLimitDetailModel>();
                if (flashrequest.Products != null && flashrequest.Products.Any())
                {
                    var item = flashrequest.Products.FirstOrDefault(g => g.Type == 9);
                    if (item != null)
                    {
                        ownerId = flashrequest.UserId;
                        pid = item.PID;
                        parameters.Add(new BuyLimitDetailModel
                        {
                            ModuleName = "sharebargain",
                            LimitObjectId = ownerId.ToString("D"),
                            ObjectType = LimitObjectTypeEnum.UserId.ToString(),
                            Remark = "砍价实物商品下单"
                        });
                        if(!string.IsNullOrWhiteSpace(flashrequest.DeviceId))
                        {
                            parameters.Add(new BuyLimitDetailModel
                            {
                                ModuleName = "sharebargain",
                                LimitObjectId = flashrequest.DeviceId,
                                ObjectType = LimitObjectTypeEnum.DeviceId.ToString(),
                                Remark = "砍价实物商品下单"
                            });
                        }
                        var val = await DalBargain.CheckBargainProductStatusByPID(ownerId, pid);
                        if (!val)
                        {
                            return new CreateOrderResult()
                            {
                                ErrorCode = -1000,
                                ErrorMessage = "您当前没有资格享受该优惠!"
                            };
                        }
                        bargainflag = true;
                    }
                }
                #endregion
                try
                {
                    using (var client = new CreateOrderClient())
                    {
                        var result = await client.CreateOrderAsync(request);
                        result.ThrowIfException();
                        if (result.Success)
                        {
                            if (counter != null)
                            {
                                await counter.AddOrderRecord(result.Result.OrderId, flashrequest.UserId, flashrequest.DeviceId, flashrequest.UseTel);
                            }
                            if (bargainflag)
                            {
                                var tal = await DalBargain.BuyBargainProductAsync(ownerId, pid, result.Result.OrderId);
                                if (tal > 0)
                                {
                                    parameters.ForEach(g => { g.ModuleProductId = tal.ToString(); g.Reference = result.Result.OrderId.ToString(); });
                                    await LimitBuyManager.AddBuyLimitInfo(parameters);
                                    Logger.Info($"UserId为{ownerId}的用户享受砍价优惠，已购买pid为{pid}的商品");
                                }
                                else
                                {
                                    Logger.Error($"UserId为{ownerId}的用户享受砍价优惠，购买pid为{pid}的商品,修改购买状态时出错");
                                }
                            }
                            flashrequest.OrderId = result.Result.OrderId;
                            if (isCheckFlash)
                            {
                                Logger.Info($"下单成功订单号=>{flashrequest.OrderId}发送消息");
                                try
                                {
                                    TuhuNotification.SendNotification(".FlashSaleCreateOrder.", flashrequest);
                                }
                                catch (Exception ex)
                                {
                                    var str = "";
                                    try
                                    {
                                        str = JsonConvert.SerializeObject(flashrequest);
                                    }
                                    catch (Exception)
                                    {
                                        str = result.Result.OrderId.ToString();
                                    }

                                    Logger.Error($"request-->{str}-->mq发送失败", ex);
                                }
                            }
                            return new CreateOrderResult
                            {
                                OrderId = result.Result.OrderId,
                                OrderNo = result.Result.OrderNo
                            };
                        }
                        else
                        {
                            // 拼团下单失败，修改虚拟库存
                            if (pintuanFlag)
                            {
                                using (var client2 = new PinTuanClient())
                                {
                                    var result2 = await client2.DecrementSoldCountAsync(pintuanProductGroupId, pintuanPid, pintuanCount);
                                    if (!(result2.Success && result2.Result.Code == 1))
                                    {
                                        Logger.Error($"拼团产品下单失败，虚拟库存修改失败-->{pintuanProductGroupId}/{pintuanPid}/{pintuanCount}");
                                    }
                                }
                            }


                            if (counter != null)
                            {
                                await counter.DecreasePurchaseCount(flashrequest.UserId, flashrequest.DeviceId, flashrequest.UseTel);
                            }
                            Logger.Info("调用下单接口失败" + result.ErrorCode + result.ErrorMessage);
                            if (isCheckFlash)
                                await FlashSaleCounter.DecrementAllFlashCount(fsRequest, flashSaleResult);
                            if (result.ErrorCode == "Order_FlashSale_Error")
                            {
                                return new CreateOrderResult()
                                {
                                    ErrorCode = -1000,
                                    ErrorMessage = CreateOrderMessageDic.GetFlashSaleErrorMessage(result.ErrorMessage)
                                };
                            }
                            if (result.ErrorCode == "Invalid_PromotionCode")
                                return new CreateOrderResult()
                                {
                                    ErrorCode = -1000,
                                    ErrorMessage = "优惠券无效!"
                                };
                            else
                            {
                                return new CreateOrderResult()
                                {
                                    ErrorCode = -1000,
                                    ErrorMessage = "下单失败!"
                                };
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    if (counter != null)
                    {
                        await counter.DecreasePurchaseCount(flashrequest.UserId, flashrequest.DeviceId, flashrequest.UseTel);
                    }
                    if (isCheckFlash)
                        await FlashSaleCounter.DecrementAllFlashCount(fsRequest, flashSaleResult);
                    Logger.Error("调用下单接口失败" + ex.Message + ex.InnerException);
                    return new CreateOrderResult()
                    {
                        ErrorCode = -1000,
                        ErrorMessage = "下单失败"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new CreateOrderResult()
                {
                    ErrorCode = -1000,
                    ErrorMessage = "下单失败"
                };
            }

        }
        public static async Task<IEnumerable<ActivityPriceModel>> FetchActivityProductPrice(ActivityPriceRequest request)
        {
            var data = request.Items.Where(g => !string.IsNullOrWhiteSpace(g.PID))
                .GroupBy(g => g.ActicityId)
                .Select(g => new ProductActivityType { ActivityId = g.Key, PIDs = g.Select(t => t.PID).ToList() });
            var result = new List<ActivityPriceModel>();
            if (data.Any())
            {
                var types = ActivityManager.SelectActivityTypeByActivityIds(data.Select(g => g.ActivityId).ToList()).Where(g => g != null);
                foreach (var item in data)
                {
                    var type = types.FirstOrDefault(g => g.ActivityId == item.ActivityId);

                    //拼团
                    if (type == null && await GroupBuyingManager.CheckProductGroupId(item.ActivityId))
                    {
                        type = new ActivityTypeModel { ActivityId = item.ActivityId, Type = 7 };
                    }

                    if (type == null) continue;
                    if (type.Type == 1)
                    {
                        var dat = await DalBargain.SelectProductActivityPrice(item.ActivityId, item.PIDs);
                        if (dat == null || dat.Count() != item.PIDs.Count)
                        {
                            return null;
                        }
                        result.AddRange(dat);
                    }
                    else if (type.Type == 9)
                    {
                        if (request.UserId == Guid.Empty)
                        {
                            break;
                        }
                        var userId = request.UserId;
                        foreach (var pid in item.PIDs)
                        {
                            result.Add(await DalBargain.FetchBargainPrice(pid, userId));
                        }
                    }
                    else if (type.Type == 7)
                    {
                        // 拼团
                        if (request.UserId == Guid.Empty)
                        {
                            break;
                        }
                        foreach (var pid in item.PIDs)
                        {
                            if (request.GroupId == Guid.Empty)
                            {

                                var priceInfo = await DalFlashSale.FetchGroupBuyingPrtoductPrice(item.ActivityId, pid, Logger);
                                if (await DalFlashSale.CheckFreeCouponInfo(item.ActivityId, request.UserId))
                                {
                                    priceInfo.ActivityPrice = 0;
                                    Logger.Info($"GroupBuyingFreeCoupon==>{request.UserId:D}/{item.ActivityId:D}/{pid}==>符合团长免单==>活动价为0");
                                }
                                result.Add(priceInfo);
                            }
                            else
                            {
                                var dat = await DalFlashSale.FetchGroupBuyingPrice(pid, request.UserId, request.GroupId, Logger);
                                if (dat.Code == 0)
                                {
                                    return null;
                                }
                                result.Add(dat);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
