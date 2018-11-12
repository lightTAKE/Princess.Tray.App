using System;
using System.Threading;
using System.Threading.Tasks;

namespace Princess.Tray.App
{
    public static class Retry
    {
        public static async Task Do(Action action, Func<bool> retryCondition,TimeSpan retryInterval, CancellationToken cancellationToken)
        {
            while (retryCondition())
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
