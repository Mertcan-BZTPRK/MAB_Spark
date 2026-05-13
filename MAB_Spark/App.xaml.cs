using System.Configuration;
using System.Data;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace MAB_Spark
{
    public partial class App : Application
    {
        private Mutex? _mutex;
        private const string MutexName = "MAB_Spark_SingleInstance";
        private StreamWriter? _logWriter;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Log output'u file'a yaz
                string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "mab_spark_debug.log");
                _logWriter = new StreamWriter(logPath, true);
                _logWriter.AutoFlush = true;
                Trace.Listeners.Add(new TextWriterTraceListener(_logWriter));

                Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] App.OnStartup started");

                // Exception handling ve logging
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var ex = args.ExceptionObject as Exception;
                    Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] FATAL ERROR: {ex?.Message}");
                    _logWriter?.Flush();
                };

                DispatcherUnhandledException += (sender, args) =>
                {
                    Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DISPATCHER ERROR: {args.Exception.Message}");
                    _logWriter?.Flush();
                    args.Handled = true;
                };

                // Single instance kontrolü
                _mutex = new Mutex(true, MutexName, out bool isNewInstance);
                Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Mutex check: isNewInstance={isNewInstance}");

                if (!isNewInstance)
                {
                    Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Another instance is running. Shutting down.");
                    this.Shutdown();
                    return;
                }

                base.OnStartup(e);
                Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] base.OnStartup completed");

                // Ana pencereyi oluştur
                MainWindow mainWindow = new MainWindow();
                this.MainWindow = mainWindow;  // WPF application lifetime'ı kontrol etmek için
                Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MainWindow created successfully");

                // Application'ı çalışmaya devam etme - tray'de yaşayacak
                // Dispatcher'a bir dummy operation ekle ki app loop'da kalsın
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () =>
                {
                    Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Dispatcher loop active");
                });

                Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] App.OnStartup completed successfully");
                _logWriter?.Flush();
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] OnStartup Exception: {ex.Message}");
                Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] StackTrace: {ex.StackTrace}");
                _logWriter?.Flush();
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] App.OnExit called");
            _logWriter?.Flush();
            _mutex?.Dispose();
            base.OnExit(e);
            _logWriter?.Close();
        }
    }
}

