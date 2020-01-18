using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using WeebReader.Data.Services;

namespace WeebReader.Web.Services
{
    public class EmailSender
    {
        private readonly SettingManager _settingManager;

        public EmailSender(SettingManager settingManager) => _settingManager = settingManager;

        public async Task<bool> SendEmail(string origin, string destination, string subject, string content)
        {
            if (await _settingManager.GetValue<bool>("EmailEnabled"))
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(origin));
                    message.ReplyTo.Add(new MailboxAddress(origin));
                    message.To.Add(new MailboxAddress(destination));
                    message.Subject = subject;
                    message.Body = new TextPart(TextFormat.Plain)
                    {
                        Text = content
                    };

                    string server = await _settingManager.GetValue("SmtpServer"),
                        port = await _settingManager.GetValue("SmtpServerPort"),
                        user = await _settingManager.GetValue("SmtpServerUser"),
                        password = await _settingManager.GetValue("SmtpServerPassword");
                    
                    using var client = new SmtpClient {ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true};
                    await client.ConnectAsync(server, int.Parse(port));
                    await client.AuthenticateAsync(user, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    return true;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            return false;
        }
    }
}