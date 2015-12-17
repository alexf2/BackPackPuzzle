using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace BackPackOptimizer.Clients.Console
{
    class BpoConsoleClient
    {
        const int BadArgs = -1;
        const int NoFile = -2;
        const int BadSize = -3;

        struct ProgramArguments
        {
            public string CsvFilePath;
            public int NumGallons;
        }

        static IWindsorContainer _container;

        static void Main(string[] args)
        {
            ConfigureIoC();

            var argsuments = ReadArguments(args);

            if (argsuments.Item1)
            {
                System.Console.WriteLine($"{argsuments.Item2.CsvFilePath}, {argsuments.Item2.NumGallons}");
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

        static void ConfigureIoC()
        {
            _container = new WindsorContainer();
            _container.Install(new[] { FromAssembly.This()});
            
        }
    }
}
