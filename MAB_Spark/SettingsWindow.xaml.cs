using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MAB_Spark.Models;
using MAB_Spark.Services;

namespace MAB_Spark
{
    public partial class SettingsWindow : Window
    {
        private DatabaseService _dbService;
        private AutoStartService _autoStartService;
        private ObservableCollection<Shortcut> _shortcuts;
        private bool _isDarkTheme = true;

        public SettingsWindow(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            _autoStartService = new AutoStartService();
            _shortcuts = new ObservableCollection<Shortcut>();

            DetectSystemTheme();
            LoadShortcuts();
            UpdateUI();
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
                // Dark Theme
                this.Resources["BackgroundColor"] = Color.FromArgb(204, 15, 15, 30);
                this.Resources["PrimaryAccent"] = Color.FromArgb(255, 99, 102, 241);
                this.Resources["SecondaryAccent"] = Color.FromArgb(255, 236, 72, 153);
                this.Resources["TertiaryAccent"] = Color.FromArgb(255, 6, 182, 212);
                this.Resources["TextColor"] = Colors.White;
                this.Resources["TextMuted"] = Color.FromArgb(255, 156, 163, 175);
                ThemeToggleButton.Content = "☀️";
            }
            else
            {
                // Light Theme
                this.Resources["BackgroundColor"] = Color.FromArgb(242, 245, 245, 245);
                this.Resources["PrimaryAccent"] = Color.FromArgb(255, 99, 102, 241);
                this.Resources["SecondaryAccent"] = Color.FromArgb(255, 236, 72, 153);
                this.Resources["TertiaryAccent"] = Color.FromArgb(255, 6, 182, 212);
                this.Resources["TextColor"] = Color.FromArgb(255, 31, 41, 55);
                this.Resources["TextMuted"] = Color.FromArgb(255, 107, 114, 128);
                ThemeToggleButton.Content = "🌙";
            }
        }

        private void LoadShortcuts()
        {
            _shortcuts.Clear();
            var shortcuts = _dbService.GetAllShortcuts();
            foreach (var shortcut in shortcuts)
            {
                _shortcuts.Add(shortcut);
            }
            // DataGrid binding'i sağla
            if (ShortcutsGrid != null)
            {
                ShortcutsGrid.ItemsSource = null;
                ShortcutsGrid.ItemsSource = _shortcuts;
            }
        }

        private void UpdateUI()
        {
            AutoStartCheckBox.IsChecked = _autoStartService.IsAutoStartEnabled();
            ShortcutsCountText.Text = $"Total Shortcuts: {_shortcuts.Count}";
            DatabasePathText.Text = $"Database: {_dbService.GetDatabasePath()}";
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            ApplyTheme();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var shortText = ShortTextBox.Text?.Trim();
            var expandedText = ExpandedTextBox.Text?.Trim();

            if (string.IsNullOrEmpty(shortText) || string.IsNullOrEmpty(expandedText))
            {
                MessageBox.Show("Please enter both shortcut and expanded text.", "Warning", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_shortcuts.Any(s => s.ShortText == shortText))
            {
                MessageBox.Show("This shortcut already exists.", "Error", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var shortcut = new Shortcut
                {
                    ShortText = shortText,
                    ExpandedText = expandedText,
                    IsEnabled = true
                };

                _dbService.AddShortcut(shortcut);
                _shortcuts.Add(shortcut);

                ShortTextBox.Clear();
                ExpandedTextBox.Clear();
                ShortTextBox.Focus();

                ShortcutsCountText.Text = $"Total Shortcuts: {_shortcuts.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShortcutsGrid.SelectedItem is Shortcut selected)
            {
                var result = MessageBox.Show($"Delete '{selected.ShortText}'?", "Confirm", 
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.DeleteShortcut(selected.Id);
                        _shortcuts.Remove(selected);
                        ShortcutsCountText.Text = $"Total Shortcuts: {_shortcuts.Count}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Delete error: {ex.Message}", "Error", 
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AutoStartCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _autoStartService.EnableAutoStart();
        }

        private void AutoStartCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _autoStartService.DisableAutoStart();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
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
                    // Ignore drag error
                }
            }
        }

        public void RefreshShortcuts()
        {
            LoadShortcuts();
            UpdateUI();
        }
    }
}
