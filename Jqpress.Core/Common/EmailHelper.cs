using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Threading;
using Jqpress.Core.Configuration;


namespace Jqpress.Core.Common
{
    /// <summary>
    /// 发邮件
    /// 说明:
    /// FromMail:同域的任何邮箱
    /// UserName:可能不需要@gmail.com
    /// </summary>
    public class EmailHelper
    {

        /// <summary>
        ///  发邮件
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void Send(string recipients, string subject, string body)
        {
            try
            {
                MailAddress from = new MailAddress(SiteConfig.GetSetting().SmtpEmail, SiteConfig.GetSetting().SiteName);
                MailMessage m = new MailMessage();
                m.From = from;
                //   m.ReplyTo = new MailAddress(recipients);

                m.To.Add(recipients);   //收件人处会显示所有人的邮箱

                //if (recipients.IndexOf(',') == -1)
                //{
                //    m.To.Add(recipients);   //收件人处会显示所有人的邮箱
                //}
                //else
                //{
                //    m.Bcc.Add(recipients);  //不会显示收件人

                //    //  m.CC.Add(recipients);  //抄送人处会显示所有人邮箱
                //}

                m.Subject = subject;
                m.Body = body;
                m.IsBodyHtml = true;
                m.Priority = MailPriority.Normal;
                m.BodyEncoding = Encoding.GetEncoding("utf-8");

                SmtpClient smtp = new SmtpClient();
                smtp.Host = SiteConfig.GetSetting().SmtpServer;
                smtp.Credentials = new System.Net.NetworkCredential(SiteConfig.GetSetting().SmtpUserName, SiteConfig.GetSetting().SmtpPassword);
                smtp.EnableSsl = Convert.ToBoolean(SiteConfig.GetSetting().SmtpEnableSsl);
                smtp.Port = SiteConfig.GetSetting().SmtpServerPost;
                smtp.Send(m);
            }
            catch
            { }
        }

        /// <summary>
        /// 异步发邮件,如果超过一百封,仅发前一百封
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendAsync(string recipients, string subject, string body)
        {
            string[] emails = recipients.Split(',');
            int i = 0;
            foreach (string email in emails)
            {
                i++;
                if (i > 100)
                {
                    break;
                }
                string temp = email;
                ThreadPool.QueueUserWorkItem(delegate { Send(temp, subject, body); });
            }
        }
    }
}
