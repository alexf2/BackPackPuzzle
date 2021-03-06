﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace BackPackOptimizer.Clients.Console
{
    /// <summary>
    /// A console client for the optimizer application. Implements the composition root.
    /// </summary>
    class BpoConsoleClient
    {
        const int BadArgs = -1;
        const int NoFile = -2;
        const int BadSize = -3;
        const int BadMinimizationFlag = -4;
        const int GenericException = -100;

        internal sealed class ProgramArguments
        {
            public string CsvFilePath; //the first command line argument
            public int NumGallons; //the second command line argument
            public bool SolveMinimizationTask; //is in app.config

            public Dictionary<string, object> ToDictionary() => new Dictionary<string, object>
            {
                //Dictionary keys should match to constructor arguments' names in the dependency tree
                {"filePath", CsvFilePath},
                {"backpackSize", NumGallons},
                {"solveMinimization", SolveMinimizationTask}
            };
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
                
                if (argsuments.Item1)
                {
                    //publishing arguments to IoC container: they are used in DynamicParameters, at lower levels of the dependencies tree 
                    container.Register(Component.For<ProgramArguments>().Instance(argsuments.Item2).LifestyleSingleton());

                    var stpw = new Stopwatch();

                    var app = container.Resolve<BpOptimizerApp>(argsuments.Item2.ToDictionary() /*passing scalar type dependencies to the roor object*/);
                    var execContext = container.Resolve<IExecutionContext>();
                    execContext.StartReading(); //starting keyboard control

                    try
                    {
                        System.Console.WriteLine($"Optimizing {Path.GetFileName(argsuments.Item2.CsvFilePath)} for {argsuments.Item2.NumGallons} gallons, using '{ConfigurationManager.AppSettings["algorithm"]}'");

                        stpw.Start();
                        var percases = app.Run().Result;
                        execContext.CancelSource.Cancel();
                        stpw.Stop();

                        System.Console.WriteLine($"Optimization took {stpw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}");
                        PrintResult(percases, argsuments.Item2.NumGallons, app.TotalGallons);
                    }
                    finally
                    {
                        container.Release(execContext);
                        container.Release(app);
                    }
                }
            }
        }

        #region Command line handling
        static void PrintResult(Purchases p, int requestedGallons, int totalGallons)
        {
            if (p.Merchendises.Count == 0)
                System.Console.WriteLine($"Suitable purchases are not found. Total gallons {totalGallons} is less than requested {requestedGallons}");
            else
            {
                System.Console.WriteLine(
                    $"Optimal purchases for {requestedGallons} gallons: {p.TotalGallons} with average price {p.AveragePriceOfGallon.ToString("#.##")} and total cost {p.TotalCost.ToString("#.##")} are:");
                foreach (var m in p.Merchendises)
                    System.Console.WriteLine($"\t{m}");
            }
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
            var solveMinimizationArg = ConfigurationManager.AppSettings["solve-mimization-task"];

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

            if (numGallons < 1)
            {
                System.Console.WriteLine($"Error: Number of gallons should be a valid positive integer greater than 0: [{numGallons}].");
                Environment.ExitCode = BadSize;
                return new Tuple<bool, ProgramArguments>(false, default(ProgramArguments));
            }

            bool solveMinimization;
            if (!bool.TryParse(solveMinimizationArg, out solveMinimization))
            {
                System.Console.WriteLine($"Error: solve-mimization-task appSettings config parameter has an invalid value: [{solveMinimizationArg}].");
                Environment.ExitCode = BadMinimizationFlag;
                return new Tuple<bool, ProgramArguments>(false, default(ProgramArguments));
            }

            return new Tuple<bool, ProgramArguments>(true, new ProgramArguments() { CsvFilePath = csvFilePath, NumGallons = numGallons, SolveMinimizationTask = solveMinimization });
        }
        #endregion Command line handling

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
