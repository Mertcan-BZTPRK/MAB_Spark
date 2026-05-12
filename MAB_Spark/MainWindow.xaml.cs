using System;
using System.Diagnostics;
using System.Windows;
using MAB_Spark.Services;

namespace MAB_Spark
{
    public partial class MainWindow : Window
    {
        private DatabaseService _dbService;
        private TextHookService _textHookService;
        private SoundService _soundService;
        private SettingsWindow? _settingsWindow;

        public MainWindow()
        {
            InitializeComponent();

            // Servisleri başlat
            _dbService = new DatabaseService();
            _soundService = new SoundService();
            _textHookService = new TextHookService(_dbService, _soundService);

            // Pencereyi gizle
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;

            // Başlangıçta hook başlat
            _textHookService.StartHooking((shortText, expandedText) =>
            {
                System.Diagnostics.Debug.WriteLine($"Genişletildi: {shortText} → {expandedText}");
            });
        }

        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_settingsWindow == null || !_settingsWindow.IsVisible)
            {
                _settingsWindow = new SettingsWindow(_dbService);
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.Focus();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _textHookService.StopHooking();
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _textHookService.StopHooking();
        }
    }
}
