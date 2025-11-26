using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;

namespace EmployeePortal.Core.Helpers
{
    public static class TaskHelper
    {
        public static BaseTask CreateTaskFromDto(TaskDto taskDto)
        {
            if (taskDto.TaskType == null)
            {
                throw new ArgumentException("TaskType cannot be null");
            }

            return taskDto.TaskType switch
            {
                nameof(TodoTask) => new TodoTask
                {
                    Id = Guid.NewGuid(),
                    Title = taskDto.Title,
                    Status = taskDto.Status,
                    Description = taskDto.Description
                },
                nameof(NetworkingTask) => new NetworkingTask
                {
                    Id = Guid.NewGuid(),
                    Title = taskDto.Title,
                    Status = taskDto.Status,
                    Location = taskDto.Location,
                    StartDate = taskDto.StartDate,
                    ExampleQuestions = taskDto.ExampleQuestions
                },
                nameof(ProjectTask) => new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    Title = taskDto.Title,
                    Status = taskDto.Status,
                    Description = taskDto.Description,
                    Priority = taskDto.Priority ?? Priority.Low,
                    Tools = taskDto.Tools,
                    StartDate = taskDto.StartDate,
                    EndDate = taskDto.EndDate
                },
                nameof(TimeScheduledTask) => new TimeScheduledTask
                {
                    Id = Guid.NewGuid(),
                    Title = taskDto.Title,
                    Status = taskDto.Status,
                    Description = taskDto.Description,
                    StartDate = taskDto.StartDate,
                    EndDate = taskDto.EndDate
                },
                _ => throw new NotSupportedException($"TaskType '{taskDto.TaskType}' is not supported")
            };
        }
    }
}