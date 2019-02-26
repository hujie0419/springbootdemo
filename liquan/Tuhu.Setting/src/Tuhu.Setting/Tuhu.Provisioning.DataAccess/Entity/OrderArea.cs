using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class OrderArea
    {
        #region OrderArea
        public int id { get; set; }

        public int parentid { get; set; }

        public bool isparent { get; set; }

        public int apptype { get; set; }

        public int version { get; set; }
        public int areatype { get; set; }

        public string bigtitle { get; set; }

        public string smalltitle { get; set; }

        public bool isothercity { get; set; }

        public bool isproduct { get; set; }

        public string modelname { get; set; }

        public int modelfloor { get; set; }

        public int showorder { get; set; }

        public int showstyle { get; set; }

        public string citycode { get; set; }

        public string districtcode { get; set; }

        public string icoimgurl { get; set; }

        public string jumph5url { get; set; }

        public int showstatic { get; set; }

        public DateTime starttime { get; set; }

        public DateTime overtime { get; set; }

        public int cpshowtype { get; set; }

        public string cpshowbanner { get; set; }

        public string appoperateval { get; set; }

        public string operatetypeval { get; set; }

        public string productNum { get; set; }

        public string keyvaluelenth { get; set; }

        public string youmen { get; set; }

        public DateTime createtime { get; set; }

        public DateTime updatetime { get; set; }

        public bool isdelete { get; set; }

        public int IsReadChild { get; set; }
        #endregion
    }
}