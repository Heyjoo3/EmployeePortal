using EmployeePortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;


namespace EmployeePortal.Services
{
    public class VacationStatusEmailService : IVacationStatusEmailService
    // responsible to send email notifications to the employee, supervisor, and substitute when they need to take action on a vacation request
    {
        private readonly SmtpMailService _mailService;
        private readonly IcsEventService _icsEventService;
        private readonly IEmployeeRepository _employeeRepository;

        public VacationStatusEmailService(SmtpMailService mailService, IcsEventService icsEventService, IEmployeeRepository employeeRepository)
        {
            _mailService = mailService;
            _employeeRepository = employeeRepository;
            _icsEventService = icsEventService;
        }

        public async Task SendEmailNotification(VacationDto vacation, string processStage)
        {
            List<(string ReceiverId, int Message)> receiverMessageList = AssignEmailContentVacationStatus(vacation.Supervisor, vacation.Substitute, vacation.EmployeeId.ToString(), vacation.Status, vacation.SubstituteStatus, vacation.SupervisorStatus, processStage);

            string requesterName = await _employeeRepository.GetFullNameById(vacation.EmployeeId.ToString());

            for (int i = 0; i < receiverMessageList.Count; i++)
            {
                (string, string) ics = (null, null);
                string receiverName = await _employeeRepository.GetFullNameById(receiverMessageList[i].ReceiverId);
                string receiverEmail = await _employeeRepository.GetEmailById(receiverMessageList[i].ReceiverId);
                string message = "Hallo " + receiverName + ","+ Environment.NewLine + GetFormattedMessage(receiverMessageList[i].Message, vacation.StartDate, vacation.EndDate, requesterName);
                if (receiverMessageList[i].Message == 1)
                {
                     ics = _icsEventService.GenerateICSFile("Urlaub", "", vacation.StartDate, vacation.EndDate);
                    //_mailService.SendMail("noreply@contechnet.de", receiverEmail, "Urlaubsantrag genehmigt", message, ics.Item1, ics.Item2);
                    System.Console.WriteLine("Mail gesendet1: " + receiverName, receiverEmail, message, ics.Item1, ics.Item2 );
                    return; 
                }

                if (receiverMessageList[i].Message == 2)
                {
                    System.Console.WriteLine("Mail gesendet2: " + receiverName, receiverEmail, message);
                    //_mailService.SendMail("noreply@contechnet.de", receiverEmail, "Urlaubsantrag abgelehnt", message, null, null);
                    return;
                }

                System.Console.WriteLine("Mail gesendet3: " + receiverName, receiverEmail, message);
                //_mailService.SendMail("noreply@contechnet.de", receiverEmail, "Urlaubsantrag braucht Begutachtung", message,  null, null );

            }
        }

        private static readonly Dictionary<int, string> MessageMap = new Dictionary<int, string>
        {
            { 1, "Dein Urlaubsantrag vom {0} bis {1} wurde angenommen." },
            { 2, "Dein Urlaubsantrag vom {0} bis {1} wurde abgelehnt." },
            { 3, "Für deinen Urlaubsantrag vom {0} bis {1} wurde eine Stornierung beantragt. Bitte akzeptiere dies im Portal." },
            { 4, "Dein Urlaubsantrag vom {0} bis {1} wurde endgültig storniert." },
            { 5, "Die Stornierung des Urlaubsantrags vom {0} bis {1} wurde abgelehnt." },
            { 6, "Ein Urlaubsantrag vom {0} bis {1} wurde von {2} erstellt. Bitte bearbeite den Antrag im Portal." },
            { 7, "Der Urlaub von {2} vom {0} bis {1} wurde bearbeitet. Bitte bearbeite den Antrag im Portal." },
            { 8, "Der Urlaub von {2} vom {0} bis {1} wurde abgelehnt." },
            { 9, "Für den Urlaubsantrag von {2} vom {0} bis {1} wurde eine Stornierung bearbeitet. Bitte bearbeite den Antrag im Portal." },
            { 10, "Die Stornierung des Urlaubs von {2} vom {0} bis {1} wurde abgelehnt." }
        };

        public static string GetFormattedMessage(int messageId, DateTime startDate, DateTime endDate, string requesterName)
        {
            if (MessageMap.TryGetValue(messageId, out string messageTemplate))
            {
                return string.Format(messageTemplate, startDate.ToString("dd.MM.yyyy"), endDate.ToString("dd.MM.yyyy"), requesterName);
            }
            return "Unbekannte Nachricht";
        }
      
        private List<(string ReceiverId, int Message)> AssignEmailContentVacationStatus(string supervisorId, string substituteId, string employeeId, VacationStatus vacationStatus, VacationStatus? substituteStatus, VacationStatus? supervisorStatus, string processStage)
        {
            List<(string ReceiverId, int Message)> receiverMessageList = new List<(string, int)>();

            if (processStage == "createVacation")
            {
                if (substituteId == null)
                {
                    receiverMessageList.Add((supervisorId, 6));
                }
                else
                {
                    receiverMessageList.Add((substituteId, 6));
                }
            }
            else if (processStage == "acceptVacation")
            {
                if (vacationStatus == VacationStatus.VacationApproved)
                {
                    receiverMessageList.Add((employeeId, 1));
                }
                else if (vacationStatus == VacationStatus.VacationRejected)
                {
                    if (substituteId != null)
                    {
                        receiverMessageList.Add((substituteId, 8));
                    }
                    receiverMessageList.Add((supervisorId, 8));
                    receiverMessageList.Add((employeeId, 2));
                }
                else if (vacationStatus == VacationStatus.VacationPending)
                {
                    if (substituteId != null && substituteStatus == VacationStatus.VacationPending)
                    {
                        receiverMessageList.Add((substituteId, 6));
                    }
                    else if ((substituteId == null || substituteStatus == VacationStatus.VacationApproved) && supervisorStatus == VacationStatus.VacationPending)
                    {
                        receiverMessageList.Add((supervisorId, 6));
                    }
                }
            }
            else if (processStage == "cancelVacation")
            {
                if (vacationStatus == VacationStatus.CancellationApproved)
                {
                    receiverMessageList.Add((employeeId, 4));
                }
                else if (vacationStatus == VacationStatus.CancellationRejected)
                {
                    if (substituteId != null)
                    {
                        receiverMessageList.Add((substituteId, 10));
                    }
                    receiverMessageList.Add((supervisorId, 10));
                    receiverMessageList.Add((employeeId, 5));
                }
                else if (vacationStatus == VacationStatus.VacationPending)
                {
                    if (substituteStatus != VacationStatus.CancellationApproved)
                    {
                        receiverMessageList.Add((substituteId, 9));
                    }
                    if (supervisorStatus != VacationStatus.CancellationApproved)
                    {
                        receiverMessageList.Add((supervisorId, 9));
                    }
                    receiverMessageList.Add((employeeId, 3));
                }
            }

            return receiverMessageList;
        }
    }
}
