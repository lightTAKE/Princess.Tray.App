using Princess.Tray.App.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Princess.Tray.App.Core
{
    public class ShutDownManager : IShutDownManager
    {
        private bool _running = false;
        private CancellationTokenSource _cancellationTokenSource;

        public ShutDownManager()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void ShutDown()
        {
            DisableIdleTracker();
            _running = true;
            ShutDownApi.ShutDown();
        }

        public async Task NightShutDown(int timeout)
        {
            DisableIdleTracker();
            _running = true;
            var action = GetShutdownAction(timeout);
            await Retry.Do(action, TimeSpan.FromSeconds((int)TimeConstant.Minute), _cancellationTokenSource.Token);
        }

        public void DisableIdleTracker()
        {
            if (_running)
            {
                _running = false;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        private Action GetShutdownAction(int timeout)
        {
            Action action = () =>
            {
                var idleSeconds = IdleTimeRetriever.GetSeconds();
                if (IsSleepyTime() && idleSeconds > timeout)
                {
                    ShutDown();
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
