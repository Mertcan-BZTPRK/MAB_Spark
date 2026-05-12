# 🎥 MAB_Spark Kullanım Rehberi (Video Tutorial)

## 00:00 - Başlangıç
Merhaba! Bu vidyoda **MAB_Spark** metin genişletme uygulamasını tanıtacağım.

TextBlaze benzeri bu uygulama, yazarken kısaltmaları otomatik olarak gerçek metne dönüştürüyor.

---

## 00:30 - Uygulamayı Başlatma

```powershell
# Windows PowerShell'de
D:\DESKTOP\SOFTWARE\GİT\MAB_Spark\MAB_Spark\bin\Debug\net10.0-windows\MAB_Spark.exe
```

Uygulama başladıktan sonra:
- Ekranda görünmez
- System Tray'e (saat yanında) bakın
- Mavi "M" simgesini göreceksiniz

---

## 01:00 - Ayarlar Penceresi Açma

**Adım 1**: System Tray'deki "M" simgesine **sağ tık** yapın

```
Context Menu:
├─ ⚙️ Ayarlar
├─ ─────────
└─ ❌ Çıkış
```

**Adım 2**: "⚙️ Ayarlar" seçeneğine tıklayın

---

## 01:30 - Kısaltma Ekleme

**Sekme 1: 📝 Kısaltmalar**

1. En altta **"Yeni"** bölümü göreceksiniz
2. Sol tarafa: Kısaltma girin (örn: `btw`)
3. Sağ tarafa: Gerçek metin girin (örn: `by the way`)
4. **"➕ Ekle"** butonuna tıklayın

```
Kısaltma     →              Gerçek Metin
┌──────────┐             ┌────────────────────┐
│    btw   │  Ekle →     │   by the way       │
└──────────┘             └────────────────────┘
```

### Örnek Kısaltmalar

```
asap    →  as soon as possible
fyi     →  for your information
imho    →  in my humble opinion
lol     →  laugh out loud
omg     →  oh my god
tbd     →  to be determined
thx     →  thanks
pls     →  please
```

---

## 02:30 - Kısaltmayı Kullanma

**Herhangi bir uygulamada (Word, Notepad, Chrome, vb.)**

1. `btw` yazın
2. **SPACE** veya **ENTER** basın
3. **DING** sesi duyarsınız
4. Metni yazılmış görürsünüz: `by the way`

---

## 03:00 - Kısaltmaları Yönetme

### Kısaltmayı Etkinleştir/Devre Dışı Bırak

- DataGrid'de **"Aktif"** sütunundaki checkbox'ı işaretleyin
- İşaretli = aktif ✅
- İşaretli değil = pasif ❌

### Kısaltmayı Sil

1. Listeden silinecek kısaltmayı seçin
2. **"Sil"** butonuna tıklayın
3. Onay için "Evet" deyin

### Mevcut Kısaltmayı Düzenle

1. DataGrid'de **"Genişletilmiş Metin"** sütununu düzenleyin
2. Değeri değiştirin
3. Otomatik kaydedilir (Enter'a basınız)

---

## 03:45 - Ayarlar Sekmesi

**Sekme 2: ⚙️ Ayarlar**

### Windows Başlangıcında Çalıştır

```
☑️ Uygulamayı Windows başlangıcında otomatik olarak başlat
```

- **Açık** = Bilgisayar açıldığında otomatik çalışır
- **Kapalı** = Manuel başlatmanız gerekir

### Durum Bilgisi

- **Uygulama durumu**: Çalışıyor / Durduruldu
- **Toplam kısaltma**: Kaç kısaltma var
- **Veritabanı yolu**: Nerede saklanıyor

---

## 04:30 - İpuçları & Püf Noktaları

### 1. Büyük/Küçük Harfe Duyarlılık
```
btw    ← bu tetiklenecek
BTW    ← bu tetiklenmeyecek
Btw    ← bu tetiklenmeyecek
```

Ayrı ayrı eklemek istiyorsanız, hepsini yanlı kaydedin.

### 2. Boşluk Delimiterleri
- **SPACE** → Genişlet
- **ENTER** → Genişlet
- **TAB** → Genişlet

### 3. Ses Bildirim
- Başarılı genişletme → "Ding" sesi
- Başlatma sırasında sorun → "Beep" sesi

### 4. Clipboard Güvenliği
- Yapıştırma sırasında Clipboard geçici olarak değişir
- Hemen sonra eski içeriği geri yüklenir

---

## 05:15 - Sorun Giderme

### Sorun: Kısaltma çalışmıyor
**Çözüm:**
1. Büyük/küçük harfi kontrol edin
2. Kısaltmanın **"Aktif"** olup olmadığını kontrol edin
3. Uygulamayı yeniden başlatın (Çıkış → Açma)

### Sorun: System Tray simgesi görünmüyor
**Çözüm:**
1. Saat simgesinin yanındaki ↑ üst ok'a tıklayın
2. "Gizli simgeleri göster" seçeneğine tıklayın
3. "M" simgesini göreceksiniz

### Sorun: Veritabanı hatası
**Çözüm:**
1. Uygulamayı kapatın
2. Şunu silin: `C:\Users\[Adınız]\AppData\Local\MAB_Spark\shortcuts.db`
3. Uygulamayı yeniden başlatın

---

## 06:00 - Kurulum & Güncelleme

### Kurulum
```powershell
# Repository'den klonla
git clone https://github.com/Mertcan-BZTPRK/MAB_Spark.git
cd MAB_Spark

# Çalıştır
dotnet run
```

### Güncelleme
```powershell
# Son sürümü al
git pull origin main

# Yeniden derle
dotnet build
```

---

## 06:30 - Sonuç

**MAB_Spark** ile:
- ✅ Hızlı yazma
- ✅ Tekrar yazma azaltma
- ✅ Verimlilik artışı
- ✅ Yazım hataları önleme

**Herhangi bir sorunuz varsa:**
- GitHub Issues: https://github.com/Mertcan-BZTPRK/MAB_Spark/issues
- Discussions: https://github.com/Mertcan-BZTPRK/MAB_Spark/discussions

**Teşekkürler!**

---

## Ek Kaynaklar

### Belgeler
- [README.md](README.md) - Genel bilgi
- [SETUP.md](SETUP.md) - Geliştirici rehberi
- [COMPLETION_SUMMARY.md](COMPLETION_SUMMARY.md) - Proje özeti

### Kodlar
```
Services/
├── DatabaseService.cs        (Veri tabanı)
├── TextHookService.cs        (Metin izleme)
├── SoundService.cs           (Ses)
└── AutoStartService.cs       (Auto-start)
```

---

**Made with ❤️ by Mertcan BZTPRK**
