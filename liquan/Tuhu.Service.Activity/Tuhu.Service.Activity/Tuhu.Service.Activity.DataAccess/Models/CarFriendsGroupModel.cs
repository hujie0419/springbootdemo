using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    /// <summary>
    /// ����Ⱥ
    /// </summary>
    public class CarFriendsGroupModel
    {
        /// <summary>
        /// ����
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// ����Ⱥ����
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// ����Ⱥ����
        /// </summary>
        public string GroupDesc { get; set; }

        /// <summary>
        /// �󶨳���
        /// </summary>
        public string BindVehicleType { get; set; }

        /// <summary>
        /// �󶨳���ID
        /// </summary>
        public int BindVehicleTypeID { get; set; }

        /// <summary>
        /// Ⱥͷ��
        /// </summary>
        public string GroupHeadPortrait { get; set; }

        /// <summary>
        /// Ⱥ��ά��
        /// </summary>
        public string GroupQRCode { get; set; }

        /// <summary>
        /// Ⱥ���0=����Ⱥ��1=������Ⱥ��2=����Ⱥ��3=����Ⱥ��4=ƴ��Ⱥ
        /// </summary>
        public int GroupCategory { get; set; }

        /// <summary>
        /// ȺȨ�أ�����ԽС��Ȩ��Խ�ߣ�
        /// </summary>
        public int GroupWeight { get; set; }

        /// <summary>
        /// �Ƿ��Ƽ�
        /// </summary>
        public bool IsRecommend { get; set; }

        /// <summary>
        /// �Ƿ�ɾ��
        /// </summary>
        public bool Is_Deleted { get; set; }

        /// <summary>
        /// Ⱥ����ʱ��
        /// </summary>
        public DateTime GroupCreateTime { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime GroupOverdueTime { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// �޸���
        /// </summary>
        public string LastUpdateBy { get; set; }
    }
}
