using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Princess.Tray.App
{
    public class NotifyIconViewModel
    {
        private ShutdownManager _shutdownManager;

        public NotifyIconViewModel()
        {
            _shutdownManager = new ShutdownManager();
            Task.Factory.StartNew(async () => await _shutdownManager.NightShutdown((int)TimeConstant.Minutes45));
        }

        public ICommand InstantShutdownCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => _shutdownManager.Shutdown(0)
                };
            }
        }

        public ICommand NightShutDown60Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () => await _shutdownManager.NightShutdown((int)TimeConstant.Hour)
                };
            }
        }

        public ICommand NightShutDown45Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () => await _shutdownManager.NightShutdown((int)TimeConstant.Minutes45)
                };
            }
        }

        public ICommand NightShutDown30Command
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = async () => await _shutdownManager.NightShutdown((int)TimeConstant.Minutes30)
                };
            }
        }

        public ICommand CancelShutDownCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => _shutdownManager.CancelShutdown()
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