using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var prog = new Program();
            await prog.MakeBreakfast();

            System.Console.WriteLine("Eating...");
        }

        private async Task MakeBreakfast()
        {
            System.Console.WriteLine("Starting breakfast");

            var toastTokenSource = new CancellationTokenSource();
            var ct = toastTokenSource.Token;

            var toastTask = MakeToastWithButterAndJamAsync(3, ct);
            var baconTask = FryBacon();
            var eggsTask = ScrambleEggs();

            await Task.WhenAll(baconTask, eggsTask);

            //tired of waiting for the toast - pull the lever
            toastTokenSource.Cancel();

            System.Console.WriteLine(toastTask.Result);
        }

        async Task<Toast> MakeToastWithButterAndJamAsync(int number, CancellationToken ct)
        {
            var toast = await ToastBreadAsync(number, ct);
            ApplyButter(toast);
            ApplyJam(toast);
            return toast;
        }

        async Task<Toast> ToastBreadAsync(int slices, CancellationToken ct)
        {
            // Were we already cancelled?
            ct.ThrowIfCancellationRequested();

            System.Console.WriteLine("in the toaster...");
            var toastCycles = (int)Math.Ceiling(slices / 2.0);

            var task = Task.Run(() =>
            {
                for (int i = 0; i < toastCycles * 5; i++)
                {
                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();

                    Thread.Sleep(1000);
                }
            }, ct);

            Toast result;

            try
            {
                await task;
            }
            catch (OperationCanceledException ex)
            {
                System.Console.WriteLine($"Someone is getting impatient waiting for toast: {ex.Message}");
            }
            finally
            {
                result = new Toast();
            }

            return result;
        }

        void ApplyButter(Toast toast)
        {
            toast.Buttered = true;
            System.Console.WriteLine("Your toast is buttered");
        }

        void ApplyJam(Toast toast)
        {
            toast.HasJam = true;
            System.Console.WriteLine("Your toast now has jam");
        }

        async Task FryBacon()
        {
            System.Console.WriteLine("sizzle...");
            await Task.Delay(2000);
            System.Console.WriteLine("Bacon is done frying");
        }

        async Task ScrambleEggs()
        {
            System.Console.WriteLine("eggs are going...");
            await Task.Delay(3000);
            System.Console.WriteLine("Eggs are ready");
        }

    }
}
