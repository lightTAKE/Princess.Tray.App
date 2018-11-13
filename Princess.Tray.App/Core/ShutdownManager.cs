using Princess.Tray.App.Helpers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Princess.Tray.App.Core
{
    public class ShutdownManager : IShutdownManager
    {
        private bool _running = false;
        private CancellationTokenSource _cancellationTokenSource;

        public ShutdownManager()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Shutdown(int delayForSeconds)
        {
            CancelShutdown();
            _running = true;
            Process.Start("shutdown", $"/s /t {delayForSeconds}");
        }

        public async Task NightShutdown(int delayForSeconds)
        {
            CancelShutdown();
            _running = true;
            var action = GetShutdownAction(delayForSeconds);
            await Retry.Do(action, TimeSpan.FromSeconds((int)TimeConstant.Minute), _cancellationTokenSource.Token);
        }

        public void CancelShutdown()
        {
            if (_running)
            {
                _running = false;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        private Action GetShutdownAction(int delayForSeconds)
        {
            Action action = () =>
            {
                var idleSeconds = IdleTimeRetriever.GetSeconds();
                if (IsSleepyTime() && idleSeconds > delayForSeconds)
                {
                    Shutdown((int)TimeConstant.Seconds30);
                }
            };

            return action;
        }

        private bool IsSleepyTime()
        {
            var today = DateTimeOffset.Now.Date;
            var yesterday = today.AddDays(-1);
            var sleepyTimeStart = new DateTimeOffset(yesterday.Year, yesterday.Month, yesterday.Day, 23, 0, 0, DateTimeOffset.Now.Offset);
            var sleepyTimeEnd = new DateTimeOffset(today.Year, today.Month, today.Day, 7, 0, 0, DateTimeOffset.Now.Offset);

            return sleepyTimeStart <= DateTimeOffset.Now && DateTimeOffset.Now <= sleepyTimeEnd;
        }
    }
}
