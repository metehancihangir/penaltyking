# Faz 8 — Cilalama ve uçtan uca test

Tamamlandı. Beş ekranın geçişlerine 0,16 saniyelik kararma/açılma eklendi. Geçiş katmanı sahne değişiminde yaşamaya devam eder ve yeni ekran açılana kadar dokunmaları engeller. Doğrudan sahne yüklemesi geçişi keserse katman güvenli şekilde temizlenir. Fade, timeScale=0 durumunda da tamamlanır.

Uçtan uca testin yakaladığı bir başlangıç hatası düzeltildi: Gameplay açılırken geçiş hâlâ sürüyorsa yeni round yanlışlıkla başlatılmıyordu. İlk round kurulumu artık tekrar oynama butonunun geçiş engelinden bağımsızdır; oyuncu girişi geçiş bitene kadar kapalı kalır.

Her iki modda skor özeti, Tekrar Oyna ve Ana Menüye Dön akışı tamamlandı. Yüksek skor daha önce tanımlanmış bir özellik olmadığı için kaynak fazdaki “varsa” maddesi mevcut skor gösterimiyle korundu. Kapsam kontrolü `final-kapsam-kontrolu.md` dosyasındadır. Kullanıcının güncel kararı gereği oyuncu figürü ve vuruş animasyonu görünür.

## Unity doğrulaması

**33/33 PlayMode testi geçti.** Sabit Round ve Endless için gerçek UI raycast'leri üzerinden ana menü → zorluk → mod → maç → skor → tekrar → skor → ana menü akışı test edildi. Geçişte dokunma engeli ve duraklatılmış zaman testi eklendi. Önceki ayar, ses, animasyon, seçim ve dokunma testleri de geçti. Ayrıntılı test listesi: `phase8-unity-results.json`.

## Android doğrulaması

Android 11/API 30 x86_64 emülatörü, WHPX donanım hızlandırması, ana makinenin RTX 4070 Laptop GPU'su, 3 GB RAM ve 4 sanal çekirdek kullanıldı. **720×1280 dikey ve 1280×720 yatay** görüntü boyutları PNG başlıklarından da doğrulandı. Android `input tap` komutlarıyla gerçek uygulama dokunmaları gönderildi; oyunun iç işlevleri çağrılarak akış atlanmadı.

- Sabit Round: 5 şut → skor → tekrar → 5 şut → skor → ana menü başarılı.
- Options → Geri akışı başarılı; ekran yalnızca iki ses kaydırıcısını içeriyor.
- Yatay Endless: ilk kurtarışta skor ekranı → ana menü başarılı.
- Son koşuda 11 gerçek şut dokunması işlendi; hata/istisna görülmedi.
- 11 adet beş saniyelik ölçüm penceresinde **59,8–60,0 FPS**.
- Pencere bazında kare süresi P95 **16,68–16,69 ms**, en uzun kare **33,35 ms**.
- Android dokunma olayı zamanından şut işleyicisine gecikme **8,52–21,37 ms**. Bu fiziksel parmak–ekran gecikmesi değildir.

Ham ölçümler `phase8-mobile-results.json`, yeniden çalıştırma aracı `Tools/android_smoke.py`, ekran görüntüleri `docs/previews/phase8-android-*.png` içindedir. Ölçüm bileşenleri yalnızca development derlemelerinde bulunur ve ürün arayüzüne kontrol eklemez. Çalışma hedefi 60 FPS olarak ayarlandı. Emülatör sonuçları fiziksel telefon veya iOS performansı garantisi değildir; bu cihazlarda ayrıca test yapılmadı.

## Teslim

`Builds/Android/PenaltyKing-development.apk` — **94,3 MiB**, ARM64 telefon ve x86_64 emülatör mimarileri, Android 8/API 26 ve üzeri, `com.penaltyking.game`. Geliştirme/test paketidir; mağazaya yayımlanmadı.

SHA-256: `90B8D6EF059EB178C487D3843F16A813785A463002FEE3FB56798234E6FF2770`

Derleme sıfır hatayla tamamlandı. Yeniden üretim: Unity `Penalty King/Phase 8/Build Android development APK`. Yerel masaüstü denemesi için `Assets/Scenes/MainMenu.unity` sahnesini açıp Play'e basın. Test için kurulan emülatör ve SDK `Builds` altında tutulur; kaynak kontrolüne girmez.
