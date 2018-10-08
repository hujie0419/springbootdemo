using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 地址信息
    /// </summary>
    [Serializable]
    public class AddressesModel
    {
        public AddressesModel()
        { }
        #region Model
        private string _u_address_id = string.Empty;
        private int _i_address_type = 0;
        private string _u_address_name = string.Empty;
        private string _u_first_name = string.Empty;
        private string _u_last_name = string.Empty;
        private string _u_description = string.Empty;
        private string _u_address_line1 = string.Empty;
        private string _u_address_line2 = string.Empty;
        private string _u_city = string.Empty;
        private string _u_region_code = string.Empty;
        private string _u_region_name = string.Empty;
        private string _u_postal_code = string.Empty;
        private string _u_country_code = string.Empty;
        private string _u_country_name = string.Empty;
        private string _u_tel_number = string.Empty;
        private string _u_tel_extension = string.Empty;
        private int _i_locale = 0;
        private string _u_user_id_changed_by = string.Empty;
        private DateTime _dt_date_last_changed = DateTime.Now;
        private DateTime _dt_csadapter_date_last_changed = DateTime.Now;
        private DateTime _dt_date_created = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public string u_address_id
        {
            set { _u_address_id = value; }
            get { return _u_address_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int i_address_type
        {
            set { _i_address_type = value; }
            get { return _i_address_type; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_address_name
        {
            set { _u_address_name = value; }
            get { return _u_address_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_first_name
        {
            set { _u_first_name = value; }
            get { return _u_first_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_last_name
        {
            set { _u_last_name = value; }
            get { return _u_last_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_description
        {
            set { _u_description = value; }
            get { return _u_description; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_address_line1
        {
            set { _u_address_line1 = value; }
            get { return _u_address_line1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_address_line2
        {
            set { _u_address_line2 = value; }
            get { return _u_address_line2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_city
        {
            set { _u_city = value; }
            get { return _u_city; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_region_code
        {
            set { _u_region_code = value; }
            get { return _u_region_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_region_name
        {
            set { _u_region_name = value; }
            get { return _u_region_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_postal_code
        {
            set { _u_postal_code = value; }
            get { return _u_postal_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_country_code
        {
            set { _u_country_code = value; }
            get { return _u_country_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_country_name
        {
            set { _u_country_name = value; }
            get { return _u_country_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_tel_number
        {
            set { _u_tel_number = value; }
            get { return _u_tel_number; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_tel_extension
        {
            set { _u_tel_extension = value; }
            get { return _u_tel_extension; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int i_locale
        {
            set { _i_locale = value; }
            get { return _i_locale; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string u_user_id_changed_by
        {
            set { _u_user_id_changed_by = value; }
            get { return _u_user_id_changed_by; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime dt_date_last_changed
        {
            set { _dt_date_last_changed = value; }
            get { return _dt_date_last_changed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime dt_csadapter_date_last_changed
        {
            set { _dt_csadapter_date_last_changed = value; }
            get { return _dt_csadapter_date_last_changed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime dt_date_created
        {
            set { _dt_date_created = value; }
            get { return _dt_date_created; }
        }
        #endregion Model

    }
}