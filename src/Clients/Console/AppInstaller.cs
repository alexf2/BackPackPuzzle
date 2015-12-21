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
                    .WithService.FirstInterface().LifestyleTransient()
                    /*.Configure((cr) => cr.DynamicParameters((kernel, dic) =>
                    {
                        dic["backpackSize"] = kernel.Resolve<BpoConsoleClient.ProgramArguments>().NumGallons;
                        dic["solveMinimization"] = kernel.Resolve<BpoConsoleClient.ProgramArguments>().SolveMinimizationTask;
                    }))*/,
                        

                Component.For<IMerchendiseProvider>().ImplementedBy(Type.GetType(ConfigurationManager.AppSettings["data-provider"])).LifestyleTransient()
                    .DynamicParameters((kernel, dic) =>
                    {
                        dic["filePath"] = kernel.Resolve<BpoConsoleClient.ProgramArguments>().CsvFilePath;
                    }),

                Component.For<IExecutionContext>().ImplementedBy<ConsoleAppExecutionContext>(),

                Component.For<BpOptimizerApp>()
            );
        }
    }
}
