using Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PJW.Web
{
    public class SendEmail
    {
        public IConfiguration configuration;
        private string smtpServer;
        private int smtpPort;
        private string mailAccount;
        private string mailPwd;
        //private string mailFrom;
        //private string mailTo;

        /// <summary>
        /// 季送信件
        /// </summary>
        /// <param name="message">主旨與內容</param>
        /// <param name="mailTo">收件者(可用 ; 分隔)</param>
        /// <param name="mailFrom">寄件者(預設空值會用系統的)</param>
        /// <returns></returns>
        public async Task SendAsync(MimeMessage message, string mailTo, string mailFrom = "")
        {
           
            smtpServer = ConfigHelper.GetConfig<string>("SMTP:smtpServer");
            smtpPort = ConfigHelper.GetConfig<int>("SMTP:smtpPort");
            mailAccount = ConfigHelper.GetConfig<string>("SMTP:mailAccount");
            mailPwd = ConfigHelper.GetConfig<string>("SMTP:mailPwd");
            mailFrom = string.IsNullOrWhiteSpace(mailFrom) ? ConfigHelper.GetConfig<string>("SMTP:mailFrom") : mailFrom;
            mailTo = string.IsNullOrWhiteSpace(mailTo) ? ConfigHelper.GetConfig<string>("SMTP:mailTo") : mailTo;

            string host = smtpServer;
            int port = smtpPort;
            bool useSsl = false;
            string from_username = mailAccount;
            string from_password = mailPwd;
            string from_name = mailFrom;
            string from_address = mailFrom;

            string[] mailToList = mailTo.Split(';');

            List<MailboxAddress> address = new List<MailboxAddress>();
            foreach (string mt in mailToList) {
                address.Add(new MailboxAddress(mt, mt));
            }

            message.From.Add(new MailboxAddress(from_name, from_address));
            message.To.AddRange(address);

            using var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            await client.ConnectAsync(host, port, useSsl);
            await client.AuthenticateAsync(from_username, from_password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
