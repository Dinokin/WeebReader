using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Web.Portal.Services
{
    public class EmailSender
    {
        private readonly BaseContext _context;

        public EmailSender(BaseContext baseContext) => _context = baseContext;

        public async Task<bool> SendEmail(string origin, string destination, string subject, string content)
        {
            if (bool.Parse((await _context.Settings.SingleAsync(setting => setting.Key == "EmailEnabled")).Value))
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

                    Setting server = await _context.Settings.SingleAsync(setting => setting.Key == "SmtpServer"),
                        port = await _context.Settings.SingleAsync(setting => setting.Key == "SmtpServerPort"),
                        user = await _context.Settings.SingleAsync(setting => setting.Key == "SmtpServerUser"),
                        password = await _context.Settings.SingleAsync(setting => setting.Key == "SmtpServerPassword");
                    
                    using var client = new SmtpClient {ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true};
                    await client.ConnectAsync(server.Value, int.Parse(port.Value));
                    await client.AuthenticateAsync(user.Value, password.Value);
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