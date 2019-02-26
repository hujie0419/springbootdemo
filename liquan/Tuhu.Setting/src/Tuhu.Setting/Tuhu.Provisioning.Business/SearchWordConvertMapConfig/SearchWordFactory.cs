using System;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    public class SearchWordFactory
    {
        public SearchWordMgr Create(SearchWordConfigType configType)
        {
            switch (configType)
            {
                // 同义词配置
                case SearchWordConfigType.Config:
                    return new SearchWordConfigHandler();
                // 新词配置
                case SearchWordConfigType.NewWord:
                    return new SearchWordNewWordHandler();
                // 车型配置
                case SearchWordConfigType.VehicleType:
                    return new SearchWordVehicleTypeHandler();
                default:
                    return null;
            }
        }
    }
}