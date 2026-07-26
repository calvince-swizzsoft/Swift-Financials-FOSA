using AutoMapper;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BankAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BankBranchAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAttachedProductAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyDebitTypeAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.LocationAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemInRoleAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemInRoleAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ReportAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInBranchAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInRoleAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class AdministrationModuleProfile : Profile
    {
        public AdministrationModuleProfile()
        {
            //ModuleNavigationItem => ModuleNavigationItemDTO
            CreateMap<ModuleNavigationItem, ModuleNavigationItemDTO>();

            //ModuleNavigationItemInRole => ModuleNavigationItemInRoleDTO
            CreateMap<ModuleNavigationItemInRole, ModuleNavigationItemInRoleDTO>();

            //NavigationItem => NavigationItemDTO
            CreateMap<NavigationItem, NavigationItemDTO>();

            CreateMap<NavigationItemInRole, NavigationItemInRoleDTO>();

            //SystemPermissionTypeInRole => SystemPermissionTypeInRoleDTO
            CreateMap<SystemPermissionTypeInRole, SystemPermissionTypeInRoleDTO>();

            //SystemPermissionTypeInBranch => SystemPermissionTypeInBranchDTO
            CreateMap<SystemPermissionTypeInBranch, SystemPermissionTypeInBranchDTO>();

            //Report => ReportDTO
            CreateMap<Report, ReportDTO>()
                .ForMember(dest => dest.ReportHost, opt => opt.Ignore())
                .ForMember(dest => dest.ReportURL, opt => opt.Ignore());

            //Bank => BankDTO
            CreateMap<Bank, BankDTO>()
               .ForMember(dest => dest.BankBranches, opt => opt.Ignore())
               .ForMember(dest => dest.PaddedCode, opt => opt.Ignore());

            //BankBranch => BankBranchDTO
            CreateMap<BankBranch, BankBranchDTO>()
                .ForMember(dest => dest.PaddedCode, opt => opt.Ignore());

            //Company => CompanyDTO
            CreateMap<Company, CompanyDTO>();

            //CompanyDTO => CompanyBindingModel
            CreateMap<CompanyDTO, CompanyBindingModel>();

            //Branch => BranchDTO
            CreateMap<Branch, BranchDTO>()
                .ForMember(dest => dest.CompanyRecoveryPriority, opt => opt.MapFrom(src => ProcessPayoutRecoveryPriority(src)));

            //CompanyDirectDebit => CompanyDirectDebitDTO
            CreateMap<CompanyDebitType, CompanyDebitTypeDTO>();

            //CompanyAttachedProduct => CompanyAttachedProductDTO
            CreateMap<CompanyAttachedProduct, CompanyAttachedProductDTO>();

            //Location => LocationDTO
            CreateMap<Location, LocationDTO>();

            //Workflow => WorkflowDTO
            CreateMap<Workflow, WorkflowDTO>()
                .ForMember(dest => dest.MatchedStatus, opt => opt.MapFrom(src => (int)(src.MatchedStatus)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)(src.Status)));

            //WorkflowDTO => WorkflowBindingModel
            CreateMap<WorkflowDTO, WorkflowBindingModel>();

            //WorkflowItem => WorkflowItemDTO
            CreateMap<WorkflowItem, WorkflowItemDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)(src.Status)));

            //WorkflowItemDTO => WorkflowItemBindingModel
            CreateMap<WorkflowItemDTO, WorkflowItemBindingModel>();

            //WorkflowItemEntry => WorkflowItemEntryDTO
            CreateMap<WorkflowItemEntry, WorkflowItemEntryDTO>();

            //WorkflowItemEntryDTO => WorkflowItemEntryBindingModel
            CreateMap<WorkflowItemEntryDTO, WorkflowItemEntryBindingModel>();

            //WorkflowSettingDTO => WorkflowSettingDTO
            CreateMap<WorkflowSettingDTO, WorkflowSettingDTO>();

            //WorkflowSettingDTO => WorkflowSettingBindingModel
            CreateMap<WorkflowSettingDTO, WorkflowSettingBindingModel>();

            //UserDTO => UserBindingModel
            CreateMap<UserDTO, UserBindingModel>();
        }

        static string ProcessPayoutRecoveryPriority(Company company)
        {
            var tuple = EnumValueDescriptionCache.GetValues(typeof(RecoveryPriority));

            if (!string.IsNullOrWhiteSpace(company.RecoveryPriority))
            {
                var buffer = company.RecoveryPriority.Split(new char[] { ',' });

                if (buffer != null && buffer.Length == tuple.Item2.Length)
                {
                    return string.Join(",", buffer);
                }
                else return string.Join(",", tuple.Item2);
            }
            else return string.Join(",", tuple.Item2);
        }

        static string ProcessPayoutRecoveryPriority(Branch branch)
        {
            var tuple = EnumValueDescriptionCache.GetValues(typeof(RecoveryPriority));
            //branch.Company.RecoveryPriority = "debittype";
            if ((branch.Company)!=null)
            {
                if (!string.IsNullOrWhiteSpace(branch.Company.RecoveryPriority))
                {
                    var buffer = branch.Company.RecoveryPriority.Split(new char[] { ',' });

                    if (buffer != null && buffer.Length == tuple.Item2.Length)
                    {
                        return string.Join(",", buffer);
                    }
                    else return string.Join(",", tuple.Item2);
                }
                else return string.Join(",", tuple.Item2);
            }
            else return string.Join(",", tuple.Item2);
        }
    }
}
