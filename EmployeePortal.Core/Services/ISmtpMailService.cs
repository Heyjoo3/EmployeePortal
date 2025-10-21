using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Services
{
    public interface ISmtpMailService
    {
        void SendMail(string fromEmail, string toEmail, string subject, string body, string icsContent = null, string filePath = null);
    }
}
