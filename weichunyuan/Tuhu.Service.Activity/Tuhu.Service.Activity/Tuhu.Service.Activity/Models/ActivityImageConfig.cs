using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Enum;

namespace Tuhu.Service.Activity.Models
{
    public class ActivityImageConfig
    {
        private static readonly IReadOnlyDictionary<string, string> Mapping = new Dictionary<string, string>
        {
            {"HeadImg","HeadImg"},{"NoProductImg","NoProductImg"},{"Img12","12"},{"Img13","13"},{"Img14","14"},{"Img15","15"},{"Img16","16"},{"Img17","17"},
            { "Img18","18"},{"Img19","19"},{"Img20","20"},{"Img21","21"},{"Img22","22"},{"Img13C","13C"},{"Img14C","14C"},{"Img15C","15C"},{"Img16C","16C"}
        };

        public Guid ActivityId { get; set; }

        public ActivityImageType Type { get; set; }

        public string ImgType
        {
            get
            {
                if (Mapping.ContainsKey(Type.ToString()))
                {
                    return Mapping[Type.ToString()];
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        public int Position { get; set; }

        public string ImgUrl { get; set; }
    }
}
