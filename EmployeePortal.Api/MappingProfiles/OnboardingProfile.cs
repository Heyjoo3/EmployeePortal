//namespace EmployeePortal.Api.MappingProfiles
//{
//    public class OnboardingProfile : AutoMapper.Profile
//    {
//        public OnboardingProfile()
//        {
//            CreateMap<Core.Models.OnboardingPlan, Core.Dto.OnboardingPlanDto>()
//                .ForMember(dest => dest.TaskGroups, opt => opt.Ignore()) // TaskGroups ausschließen
//                .ReverseMap()
//                .ForMember(dest => dest.TaskGroups, opt => opt.Ignore()); // TaskGroups auch in der Rückrichtung ausschließen

//            CreateMap<Core.Models.TaskGroup, Core.Dto.TaskGroupDto>()
//                .ForMember(dest => dest.Tasks, opt => opt.Ignore()) // Tasks explizit ignorieren
//                .ReverseMap()
//                .ForMember(dest => dest.Tasks, opt => opt.Ignore()); // Tasks auch in der Rückrichtung ignorieren

//            // Basis-Mapping für Tasks; für polymorphe Fälle ggf. konkret erweitern
//            CreateMap<Core.Models.BaseTask, Core.Dto.TaskDto>().ReverseMap();
//        }
//    }
//}

using System;
using System.Collections.Generic;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Dto;

namespace EmployeePortal.Api.MappingProfiles
{
    public class OnboardingProfile : AutoMapper.Profile
    {
        public OnboardingProfile()
        {
            CreateMap<OnboardingPlan, OnboardingPlanDto>()
                .ForMember(dest => dest.TaskGroups, opt => opt.MapFrom(src => MapTaskGroupsToDtos(src.TaskGroups ?? new List<TaskGroup>())))
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.CalculateProgress()))
                .ReverseMap()
                .ForMember(dest => dest.TaskGroups, opt => opt.Ignore());

            CreateMap<TaskGroup, TaskGroupDto>()
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => MapTasksToDtos(src.Tasks)))
                .ReverseMap()
                .ForMember(dest => dest.Tasks, opt => opt.Ignore());

            CreateMap<BaseTask, TaskDto>().ReverseMap();
        }

        private static List<TaskGroupDto> MapTaskGroupsToDtos(List<TaskGroup>? taskGroups)
        {
            var taskGroupDtos = new List<TaskGroupDto>();

            if (taskGroups == null)
                return taskGroupDtos;

            foreach (var taskGroup in taskGroups)
            {
                var taskGroupDto = new TaskGroupDto
                {
                    Id = taskGroup.Id,
                    Title = taskGroup.Title,
                    OnboardingPlanId = taskGroup.OnboardingPlanId ?? Guid.Empty,
                    ReferencePerson = taskGroup.ReferencePerson,
                    Tasks = MapTasksToDtos(taskGroup.Tasks)
                };

                taskGroupDtos.Add(taskGroupDto);
            }

            return taskGroupDtos;
        }

        private static List<TaskDto> MapTasksToDtos(List<BaseTask> tasks)
        {
            var taskDtos = new List<TaskDto>();

            foreach (var task in tasks)
            {
                var taskDto = new TaskDto
                {
                    Id = task.Id.ToString(),
                    Title = task.Title,
                    TaskType = task.TaskType,
                    TaskGroupId = task.TaskGroupId,
                    Status = task.Status,
                    Description = task is TodoTask todoTask ? todoTask.Description : null,
                    Location = task is NetworkingTask networkingTask ? networkingTask.Location : null,
                    StartDate = task is NetworkingTask networkingTask1 ? networkingTask1.StartDate
                                : task is ProjectTask projectTask1 ? projectTask1.StartDate
                                : task is TimeScheduledTask timeScheduledTask1 ? timeScheduledTask1.StartDate
                                : null,
                    EndDate = task is ProjectTask projectTask2 ? projectTask2.EndDate
                              : task is TimeScheduledTask timeScheduledTask2 ? timeScheduledTask2.EndDate
                              : null,
                    ExampleQuestions = task is NetworkingTask networkingTask2 ? networkingTask2.ExampleQuestions : null,
                    Priority = task is ProjectTask projectTask3 ? projectTask3.Priority : null,
                    Tools = task is ProjectTask projectTask4 ? projectTask4.Tools : null
                };

                taskDtos.Add(taskDto);
            }

            return taskDtos;
        }
    }
}