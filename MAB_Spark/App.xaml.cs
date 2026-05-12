using System.Configuration;
using System.Data;
using System.Windows;

namespace MAB_Spark
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ana pencereyi başlat
            MainWindow mainWindow = new MainWindow();
            // Görünür olmadığı için Show() çağrılmaz
        }
    }
}

