namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 修改CPS流水记录返回实体
    /// </summary>
    public class CpsUpdateRunningResponse
    {
        /// <summary>
        /// true:操作成功; false:操作失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 失败消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
