using System.Threading.Tasks;

namespace Princess.Tray.App
{
    public interface IShutdownManager
    {
        void Shutdown(int delayForSeconds);

        Task NightShutdown(int delayForSeconds);

        void CancelShutdown();
    }
}
