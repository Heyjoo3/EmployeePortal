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

        public OnboardingService(IMapper mapper, IOnboardingRepository onboardingRepository)
        {
            _mapper = mapper;
            _onboardingRepository = onboardingRepository;
        }

        public async Task<OnboardingPlanDto> CreateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto)
        {
            try { 
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

        public Task<bool> DeleteOnboardingPlan(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OnboardingPlanDto?> GetOnboardingPlan(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto)
        {
            throw new NotImplementedException();
        }
    }
}
