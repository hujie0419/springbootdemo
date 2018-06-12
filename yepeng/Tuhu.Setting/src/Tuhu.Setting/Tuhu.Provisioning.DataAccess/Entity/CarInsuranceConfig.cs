using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CarInsuranceBanner
    {
        public int PKID { get; set; }
        
        public string Name { set; get; }

        public string Img { set; get; }

        public string LinkUrl { set; get; }

        public string DisplayIndex { set; get; }

        public string DisplayPage { set; get; }

        public int IsEnable { set; get; }

    }

    public class CarInsurancePartner
    {
        public int PKID { set; get; }

        public string Name { set; get; }

        public string Img { set; get; }

        public string LinkUrl { set; get; }

        public string InsuranceId { set; get; }

        public string Remarks { set; get; }

        public string Title { set; get; }

        public string SubTitle { set; get; }

        public string TagText { set; get; }

        public string TagColor { set; get; }

        public int DisplayIndex { set; get; }

        public int IsEnable { set; get; }

        public string Regions { set; get; }

        public string ProviderCode { get; set; }

        public string RegionCode { get; set; }
    }
}

public  class CarInsuranceRegion
{
    public int PKID { set; get; }

    public int ProvinceId { set; get; }

    public string ProvinceName { set; get; }

    public int CityId { set; get; }

    public string CityName { set; get; }

    public int InsurancePartnerId { set; get; }

}
