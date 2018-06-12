using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Tuhu.C.Job.Model
{
    public class Wzcitys
    {
        public string Resultcode { set; get; }
        public string Reason { set; get; }
        public string ErrorCode { set; get; }
        public Dictionary<string, WzCityList> Result { set; get; }
    }
    public class WzCityList
    {
        public string Province { set; get; }
        public string ProvinceCode { get; set; }
        public IEnumerable<WzCityDetail> Citys { set; get; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    public class WzCityDetail
    {
        public string CityName { set; get; }
        public string CityCode { set; get; }
        public string Abbr { set; get; }
        public string Engine { set; get; }
        public string Engineno { set; get; }
        public string Class { set; get; }
        public string Classno { set; get; }
        public string Regist { set; get; }
        public string Registno { set; get; }
    }

    public class WzcxyCity
    {
        public string Reason { set; get; }
        public IEnumerable<WzcxyCityResult> Result { set; get; }
        public string ErrorCode { set; get; }
    }

    public class WzcxyCityResult
    {
        public string ProvinceName { set; get; }
        public string ProvincePrefix { set; get; }
        public IEnumerable<WzcxyCityResultDetail> Citys { set; get; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    public class WzcxyCityResultDetail
    {
        public string CityId { set; get; }
        public string CityName { set; get; }
        public string CarnoPrefix { set; get; }
        public string Classno { set; get; }
        public string Engineno { set; get; }
    }

    public class CwzProvince
    {
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }
        [JsonProperty("provincePrefix")]
        public string ProvincePrefix { get; set; }

        [JsonProperty("citys")]
        public IEnumerable<CwzCity> Citys { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    public class CwzCity
    {
        [JsonProperty("city_name")]
        public string CityName { set; get; }
        [JsonProperty("city_code")]
        public string CityCode { set; get; }
        [JsonProperty("abbr")]
        public string Abbr { get; set; }
        [JsonProperty("engine")]
        public string Engine { set; get; }
        [JsonProperty("engineno")]
        public string Engineno { set; get; }
        [JsonProperty("Class")]
        public string Class { set; get; }
        [JsonProperty("classno")]
        public string ClassNo { set; get; }
        [JsonProperty("carnoPrefix")]
        public string CarNoPrefix { set; get; }
        [JsonProperty("regist")]
        public string Regist { set; get; }
        [JsonProperty("registno")]
        public string RegistNo { set; get; }

        [JsonProperty("wzcxyCityId")]
        public string WzcxyCityId { get; set; }
        [JsonProperty("imooboxCityId")]
        public string ImooboxCityId { get; set; }
    }
}
