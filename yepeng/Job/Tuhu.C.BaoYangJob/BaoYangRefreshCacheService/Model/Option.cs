using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model.Option
{
    public class Option
    {
        public int Returncode { get; set; }

        public string Message { get; set; }

        public Result Result { get; set; }
    }

    public class Result
    {
        public int SeriesId { get; set; }

        public List<Specs> SpecsList { get; set; }

        public List<ConfigTypeItem> ConfigTypeItems { get; set; }

    }

    public class Specs
    {
        public int SpecId { get; set; }

        public int SpecState { get; set; }

        public int ShowState { get; set; }
    }

    public class ConfigTypeItem
    {
        public string Name { get; set; }

        public List<ConfigItem> ConfigItems { get; set; }
    }

    public class ConfigItem
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public List<ValueItem> ValueItems { get; set; }
    }

    public class ValueItem
    {
        public int SpecId { get; set; }

        public string Value { get; set; }

        public List<PriceItem> Price { get; set; }
    }

    public class PriceItem
    {
        public string SubName { get; set; }

        public int Price { get; set; }
    }

}
