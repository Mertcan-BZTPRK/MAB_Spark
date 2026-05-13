using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using MAB_Spark.Services;

namespace MAB_Spark
{
    public partial class MainWindow : Window
    {
        private DatabaseService _dbService;
        private TextHookService _textHookService;
        private SoundService _soundService;
        private SettingsWindow? _settingsWindow;
        private TaskbarIcon? _trayIcon;
        private bool _isDarkTheme = true;
        private bool _isClosing = false;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] InitializeComponent completed");

                // Sistem temasını algıla
                DetectSystemTheme();
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DetectSystemTheme completed");

                // Servisleri başlat
                _dbService = new DatabaseService();
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DatabaseService created");

                _soundService = new SoundService();
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] SoundService created");

                _textHookService = new TextHookService(_dbService, _soundService);
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] TextHookService created");

                // Pencereyi başlat
                this.Show();
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Window shown");

                // Sonra gizle
                this.Hide();
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Window hidden");

                // Hook başlat
                _textHookService.StartHooking((shortText, expandedText) =>
                {
                    Debug.WriteLine($"Expanded: {shortText} → {expandedText}");
                    UpdateStatus();
                });
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Hook started");

                UpdateStatus();

                // Tray icon'u hemen oluştur
                InitializeTrayIcon(); System.Threading.Thread.Sleep(100);

                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MainWindow constructor completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MainWindow constructor error: {ex.Message}");
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        private void InitializeTrayIcon()
        {
            try
            {
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] InitializeTrayIcon started");

                _trayIcon = new TaskbarIcon();
                _trayIcon.ToolTipText = "MAB_Spark - Text Expansion";
                _trayIcon.Visibility = Visibility.Visible;
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] TaskbarIcon created");

                // Context menu oluştur
                var contextMenu = new System.Windows.Controls.ContextMenu();
                contextMenu.Background = new SolidColorBrush(Color.FromArgb(255, 26, 26, 46));
                contextMenu.Foreground = new SolidColorBrush(Colors.White);

                var dashboardItem = new System.Windows.Controls.MenuItem { Header = "📊 Dashboard" };
                dashboardItem.Click += (s, args) => ShowWindow_Click(s, args);

                var settingsItem = new System.Windows.Controls.MenuItem { Header = "⚙️ Settings" };
                settingsItem.Click += (s, args) => OpenSettings_Click(s, args);

                var separator = new System.Windows.Controls.Separator();
                separator.Background = new SolidColorBrush(Color.FromArgb(80, 99, 102, 241));

                var exitItem = new System.Windows.Controls.MenuItem { Header = "❌ Exit" };
                exitItem.Click += (s, args) => ExitApp_Click(s, args);

                contextMenu.Items.Add(dashboardItem);
                contextMenu.Items.Add(settingsItem);
                contextMenu.Items.Add(separator);
                contextMenu.Items.Add(exitItem);

                _trayIcon.ContextMenu = contextMenu;

                // Tray icon double-click event
                _trayIcon.TrayMouseDoubleClick += (s, args) =>
                {
                    this.Show();
                    this.Activate();
                    this.Focus();
                    this.WindowState = WindowState.Normal;
                };

                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] InitializeTrayIcon completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] InitializeTrayIcon error: {ex.Message}");
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] StackTrace: {ex.StackTrace}");
            }
        }

        private void DetectSystemTheme()
        {
            try
            {
                var registry = Microsoft.Win32.Registry.CurrentUser;
                var key = registry.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                _isDarkTheme = value == null || (int)value == 0;
            }
            catch
            {
                _isDarkTheme = true;
            }

            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (_isDarkTheme)
            {
                ThemeBtn.Content = "☀️";
                this.Resources["BackgroundColor"] = Color.FromArgb(204, 15, 15, 30);
                this.Resources["PrimaryAccent"] = Color.FromArgb(255, 99, 102, 241);
                this.Resources["TextColor"] = Colors.White;
            }
            else
            {
                ThemeBtn.Content = "🌙";
                this.Resources["BackgroundColor"] = Color.FromArgb(242, 245, 245, 245);
                this.Resources["PrimaryAccent"] = Color.FromArgb(255, 99, 102, 241);
                this.Resources["TextColor"] = Color.FromArgb(255, 31, 41, 55);
            }
        }

        private void UpdateStatus()
        {
            var shortcuts = _dbService.GetAllShortcuts();
            ShortcutCountBlock.Text = shortcuts.Count.ToString();
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            ApplyTheme();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_settingsWindow == null || !_settingsWindow.IsVisible)
            {
                _settingsWindow = new SettingsWindow(_dbService);
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.Focus();
                _settingsWindow.Activate();
            }
        }

        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.Activate();
            this.Focus();
            this.WindowState = WindowState.Normal;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        private void ExitApplication()
        {
            System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ExitApplication called");
            _isClosing = true;
            _textHookService.StopHooking();
            if (_settingsWindow != null)
            {
                _settingsWindow.Close();
            }
            if (_trayIcon != null)
            {
                _trayIcon.Dispose();
            }
            Application.Current.Shutdown();
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch
                {
                    // Ignore
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] OnClosed called");
            base.OnClosed(e);
            if (!_isClosing)
            {
                _textHookService.StopHooking();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Window_Closing called");
                // Normal Close() çağrısı değilse (örn. Alt+F4), pencereyi gizle
                if (!_isClosing)
                {
                    System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Window_Closing - hiding window instead");
                    e.Cancel = true;
                    this.Hide();
                }
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Window_Closing completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Window_Closing error: {ex.Message}");
                System.Diagnostics.Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] StackTrace: {ex.StackTrace}");
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }
    }
}

