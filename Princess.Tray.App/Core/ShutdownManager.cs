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
            var currentHour = DateTimeOffset.Now.Hour;
            return 23 <= currentHour || currentHour <= 7;
        }
    }
}
