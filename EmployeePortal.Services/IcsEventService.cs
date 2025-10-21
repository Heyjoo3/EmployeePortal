using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Services
{
    public class IcsEventService
    {
        public (string IcsContent, string FilePath) GenerateICSFile(string summary, string description, DateTime startTime, DateTime endTime)
        {
            string icsContent = GenerateICS(summary, description, startTime, endTime); // time must be in UTC
            string filePath = Path.Combine(Path.GetTempPath(), "Urlaub.ics");
            File.WriteAllText(filePath, icsContent, Encoding.UTF8);

            Console.WriteLine($"ICS file successfully created: {filePath}");

            return (icsContent, filePath);
        }
        
        private static string GenerateICS(string summary, string description, DateTime startDate, DateTime endDate)
        {
            string uid = Guid.NewGuid().ToString();
            string dtStamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            string dtStart = startDate.ToString("yyyyMMdd");
            string dtEnd = endDate.AddDays(1).ToString("yyyyMMdd");

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//MyCalendar//EN");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{uid}");
            sb.AppendLine($"DTSTAMP:{dtStamp}");
            sb.AppendLine($"DTSTART;VALUE=DATE:{dtStart}");
            sb.AppendLine($"DTEND;VALUE=DATE:{dtEnd}");
            sb.AppendLine($"SUMMARY:{summary}");
            sb.AppendLine($"DESCRIPTION:{description}");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }

    }
}
