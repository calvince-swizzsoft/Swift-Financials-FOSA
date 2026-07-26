using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using AutoMapper;
using Infrastructure.Crosscutting.Framework.Adapter;

namespace Application.MainBoundedContext.DTO.TypeAdapterFactory
{
    public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
    {
        #region Constructor

        public AutomapperTypeAdapterFactory()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ApplicationProfile>();
                cfg.AddProfile<AccountsModuleProfile>();
                cfg.AddProfile<AdministrationModuleProfile>();
                cfg.AddProfile<BackOfficeModuleProfile>();
                cfg.AddProfile<FrontOfficeModuleProfile>();
                cfg.AddProfile<HumanResourcesModuleProfile>();
                cfg.AddProfile<MessagingModuleProfile>();
                cfg.AddProfile<MicroCreditModuleProfile>();
                cfg.AddProfile<MicroCreditModuleProfile>();
                cfg.AddProfile<RegistryModuleProfile>();
            });
        }

        #endregion

        #region ITypeAdapterFactory Members

        public ITypeAdapter Create()
        {
            return new AutomapperTypeAdapter();
        }

        #endregion
    }
}
