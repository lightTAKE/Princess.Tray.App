using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Princess.Tray.App.Core;
using Princess.Tray.App.Helpers;

namespace Princess.Tray.App
{
    public class NotifyIconViewModel
    {
        private ShutDownManager _shutDownManager;

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

        public ICommand NightShutDown60Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () => await _shutDownManager.NightShutDown((int)TimeConstant.Hour)
                };
            }
        }

        public ICommand NightShutDown45Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () => await _shutDownManager.NightShutDown((int)TimeConstant.Minutes45)
                };
            }
        }

        public ICommand NightShutDown30Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () => await _shutDownManager.NightShutDown((int)TimeConstant.Minutes30)
                };
            }
        }

        public ICommand DisableSleepyTimeTracker
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => _shutDownManager.DisableIdleTracker()
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
    }
}