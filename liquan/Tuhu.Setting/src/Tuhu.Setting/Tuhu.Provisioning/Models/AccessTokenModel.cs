using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class AccessTokenModel
    {
        public string Access_token { get; set; }
        public string Expires_in { get; set; }

        public string Errcode { get; set; }

        public string Errmsg { get; set; }


        public static IEnumerable<SelectListItem> GetOutColors(int? selectedColorIndex)
        {
           var Colors = new List<WeiXinColor> {
                        new WeiXinColor()
                    {
                        ColorIndex = 1,
                        ColorValue = "Color010"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 2,
                        ColorValue = "Color020"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 3,
                        ColorValue = "Color030"
                    },
                            new WeiXinColor()
                    {
                        ColorIndex = 4,
                        ColorValue = "Color040"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 5,
                        ColorValue = "Color050"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 6,
                        ColorValue = "Color060"
                    },
                            new WeiXinColor()
                    {
                        ColorIndex = 7,
                        ColorValue = "Color070"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 8,
                        ColorValue = "Color080"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 9,
                        ColorValue = "Color081"
                    },
                             new WeiXinColor()
                    {
                        ColorIndex = 10,
                        ColorValue = "Color082"
                    },
                            new WeiXinColor()
                    {
                        ColorIndex = 11,
                        ColorValue = "Color090"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 12,
                        ColorValue = "Color100"
                    },
                        new WeiXinColor()
                    {
                        ColorIndex = 13,
                        ColorValue = "Color101"
                    },
                             new WeiXinColor()
                    {
                        ColorIndex = 13,
                        ColorValue = "Color102"
                    }
                    };


            return new SelectList(Colors,
                "ColorIndex", "ColorValue"
                , selectedColorIndex.Value); // add this parameter
        }
    }


    
}