using EmployeePortal.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Services
{
    public interface IVacationStatusEmailService
    {
        Task SendEmailNotification(VacationDto vacation, string processStage);
    }
}
