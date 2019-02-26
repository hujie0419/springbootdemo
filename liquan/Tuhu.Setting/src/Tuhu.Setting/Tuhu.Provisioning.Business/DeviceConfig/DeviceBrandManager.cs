using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Aop.Api.Request;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json.Linq;
using Tuhu.Provisioning.DataAccess.DAO.DeviceConfig;
using Tuhu.Provisioning.DataAccess.Entity.DeviceConfig;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;

namespace Tuhu.Provisioning.Business.DeviceConfig
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceBrandManager
    {

        public List<DeviceModelInfo> GetAllByBrandKeys(int brandId = 0, string keyword = null)
        {
            var allList = DalDeviceBrand.GetAllInfo();

            var query = allList;


            if (brandId > 0)
            {
                query = query.Where(m => m.BrandID == brandId);
            }

            keyword = keyword?.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(m =>
                    !string.IsNullOrEmpty(m.DeviceType) && m.DeviceType.ToLower().Contains(keyword.ToLower())
                    || !string.IsNullOrEmpty(m.DeviceModel) && m.DeviceModel.ToLower().Contains(keyword.ToLower()));
            }

            return query.ToList();
        }




        private void CheckBasicInfo(DeviceModelFormList data, List<DeviceModelInfo> list)
        {
            if (data == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(data.DeviceType.Trim()))
            {
                throw new Exception($"机型信息不允许为空!");
            }

            if (!data.Models.Any() || data.Models.Any(m => string.IsNullOrEmpty(m.ModelName.Trim())))
            {
                throw new Exception($"设备号不允许为空!");
            }

            if (list.All(m => m.BrandID != data.BrandID))
            {
                throw new Exception("厂商信息填写错误,请核对厂商信息!");
            }

            var count = data.Models.Select(m => m?.ModelName.Trim()).Where(m => !string.IsNullOrEmpty(m)).Distinct().Count();
            if (count != data.Models.Count)
            {
                throw new Exception($"多个设备号中存在重复值");
            }

            var leaveTypeList = list.Where(m => m.TypeID != data.TypeID && !string.IsNullOrEmpty(m.DeviceType) && !string.IsNullOrEmpty(data.DeviceType?.Trim())).ToList();
            if (leaveTypeList.Any())
            {
                var leaveTypeFlag = leaveTypeList.All(m => !m.DeviceType.Trim().Equals(data.DeviceType.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (!leaveTypeFlag)
                {
                    throw new Exception($"已有{data.DeviceType}机型,请确认.");
                }
            }

            data.Models.ForEach(m =>
            {
                var leaveModelsList = list.Where(i => i.ModelID != m.ModelId && !string.IsNullOrEmpty(data.DeviceModel?.Trim()));

                var leaveModelFlag = leaveModelsList.All(i => !i.DeviceModel.Equals(m.ModelName, StringComparison.InvariantCultureIgnoreCase));
                if (!leaveModelFlag)
                {
                    throw new Exception($"厂商({data.DeviceBrand})-机型({data.DeviceType})-设备号({m.ModelName}),已存在请确认.");
                }
            });
        }


        /// <summary> 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool BatchHandleDeviceModel(DeviceModelFormList data, string userName)
        {
            var list = GetAllByBrandKeys();
            CheckBasicInfo(data, list);

            var typeIdInfo = list.FirstOrDefault(m => m.TypeID == data.TypeID && m.TypeID > 0);
            if (typeIdInfo != null)
            {// 修改机型信息 
                typeIdInfo.BrandID = data.BrandID;
                typeIdInfo.DeviceType = data.DeviceType;
                typeIdInfo.UpdateUser = userName;

                var result = DalDeviceBrand.UpdateType(new DeviceTypeEntity()
                {
                    PKID = data.TypeID,
                    BrandId = data.BrandID,
                    DeviceType = data.DeviceType,
                    UpdateUser = userName,
                });

                if (!result)
                    throw new Exception($"修改机型({data.TypeID}),出错!");
            }
            else
            {// 新增机型信息
                var result = DalDeviceBrand.InsertType(new DeviceTypeEntity()
                {
                    BrandId = data.BrandID,
                    DeviceType = data.DeviceType,
                    CreateUser = userName
                });

                if (result > 0)
                {
                    typeIdInfo = new DeviceModelInfo()
                    {
                        BrandID = data.BrandID,
                        DeviceBrand = data.DeviceBrand,

                        TypeID = result,
                        DeviceType = data.DeviceType,
                    };
                }
                else
                {
                    throw new Exception($"添加机型信息出错");
                }
            }

            if (data.Models != null && data.Models.Any())
            {
                foreach (var item in data.Models)
                {
                    //var modelInfo = list.FirstOrDefault(m => m.ModelID == item.ModelId);
                    if (item.ModelId > 0)
                    {//更新 
                        var result = DalDeviceBrand.UpdateModel(new DeviceModelEntity()
                        {
                            PKID = item.ModelId,
                            TypeId = typeIdInfo.TypeID,
                            DeviceModel = item.ModelName,
                            UpdateUser = userName,
                        });

                        if (!result)
                            throw new Exception($"修改机型({data.TypeID}),出错!");
                    }
                    else
                    {//新增
                        var result = DalDeviceBrand.InsertModel(new DeviceModelEntity()
                        {
                            DeviceModel = item.ModelName,
                            TypeId = typeIdInfo.TypeID,
                            CreateUser = userName,
                        });
                        if (result <= 0)
                        {
                            throw new Exception($"添加机型信息({item.ModelName})出错");
                        }
                    }
                }
            }

            return true;
        }


        public IEnumerable<DeviceBrandEntity> GetAllBrandList()
        {
            return DalDeviceBrand.GetAllBrandList();
        }



        public bool DeleteBrand(int pkid)
        {
            return DalDeviceBrand.DeleteBrand(pkid);
        }

        public bool DeleteType(int pkid)
        {
            return DalDeviceBrand.DeleteType(pkid);
        }


        public bool DeleteModel(int pkid)
        {
            return DalDeviceBrand.DeleteModel(pkid);
        }




    }

}
