using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model.Config
{
    public class Config
    {
        public int Returncode { get; set; }

        public string Message { get; set; }

        public Result Result { get; set; }
    }

    public class Result
    {
        public int SeriesId { get; set; }

        public List<Specs> SpecsList { get; set; }

        public List<ParamTypeItem> ParamTypeItems { get; set; }
    }

    public class Specs
    {
        public int SpecId { get; set; }

        public int SpecState { get; set; }

        public int ShowState { get; set; }
    }

    public class ParamTypeItem
    {
        public string Name { get; set; }

        public List<ParamItem> ParamItems { get; set; }
    }

    public class ParamItem
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public List<ValueItem> ValueItems { get; set; }
    }

    public class ValueItem
    {
        public int SpecId { get; set; }

        public string Value { get; set; }
    }
}
