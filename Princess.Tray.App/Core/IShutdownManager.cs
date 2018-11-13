using System.Threading.Tasks;

namespace Princess.Tray.App.Core
{
    public interface IShutdownManager
    {
        void Shutdown(int delayForSeconds);

        Task NightShutdown(int delayForSeconds);

        void CancelShutdown();
    }
}
