using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class tbl_UserObjectModel
    {
        #region UserObject
        public string u_user_id { get; set; }

        public string u_org_id { get; set; }

        public string u_user_type { get; set; }

        public string u_first_name { get; set; }

        public string u_last_name { get; set; }

        public string u_email_address { get; set; }

        public string u_preferred_address { get; set; }

        public string u_addresses { get; set; }

        public string u_preferred_credit_card { get; set; }

        public string u_credit_cards { get; set; }

        public string u_tel_number { get; set; }

        public string u_tel_extension { get; set; }

        public string u_fax_number { get; set; }

        public string u_fax_extension { get; set; }

        public string u_user_security_password { get; set; }

        public string u_user_id_changed_by { get; set; }

        public string u_account_status { get; set; }

        public string u_user_catalog_set { get; set; }

        public DateTime dt_date_registered { get; set; }

        public string u_campaign_history { get; set; }

        public DateTime dt_date_last_changed { get; set; }

        public DateTime dt_csadapter_date_last_changed { get; set; }

        public DateTime dt_date_created { get; set; }

        public string u_language { get; set; }

        public string u_currency { get; set; }

        public string u_msn { get; set; }

        public string u_qq { get; set; }

        public string u_gmail { get; set; }

        public string u_yahoo { get; set; }

        public string u_Pref1 { get; set; }

        public string u_Pref2 { get; set; }

        public string u_Pref3 { get; set; }

        public string u_Pref4 { get; set; }

        public string u_Pref5 { get; set; }

        public string u_password_question { get; set; }

        public string u_password_answer { get; set; }

        public string u_logon_error_dates { get; set; }

        public string u_password_answer_error_dates { get; set; }

        public int i_keyindex { get; set; }

        public int i_access_level_id { get; set; }

        public bool b_change_password { get; set; }

        public DateTime dt_date_last_password_changed { get; set; }

        public DateTime dt_last_logon { get; set; }

        public DateTime dt_last_lockedout_date { get; set; }

        public string u_application_name { get; set; }

        public DateTime dt_last_activity_date { get; set; }

        public bool b_express_checkout { get; set; }

        public DateTime dt_date_address_list_last_changed { get; set; }

        public DateTime dt_date_credit_card_list_last_changed { get; set; }

        public string u_mobile_number { get; set; }

        public int i_dataid { get; set; }

        public DateTime dt_birthday { get; set; }

        public int i_gender { get; set; }

        public bool b_interest { get; set; }

        public int i_point { get; set; }

        public string u_attitude { get; set; }

        public string u_remark { get; set; }

        public string u_Imagefile { get; set; }

        public string u_date_BlackListEndTime { get; set; }

        public string u_know_channel { get; set; }

        public string Category { get; set; }
        #endregion
    }
}



      
