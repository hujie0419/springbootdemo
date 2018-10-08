namespace Tuhu.Provisioning.Models.Push
{
    public class ResponseModel
    {
        /// <summary>
        /// 成功与否
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 数据对象
        /// </summary>
        public object ObjectData { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string OutMessage { get; set; }
    }
}