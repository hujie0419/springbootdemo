using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     ������ �����û�������ʷ
    /// </summary>
    public class SearchQuestionAnswerHistoryRequest
    {
        /// <summary>
        ///     �û�ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     �ID
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///    true = ֻ��ʾ�û��Լ������ݣ�������ʾ��Ŀ����
        /// </summary>
        [Obsolete("û��������ֶ�")]
        public bool? NoShowLottery { get; set; }

        /// <summary>
        ///     0 Ĭ��
        ///     1 ֻ��ʾ �û����Ⲣ���Ѿ������Ĵ� ����Ŀ
        /// </summary>
        public int ShowFlag { get; set; }

        /// <summary>
        ///     ҳ��
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        ///     ��ҳ��С
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
