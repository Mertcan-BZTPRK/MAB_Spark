using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MAB_Spark.Models;
using MAB_Spark.Services;

namespace MAB_Spark
{
    public partial class SettingsWindow : Window
    {
        private DatabaseService _dbService;
        private AutoStartService _autoStartService;
        private ObservableCollection<Shortcut> _shortcuts;

        public SettingsWindow(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            _autoStartService = new AutoStartService();
            _shortcuts = new ObservableCollection<Shortcut>();

            LoadShortcuts();
            UpdateUI();
        }

        private void LoadShortcuts()
        {
            _shortcuts.Clear();
            var shortcuts = _dbService.GetAllShortcuts();
            foreach (var shortcut in shortcuts)
            {
                _shortcuts.Add(shortcut);
            }
            ShortcutsGrid.ItemsSource = _shortcuts;
        }

        private void UpdateUI()
        {
            // Auto-start durumunu kontrol et
            AutoStartCheckBox.IsChecked = _autoStartService.IsAutoStartEnabled();

            // Kısaltma sayısını göster
            ShortcutsCountText.Text = $"Toplam kısaltma: {_shortcuts.Count}";

            // Veritabanı yolunu göster
            DatabasePathText.Text = $"Veritabanı: {_dbService.GetDatabasePath()}";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var shortText = ShortTextBox.Text?.Trim();
            var expandedText = ExpandedTextBox.Text?.Trim();

            if (string.IsNullOrEmpty(shortText) || string.IsNullOrEmpty(expandedText))
            {
                MessageBox.Show("Lütfen hem kısaltma hem de gerçek metni girin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Çift olup olmadığını kontrol et
            if (_shortcuts.Any(s => s.ShortText == shortText))
            {
                MessageBox.Show("Bu kısaltma zaten mevcut.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
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

                ShortcutsCountText.Text = $"Toplam kısaltma: {_shortcuts.Count}";
                MessageBox.Show($"'{shortText}' → '{expandedText}' başarıyla eklendi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShortcutsGrid.SelectedItem is Shortcut selected)
            {
                var result = MessageBox.Show($"'{selected.ShortText}' silinsin mi?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.DeleteShortcut(selected.Id);
                        _shortcuts.Remove(selected);
                        ShortcutsCountText.Text = $"Toplam kısaltma: {_shortcuts.Count}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Silme hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir kısaltma seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void RefreshShortcuts()
        {
            LoadShortcuts();
            UpdateUI();
        }
    }
}
