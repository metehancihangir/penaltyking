# Faz 7 — Ses entegrasyonu

Tamamlandı. Gameplay'e sürekli düşük seviyeli stadyum ambiyansı, topa vuruş, gol kutlaması ve kurtarış sonrası kalabalık hayal kırıklığı kayıtları eklendi.

Vuruş sesi `ShotPresentation.Kick` ile 0,24 saniyede; gol/kurtarış sesi `Impact` ile 0,90 saniyede başlar. Sesler ayrı bekleme zamanlayıcıları kullanmaz, animasyon olaylarını izler. Olay kayıtları 1,18 saniyede tamamlandığından sonraki şuta sarkmaz. 24 saniyelik ambiyans döngüsü sonuç panelinde de devam eder; ana menüye çıkarken hem ambiyans hem olay sesi durur. Zorluk/mod seçilmeden açılan koruma ekranı sessizdir.

**SFX Volume tüm stadyum seslerini, kalabalık ambiyansı dahil, anında kontrol eder.** Music Volume ayrı müzik kanalını korur; bu fazda müzik eklenmedi. Options ekranında hâlâ yalnızca iki kaydırıcı vardır. Ses seviyesi sıfırlanınca döngü yeniden başlamaz; sessiz devam ettiği yerden açılır.

Sesler PCM olarak önceden yüklenir. Ambiyansın döngü birleşiminde yarım saniyelik yumuşak geçiş; tek seferlik seslerde başlangıç/bitiş yumuşatma ve seviye dengeleme vardır. En yüksek ayarda ambiyansla vuruşun toplam teorik tepe değeri 0,96 altında kalır.

Kaynak kayıtlar `Assets/Audio/Source`, oyun WAV'ları `Assets/Audio/Stadium`, lisans ve uyarlama kaydı `docs/audio-source/README.md` içindedir. Dört kaynak CC0 olarak doğrulandı. Yeniden üretim için Unity menüsü: `Penalty King/Phase 7/Add stadium audio`. Bu araç mevcut Faz 6 sahnesine yalnızca ses katmanını ekler.

Doğrulama: **31/31 Unity PlayMode testi geçti** (`TestResults/phase7.xml`). Yeni kontroller gerçek AudioSource oynatımını, gol ve kurtarış için doğru klip seçimini, temastan önce sessizliği, tek seferlik olayları, aktif seslerde bağımsız Music/SFX seviyelerini, menüye çıkışta durmayı, seçim korumasını, ön yüklenmiş örneklerde sinyal/tepe değerlerini ve döngü birleşimini kapsar. Önceki 27 test de geçti.

`docs/previews/phase7-audio-demo.wav` dosyası aynı seslerle hazırlanmış altı saniyelik dinleme örneğidir: önce gol, sonra kurtarış. Çalışan Unity oturumunun kaydı değildir; `Tools/preview_audio.py` tarafından oyun olay zamanlarıyla birleştirilmiştir. Bu ortamda işitsel dinleme doğrulaması yapılamadı; teknik ses kontrolleri ve Unity oynatma testleri tamamlandı. Kullanıcı Unity'de MainMenu → Singleplayer → zorluk → mod akışıyla sesli oyunu deneyebilir.

Faz 8 cilalama ve uçtan uca/cihaz testleri için kullanıcı onayı beklenir.
