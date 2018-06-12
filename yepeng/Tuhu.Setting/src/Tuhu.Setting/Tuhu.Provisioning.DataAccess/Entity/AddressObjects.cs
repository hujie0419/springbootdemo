using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class AddressObjects
    {
        public string u_address_id { get; set; }
        public int i_address_type { get; set; }
        public string u_address_name { get; set; }
        public string u_first_name { get; set; }
        public string u_last_name { get; set; }
        public string u_description { get; set; }
        public string u_address_line1 { get; set; }
        public string u_address_line2 { get; set; }
        public string u_city { get; set; }
        public string u_region_code { get; set; }
        public string u_region_name { get; set; }
        public string u_postal_code { get; set; }
        public string u_country_code { get; set; }
        public string u_country_name { get; set; }
        public string u_tel_number { get; set; }
        public string u_tel_extension { get; set; }
        public int i_locale { get; set; }
        public string u_user_id_changed_by { get; set; }
        public DateTime? dt_date_last_changed { get; set; }
        public DateTime? dt_csadapter_date_last_changed { get; set; }
        public DateTime? dt_date_created { get; set; }

        /// <summary>
        /// 省Id
        /// </summary>
        public int u_region_provinceId { get; set; }

        /// <summary>
        /// 省名字
        /// </summary>
        public string u_region_provinceName { get; set; }


        /// <summary>
        /// 市Id
        /// </summary>
        public int u_region_cityId { get; set; }

        /// <summary>
        /// 市名字
        /// </summary>
        public string u_region_cityName { get; set; }

        /// <summary>
        /// 区Id
        /// </summary>
        public int? u_region_countyId { get; set; }

        /// <summary>
        /// 区名字
        /// </summary>
        public string u_region_countyName { get; set; }
    }
}
