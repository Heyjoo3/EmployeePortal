using AutoMapper;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Services
{
    public class TaskGroupService : ITaskGroupService
    {
        private readonly IMapper _mapper;
        private readonly ITaskGroupRepository _taskGroupRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public TaskGroupService(IMapper mapper, ITaskGroupRepository taskGroupRepository, IEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _taskGroupRepository = taskGroupRepository;
            _employeeRepository = employeeRepository;

        }

        public async Task<TaskGroupDto> CreateTaskGroup(TaskGroupDto taskGroupDto)
        {
            TaskGroup taskGroup = _mapper.Map<TaskGroup>(taskGroupDto);
            taskGroup.Id = Guid.NewGuid();
            if (taskGroupDto.Tasks != null)
            {
                taskGroup.Tasks = new List<BaseTask>();
                foreach (var taskDto in taskGroupDto.Tasks)
                {
                    var task = TaskHelper.CreateTaskFromDto(taskDto);
                    task.TaskGroupId = taskGroup.Id;
                    taskGroup.Tasks.Add(task);
                }
            }
            await _taskGroupRepository.Add(taskGroup);
            await _taskGroupRepository.Commit();

            return _mapper.Map<TaskGroupDto>(taskGroup);
        }

        public async Task<BaseResult> DeleteTaskGroup(Guid id)
        {
            var existingTaskGroup = await _taskGroupRepository.GetById(id);
            if (existingTaskGroup == null)
            {
                return new BaseResult
                {
                    IsSuccessfull = false,
                    Message = $"TaskGroup not found."
                };
            }

            _taskGroupRepository.Remove(existingTaskGroup);
            await _taskGroupRepository.Commit();

            return new BaseResult
            {
                IsSuccessfull = true,
                Message = "TaskGroup successfully deleted."
            };
        }

        public async Task<TaskGroupDto> UpdateTaskGroup(TaskGroupDto taskGroupDto)
        {
            if (taskGroupDto == null || taskGroupDto.Id == null)
            {
                throw new ArgumentNullException(nameof(taskGroupDto), "TaskGroupDto or its Id cannot be null.");
            }

            var existingTaskGroup = await _taskGroupRepository.GetById(taskGroupDto.Id.Value);
            if (existingTaskGroup == null)
            {
                throw new KeyNotFoundException($"TaskGroup with Id {taskGroupDto.Id} not found.");
            }

            // Update properties
            existingTaskGroup.Title = taskGroupDto.Title;
            existingTaskGroup.OnboardingPlanId = taskGroupDto.OnboardingPlanId;

            // Check if the ReferencePerson exists
            if (!string.IsNullOrEmpty(taskGroupDto.ReferencePerson))
            {
                var referenceEmployee = await _employeeRepository.FindByIdAsync(taskGroupDto.ReferencePerson);
                if (referenceEmployee != null)
                {
                    existingTaskGroup.ReferencePerson = taskGroupDto.ReferencePerson;
                }
                else
                {
                    // Keep the old ReferencePerson if the new one does not exist
                    existingTaskGroup.ReferencePerson = existingTaskGroup.ReferencePerson;
                }
            }

            // Update tasks
            if (taskGroupDto.Tasks != null)
            {
                existingTaskGroup.Tasks.Clear();
                foreach (var taskDto in taskGroupDto.Tasks)
                {
                    var task = TaskHelper.CreateTaskFromDto(taskDto);
                    task.TaskGroupId = existingTaskGroup.Id;
                    existingTaskGroup.Tasks.Add(task);
                }
            }

            _taskGroupRepository.Update(existingTaskGroup);
            await _taskGroupRepository.Commit();

            return _mapper.Map<TaskGroupDto>(existingTaskGroup);
        }
    }
}
