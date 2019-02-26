using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class ShareOrderConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareOrderConfig");

        public List<ShareOrderConfig> GetShareOrderConfigList(ShareOrderConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DalShareOrderConfig.GetShareOrderConfig(model, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetShareOrderConfigList");
                throw ex;
            }
        }

        public ShareOrderConfig GetShareOrderConfigById(int id)
        {
            try
            {
                return DalShareOrderConfig.GetShareOrderConfigById(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetShareOrderConfig");
                throw ex;
            }
        }

        public bool UpdateShareOrderConfig(ShareOrderConfig model)
        {
            try
            {
                return DalShareOrderConfig.UpdateShareOrderConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateShareOrderConfig");
                throw ex;
            }

        }

        public bool InsertShareOrderConfig(ShareOrderConfig model, ref int newId)
        {
            try
            {
                return DalShareOrderConfig.InsertShareOrderConfig(model, ref newId);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertShareOrderConfig");
                throw ex;
            }
        }
        public bool DeleteShareOrderConfig(int id)
        {
            try
            {
                return DalShareOrderConfig.DeleteShareOrderConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteShareOrderConfig");
                throw ex;
            }
        }


        //------推送消息配置
        public List<OrderSharedPushMessageConfig> GetOrderSharedPushMessageConfig(OrderSharedPushMessageConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DalShareOrderConfig.GetOrderSharedPushMessageConfig(model, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetOrderSharedPushMessageConfig");
                throw ex;
            }
        }

        public OrderSharedPushMessageConfig GetOrderSharedPushMessageConfig(int id)
        {
            try
            {
                return DalShareOrderConfig.GetOrderSharedPushMessageConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetOrderSharedPushMessageConfig");
                throw ex;
            }
        }

        public OrderSharedPushMessageConfig GetOrderSharedPushMessageConfig()
        {
            try
            {
                OrderSharedPushMessageConfig model = DalShareOrderConfig.GetOrderSharedPushMessageConfig();

                if (model != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.AndroidCommunicationValue))
                    {
                        model.AndriodModel = JsonConvert.DeserializeObject<AndriodModel>(model.AndroidCommunicationValue);
                    }
                    if (!string.IsNullOrWhiteSpace(model.IOSCommunicationValue))
                    {
                        model.IOSModel = JsonConvert.DeserializeObject<IOSModel>(model.IOSCommunicationValue);
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetOrderSharedPushMessageConfig");
                throw ex;
            }
        }

        public bool UpdateOrderSharedPushMessageConfig(OrderSharedPushMessageConfig model)
        {
            try
            {
                return DalShareOrderConfig.UpdateOrderSharedPushMessageConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateOrderSharedPushMessageConfig");
                throw ex;
            }

        }

        public bool InsertOrderSharedPushMessageConfig(OrderSharedPushMessageConfig model, ref int newId)
        {
            try
            {
                return DalShareOrderConfig.InsertOrderSharedPushMessageConfig(model, ref newId);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertOrderSharedPushMessageConfig");
                throw ex;
            }
        }
        public bool DeleteOrderSharedPushMessageConfig(int id)
        {
            try
            {
                return DalShareOrderConfig.DeleteOrderSharedPushMessageConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteOrderSharedPushMessageConfig");
                throw ex;
            }
        }
    }
}
