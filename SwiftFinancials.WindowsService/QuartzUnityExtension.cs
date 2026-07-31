using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using Quartz.Spi;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace SwiftFinancials.WindowsService
{
    public class QuartzUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.Container.RegisterType<IJobFactory, UnityJobFactory>(new ContainerControlledLifetimeManager());

            this.Container.RegisterType<ISchedulerFactory, UnitySchedulerFactory>(new ContainerControlledLifetimeManager());

            this.Container.RegisterType<ILogger, SerilogLogger>(new ContainerControlledLifetimeManager());
        }
    }
}