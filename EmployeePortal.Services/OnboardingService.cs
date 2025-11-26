using AutoMapper;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using EmployeePortal.Data;
using System.Threading.Tasks;

namespace EmployeePortal.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IMapper _mapper;
        private readonly IOnboardingRepository _onboardingRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITaskGroupRepository _taskGroupRepository;
        private readonly ITaskRepository _taskRepository;

        public OnboardingService(IMapper mapper, IOnboardingRepository onboardingRepository, IEmployeeRepository employeeRepository,ITaskGroupRepository taskGroupRepository, ITaskRepository taskRepository)
        {
            _mapper = mapper;
            _onboardingRepository = onboardingRepository;
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
            _taskGroupRepository = taskGroupRepository;
        }

        public async Task<bool> DeleteOnboardingPlan(Guid id)
        {
            try
            {
                await _onboardingRepository.Delete(id);
                await _onboardingRepository.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<OnboardingPlanDto> CreateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto)
        {
            try
            {
                OnboardingPlan onboardingPlan = _mapper.Map<OnboardingPlan>(onboardingPlanDataDto);
                onboardingPlan.OnboardingId = Guid.NewGuid();

                if (onboardingPlanDataDto.TaskGroups != null)
                {
                    onboardingPlan.TaskGroups = new List<TaskGroup>();
                    foreach (var taskGroupDto in onboardingPlanDataDto.TaskGroups)
                    {
                        var taskGroup = _mapper.Map<TaskGroup>(taskGroupDto);
                        taskGroup.Id = Guid.NewGuid();
                        taskGroup.OnboardingPlanId = onboardingPlan.OnboardingId;
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
                        onboardingPlan.TaskGroups.Add(taskGroup);
                    }
                }

                await _onboardingRepository.Add(onboardingPlan);
                await _onboardingRepository.Commit();


                return _mapper.Map<OnboardingPlanDto>(onboardingPlan);
            }
            catch (Exception ex)
            {
                throw new Exception ("Creation not possible");
            }
        }

        public async Task<OnboardingPlanDto> UpdateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto)
        {
            var existingPlan = await _onboardingRepository.GetPlanById(onboardingPlanDataDto.OnboardingId.Value);
            if (existingPlan == null)
            {
                throw new Exception("OnboardingPlan not found");
            }

            await UpdatePlanDetails(existingPlan, onboardingPlanDataDto);
            await UpdateTaskGroups(existingPlan, onboardingPlanDataDto.TaskGroups);

            await _onboardingRepository.Update(existingPlan);
            await _onboardingRepository.Commit();

            return _mapper.Map<OnboardingPlanDto>(existingPlan);
        }

        private async Task UpdatePlanDetails(OnboardingPlan existingPlan, OnboardingPlanDto onboardingPlanDataDto)
        {
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
            else if (onboardingPlanDataDto.ReferencePerson == null)
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

            var taskGroupDtoIds = taskGroupsDto?.Select(dto => dto.Id).Where(id => id.HasValue).Select(id => id.Value).ToHashSet() ?? new HashSet<Guid>();

            // Entferne TaskGroups, die in existingPlan.TaskGroups existieren, aber nicht in taskGroupsDto
            var taskGroupsToRemove = existingPlan.TaskGroups
                .Where(tg => !taskGroupDtoIds.Contains(tg.Id))
                .ToList();

            foreach (var taskGroup in taskGroupsToRemove)
            {
                await _taskRepository.DeleteTask(taskGroup.Id); // Lösche die TaskGroup aus dem Repository
                existingPlan.TaskGroups.Remove(taskGroup); // Entferne die TaskGroup aus dem Plan
            }

            // Aktualisiere oder füge TaskGroups hinzu
            if (taskGroupsDto != null)
            {
                foreach (var taskGroupDto in taskGroupsDto)
                {
                    var existingTaskGroup = existingPlan.TaskGroups.FirstOrDefault(tg => tg.Id == (taskGroupDto.Id ?? Guid.Empty));
                    if (existingTaskGroup != null)
                    {
                       await UpdateTaskGroupDetails(existingTaskGroup, taskGroupDto);
                    }
                    else
                    {
                        // Füge neue TaskGroup hinzu
                        var newTaskGroup = _mapper.Map<TaskGroup>(taskGroupDto);
                        newTaskGroup.Id = Guid.NewGuid();
                        newTaskGroup.OnboardingPlanId = existingPlan.OnboardingId;

                        if (taskGroupDto.Tasks != null)
                        {
                            newTaskGroup.Tasks = new List<BaseTask>();
                            foreach (var taskDto in taskGroupDto.Tasks)
                            {
                                var newTask = TaskHelper.CreateTaskFromDto(taskDto);
                                newTask.TaskGroupId = newTaskGroup.Id;
                                //await _taskRepository.Add(newTask);
                                newTaskGroup.Tasks.Add(newTask);
                            }
                        }

                        await _taskGroupRepository.Add(newTaskGroup); // Füge die neue TaskGroup ins Repository ein
                        await _taskGroupRepository.Commit();
                        existingPlan.TaskGroups.Add(newTaskGroup);
                    }
                }
            }
        }

        private async Task UpdateTaskGroupDetails(TaskGroup existingTaskGroup, TaskGroupDto taskGroupDto)
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
            else if (taskGroupDto.ReferencePerson == null)
            {
                existingTaskGroup.ReferencePerson = null;
            }

            UpdateTasks(existingTaskGroup, taskGroupDto.Tasks);
        }

        private async Task UpdateTasks(TaskGroup existingTaskGroup, List<TaskDto> taskDtos)
        {
            // Entferne Aufgaben, die in existingTaskGroup.Tasks existieren, aber nicht in taskDtos
            var taskDtoIds = taskDtos
                .Where(dto => !string.IsNullOrEmpty(dto.Id) && Guid.TryParse(dto.Id, out _))
                .Select(dto => Guid.Parse(dto.Id))
                .ToHashSet();

            var tasksToRemove = existingTaskGroup.Tasks
                .Where(task => !taskDtoIds.Contains(task.Id))
                .ToList();

            foreach (var task in tasksToRemove)
            {
                await _taskRepository.DeleteTask(task.Id); // Lösche die Aufgabe aus dem Repository
                existingTaskGroup.Tasks.Remove(task); // Entferne die Aufgabe aus der TaskGroup
            }

            foreach (var taskDto in taskDtos)
            {
                Guid taskDtoGuid;
                // Try to parse the string Id to Guid, skip if not valid
                if (!string.IsNullOrEmpty(taskDto.Id) && Guid.TryParse(taskDto.Id, out taskDtoGuid))
                {
                    var existingTask = existingTaskGroup.Tasks.FirstOrDefault(t => t.Id == taskDtoGuid);
                    if (existingTask == null)
                    {
                        // Erstelle einen neuen Task
                        var newTask = TaskHelper.CreateTaskFromDto(taskDto);
                        newTask.TaskGroupId = existingTaskGroup.Id;
                        await _taskRepository.Add(newTask);
                        await _taskGroupRepository.Commit();
                        existingTaskGroup.Tasks.Add(newTask);
                    }
                    else if (existingTask.TaskType != taskDto.TaskType)
                    {
                        // Lösche den alten Task
                        await _taskRepository.DeleteTask(existingTask.Id);
                        existingTaskGroup.Tasks.Remove(existingTask);

                        // Erstelle einen neuen Task
                        var newTask = TaskHelper.CreateTaskFromDto(taskDto);
                        newTask.TaskGroupId = existingTaskGroup.Id;
                        await _taskRepository.Add(newTask);
                        await _taskGroupRepository.Commit();
                        existingTaskGroup.Tasks.Add(newTask);
                    }
                    else
                    {
                        // Aktualisiere den bestehenden Task
                        //existingTask.Title = taskDto.Title;
                        //existingTask.Status = taskDto.Status;
                        //UpdateSpecificTaskType(existingTask, taskDto);
                        existingTask.UpdateTask(taskDto);
                    }
                    //if (existingTask != null)
                    //{
                    //    //existingTask.Title = taskDto.Title;
                    //    //existingTask.Status = taskDto.Status;
                    //    //UpdateSpecificTaskType(existingTask, taskDto);
                    //    existingTask.UpdateTask(taskDto);
                    //}
                    //else
                    //{
                    //    var newTask = TaskHelper.CreateTaskFromDto(taskDto);
                    //    newTask.TaskGroupId = existingTaskGroup.Id;
                    //    await _taskRepository.Add(newTask);
                    //    existingTaskGroup.Tasks.Add(newTask);
                    //}
                }
                else
                {
                    // If taskDto.Id is null or not a valid Guid, treat as new task
                    var newTask = TaskHelper.CreateTaskFromDto(taskDto);
                    newTask.TaskGroupId = existingTaskGroup.Id;
                    await _taskRepository.Add(newTask);
                    await _taskGroupRepository.Commit();
                    existingTaskGroup.Tasks.Add(newTask);
                }
            }
        }
        //private void UpdateSpecificTaskType(BaseTask existingTask, TaskDto taskDto)
        //{
        //    switch (existingTask)
        //    {
        //        case TodoTask todoTask:
        //            todoTask.Description = taskDto.Description;
        //            break;
        //        case NetworkingTask networkingTask:
        //            networkingTask.StartDate = taskDto.StartDate;
        //            networkingTask.Status = taskDto.Status;
        //            networkingTask.Location = taskDto.Location;
        //            break;
        //        case ProjectTask projectTask:
        //            projectTask.EndDate = taskDto.EndDate;
        //            projectTask.StartDate = taskDto.StartDate;
        //            projectTask.Description = taskDto.Description;
        //            projectTask.Tools = taskDto.Tools;
        //            projectTask.Priority = taskDto.Priority ?? Priority.Low;
        //            break;
        //        case TimeScheduledTask timeScheduledTask:
        //            timeScheduledTask.StartDate = taskDto.StartDate;
        //            timeScheduledTask.EndDate = taskDto.EndDate;
        //            timeScheduledTask.Description = taskDto.Description;
        //            break;
        //    }
        //}

        //private BaseTask CreateTaskFromDto(TaskDto taskDto)
        //{
        //    if (taskDto.TaskType == null)
        //    {
        //        throw new ArgumentException("TaskType cannot be null");
        //    }

        //    return taskDto.TaskType switch
        //    {
        //        nameof(TodoTask) => new TodoTask
        //        {
        //            Id = Guid.NewGuid(),
        //            Title = taskDto.Title,
        //            Status = taskDto.Status,
        //            Description = taskDto.Description
        //        },
        //        nameof(NetworkingTask) => new NetworkingTask
        //        {
        //            Id = Guid.NewGuid(),
        //            Title = taskDto.Title,
        //            Status = taskDto.Status,
        //            Location = taskDto.Location,
        //            StartDate = taskDto.StartDate,
        //            ExampleQuestions = taskDto.ExampleQuestions
        //        },
        //        nameof(ProjectTask) => new ProjectTask
        //        {
        //            Id = Guid.NewGuid(),
        //            Title = taskDto.Title,
        //            Status = taskDto.Status,
        //            Description = taskDto.Description,
        //            Priority = taskDto.Priority ?? Priority.Low,
        //            Tools = taskDto.Tools,
        //            StartDate = taskDto.StartDate,
        //            EndDate = taskDto.EndDate
        //        },
        //        nameof(TimeScheduledTask) => new TimeScheduledTask
        //        {
        //            Id = Guid.NewGuid(),
        //            Title = taskDto.Title,
        //            Status = taskDto.Status,
        //            Description = taskDto.Description,
        //            StartDate = taskDto.StartDate,
        //            EndDate = taskDto.EndDate
        //        },
        //        _ => throw new NotSupportedException($"TaskType '{taskDto.TaskType}' is not supported")
        //    };
        //}
        
        public async Task<OnboardingPlanDto> GetOnboardingPlanByEmployee(Guid id)
        {
            var onboardingPlan = await _onboardingRepository.GetByEmployeeId(id);
            if (onboardingPlan == null)
            {
                throw new Exception("Onboarding plan not found for the given employee.");
            }

            var dto = _mapper.Map<OnboardingPlanDto>(onboardingPlan);
            //dto.TaskGroups = MapTaskGroupsToDtos(onboardingPlan.TaskGroups);

            return dto;
        }

        public async Task<IEnumerable<OnboardingPlanDto>> GetAllOnboardingPlans()
        {
            var onboardingPlans = await _onboardingRepository.GetAllWithDetails();
            return onboardingPlans.Select(plan =>
            {
                var dto = _mapper.Map<OnboardingPlanDto>(plan);
                //dto.TaskGroups = MapTaskGroupsToDtos(plan.TaskGroups ?? new List<TaskGroup>());
                return dto;
            });
        }

        //private List<TaskGroupDto> MapTaskGroupsToDtos(List<TaskGroup> taskGroups)
        //{
        //    var taskGroupDtos = new List<TaskGroupDto>();

        //    foreach (var taskGroup in taskGroups)
        //    {
        //        var taskGroupDto = new TaskGroupDto
        //        {
        //            Id = taskGroup.Id,
        //            Title = taskGroup.Title,
        //            OnboardingPlanId = taskGroup.OnboardingPlanId ?? Guid.Empty,
        //            ReferencePerson = taskGroup.ReferencePerson,
        //            Tasks = MapTasksToDtos(taskGroup.Tasks)
        //        };

        //        taskGroupDtos.Add(taskGroupDto);
        //    }

        //    return taskGroupDtos;
        //}

        //private List<TaskDto> MapTasksToDtos(List<BaseTask> tasks)
        //{
        //    var taskDtos = new List<TaskDto>();

        //    foreach (var task in tasks)
        //    {
        //        var taskDto = new TaskDto
        //        {
        //            Id = task.Id.ToString(),
        //            Title = task.Title,
        //            TaskType = task.TaskType,
        //            TaskGroupId = task.TaskGroupId,
        //            Status = task.Status,
        //            Description = task is TodoTask todoTask ? todoTask.Description : null,
        //            Location = task is NetworkingTask networkingTask ? networkingTask.Location : null,
        //            StartDate = task is NetworkingTask networkingTask1 ? networkingTask1.StartDate
        //                        : task is ProjectTask projectTask1 ? projectTask1.StartDate
        //                        : task is TimeScheduledTask timeScheduledTask1 ? timeScheduledTask1.StartDate
        //                        : null,
        //            EndDate = task is ProjectTask projectTask2 ? projectTask2.EndDate
        //                      : task is TimeScheduledTask timeScheduledTask2 ? timeScheduledTask2.EndDate
        //                      : null,
        //            ExampleQuestions = task is NetworkingTask networkingTask2 ? networkingTask2.ExampleQuestions : null,
        //            Priority = task is ProjectTask projectTask3 ? projectTask3.Priority : null,
        //            Tools = task is ProjectTask projectTask4 ? projectTask4.Tools : null
        //        };

        //        taskDtos.Add(taskDto);
        //    }

        //    return taskDtos;
        //}
    }
}
