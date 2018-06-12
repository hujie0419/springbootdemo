using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Initialization.Model;

namespace Tuhu.C.Job.Initialization.Dal
{
    public static class DalPinTuan
    {
        public static List<string> GetPinTuanProduct()
        {
            const string sqlStr = @"
select distinct
       T.PID
from Configuration..GroupBuyingProductConfig as T with (nolock)
where T.IsDelete = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteQuery(true, cmd, dt => dt.ToList<string>())?.ToList() ?? new List<string>();
            }
        }

        public static List<PinTuanProductModel> GetPinTuanProductList(string pid)
        {
            const string sqlStr = @"
select T.ProductGroupId,
       T.PID,
       T.FinalPrice as ActivityPrice
from Configuration..GroupBuyingProductConfig as T with (nolock)
where T.IsDelete = 0
      and T.PID = @pid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                return DbHelper.ExecuteSelect<PinTuanProductModel>(true, cmd)?.ToList() ??
                       new List<PinTuanProductModel>();
            }
        }

        public static List<PinTuanOriginalStockModel> GetOriginalStockCount(string pid)
        {
            const string sqlStr = @"
select PID,
       WAREHOUSEID as Warehouseid,
       TotalAvailableStockQuantity as StockCount
from Tuhu_bi..dw_ProductAvaibleStockQuantity as sq with (nolock)
where sq.WAREHOUSEID in ( 7295, 8598, 8634, 11410, 28790 )
      and sq.PID = @pid;";
            using(var dbHelper=DbHelper.CreateLogDbHelper(true))
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                return dbHelper.ExecuteSelect<PinTuanOriginalStockModel>(cmd)?.ToList() ??
                       new List<PinTuanOriginalStockModel>();
            }
        }

        public static bool SetPinTuanStock(string productGroupId, string pid, int count)
        {
            const string sqlStr = @"
update Configuration..GroupBuyingProductConfig with (rowlock)
set TotalStockCount = @count,
    LastUpdateDateTime = GETDATE()
where ProductGroupId = @productGroupId
      and PID = @pid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@count", count);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@pid", pid);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static List<PinTuanProductModel> GetOriginalPinTuanProduct()
        {
            const string sqlStr = @"select ProductGroupId,
       PID,
       DisPlay as Display
from Configuration..GroupBuyingProductConfig with (nolock)
where ProductGroupId in ( 'PT10372418', 'PT-2118240', 'PT94793088', 'PT-6603455', 'PT-7033950', 'PT43706792',
                          'PT-6727506', 'PT-1872331', 'PT13991314', 'PT-8740591', 'PT-4126068', 'PT64851258',
                          'PT15205363', 'PT-7097041', 'PT-1649061', 'PT-1965801', 'PT64650343', 'PT28417027',
                          'PT18646651', 'PT-2067992', 'PT-1464738', 'PT38492519', 'PT-1481210', 'PT-2440181',
                          'PT-1759030', 'PT-5210756', 'PT18530419', 'PT-1306114', 'PT10260966', 'PT21286683',
                          'PT85545120', 'PT31857077', 'PT-1707950', 'PT43566631', 'PT47418860', 'TH-3256954',
                          'PT-1262397', 'PT59892282', 'PT23580227', 'PT10372504', 'PT-1862856', 'PT18410821',
                          'PT-8152363', 'PT-9045771', 'PT-1828978', 'PT94675653', 'PT55239223', 'PT13520102',
                          'PT24117571', 'PT12047369', 'PT-4957778', 'PT-1952176', 'PT-1772866', 'PT-1120285',
                          'PT-3517950', 'PT-3955452', 'PT-1705932', 'TH20586059', 'PT-1775698', 'PT-2889423',
                          'PT-1765801', 'PT56799889', 'PT65681424', 'PT-5751400', 'PT-1525368', 'PT-1855209',
                          'PT-1457369', 'PT18251596', 'PT-1336931', 'PT12630295', 'PT-1321006', 'PT20356386',
                          'PT-8212356', 'PT-2801207', 'PT11750162', 'PT-2053850', 'PT17898844', 'PT-3840364',
                          'PT53992994', 'PT16886414', 'PT15677984', 'PT-1932519', 'PT-3330327', 'PT19721386',
                          'PT47405985', 'PT76617873', 'PT97458229', 'PT50578852', 'PT-8383832', 'PT-2078489',
                          'PT-1056261', 'PT19854270', 'PT-3422144', 'PT-1060366', 'PT14712925', 'PT32531632',
                          'PT20623814', 'TH49723003', 'PT10360796', 'PT12754521', 'PT37222245', 'PT-3133246',
                          'PT22545930', 'PT-2001247', 'PT18685259', 'PT29137363', 'PT-1486916', 'PT-1412991',
                          'PT20468705', 'PT-1121531', 'PT31406602', 'PT77172947', 'PT15514325', 'PT58190556',
                          'PT-1897619', 'PT18868950', 'PT-4256076', 'PT50118470', 'PT-7970273', 'PT-1624063',
                          'PT-1262048', 'PT-1911115', 'PT95470371', 'PT-1845689', 'PT-7900322', 'PT86410473',
                          'PT-1826889', 'PT36978639', 'PT16991226', 'PT55600349', 'PT-6135433', 'PT15572849',
                          'PT-2104080', 'PT-1422093', 'PT13831476', 'PT-1807783', 'PT-1306065', 'PT11003164',
                          'PT19118765', 'PT10338591', 'PT-1668435', 'PT36899178', 'PT-8240266', 'PT-7569006',
                          'PT18020253', 'PT-3746584', 'PT13351104', 'PT-9815498', 'PT11513986', 'PT11675182',
                          'PT17754539', 'PT16312573', 'PT10679844', 'PT21335384', 'PT-9039574', 'PT-2547078',
                          'PT-1852188', 'PT18958288', 'PT14661252', 'PT-2110974', 'PT20334347', 'PT15453441',
                          'PT-1810046', 'PT-5844196', 'PT14046277', 'PT34761060', 'PT-1165609', 'PT12412509',
                          'PT-7409027', 'PT19836988', 'PT-4326209', 'PT-2071243', 'PT-3936391', 'PT11637869',
                          'PT-1516200', 'PT-1895508', 'PT24879402', 'PT43250250', 'PT-2132321', 'PT-8643280',
                          'PT-2209237', 'PT33850163', 'PT-9059595', 'PT-1790448', 'PT11786298', 'PT-2110105',
                          'PT-1306082', 'PT89384125', 'PT-4695693', 'PT19100185', 'PT-1492563', 'PT17192645',
                          'PT18244520', 'PT96407874', 'PT62752921', 'PT20176923', 'PT13354869', 'PT-8848913',
                          'PT-1441467', 'PT20798974', 'PT13789437', 'PT-7126064', 'PT-9893495', 'PT10120162',
                          'PT-2433986', 'PT-2037023', 'PT16069721', 'PT17299869', 'PT20057321', 'PT20252423',
                          'PT14906602', 'PT-2054981', 'PT-8048387', 'PT19472280', 'PT18018142', 'PT-1691480',
                          'PT51257228', 'PT-1030559', 'PT12234380', 'PT-2084891', 'PT-1658422', 'PT45613562',
                          'PT-2825050', 'PT-1646010', 'PT48602635', 'PT10469946', 'PT14809824', 'PT71038711',
                          'PT10754816', 'PT23800566', 'PT52172534', 'PT-8887110', 'PT20715503', 'PT11546890',
                          'PT-1735175', 'PT40105403', 'PT-7655302', 'PT-1485236', 'PT-5491153', 'PT-1079113',
                          'PT-1207353', 'PT63759881', 'PT-2072284', 'PT-1465632', 'PT19186671', 'PT14043198',
                          'PT-5900016' );";
            using(var cmd=new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteSelect<PinTuanProductModel>(true, cmd)?.ToList() ?? new List<PinTuanProductModel>();
            }
        }

        public static int AddPinTuanProductInfo(List<PinTuanProductModel> models,string productGroupId)
        {
            const string sqlStr = @"select ProductCode
from Tuhu_productcatalog..vw_Products with (nolock)
where PID = @pid;";
            var productCode = default(string);
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@pid", models.FirstOrDefault(g=>g.Display)?.PId);
                productCode = DbHelper.ExecuteScalar(true, cmd)?.ToString();
            }
            if (string.IsNullOrWhiteSpace(productCode)) return 0;
            var sqlStr2 = $@"
insert into Configuration.dbo.GroupBuyingProductConfig
(
    ProductName,
    SimpleProductName,
    PID,
    ProductGroupId,
    OriginalPrice,
    FinalPrice,
    SpecialPrice,
    DisPlay,
    Creator,
    IsDelete,
    CreateDateTime,
    LastUpdateDateTime,
    UseCoupon,
    IsShow,
    BuyLimitCount,
    UpperLimitPerOrder,
    TotalStockCount,
    IsShowApp,
    IsAutoStock
)
select T.DisplayName,
       '',
       T.PID,
       @productGroupId,
       cy_list_price,
       cy_list_price,
       cy_list_price,
       0,
       'SystemAuto',
       0,
       GETDATE(),
       GETDATE(),
       0,
       0,
       0,
       1,
       0,
       0,
       0
from Tuhu_productcatalog..vw_Products as T with (nolock)
where T.ProductCode = @productCode
      and T.PID not in ( '{string.Join("','", models.Select(g => g.PId).ToList())}' )
      and T.OnSale = 1
      and T.stockout = 0;";
            using(var cmd=new SqlCommand(sqlStr2))
            {
                cmd.Parameters.AddWithValue("@productCode", productCode);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                return DbHelper.ExecuteNonQuery(cmd);
            }

        }
    }
}
