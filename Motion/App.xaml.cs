using Motion.Core;
using System.Windows;
using System.Windows.Interop;

namespace Motion
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MutexName = "Motion";
        private Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            mutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                // 如果已经存在同名的Mutex，说明已有应用程序实例在运行
                MessageBox.Show("应用程序已经在运行！");
                Shutdown();
            }
            else
            {
                // 正常启动应用程序
                base.OnStartup(e);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
            base.OnExit(e);
        }
    }
}