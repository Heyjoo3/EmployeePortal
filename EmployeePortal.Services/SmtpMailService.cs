using EmployeePortal.Core.Models;
using EmployeePortal.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Services
{
    public class SmtpMailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly ILogger<SmtpMailService> _logger;

        public SmtpMailService(ILogger<SmtpMailService> logger)
        {
            _logger = logger;
            _smtpClient = new SmtpClient("contechnet-de.mail.protection.outlook.com", 25)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
            };
        }

        public void SendMail(string fromEmail, string toEmail, string subject, string body, string icsContent = null, string filePath = null)
        {
            try
            {
                using (MailMessage mail = CreateMailMessage(fromEmail, toEmail, subject, body, icsContent, filePath))
                {
                    //_smtpClient.Send(mail);
                }

                Console.WriteLine("E-Mail erfolgreich gesendet.");
                _logger.LogInformation("E-Mail erfolgreich gesendet.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Senden der E-Mail: {ex.Message}");
                _logger.LogError(ex, "Fehler beim Senden der E-Mail.");
            }
        }

        static MailMessage CreateMailMessage(string fromEmail, string toEmail, string subject, string body, string icsContent = null, string filePath = null)
        {
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                To = { toEmail },
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            if (!string.IsNullOrEmpty(icsContent) && !string.IsNullOrEmpty(filePath))
            {
                Attachment attachment = new Attachment(filePath);
                mail.Attachments.Add(attachment);
            }

            return mail;
        }
    }
}
