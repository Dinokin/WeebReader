using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using WeebReader.Data.Services;
using Parameter = WeebReader.Data.Entities.Parameter;

namespace WeebReader.Web.Services
{
    public class EmailSender
    {
        private readonly ParametersManager _parameterManager;

        public EmailSender(ParametersManager parameterManager) => _parameterManager = parameterManager;

        public async Task<bool> SendEmail(string replyTo, string destination, string subject, string content)
        {
            if (await _parameterManager.GetValue<bool>(Parameter.Types.EmailSenderEnabled))
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(await _parameterManager.GetValue<string>(Parameter.Types.SiteEmail)));
                message.ReplyTo.Add(new MailboxAddress(replyTo));
                message.To.Add(new MailboxAddress(destination));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Plain)
                {
                    Text = content
                };

                string server = await _parameterManager.GetValue<string>(Parameter.Types.SmtpServer),
                    user = await _parameterManager.GetValue<string>(Parameter.Types.SmtpServerUser),
                    password = await _parameterManager.GetValue<string>(Parameter.Types.SmtpServerPassword);
                var port = await _parameterManager.GetValue<int>(Parameter.Types.SmtpServerPort);
                    
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