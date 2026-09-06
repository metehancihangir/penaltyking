# Faz 2 — Options / Ses ayarları

## Uygulanan kapsam

- Options sahnesindeki altyapı panelinin yerine ana menüyle uyumlu bir ses ayarları ekranı eklendi.
- Yalnızca **Music Volume** ve **SFX Volume** olmak üzere iki kaydırıcı bulunur. Değerler yüzde olarak gösterilir; başka ayar eklenmedi.
- Music Volume müzik AudioSource'unu; SFX Volume hem efekt hem kalabalık ambiyansı AudioSource'larını anında günceller.
- Değerler PlayerPrefs ile saklanır. Sürükleme sırasında ses kontrolü anlıktır; disk kaydı son değişiklikten 0,3 saniye sonra yapılır. Geri dönüş, ekran kapanışı, uygulamanın odağını kaybetmesi/arka plana alınması ve normal kapanışta bekleyen kayıt hemen tamamlanır.
- GameManager açılışta kayıtlı değerleri yükler; ardından AudioManager bu seviyelerle başlar. Daha önce kayıt yoksa Music %70, SFX %80 kullanılır.
- **Geri** butonu ana menüye döner.

## Dosyalar

| Dosya | Değişiklik |
|---|---|
| `Assets/Scenes/Options.unity` | İki kaydırıcı, yüzde etiketleri, Geri, Canvas ve Input System EventSystem |
| `Assets/Scripts/UI/OptionsController.cs` | UI bağlantıları, anlık güncelleme, geciktirilmiş/lifecycle kayıt ve geri dönüş |
| `Assets/Scripts/Core/AudioPreferences.cs` | Yalnızca iki ses değeri için PlayerPrefs okuma/yazma |
| `Assets/Scripts/Core/GameManager.cs` | AudioManager kurulmadan önce kalıcı ses değerlerini yükleme |
| `Assets/Editor/PhaseTwoSetup.cs` | Options sahnesinin tekrar üretilebilir kurulumu |
| `Assets/Editor/PhaseOneSetup.cs` | Mevcut menü görsel yardımcılarını Options kurucusuyla paylaşma |
| `Assets/Editor/MenuPreview.cs` | Options önizlemeleri ve slider/buton ekran sınırı kontrolleri |
| `Assets/Editor/PhaseTwoVerification.cs` | İki ayrı Unity işlemiyle kayıt kalıcılığını doğrulama; önceki değerleri yedekleme/geri yükleme |
| `Assets/Tests/PlayMode/OptionsTests.cs` | Sürükleme, kanal bağımsızlığı, kayıt, geri dönüş ve yeniden yükleme testleri |
| `README.md`, `docs/kararlar.md` | Güncel açılış, kapsam ve onay bilgileri |

## Çıktı / test kriteri

**Karşılandı:** Slider değişiklikleri ses kanallarına anında uygulanıyor ve kaydedilen ayarlar yeni oturumda geri yükleniyor.

Unity 6000.4.4f1 Play Mode: **10 test geçti, 0 başarısız**. Önceki fazların yedi testi korunur. Faz 2'nin üç testi şunları doğrular:

1. Ekranda tam iki slider ve tek geri butonu var. UI raycast/pointer sürüklemesi ses kanallarını hemen değiştiriyor. SFX sıfırken efekt ve kalabalık susacak seviyeye geliyor, müzik bağımsız kalıyor. Bekleme sonrası değer kaydediliyor.
2. Geri butonu bekleyen kaydı hemen tamamlıyor ve MainMenu'ye dönüyor.
3. Oyun/ses yöneticileri yeniden oluşturulduğunda ve Options tekrar açıldığında kayıtlı seviyeler hem slider'lara hem AudioSource'lara yansıyor.

**Ayrı süreç testi:** Unity işlemi A Music=0,23 ve SFX=0,67 kaydedip kapandı. Yeni Unity işlemi B bu değerleri başarıyla okudu. Test öncesindeki iki tercih geri yüklendi; başka PlayerPrefs anahtarlarına dokunulmadı.

**Görsel kontrol:** Unity renderer'ından 1280×720 ve 390×844 önizlemeleri alınıp incelendi. Kaydırıcılar ve Geri butonu ekran sınırları içinde.

- Otomatik test çıktısı: `TestResults/phase2.xml`
- Ayrı süreç kalıcılık sonucu: `TestResults/phase2-restart.txt`
- Günlük: `Logs/phase2-tests.log`
- Önizlemeler: `docs/previews/options-landscape.png`, `options-portrait.png`

Ses klipleri Faz 7 kapsamında olduğundan bu fazda AudioSource seviyeleri doğrulandı; gerçek müzik/tezahüratın işitsel kontrolü yapılmadı. Gerçek Android/iOS cihaz testi sonraki ilgili fazlarda yapılacak.

## Sonraki faz

Ana prompttaki onay akışı gereği Faz 3 başlatılmadı. Onay sonrası zorluk seçimi ve ardından Sabit Round / Endless mod seçimi ekranları hazırlanacak.
