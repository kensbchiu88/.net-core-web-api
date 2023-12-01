using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using PolarBearEapApi.ApplicationCore.Interfaces;
using System.Diagnostics;

namespace PolarBearEapApi.Infra.Services
{
    /** Email Service */
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _host;
        private readonly int _port;
        private readonly string _sender;
        private readonly string _username;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _host = _configuration["SmtpSettings:Host"];
            if (string.IsNullOrEmpty(_configuration["SmtpSettings:Port"]))
            {
                _port = 25;
            }
            else
            {
                _port = int.Parse(_configuration["SmtpSettings:Port"]!);
            }

            _sender = _configuration["SmtpSettings:Sender"];
            _username = _configuration["SmtpSettings:Username"];
            _password = _configuration["SmtpSettings:Password"];

            Debug.WriteLine("--------");
            Debug.WriteLine($"{_host}:{_port} {_sender} {_username} {_password}");
        }
        public async Task Send(string recipients, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_sender));
            email.To.Add(MailboxAddress.Parse(recipients));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>{body}</h1>" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_host, _port, SecureSocketOptions.None);

            if (!string.IsNullOrEmpty(_username) || !string.IsNullOrEmpty(_password))
            {
                smtp.Authenticate(_username, _password);
            }
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
