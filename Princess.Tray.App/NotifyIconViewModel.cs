using Princess.Tray.App.Annotations;
using Princess.Tray.App.Core;
using Princess.Tray.App.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Princess.Tray.App
{
    public class NotifyIconViewModel : INotifyPropertyChanged
    {
        private readonly ShutDownManager _shutDownManager;
        private bool _isNightShutDownAfter60MinutesActive;
        private bool _isNightShutDownAfter45MinutesActive;
        private bool _isNightShutDownAfter30MinutesActive;

        public NotifyIconViewModel()
        {
            _shutDownManager = new ShutDownManager();
            Task.Factory.StartNew(async () => await _shutDownManager.NightShutDown((int)TimeConstant.Minutes45));
        }

        public ICommand InstantShutDownCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => _shutDownManager.ShutDown()
                };
            }
        }

        public bool IsNightShutDownAfter60MinutesActive
        {
            get => _isNightShutDownAfter60MinutesActive;
            set
            {
                if (value == _isNightShutDownAfter60MinutesActive) return;
                _isNightShutDownAfter60MinutesActive = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSleepyTimeTrackerActive));

                if (value)
                {
                    _ = _shutDownManager.NightShutDown((int)TimeConstant.Hour);
                }
            }
        }

        public bool IsNightShutDownAfter45MinutesActive
        {
            get => _isNightShutDownAfter45MinutesActive;
            set
            {
                if (value == _isNightShutDownAfter45MinutesActive) return;
                _isNightShutDownAfter45MinutesActive = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSleepyTimeTrackerActive));

                if (value)
                {
                    _ = _shutDownManager.NightShutDown((int)TimeConstant.Minutes45);
                }
            }
        }

        public bool IsNightShutDownAfter30MinutesActive
        {
            get => _isNightShutDownAfter30MinutesActive;
            set
            {
                if (value == _isNightShutDownAfter30MinutesActive) return;
                _isNightShutDownAfter30MinutesActive = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSleepyTimeTrackerActive));

                if (value)
                {
                    _ = _shutDownManager.NightShutDown((int)TimeConstant.Minutes30);
                }
            }
        }

        public bool IsSleepyTimeTrackerActive =>
            IsNightShutDownAfter30MinutesActive
            || IsNightShutDownAfter45MinutesActive
            || IsNightShutDownAfter60MinutesActive;

        public ICommand DisableSleepyTimeTracker
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = DisableTracker
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DisableTracker()
        {
            IsNightShutDownAfter30MinutesActive = false;
            IsNightShutDownAfter45MinutesActive = false;
            IsNightShutDownAfter60MinutesActive = false;

            _shutDownManager.DisableIdleTracker();
        }
    }
}