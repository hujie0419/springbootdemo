using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.SendEmail.Comm
{
    /// <summary>
    ///     邮件发送类
    /// </summary>
    public class SendEmailHelper
    {
        private readonly MailMessage _mailMessage = new MailMessage(); //实例化一个邮件类  

        #region 构造函数  

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="toAddresses">收件人地址（多个以,号分开）</param>
        /// <param name="fromAddress">发件人地址</param>
        /// <param name="title">主题</param>
        /// <param name="body">正文</param>
        public SendEmailHelper(string toAddresses, string fromAddress, string title, string body)
            : this(toAddresses, fromAddress, "", "", title, body, false)
        {
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="toAddress">收件人地址</param>
        /// <param name="fromAddress">发件人地址</param>
        /// <param name="toName">收件人名字</param>
        /// <param name="fromName">发件人姓名</param>
        /// <param name="title">主题</param>
        /// <param name="body">正文</param>
        /// <param name="isBodyHtml">正文是否为html格式</param>
        public SendEmailHelper(string toAddress, string fromAddress, string toName, string fromName, string title,
            string body, bool isBodyHtml)
        {
            _mailMessage.From = new MailAddress(fromAddress, fromName, Encoding.GetEncoding(936));
            if (toName.Equals(""))
                _mailMessage.To.Add(toAddress);
            else
                _mailMessage.To.Add(new MailAddress(toAddress, toName, Encoding.GetEncoding(936)));

            _mailMessage.Subject = title;
            _mailMessage.SubjectEncoding = Encoding.GetEncoding(936);

            _mailMessage.Body = body;
            _mailMessage.IsBodyHtml = isBodyHtml;
            _mailMessage.BodyEncoding = Encoding.GetEncoding(936);
        }

        #endregion

        /// <summary>
        ///     设置SMTP，并且将邮件发送出去
        ///     所有参数都设置完成后再调用该方法
        /// </summary>
        /// <param name="password">发件人密码</param>
        /// <param name="smtpHost">SMTP服务器地址</param>
        public void SetSmtp(string password, string smtpHost)
        {
            SetSmtp(_mailMessage.From.Address, password, smtpHost, 25, false, MailPriority.Normal);
        }

        /// <summary>
        /// 设置SMTP，并且将邮件发送出去
        /// 所有参数都设置完成后再调用该方法
        /// </summary>
        /// <param name="address">发件人地址（必须为真实有效的email地址）</param>
        /// <param name="password">发件人密码</param>
        /// <param name="smtpHost">SMTP服务器地址</param>
        /// <param name="smtpPort">SMTP服务器的端口</param>
        /// <param name="isEnableSsl">SMTP服务器是否启用SSL加密</param>
        /// <param name="priority">邮件的优先级</param>
        public void SetSmtp(string address, string password, string smtpHost, int smtpPort, bool isEnableSsl = false,
            MailPriority priority = MailPriority.Normal)
        {
            var smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(address, password);
            smtp.Host = smtpHost;
            smtp.Port = smtpPort;
            smtp.EnableSsl = isEnableSsl;

            _mailMessage.Priority = priority;
            smtp.Send(_mailMessage); //发送邮件  
        }

        #region //设置邮件地址  

        /// <summary>
        ///     设置更多收件人
        /// </summary>
        /// <param name="toAddresses">收件人地址</param>
        public void SetMoreToAddress(string toAddresses)
        {
            _mailMessage.To.Add(toAddresses);
        }

        /// <summary>
        ///     设置更多收件人
        /// </summary>
        /// <param name="toAddress">收件人地址</param>
        /// <param name="toName">收件人名字</param>
        public void SetMoreToAddress(string toAddress, string toName)
        {
            _mailMessage.To.Add(new MailAddress(toAddress, toName, Encoding.GetEncoding(936)));
        }

        /// <summary>
        ///     设置抄送者（多个以,号分开）
        /// </summary>
        /// <param name="ccAddresses">抄送者地址</param>
        public void SetCarbonCopyFor(string ccAddresses)
        {
            _mailMessage.CC.Add(ccAddresses);
        }

        /// <summary>
        ///     设置抄送者
        /// </summary>
        /// <param name="ccAddress">抄送者地址</param>
        /// <param name="ccName">抄送者名字</param>
        public void SetCarbonCopyFor(string ccAddress, string ccName)
        {
            _mailMessage.Bcc.Add(new MailAddress(ccAddress, ccName, Encoding.GetEncoding(936)));
        }

        /// <summary>
        ///     设置密送者（多个以,号分开）
        /// </summary>
        /// <param name="bccAddresses">密送者</param>
        public void SetBlindCarbonCopyFor(string bccAddresses)
        {
            _mailMessage.Bcc.Add(bccAddresses);
        }

        /// <summary>
        ///     设置密送者
        /// </summary>
        /// <param name="bccAddress">密送者</param>
        /// <param name="bccName">密送者名字</param>
        public void SetBlindCarbonCopyFor(string bccAddress, string bccName)
        {
            _mailMessage.Bcc.Add(new MailAddress(bccAddress, bccName, Encoding.GetEncoding(936)));
        }

        #endregion

        #region 添加附件  

        /// <summary>
        ///     添加附件（自动识别文件类型）
        /// </summary>
        /// <param name="fileName">单个文件的路径</param>
        public void Attachments(string fileName)
        {
            _mailMessage.Attachments.Add(new Attachment(fileName));
        }

        /// <summary>
        ///     添加附件（默认为富文本RTF格式）
        /// </summary>
        /// <param name="fileName">单个文件的路径</param>
        public void AttachmentsForRtf(string fileName)
        {
            _mailMessage.Attachments.Add(new Attachment(fileName, MediaTypeNames.Application.Rtf));
        }

        #endregion
    }
}