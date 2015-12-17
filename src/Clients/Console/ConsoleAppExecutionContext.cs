using System;
using System.Threading;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;

namespace BackPackOptimizer.Clients.Console
{
    public sealed class ConsoleAppExecutionContext: IExecutionContext, IProgress<ProgressInfo>
    {        
        readonly CancellationTokenSource _cancelScr;

        public ConsoleAppExecutionContext()
        {
            _cancelScr = new CancellationTokenSource();
        }

        public IProgress<ProgressInfo> ProgressCallback => this;

        public CancellationToken Token => _cancelScr.Token;

        public void Report(ProgressInfo value)
        {
            long percent = value.TotalIterations < 0 ? 100 / value.TotalIterations * value.Iteration:100;
            System.Console.WriteLine($"{percent}%: {value.Iteration} of {value.TotalIterations}");
        }

        public void StartReading()
        {
            while (true)
            {
                var keyInfo = System.Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.X) break;
            }

            var cancelWatcher = new ConsoleCancelEventHandler((sender, args) => {
                _cancelScr.Cancel();
                args.Cancel = true;
            });
            System.Console.CancelKeyPress += cancelWatcher;

            Task.Factory.StartNew(() =>
            {                
                while (!Token.IsCancellationRequested)
                {
                    if (System.Console.KeyAvailable && System.Console.ReadKey().Key == ConsoleKey.Q)
                        _cancelScr.Cancel();
                    else
                        Task.Delay(200, Token).Wait();
                }

                
            }).ContinueWith(delegate { System.Console.CancelKeyPress -= cancelWatcher; });
        }
    }
}
