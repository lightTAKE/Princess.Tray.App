using System.Threading.Tasks;

namespace Princess.Tray.App.Core
{
    public interface IShutDownManager
    {
        void ShutDown();

        Task NightShutDown(int timeout);

        void DisableIdleTracker();
    }
}
