# 🚀 MAB_Spark Kurulum & Geliştirme Rehberi

## Hızlı Başlangıç

### Sistem Gereksinimleri
- Windows 10, 11 veya 12
- .NET 10.0 SDK (veya daha yüksek)
- Visual Studio 2022 Community (veya VS Code)

### Adım 1: Repository'yi Clone'la

```powershell
git clone https://github.com/Mertcan-BZTPRK/MAB_Spark.git
cd MAB_Spark
```

### Adım 2: Paketleri Restore Et

```powershell
dotnet restore
```

### Adım 3: Projeyi Derle

```powershell
dotnet build
```

### Adım 4: Uygulamayı Çalıştır

```powershell
dotnet run --project MAB_Spark/MAB_Spark.csproj
```

Veya doğrudan executable'ı çalıştır:

```powershell
./MAB_Spark/bin/Debug/net10.0-windows/MAB_Spark.exe
```

## 🧪 Test Kısaltmaları Ekle

Veritabanına örnek kısaltmalar eklemek için:

```powershell
# PowerShell script ile
& ./tests/add_sample_shortcuts.ps1 -Action "add"

# Kısaltmaları listele
& ./tests/add_sample_shortcuts.ps1 -Action "list"

# Tümünü sil (dikkat!)
& ./tests/add_sample_shortcuts.ps1 -Action "clear"
```

## 📁 Proje Yapısı

```
MAB_Spark/
├── Models/                      # Veri modelleri
│   └── Shortcut.cs             # Kısaltma sınıfı
├── Services/                    # İş mantığı servisleri
│   ├── DatabaseService.cs       # SQLite CRUD işlemleri
│   ├── TextHookService.cs       # Global metin hook (system-wide monitoring)
│   ├── SoundService.cs          # Ses oynatma
│   └── AutoStartService.cs      # Windows Registry'ye yazma
├── Resources/                   # Kaynaklar
│   └── icon.png                 # Uygulama ikonu
├── SettingsWindow.xaml(.cs)    # Ayarlar penceresi (XAML + Code-behind)
├── MainWindow.xaml(.cs)        # System Tray ana penceresi
├── App.xaml(.cs)               # WPF uygulaması başlangıcı
├── MAB_Spark.csproj            # Proje dosyası (.NET 10)
└── AssemblyInfo.cs             # Montaj bilgileri

tests/
└── add_sample_shortcuts.ps1    # Test veri ekleme betiği

README.md                         # Kullanıcı rehberi
SETUP.md                          # Bu dosya
```

## 🛠️ Mimari

### TextHookService (Kalp)
- **Amacı**: Sistem genelinde yazılan metni izle
- **Teknik**: Windows Low-Level Keyboard Hook (WH_KEYBOARD_LL)
- **İşlem**: Yazarken kelimeleri topla → Veritabanında arama → Eşleşme bulunursa yapıştır

### DatabaseService (Bellek)
- **Amacı**: SQLite veritabanını yönet
- **Tablosu**: `Shortcuts` (Id, ShortText, ExpandedText, IsEnabled, CreatedAt, UpdatedAt)
- **Konum**: `%LOCALAPPDATA%\MAB_Spark\shortcuts.db`

### SettingsWindow (Arayüz)
- **TabItem 1**: Kısaltmalar (DataGrid + CRUD)
- **TabItem 2**: Ayarlar (Auto-start checkbox + Info)

### MainWindow (System Tray)
- **İçeriği**: Neredeyse boş (pencere gizli)
- **Görevi**: System Tray ikonu ve Context Menu'yü barındır
- **Sağ Tık**: "⚙️ Ayarlar" ve "❌ Çıkış" seçenekleri

## 🔧 Geliştirme İpuçları

### Debugging
Visual Studio'da `F5` veya:
```powershell
dotnet run
```

### NuGet Paketlerini Güncelle
```powershell
dotnet add package [PackageName] --version [Version]
```

### Veritabanını Sıfırla
```powershell
Remove-Item $env:LOCALAPPDATA\MAB_Spark\shortcuts.db
```

### Database Viewer
SQLite veritabanını görmek için:
- **SQLiteBrowser**: https://sqlitebrowser.org/
- **DBeaver**: https://dbeaver.io/

## 🐛 Bilinen Sorunlar

1. **Hook kesintiye uğrayabiliyor** - Alt+Tab veya diğer sistem modalları sırasında
2. **Veritabanı kilitli hata** - Birden fazla instance çalışıyorsa (fix: mutex ekle)
3. **Büyük metinlerde yavaşlık** - SendKeys senkron işlem yapıyor

## 📋 TODO (İleride Yapılacaklar)

- [ ] Multiple monitor support
- [ ] Custom hotkeyler
- [ ] Cloud sync (iCloud / OneDrive)
- [ ] JSON export/import
- [ ] Kısaltma kategorileri
- [ ] Statistics (most used shortcuts)
- [ ] Regex pattern support
- [ ] Türkçe UI (tamamlandi ✅)
- [ ] Multilingual UI
- [ ] Modern UI (WinUI 3 migration)

## 🔐 Güvenlik Notları

- ⚠️ Hook işlemi yönetici izni gerektirmez (güvenli)
- 💾 Kısaltmalar lokal olarak saklanır, cloud'a gelmez
- 🔒 Registry erişimi sadece kendi kullanıcı hesabı için

## 📞 Destek

Sorularınız varsa:
1. GitHub Issues açın
2. Discussions sekmesini kontrol edin
3. Pull Request gönderin

---

**Happy Text Expansion! 🎉**
