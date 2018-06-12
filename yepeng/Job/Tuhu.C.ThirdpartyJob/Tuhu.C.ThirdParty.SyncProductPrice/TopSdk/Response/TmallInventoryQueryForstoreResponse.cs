using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// TmallInventoryQueryForstoreResponse.
    /// </summary>
    public class TmallInventoryQueryForstoreResponse : TopResponse
    {
        /// <summary>
        /// 错误code
        /// </summary>
        [XmlElement("errorcode")]
        public string Errorcode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlElement("errormessage")]
        public string Errormessage { get; set; }

        /// <summary>
        /// 整体成功或失败
        /// </summary>
        [XmlElement("issuccess")]
        public bool Issuccess { get; set; }

        /// <summary>
        /// 查询结果
        /// </summary>
        [XmlElement("result")]
        public InventoryQueryResultDomain Result { get; set; }

	/// <summary>
/// InventorySubDetailDtoDomain Data Structure.
/// </summary>
[Serializable]

public class InventorySubDetailDtoDomain : TopObject
{
	        /// <summary>
	        /// ONLINE_INVENTORY:线上可售卖库存。SHARE_INVENTORY：线下独享库存，门店自提可用
	        /// </summary>
	        [XmlElement("inv_biz_code")]
	        public string InvBizCode { get; set; }
	
	        /// <summary>
	        /// 占用库存数
	        /// </summary>
	        [XmlElement("occupy_quantity")]
	        public long OccupyQuantity { get; set; }
	
	        /// <summary>
	        /// 仓库物理库存数
	        /// </summary>
	        [XmlElement("quantity")]
	        public long Quantity { get; set; }
	
	        /// <summary>
	        /// 预扣库存数
	        /// </summary>
	        [XmlElement("reserve_quantity")]
	        public long ReserveQuantity { get; set; }
}

	/// <summary>
/// InventoryInfoDetailDtoDomain Data Structure.
/// </summary>
[Serializable]

public class InventoryInfoDetailDtoDomain : TopObject
{
	        /// <summary>
	        /// distType
	        /// </summary>
	        [XmlElement("inv_store_type")]
	        public long InvStoreType { get; set; }
	
	        /// <summary>
	        /// 占用库存数
	        /// </summary>
	        [XmlElement("occupy_quantity")]
	        public long OccupyQuantity { get; set; }
	
	        /// <summary>
	        /// 仓库物理库存数
	        /// </summary>
	        [XmlElement("quantity")]
	        public long Quantity { get; set; }
	
	        /// <summary>
	        /// 预扣库存数
	        /// </summary>
	        [XmlElement("reserve_quantity")]
	        public long ReserveQuantity { get; set; }
	
	        /// <summary>
	        /// 后端商品code
	        /// </summary>
	        [XmlElement("sc_item_code")]
	        public string ScItemCode { get; set; }
	
	        /// <summary>
	        /// 后端商品id
	        /// </summary>
	        [XmlElement("sc_item_id")]
	        public long ScItemId { get; set; }
	
	        /// <summary>
	        /// 仓库code
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// subList
	        /// </summary>
	        [XmlArray("sub_list")]
	        [XmlArrayItem("inventory_sub_detail_dto")]
	        public List<InventorySubDetailDtoDomain> SubList { get; set; }
}

	/// <summary>
/// TipInfoDomain Data Structure.
/// </summary>
[Serializable]

public class TipInfoDomain : TopObject
{
	        /// <summary>
	        /// errorCode
	        /// </summary>
	        [XmlElement("errorcode")]
	        public string Errorcode { get; set; }
	
	        /// <summary>
	        /// errorMessage
	        /// </summary>
	        [XmlElement("errormessage")]
	        public string Errormessage { get; set; }
	
	        /// <summary>
	        /// invStoreType
	        /// </summary>
	        [XmlElement("inv_store_type")]
	        public long InvStoreType { get; set; }
	
	        /// <summary>
	        /// scItemCode
	        /// </summary>
	        [XmlElement("sc_item_code")]
	        public string ScItemCode { get; set; }
	
	        /// <summary>
	        /// scItemId
	        /// </summary>
	        [XmlElement("sc_item_id")]
	        public long ScItemId { get; set; }
	
	        /// <summary>
	        /// storeCode
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
}

	/// <summary>
/// InventoryQueryResultDomain Data Structure.
/// </summary>
[Serializable]

public class InventoryQueryResultDomain : TopObject
{
	        /// <summary>
	        /// 查询成功列表
	        /// </summary>
	        [XmlArray("item_inventorys")]
	        [XmlArrayItem("inventory_info_detail_dto")]
	        public List<InventoryInfoDetailDtoDomain> ItemInventorys { get; set; }
	
	        /// <summary>
	        /// tipInfos
	        /// </summary>
	        [XmlArray("tip_infos")]
	        [XmlArrayItem("tip_info")]
	        public List<TipInfoDomain> TipInfos { get; set; }
}

    }
}
