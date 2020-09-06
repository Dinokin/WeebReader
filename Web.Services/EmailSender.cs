using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using WeebReader.Data.Services;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Extensions;

namespace WeebReader.Web.Services
{
    public class EmailSender
    {
        private readonly ParametersManager _parameterManager;

        public EmailSender(ParametersManager parameterManager) => _parameterManager = parameterManager;

        public async Task<bool> SendEmail(string replyTo, string destination, string subject, string content)
        {
            if (await _parameterManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled))
            {
                var message = new MimeMessage();
                
                message.From.Add(MailboxAddress.Parse(await _parameterManager.GetValue<string>(ParameterTypes.SiteEmail)));
                message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
                message.To.Add(MailboxAddress.Parse(destination));
                message.Subject = subject;
                message.Body = new BodyBuilder
                {
                    HtmlBody = content
                }.ToMessageBody();

                string server = await _parameterManager.GetValue<string>(ParameterTypes.SmtpServer),
                    user = await _parameterManager.GetValue<string>(ParameterTypes.SmtpServerUser),
                    password = await _parameterManager.GetValue<string>(ParameterTypes.SmtpServerPassword);
                var port = await _parameterManager.GetValue<int>(ParameterTypes.SmtpServerPort);
                    
                using var client = new SmtpClient {ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true};
                await client.ConnectAsync(server, port);
                await client.AuthenticateAsync(user, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }

            return false;
        }
    }
}