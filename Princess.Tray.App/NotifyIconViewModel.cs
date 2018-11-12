using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Princess.Tray.App
{
    public class NotifyIconViewModel
    {
        private bool _running = false;
        private CancellationTokenSource _cancellationTokenSource;

        public NotifyIconViewModel()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(async () => await NightShutDown(2700));
        }

        public ICommand InstantShutdownCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        CancelLastShutDownCommand();
                        ShutDown(0);
                    }
                };
            }
        }

        public ICommand NightShutDown60Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () =>
                    {
                        CancelLastShutDownCommand();
                        await NightShutDown(3600);
                    }
                };
            }
        }

        public ICommand NightShutDown45Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () =>
                    {
                        CancelLastShutDownCommand();
                        await NightShutDown(2700);
                    }
                };
            }
        }

        public ICommand NightShutDown30Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () =>
                    {
                        CancelLastShutDownCommand();
                        await NightShutDown(1800);
                    }
                };
            }
        }

        public ICommand CancelShutDownCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => CancelLastShutDownCommand()
                };
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        private void CancelLastShutDownCommand()
        {
            if (_running)
            {
                _running = false;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        private void ShutDown(int seconds)
        {
            Process.Start("shutdown", $"/s /t {seconds}");
        }

        private async Task NightShutDown(int seconds)
        {
            _running = true;
            var retryCondition = GetRetyCondition();
            var action = GetAction(seconds);
            await Retry.Do(action, retryCondition, TimeSpan.FromMinutes(1), _cancellationTokenSource.Token);
        }

        private Func<bool> GetRetyCondition()
        {
            Func<bool> retryCondition = () =>
            {
                var today = DateTimeOffset.Now.Date;
                var yesterday = today.AddDays(-1);
                var sleepTimeStart = new DateTimeOffset(yesterday.Year, yesterday.Month, yesterday.Day, 23, 0, 0, DateTimeOffset.Now.Offset);
                var sleepTimeEnd = new DateTimeOffset(today.Year, today.Month, today.Day, 7, 0, 0, DateTimeOffset.Now.Offset);

                var condition = sleepTimeStart <= DateTimeOffset.Now && DateTimeOffset.Now <= sleepTimeEnd;

                return condition; 
            };

            return retryCondition;
        }

        private Action GetAction(int seconds)
        {
            Action action = () =>
            {
                var idleSeconds = IdleTimeRetriever.GetSeconds();
                Console.WriteLine($"Greetings, sir! Idle time: {idleSeconds}");
                if (idleSeconds > seconds)
                {
                    ShutDown(30);
                }
            };

            return action;
        }
    }
}