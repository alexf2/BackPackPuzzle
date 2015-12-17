using System;
using System.Configuration;
using BackPackOptimizer.Contract;
using BackPackOptimizer.Runtime;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DataProviders.Contract;

namespace BackPackOptimizer.Clients.Console
{
    public sealed class AppInstaller: IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromAssemblyContaining<BpoBase>()
                    .Where(t => t.Name.Equals(ConfigurationManager.AppSettings["algorithm"], StringComparison.Ordinal))
                    .WithService.FirstInterface().LifestyleTransient(),

                Component.For<IMerchendiseProvider>().ImplementedBy(Type.GetType(ConfigurationManager.AppSettings["data-provider"])).LifestyleTransient(),

                Component.For<IExecutionContext>().ImplementedBy<ConsoleAppExecutionContext>(),

                Component.For<BpOptimizerApp>()
            );
        }
    }
}
