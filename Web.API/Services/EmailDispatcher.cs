using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using WeebReader.Web.API.Settings;

namespace WeebReader.Web.API.Services
{
    public class EmailDispatcher
    {
        private readonly Configuration _configuration;
        private readonly SmtpClient _smtpClient;
        private readonly MailboxAddress _senderEmail;

        public EmailDispatcher(Configuration configuration, SmtpClient smtpClient)
        {
            _configuration = configuration;
            _smtpClient = new() {ServerCertificateValidationCallback = (_, _, _, _) => true};

            _senderEmail = new(_configuration.Email.SenderName, _configuration.Email.SenderEmailAddress);
        }

        public async void SendEmail(string destination, string subject, string content)
        {
            if (!_configuration.Email.Enabled)
                return;

            if (!await Connect())
                return;

            var message = new MimeMessage();
            
            message.From.Add(_senderEmail);
            message.ReplyTo.Add(_senderEmail);
            message.To.Add(MailboxAddress.Parse(destination));
            message.Subject = subject;
            message.Body = new BodyBuilder {HtmlBody = content}.ToMessageBody();

            await _smtpClient.SendAsync(message);
        }

        private async Task<bool> Connect()
        {
            if (!_smtpClient.IsConnected)
                await _smtpClient.ConnectAsync(_configuration.Email.SmtpServerAddress, _configuration.Email.SmtpServerPort);

            if (!_smtpClient.IsAuthenticated)
                await _smtpClient.AuthenticateAsync(_configuration.Email.SmtpServerUser, _configuration.Email.SmtpServerPassword);

            return _smtpClient.IsConnected && _smtpClient.IsAuthenticated;
        }
    }
}