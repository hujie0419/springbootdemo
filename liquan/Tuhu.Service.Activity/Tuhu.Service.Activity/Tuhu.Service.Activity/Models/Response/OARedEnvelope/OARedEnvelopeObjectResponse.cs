namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     公众号领红包 生成的红包对象
    /// </summary>
    public class OARedEnvelopeObjectResponse
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
