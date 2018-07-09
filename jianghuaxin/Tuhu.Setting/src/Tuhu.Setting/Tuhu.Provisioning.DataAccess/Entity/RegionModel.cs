using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SimpleRegionModel : BaseModel
    {
        public int PKID { get; set; }
        public string RegionName { get; set; }
        public int ParentID { get; set; }
        public string PinYin { get; set; }
        public IEnumerable<SimpleRegionModel> ChildrenRegion { get; set; }

        public SimpleRegionModel()
        {
            ChildrenRegion = new SimpleRegionModel[0];
        }
        public SimpleRegionModel(DataRow row) : base(row) { }

        public static IEnumerable<SimpleRegionModel> Parse(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new SimpleRegionModel[0];

            return GetChildrenRegion(0, dt.Rows
                .Cast<DataRow>()
                .Select(row => new SimpleRegionModel(row))
                .GroupBy(region => region.ParentID)
                .ToDictionary(group => group.Key, group => group.ToArray()));
        }

        private static IEnumerable<SimpleRegionModel> GetChildrenRegion(int parentID, IDictionary<int, SimpleRegionModel[]> source)
        {
            if (!source.ContainsKey(parentID))
                return new SimpleRegionModel[0];

            var childrenRegions = source[parentID];
            foreach (var model in childrenRegions)
            {
                model.ChildrenRegion = GetChildrenRegion(model.PKID, source);
            }

            return childrenRegions;
        }

    }
}