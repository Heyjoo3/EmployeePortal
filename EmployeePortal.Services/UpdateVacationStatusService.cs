using AutoMapper;
using EmployeePortal.Core.Data;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;

namespace EmployeePortal.Services
{
    public class UpdateVacationStatusService : IUpdateVacationStatusService
    {
        private readonly IVacationRepository _vacationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IPublicHolidayRepository _publicHolidayRepository;
        private readonly IMapper _mapper;
        private readonly EmployeePortalContext _employeePlannerContext;
        private readonly IVacationStatusEmailService _vacationStatusEmailService;

        public UpdateVacationStatusService(
            IVacationRepository vacationRepository,
            IEmployeeRepository employeeRepository,
            IAbsenceRepository absenceRepository,
            IPublicHolidayRepository publicHolidayRepository,
            IMapper mapper,
            EmployeePortalContext employeePlannerContext,
            IVacationStatusEmailService vacationStatusEmailService) 
        {
            _employeePlannerContext = employeePlannerContext;
            _mapper = mapper;
            _vacationRepository = vacationRepository;
            _employeeRepository = employeeRepository;
            _absenceRepository = absenceRepository;
            _publicHolidayRepository = publicHolidayRepository;
            _vacationStatusEmailService = vacationStatusEmailService;
        }

       async public Task<BaseResult> AcceptVacation(VacationDto vacationDto)
        {
            var vacationInput = _mapper.Map<VacationDto, Vacation>(vacationDto);

            if (vacationInput.Substitute == null)
            {
                vacationInput.SubstituteStatus = VacationStatus.VacationApproved;
            }

            if (vacationIsApprovedAndDocumented(vacationInput))
            {
                vacationInput.Status = VacationStatus.VacationApproved;

                vacationInput.EmployeeStatus = VacationStatus.CancellationPending;
                vacationInput.SubstituteStatus = VacationStatus.VacationApproved;
                vacationInput.SupervisorStatus = VacationStatus.VacationApproved;
                vacationInput.AdminStatus = VacationStatus.VacationApproved;
            }
            else if (vacationIsApproved(vacationInput))
            {
                vacationInput.Status = VacationStatus.VacationApproved;

                vacationInput.EmployeeStatus = VacationStatus.NotRelevant;
                vacationInput.SubstituteStatus = VacationStatus.VacationApproved;
                vacationInput.SupervisorStatus = VacationStatus.VacationApproved;
                vacationInput.AdminStatus = VacationStatus.VacationPending;
            }
            else if (vacationIsRejected(vacationInput))
            {
                vacationInput.Status = VacationStatus.VacationRejected;

                vacationInput.EmployeeStatus = VacationStatus.NotRelevant;
            }
            else if (vacationIsPending(vacationInput))
            {
                vacationInput.Status = VacationStatus.VacationPending;

                vacationInput.EmployeeStatus = VacationStatus.NotRelevant;
            }

            var respone = await UpdateVacationData(vacationInput);

            var updatedVacationDto = _mapper.Map<Vacation, VacationDto>(vacationInput);

            await _vacationStatusEmailService.SendEmailNotification(updatedVacationDto, "acceptVacation");

            return respone; 
        }

        async public Task<BaseResult> CancelVacation(VacationDto vacationDto)
        {
            var vacationInput = _mapper.Map<VacationDto, Vacation>(vacationDto);

            if (vacationInput.Substitute == null)
            {
                vacationInput.SubstituteStatus = VacationStatus.CancellationApproved;
            }

            if (cancellationIsApprovedAndDocumented(vacationInput))
            {
                vacationInput.Status = VacationStatus.CancellationApproved;

                vacationInput.EmployeeStatus = VacationStatus.CancellationApproved;
                vacationInput.SubstituteStatus = VacationStatus.CancellationApproved;
                vacationInput.SupervisorStatus = VacationStatus.CancellationApproved;
                vacationInput.AdminStatus = VacationStatus.CancellationApproved;
            } 
            else if (cancellationIsApproved(vacationInput))
            {
                vacationInput.Status = VacationStatus.CancellationApproved;

                vacationInput.EmployeeStatus = VacationStatus.CancellationApproved;
                vacationInput.SubstituteStatus = VacationStatus.CancellationApproved;
                vacationInput.SupervisorStatus = VacationStatus.CancellationApproved;
                vacationInput.AdminStatus = VacationStatus.CancellationPending;
            }
            else if (cancellationIsStartedAndPending(vacationInput))
            {
                vacationInput.Status = VacationStatus.CancellationPending;

                vacationInput.EmployeeStatus = vacationInput.EmployeeStatus == VacationStatus.CancellationApproved? VacationStatus.CancellationApproved : VacationStatus.CancellationPending;
                vacationInput.SubstituteStatus = vacationInput.SubstituteStatus == VacationStatus.CancellationApproved ? VacationStatus.CancellationApproved : VacationStatus.CancellationPending;
                vacationInput.SupervisorStatus = vacationInput.SupervisorStatus == VacationStatus.CancellationApproved ? VacationStatus.CancellationApproved : VacationStatus.CancellationPending;
                vacationInput.AdminStatus = VacationStatus.CancellationPending;
            }
            else if (cancellationIsPending(vacationInput))
            {
                vacationInput.Status = VacationStatus.CancellationPending;

                vacationInput.EmployeeStatus = vacationInput.EmployeeStatus == VacationStatus.NotRelevant? VacationStatus.CancellationPending : vacationInput.EmployeeStatus;
                vacationInput.SubstituteStatus = vacationInput.SubstituteStatus;
                vacationInput.SupervisorStatus = vacationInput.SupervisorStatus;
                vacationInput.AdminStatus = VacationStatus.CancellationPending;
            }
            else if (cancellationIsRejectedAndReseted(vacationInput))
            {
                vacationInput.Status = VacationStatus.VacationApproved;

                vacationInput.EmployeeStatus = VacationStatus.CancellationPending;
                vacationInput.SubstituteStatus = VacationStatus.VacationApproved;
                vacationInput.SupervisorStatus = VacationStatus.VacationApproved;
                vacationInput.AdminStatus = VacationStatus.VacationApproved;
            }
            else if (cancellationIsRejected(vacationInput))
            {
                vacationInput.Status = VacationStatus.CancellationRejected;

                vacationInput.EmployeeStatus = vacationInput.EmployeeStatus;
                vacationInput.SubstituteStatus = vacationInput.SubstituteStatus;
                vacationInput.SupervisorStatus = vacationInput.SupervisorStatus;
                vacationInput.AdminStatus = VacationStatus.CancellationPending;
            }

            var response = await  UpdateVacationData(vacationInput);

            var updatedVacationDto = _mapper.Map<Vacation, VacationDto>(vacationInput);

            await _vacationStatusEmailService.SendEmailNotification(updatedVacationDto, "cancelVacation");

            return response; 
        }   

        // Update

        async private Task<BaseResult> UpdateVacationData(Vacation vacationData)
        {
            var vacation = await _vacationRepository.GetById(vacationData.VacationId);
            if (vacation == null)
            {
                return new BaseResult { IsSuccessfull = false, Message = "Vacation not found" };
            }

            vacation.StartDate = vacationData.StartDate;
            vacation.EndDate = vacationData.EndDate;
            vacation.IsHalfStartDay = vacationData.IsHalfStartDay;
            vacation.IsHalfEndDay = vacationData.IsHalfEndDay;
            vacation.Supervisor = vacationData.Supervisor;
            vacation.Substitute = vacationData.Substitute;
            vacation.Type = vacationData.Type;
            vacation.Vacationdays = vacationData.Vacationdays;
            vacation.Description = vacationData.Description;
            vacation.Status = vacationData.Status;
            vacation.EmployeeStatus = vacationData.EmployeeStatus;
            vacation.SupervisorStatus = vacationData.SupervisorStatus;
            vacation.SubstituteStatus = vacationData.SubstituteStatus;
            vacation.AdminStatus = vacationData.AdminStatus;

            if (await _vacationRepository.Commit() > 0)
            {
                return new BaseResult { IsSuccessfull = true, Message = "Vacation updated successfully", Data = vacationData };
            }
            return new BaseResult { IsSuccessfull = false, Message = "Unable to Save Vacation Update" };
        }


        //Cases when changing VacationStatus

        private bool vacationIsApproved(Vacation vacationInput)
        {
            return vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                && vacationInput.SubstituteStatus == VacationStatus.VacationApproved;
        }
        private bool vacationIsApprovedAndDocumented(Vacation vacationInput)
        {
            return vacationInput.SupervisorStatus == VacationStatus.VacationApproved
              && vacationInput.SubstituteStatus == VacationStatus.VacationApproved
              && vacationInput.AdminStatus == VacationStatus.VacationApproved;
        }
        private bool vacationIsRejected(Vacation vacationInput)
        {
            return (vacationInput.SubstituteStatus == VacationStatus.VacationRejected
              && vacationInput.SupervisorStatus == VacationStatus.VacationPending
              && vacationInput.AdminStatus == VacationStatus.VacationPending) ||
              //(vacationInput.SubstituteStatus == VacationStatus.VacationPending
              //&& vacationInput.SupervisorStatus == VacationStatus.VacationRejected
              //&& vacationInput.AdminStatus == VacationStatus.VacationPending) ||
              (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
              && vacationInput.SupervisorStatus == VacationStatus.VacationRejected
              && vacationInput.AdminStatus == VacationStatus.VacationPending) 
              //||(vacationInput.SubstituteStatus == VacationStatus.VacationRejected
              //&& vacationInput.SupervisorStatus == VacationStatus.VacationApproved
              //&& vacationInput.AdminStatus == VacationStatus.VacationPending)
              ;
        }
        private bool vacationIsPending(Vacation vacationInput)
        {
            return (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
              && vacationInput.SupervisorStatus == VacationStatus.VacationPending
              && vacationInput.AdminStatus == VacationStatus.VacationPending) 
              //||(vacationInput.SubstituteStatus == VacationStatus.VacationPending
              //&& vacationInput.SupervisorStatus == VacationStatus.VacationApproved
              //&& vacationInput.AdminStatus == VacationStatus.VacationPending)
            ;
        }


        //Cases when changing CancellationStatus

        private bool cancellationIsApproved(Vacation vacationInput)
        {
            return vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved;
        }
        private bool cancellationIsApprovedAndDocumented(Vacation vacationInput)
        {
            return vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationApproved
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved;
        }
        private bool cancellationIsRejectedAndReseted(Vacation vacationInput)
        {
            return (vacationInput.SubstituteStatus == VacationStatus.CancellationRejected
                && vacationInput.SupervisorStatus == VacationStatus.CancellationPending
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationPending
                && vacationInput.SupervisorStatus == VacationStatus.CancellationRejected
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)

                //|| (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
                //&& vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                //&& vacationInput.AdminStatus == VacationStatus.CancellationPending
                //&& vacationInput.EmployeeStatus == VacationStatus.CancellationRejected)
            ;
        }
        private bool cancellationIsRejected(Vacation vacationInput)
        {
            return (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationRejected
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)

                //|| (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                //&& vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                //&& vacationInput.AdminStatus == VacationStatus.CancellationPending
                //&& vacationInput.EmployeeStatus == VacationStatus.CancellationRejected)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationRejected
                && vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)

                //|| (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
                //&& vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                //&& vacationInput.AdminStatus == VacationStatus.CancellationPending
                //&& vacationInput.EmployeeStatus == VacationStatus.CancellationRejected)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationRejected
                && vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

                || (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationRejected
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

                //|| (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                //&& vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                //&& vacationInput.AdminStatus == VacationStatus.CancellationPending
                //&& vacationInput.EmployeeStatus == VacationStatus.CancellationRejected)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationRejected
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationRejected
                && vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationRejected
                && vacationInput.SupervisorStatus == VacationStatus.CancellationPending
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)
            ;
        }
        private bool cancellationIsPending(Vacation vacationInput)
        {
            return (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

                || (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationPending
                && vacationInput.AdminStatus == VacationStatus.CancellationPending
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

               || (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                && vacationInput.AdminStatus == VacationStatus.VacationApproved
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)


            ;
        }
        private bool cancellationIsStartedAndPending(Vacation vacationInput)
        {
            return (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)

                || (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)

                || (vacationInput.SubstituteStatus == VacationStatus.VacationApproved
                && vacationInput.SupervisorStatus == VacationStatus.VacationApproved
                && vacationInput.EmployeeStatus == VacationStatus.CancellationApproved)

                || (vacationInput.SubstituteStatus == VacationStatus.CancellationApproved
                && vacationInput.SupervisorStatus == VacationStatus.CancellationApproved
                && vacationInput.EmployeeStatus == VacationStatus.CancellationPending)
            ;
        }
    }
} 
