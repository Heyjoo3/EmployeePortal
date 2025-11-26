using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{
    public class NetworkingTask : BaseTask
    {
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public string? ExampleQuestions { get; set; }

        public override async void UpdateTask(TaskDto taskDto)
        {
            if (taskDto == null)
            {
                throw new ArgumentNullException(nameof(taskDto));
            }

            Title = taskDto.Title;
            Status = taskDto.Status;
            TaskType = taskDto.TaskType ?? TaskType;
            ReferencePerson = await EmployeeHelper.ValidateReferencePersonAsync(taskDto.ReferencePerson);

            Location = taskDto.Location;
            StartDate = taskDto.StartDate;
            ExampleQuestions = taskDto.ExampleQuestions;
        }
    }
}
