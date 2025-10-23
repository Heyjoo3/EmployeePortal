using AutoMapper;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using EmployeePortal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IMapper _mapper;
        private readonly IOnboardingRepository _onboardingRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public OnboardingService(IMapper mapper, IOnboardingRepository onboardingRepository, IEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _onboardingRepository = onboardingRepository;
            _employeeRepository = employeeRepository;

        }


        public Task<bool> DeleteOnboardingPlan(Guid id)
        {

            _onboardingRepository.GetAllWithDetails();
            _onboardingRepository.Delete(id);


            _onboardingRepository.Create(new OnboardingPlan());
            _onboardingRepository.Create(new OnboardingPlan());

            _onboardingRepository.Commit();
            throw new NotImplementedException();
        }

        public Task<OnboardingPlanDto?> GetOnboardingPlan(Guid id)
        {
            _onboardingRepository.GetByEmployeeId(id);
            throw new NotImplementedException();
        }

        public async Task<OnboardingPlanDto> CreateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto)
        {
            try
            {
                OnboardingPlan onboardingPlan = _mapper.Map<OnboardingPlan>(onboardingPlanDataDto);
                onboardingPlan.OnbardingId = Guid.NewGuid();

                await _onboardingRepository.Add(onboardingPlan);

                await _onboardingRepository.Commit();


                return _mapper.Map<OnboardingPlanDto>(onboardingPlan);
            }
            catch (Exception ex)
            {
                var er = ex;
                return onboardingPlanDataDto;
            }



        }

        public async Task<OnboardingPlanDto> UpdateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto)
        {
            var existingPlan = await _onboardingRepository.GetById(onboardingPlanDataDto.OnboardingId.Value);
            if (existingPlan == null)
            {
                throw new Exception("OnboardingPlan not found");
            }

            UpdatePlanDetails(existingPlan, onboardingPlanDataDto);
            await UpdateTaskGroups(existingPlan, onboardingPlanDataDto.TaskGroups);

            await _onboardingRepository.Update(existingPlan);
            await _onboardingRepository.Commit();

            return _mapper.Map<OnboardingPlanDto>(existingPlan);
        }

        private void UpdatePlanDetails(OnboardingPlan existingPlan, OnboardingPlanDto onboardingPlanDataDto)
        {
            existingPlan.StartDate = onboardingPlanDataDto.StartDate;
            existingPlan.EndDate = onboardingPlanDataDto.EndDate;
            existingPlan.Status = onboardingPlanDataDto.Status;

            if (onboardingPlanDataDto.ReferencePerson != null && existingPlan.ReferencePerson != onboardingPlanDataDto.ReferencePerson)
            {
                var referenceEmployee = _employeeRepository.FindByIdAsync(onboardingPlanDataDto.ReferencePerson).Result;
                if (referenceEmployee == null)
                {
                    throw new Exception("Employee not found");
                }
                existingPlan.ReferencePerson = onboardingPlanDataDto.ReferencePerson;
            }
            else
            {
                existingPlan.ReferencePerson = null;
            }
        }

        private async Task UpdateTaskGroups(OnboardingPlan existingPlan, List<TaskGroupDto>? taskGroupsDto)
        {
            if (existingPlan.TaskGroups == null)
            {
                existingPlan.TaskGroups = new List<TaskGroup>();
            }

            if (taskGroupsDto != null)
            {
                foreach (var taskGroupDto in taskGroupsDto)
                {
                    var existingTaskGroup = existingPlan.TaskGroups.FirstOrDefault(tg => tg.Id == taskGroupDto.Id);
                    if (existingTaskGroup != null)
                    {
                        UpdateTaskGroupDetails(existingTaskGroup, taskGroupDto);
                    }
                }
            }
        }

        private void UpdateTaskGroupDetails(TaskGroup existingTaskGroup, TaskGroupDto taskGroupDto)
        {
            existingTaskGroup.Title = taskGroupDto.Title;

            if (taskGroupDto.ReferencePerson != null && existingTaskGroup.ReferencePerson != taskGroupDto.ReferencePerson)
            {
                var referenceEmployee = _employeeRepository.FindByIdAsync(taskGroupDto.ReferencePerson).Result;
                if (referenceEmployee == null)
                {
                    throw new Exception("Employee not found");
                }
                existingTaskGroup.ReferencePerson = taskGroupDto.ReferencePerson;
            }
            else
            {
                existingTaskGroup.ReferencePerson = null;
            }

            UpdateTasks(existingTaskGroup, taskGroupDto.Tasks);
        }

        private void UpdateTasks(TaskGroup existingTaskGroup, List<TaskDto> taskDtos)
        {
            foreach (var taskDto in taskDtos)
            {
                var existingTask = existingTaskGroup.Tasks.FirstOrDefault(t => t.Id == taskDto.Id);
                if (existingTask != null)
                {
                    existingTask.Title = taskDto.Title;
                    existingTask.Status = Enum.Parse<Status>(taskDto.Status);
                    updateSpecificTaskType(existingTask, taskDto);
                }
                else
                {
                    var newTask = CreateTaskFromDto(taskDto);
                    existingTaskGroup.Tasks.Add(newTask);
                }
            }
        }
        private void updateSpecificTaskType(BaseTask existingTask, TaskDto taskDto)
        {
            //switch (existingTask)
            //{
            //    case TodoTask todoTask:
            //        var todoDto = taskDto as TodoTaskDto;
            //        if (todoDto != null)
            //        {
            //            todoTask.Description = todoDto.Description;
            //        }
            //        break;
            //    case NetworkingTask networkingTask:
            //        var networkingDto = taskDto as NetworkingTaskDto;
            //        if (networkingDto != null)
            //        {
            //            networkingTask.StartDate = networkingDto.StartDate;
            //        }
            //        break;
            //    case ProjectTask projectTask:
            //        var projectDto = taskDto as ProjectTaskDto;
            //        if (projectDto != null)
            //        {
            //            projectTask.EndDate = projectDto.EndDate;
            //        }
            //        break;
            //    case TimeScheduledTask timeScheduledTask:
            //        var timeScheduledDto = taskDto as TimeScheduledTaskDto;
            //        if (timeScheduledDto != null)
            //        {
            //            timeScheduledTask.ScheduledTime = timeScheduledDto.ScheduledTime;
            //        }
            //        break;
            //}
        }

        private BaseTask CreateTaskFromDto(TaskDto taskDto)
        {
            throw new NotImplementedException();
        }

        public Task<OnboardingPlanDto> GetOnboardingPlanByEmployee(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OnboardingPlanDto>> GetAllOnboardingPlans()
        {
            throw new NotImplementedException();
        }
    }

  

    private object CreateTaskFromDto(TaskDto taskDto)
    {
        throw new NotImplementedException();
    }

    public async Task<OnboardingPlanDto> UpdateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto)
    {
        var existingPlan = await _onboardingRepository.GetById(onboardingPlanDataDto.OnboardingId.Value);
        if (existingPlan == null)
        {
            throw new Exception("OnboardingPlan not found");
        }

        existingPlan.StartDate = onboardingPlanDataDto.StartDate;
        existingPlan.EndDate = onboardingPlanDataDto.EndDate;
        existingPlan.Status = onboardingPlanDataDto.Status;

        if (onboardingPlanDataDto.ReferencePerson != null && existingPlan.ReferencePerson != onboardingPlanDataDto.ReferencePerson)
        {
            var referenceEmployee = await _employeeRepository.FindByIdAsync(onboardingPlanDataDto.ReferencePerson);
            if (referenceEmployee == null)
            {
                throw new Exception("Employee not found");
            }
            existingPlan.ReferencePerson = onboardingPlanDataDto.ReferencePerson;
        }
        else
        {
            existingPlan.ReferencePerson = null;
        }

        if (existingPlan.TaskGroups == null)
        {
            existingPlan.TaskGroups = new List<TaskGroup>();
        }

        if (onboardingPlanDataDto.TaskGroups != null)
        {
            foreach (var taskGroupDto in onboardingPlanDataDto.TaskGroups)
            {
                var existingTaskGroup = existingPlan.TaskGroups.FirstOrDefault(tg => tg.Id == taskGroupDto.Id);
                if (existingTaskGroup != null)
                {
                    existingTaskGroup.Title = taskGroupDto.Title;
                    if (taskGroupDto.ReferencePerson != null && existingTaskGroup.ReferencePerson != taskGroupDto.ReferencePerson)
                    {
                        var referenceEmployee = await _employeeRepository.FindByIdAsync(taskGroupDto.ReferencePerson);
                        if (referenceEmployee == null)
                        {
                            throw new Exception("Employee not found");
                        }
                        existingTaskGroup.ReferencePerson = taskGroupDto.ReferencePerson;
                    }
                    else
                    {
                        existingTaskGroup.ReferencePerson = null;
                    }

                    foreach (var taskDto in taskGroupDto.Tasks)
                    {
                        var existingTask = existingTaskGroup.Tasks.FirstOrDefault(t => t.Id == taskDto.Id);
                        if (existingTask != null)
                        {
                            existingTask.Title = taskDto.Title;
                            existingTask.Status = Enum.Parse<Status>(taskDto.Status);
                            updateSpecificTaskType(existingTask, taskDto);
                        }
                        else
                        {
                            var newTask = CreateTaskFromDto(taskDto);
                            existingTaskGroup.Tasks.Add(newTask);
                        }
                    }
                }

            }
        }

        await _onboardingRepository.Update(existingPlan);
        await _onboardingRepository.Commit();

        return _mapper.Map<OnboardingPlanDto>(existingPlan);
    }
}


    
