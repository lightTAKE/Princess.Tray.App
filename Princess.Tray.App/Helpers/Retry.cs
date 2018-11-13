using System;
using System.Threading;
using System.Threading.Tasks;

namespace Princess.Tray.App.Helpers
{
    public static class Retry
    {
        public static async Task Do(Action action, TimeSpan retryInterval, CancellationToken cancellationToken)
        {
            while (true)
            {
                action();

                await Task.Delay(retryInterval);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
