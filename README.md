# 🚀 MAB_Spark - TextBlaze Benzeri Metin Genişletme Uygulaması

MAB_Spark, Windows sistemlerinizde arka planda çalışan, TextBlaze benzeri metin genişletme uygulamasıdır. Kısaltılmış metinleri otomatik olarak gerçek metinlere dönüştüren güçlü bir araçtır.

## ✨ Özellikler

- ✅ **Arka Planda Çalışma** - Sistem tepsisinde gizli çalışır
- ✅ **Sistem Genelinde İzleme** - Yazarken anlık metin kısaltması izleme
- ✅ **SQLite Veritabanı** - Kısaltmaları güvenli şekilde saklama
- ✅ **Yönetim Paneli** - Kısaltmaları ekle/düzenle/sil
- ✅ **Otomatik Genişletme** - Yazarken boşluk/Enter'a bastığında otomatik dönüşüm
- ✅ **Ses Bildirimi** - Başarılı genişletme sesi
- ✅ **Büyük/Küçük Harf Duyarlı** - BTW ≠ btw
- ✅ **Windows Auto-Start** - İsteğe bağlı otomatik başlangıç
- ✅ **Kolay Kullanım** - Sistem tray'de sağ tık → Ayarlar

## 📥 Kurulum

### Gereksinimler
- Windows 10/11/12
- .NET 10.0 Runtime

### Yükleme

1. **Release'den indir** (henüz yayınlanmadı)
   ```powershell
   # veya kaynak koddan derle
   git clone https://github.com/Mertcan-BZTPRK/MAB_Spark.git
   cd MAB_Spark
   dotnet build
   dotnet run
   ```

2. **Uygulamayı Başlat**
   ```powershell
   ./MAB_Spark/bin/Release/net10.0-windows/MAB_Spark.exe
   ```

## 🎯 Kullanım

### Kısaltma Ekleme

1. **Sistem Tray'de Simgeye Sağ Tık** yapın
2. **"⚙️ Ayarlar"** seçeneğine tıklayın
3. **"📝 Kısaltmalar"** sekmesine gidin
4. **Yeni kısaltma bölümünde:**
   - Sol tarafa: **Kısaltmayı** girin (örn: `btw`)
   - Sağ tarafa: **Gerçek metni** girin (örn: `by the way`)
5. **"➕ Ekle"** butonuna tıklayın

### Kullanım Örneği

Kısaltmalar ekledikten sonra:

```
Yazarken: "btw bu harika" yazıp boşluk basarsanız
Otomatik: "by the way bu harika" olur
```

### Ayarlar

**"⚙️ Ayarlar"** sekmesinde:
- ☑️ **Windows Başlangıcında Çalıştır** - Otomatik başlatma
- 📊 **Durum Bilgisi** - Kısaltma sayısı ve veritabanı yolu
- ℹ️ **Hakkında** - Versiyon bilgisi

## 🗂️ Dosya Yapısı

```
MAB_Spark/
├── Models/
│   └── Shortcut.cs                 # Kısaltma veri modeli
├── Services/
│   ├── DatabaseService.cs          # SQLite işlemleri
│   ├── TextHookService.cs          # Sistem metin izleme
│   ├── SoundService.cs             # Ses çalıştırma
│   └── AutoStartService.cs         # Windows başlangıç entegrasyonu
├── Resources/
│   └── icon.png                    # Uygulama ikonu
├── SettingsWindow.xaml(.cs)        # Ayarlar penceresi
├── MainWindow.xaml(.cs)            # System Tray ana penceresi
├── App.xaml(.cs)                   # WPF uygulaması
└── MAB_Spark.csproj               # Proje dosyası
```

## 💾 Veri Depolama

Tüm kısaltmalar **AppData\Local\MAB_Spark\shortcuts.db** klasöründe SQLite veritabanında saklanır:

```
C:\Users\[YourUsername]\AppData\Local\MAB_Spark\shortcuts.db
```

## 🔌 NuGet Paketleri

- **System.Data.SQLite** - Veritabanı yönetimi
- **Hardcodet.NotifyIcon.Wpf** - System Tray ikonu
- **GlobalHotKey** - Küresel kısayol tuşu (ileride)
- **System.Windows.Forms** - SendKeys işlemleri

## 🐛 Sorun Giderme

### Uygulama başlamıyor
```powershell
# .NET Runtime yüklü mü kontrol et
dotnet --version

# Veritabanı dosyasını sil ve yeniden başlat
Remove-Item $env:LOCALAPPDATA\MAB_Spark\shortcuts.db
```

### Genişletme çalışmıyor
- Kısaltmanın büyük/küçük harfini kontrol edin
- Ayarlar penceresinde kısaltmanın **"Aktif"** olduğunu kontrol edin
- Uygulamayı yeniden başlatın

### System Tray simgesi görünmüyor
- Sistem tray'i kontrol edin (saati sağ tıklayın)
- Gizli simgeleri göster seçeneğini kontrol edin

## 📝 Örnek Kısaltmalar

```
btw     → by the way
asap    → as soon as possible
fyi     → for your information
tbd     → to be determined
etc     → et cetera
wrt     → with respect to
imho    → in my humble opinion
```

## 🎨 Teknoloji Stack

- **Framework**: .NET 10.0 WPF
- **Veritabanı**: SQLite
- **Dil**: C# 13
- **UI**: XAML

## 📄 Lisans

Bu proje açık kaynak kodlu olup, [MIT Lisansı](LICENSE) altında dağıtılmaktadır.

## 👤 Geliştirici

**Mertcan BZTPRK**
- GitHub: [@Mertcan-BZTPRK](https://github.com/Mertcan-BZTPRK)
- Repository: [MAB_Spark](https://github.com/Mertcan-BZTPRK/MAB_Spark)

## 🤝 Katkıda Bulunma

Katkıda bulunmak isterseniz:

1. Repository'yi fork'layın
2. Feature branch'i oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişiklikleri commit'leyin (`git commit -m 'Add amazing feature'`)
4. Branch'e push'layın (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📧 İletişim

Sorularınız veya önerileriniz için GitHub Issues'da bir konu açabilirsiniz.

---

**Made with ❤️ in Turkey** 🇹🇷
