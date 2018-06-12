namespace Tuhu.Provisioning.Business
{
    public static class BizErrorCode
    {
        #region Order

        /// <summary>
        /// The order does NOT exist.
        /// </summary>
        public const int OrderNotExist = 0x05;

        /// <summary>
        /// The money is NOT consistent with summoney in order.
        /// </summary>
        public const int InConsistentMoney = 0x07;

        /// <summary>
        /// The Order has been paid.
        /// </summary>
        public const int AccountPaid = 0x0D;

        /// <summary>
        /// Order payment has canceled.
        /// </summary>
        public const int PaymentCanceled = 0x0E;

        /// <summary>
        /// Unknown system error.
        /// </summary>
        public const int SystemError = 0xFF;

        #endregion
    }
}
