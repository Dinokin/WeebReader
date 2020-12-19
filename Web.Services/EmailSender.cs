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
        private readonly SmtpClient _smtpClient;

        public EmailSender(ParametersManager parameterManager)
        {
            _parameterManager = parameterManager;
            _smtpClient = new SmtpClient {ServerCertificateValidationCallback = (_, _, _, _) => true};
        }

        public async void SendEmail(string replyTo, string destination, string subject, string content)
        {
            if (!await _parameterManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled))
                return;

            if (!await Connect())
                return;
            
            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(await _parameterManager.GetValue<string>(ParameterTypes.SiteEmail)));
            message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
            message.To.Add(MailboxAddress.Parse(destination));
            message.Subject = subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = content
            }.ToMessageBody();
            
            await _smtpClient.SendAsync(message);
        }

        private async Task<bool> Connect()
        {
            if (!_smtpClient.IsConnected)
                await _smtpClient.ConnectAsync(await _parameterManager.GetValue<string>(ParameterTypes.SmtpServer), await _parameterManager.GetValue<int>(ParameterTypes.SmtpServerPort));

            if (!_smtpClient.IsAuthenticated)
                await _smtpClient.AuthenticateAsync(await _parameterManager.GetValue<string>(ParameterTypes.SmtpServerUser), await _parameterManager.GetValue<string>(ParameterTypes.SmtpServerPassword));

            return _smtpClient.IsConnected && _smtpClient.IsAuthenticated;
        }
    }
}