using System.Configuration;
using System.Data;
using System.Windows;
using System.Diagnostics;
using System.Threading;

namespace MAB_Spark
{
    public partial class App : Application
    {
        private Mutex? _mutex;
        private const string MutexName = "MAB_Spark_SingleInstance";

        protected override void OnStartup(StartupEventArgs e)
        {
            // Single instance kontrolü
            _mutex = new Mutex(true, MutexName, out bool isNewInstance);

            if (!isNewInstance)
            {
                // Zaten bir instance açık, kapat
                this.Shutdown();
                return;
            }

            base.OnStartup(e);

            // Ana pencereyi başlat
            MainWindow mainWindow = new MainWindow();
            // Görünür olmadığı için Show() çağrılmaz
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}

