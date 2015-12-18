using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace BackPackOptimizer.Clients.Console
{
    class BpoConsoleClient
    {
        const int BadArgs = -1;
        const int NoFile = -2;
        const int BadSize = -3;
        const int GenericException = -100;

        internal sealed class ProgramArguments
        {
            public string CsvFilePath;
            public int NumGallons;
        }
        
        public static void Main(string[] args)
        {
            try
            {
                ExecuteCompositionRoot(args);
            }
            catch (TaskCanceledException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            catch (AggregateException ex)
            {
                ex.Handle(e =>
                {
                    if (e is TaskCanceledException)
                        System.Console.WriteLine(e.Message);
                    else
                        System.Console.WriteLine(ex);
                    return true;
                });
            }
            catch (Exception ex)
            {
                Environment.ExitCode = GenericException;
                System.Console.WriteLine(ex);
            }
        }

        static void ExecuteCompositionRoot(string[] args)
        {
            using (var container = ConfigureIoC())
            {
                var argsuments = ReadArguments(args);
                container.Register(Component.For<ProgramArguments>().Instance(argsuments.Item2).LifestyleSingleton());

                if (argsuments.Item1)
                {
                    var stpw = new Stopwatch();

                    var app = container.Resolve<BpOptimizerApp>(new Arguments(new { filePath = argsuments.Item2.CsvFilePath, backpackSize = argsuments.Item2.NumGallons }));
                    var execContext = container.Resolve<IExecutionContext>();
                    execContext.StartReading();

                    try
                    {
                        System.Console.WriteLine(
                            $"Optimizing {Path.GetFileName(argsuments.Item2.CsvFilePath)} for {argsuments.Item2.NumGallons} gallons");

                        stpw.Start();
                        var percases = app.Run().Result;
                        execContext.CancelSource.Cancel();
                        stpw.Stop();

                        System.Console.WriteLine($"Optimization took {stpw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}");
                        PrintResult(percases);
                    }
                    finally
                    {
                        container.Release(execContext);
                        container.Release(app);
                    }
                }
            }
        }

        static void PrintResult(Purchases p)
        {
            System.Console.WriteLine(
                $"Optimal purchases for {p.NumberOfGallons} with average price {p.AveragePriceOfGallon.ToString("#.##")} are:");
            foreach (var m in p.Merchendises)
                System.Console.WriteLine($"\t{m}");
        }

        static Tuple<bool, ProgramArguments> ReadArguments(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                Environment.ExitCode = BadArgs;
                return new Tuple<bool, ProgramArguments>(false, default(ProgramArguments));
            }

            var csvFilePath = args[0];
            var numGallonsArg = args[1];

            if (!File.Exists(csvFilePath))
            {
                System.Console.WriteLine($"Error: CSV file [{csvFilePath}] is not found.");
                Environment.ExitCode = NoFile;
                return new Tuple<bool, ProgramArguments>(false, default(ProgramArguments));
            }

            int numGallons;
            if (!int.TryParse(numGallonsArg, out numGallons))
            {
                System.Console.WriteLine($"Error: Number of gallons should be a valid positive integer: [{numGallonsArg}].");
                Environment.ExitCode = BadSize;
                return new Tuple<bool, ProgramArguments>(false, default(ProgramArguments));
            }

            return new Tuple<bool, ProgramArguments>(true, new ProgramArguments() { CsvFilePath = csvFilePath, NumGallons = numGallons });
        }

        static void PrintUsage()
        {
            System.Console.WriteLine(
                $"Usage:\r\n" +
                $"\t{Path.GetFileName(typeof(BpoConsoleClient).Assembly.CodeBase)} CSV_path Gallons_Number\r\n" +
                $"\twhere:\r\n" +
                $"\t\tCSV_path - path to the CSV file, containing the list of merchendises. Should be quoted, if there are some spaces inside;\r\n" +
                $"\t\tGallons_Number - an integer < 0, spicifying required number of gallons.\r\n"
            );
        }

        static IWindsorContainer ConfigureIoC()
        {
            var container = new WindsorContainer();
            container.Install(new[] { FromAssembly.This()});
            return container;
        }
    }
}
