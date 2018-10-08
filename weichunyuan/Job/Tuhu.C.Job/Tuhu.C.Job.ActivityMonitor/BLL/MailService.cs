using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push.WeiXinWork;

namespace Tuhu.C.Job.ActivityMonitor.BLL
{
    public class MailService
    {
        public static void SendMail(string subject, string to, Attachment attachment)
        {
            using (var smtp = new SmtpClient())
            using (var mail = new MailMessage())
            {
                mail.Subject = subject;

                foreach (var a in to.Split(';'))
                {
                    mail.To.Add(a);
                }

                mail.Attachments.Add(attachment);

                smtp.Send(mail);
            }
        }

        public static void SendMail(string subject, string to,string body)
        {
            using (var smtp = new SmtpClient())
            using (var mail = new MailMessage())
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                foreach (var a in to.Split(';'))
                {
                    mail.To.Add(a);
                }              
                smtp.Send(mail);
            }
        }

        public static  void SendWxFileMessage(string users,string message)
        {
           
                using (var client = new Tuhu.Service.Push.WeiXinWorkClient())
                {
                TextMessage messageinfo = new TextMessage()
                {
                    Content = message,
                    Users = users.Replace("@tuhu.cn","").Split(';')
                };
                var result = client.SendText(messageinfo);
                result.ThrowIfException(true);


                }
           
        }

        public static void AddFileToZip(string zipFilename, List<string> filesToAdd, CompressionOption compression = CompressionOption.Normal)
        {
            using (Package zip = Package.Open(zipFilename, FileMode.OpenOrCreate))
            {
                foreach (var file in filesToAdd)
                {
                    string destFilename = file;
                    Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                    if (zip.PartExists(uri))
                    {
                        zip.DeletePart(uri);
                    }

                    PackagePart part = zip.CreatePart(uri, "", compression);
                    using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream dest = part.GetStream())
                        {
                            fileStream.CopyTo(dest);
                        }
                    }
                }

            }
        }
    }
}
