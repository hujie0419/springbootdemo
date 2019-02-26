namespace Tuhu.Service.Activity.Server.Model
{
    /// <summary>
    ///     公众号领红包 - 缓存对象
    /// </summary>
    public class OARedEnvelopeObjectModel
    {
        /// <summary>
        ///     一个红包的金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        ///     红包的位置
        /// </summary>
        public int Position { get; set; }
    }
}
