using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MAB_Spark.Services;

namespace MAB_Spark
{
    public partial class MainWindow : Window
    {
        private DatabaseService _dbService;
        private TextHookService _textHookService;
        private SoundService _soundService;
        private SettingsWindow? _settingsWindow;
        private bool _isDarkTheme = true;

        public MainWindow()
        {
            InitializeComponent();

            // Sistem temasını algıla
            DetectSystemTheme();

            // Servisleri başlat
            _dbService = new DatabaseService();
            _soundService = new SoundService();
            _textHookService = new TextHookService(_dbService, _soundService);

            // Pencereyi gizle
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;

            // Hook başlat
            _textHookService.StartHooking((shortText, expandedText) =>
            {
                Debug.WriteLine($"Expanded: {shortText} → {expandedText}");
                UpdateStatus();
            });

            UpdateStatus();
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
                // Dark theme colors
                this.Resources["BackgroundColor"] = Color.FromArgb(204, 15, 15, 30);
                this.Resources["PrimaryAccent"] = Color.FromArgb(255, 99, 102, 241);
                this.Resources["TextColor"] = Colors.White;
            }
            else
            {
                ThemeBtn.Content = "🌙";
                // Light theme colors
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
            _textHookService.StopHooking();
            if (_settingsWindow != null)
            {
                _settingsWindow.Close();
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
            base.OnClosed(e);
            _textHookService.StopHooking();
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
