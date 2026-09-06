# Faz 1 — Ana Menü

## Uygulanan kapsam

- **Singleplayer** butonu `DifficultySelect` sahnesine açılır.
- **Multiplayer** görünür, `interactable = false` ve **YAKINDA** etiketlidir. Yönlendirme dinleyicisi yoktur.
- **Options** butonu mevcut `Options` sahnesine açılır. Ses ayarları ekranı Faz 2 onayından sonra hazırlanacak.
- Gece gökyüzü, projektörler, tribün ve çimden oluşan özgün, geçici piksel stadyum arka planı eklendi. Referanslardan görsel kopyalanmadı. Final görsel varlıklar Faz 5 kapsamındadır.
- Piksel başlık, keskin kenarlı butonlar, normal/hover/pressed/disabled renkleri eklendi. Basışta buton yüzeyi dört tasarım birimi aşağı iner; dokunma alanı sabit kalır.
- Menü uGUI Canvas ve Input System UI modülü kullanır. Ekran oranına ölçeklenir; kontroller cihazın güvenli alanı içinde merkezlenir.
- MainMenu'deki Faz 0 tanılama paneli kaldırıldı. Henüz geliştirilmemiş diğer sahnelerde altyapı paneli korunur.

## Oluşturulan / değiştirilen dosyalar

| Dosya | Görev |
|---|---|
| `Assets/Scenes/MainMenu.unity` | Kamera, menü Canvas'ı, EventSystem, üç buton ve bağlantıları |
| `Assets/Scripts/UI/MainMenuController.cs` | Singleplayer/Options yönlendirmeleri; Multiplayer kilidi |
| `Assets/Scripts/UI/PixelMenuButton.cs` | Renk durumları ve basışta yüzey hareketi |
| `Assets/Scripts/UI/SafeAreaPanel.cs` | Ekran boyutu/güvenli alan değişimine uyum |
| `Assets/UI/Menu/StadiumPlaceholder.png`, `Title.png` | Özgün piksel menü görselleri; point filtering, tek sprite |
| `Assets/Editor/MenuPixelArt.cs` | Geçici görsellerin tekrar üretilebilir çizim kodu |
| `Assets/Editor/PhaseOneSetup.cs` | MainMenu içindeki üretilmiş menüyü kuran editör aracı |
| `Assets/Editor/MenuPreview.cs` | Unity renderer'ı ile iki ekran oranında önizleme ve buton sınır kontrolü |
| `Assets/Tests/PlayMode/MainMenuTests.cs` | UI raycast üzerinden yönlendirme, pasif buton ve görsel tepki kontrolleri |
| Üç `.asmdef`, `Packages/manifest.json`, `packages-lock.json` | uGUI referansları ve PNG önizleme için imageconversion modülü |
| `README.md`, `docs/kararlar.md` | Güncel açılış, kapsam ve faz onayı bilgileri |

## Çıktı / test kriteri

**Karşılandı:** Üç buton görünür. Singleplayer ve Options tıklanabilir ve doğru sahnelere gider. Multiplayer tıklanınca sahne değişmez.

Unity 6000.4.4f1 Play Mode testleri: **7/7 geçti** (Faz 0'dan dört altyapı testi + üç menü testi). Menü testleri butonları UI raycast ile bulur ve pointer click olayını iletir; yalnızca doğrudan fonksiyon çağırarak yönlendirme kontrolü yapmaz.

Normal → hover → pressed → release durumlarının farklı renk/yüzey konumları oluşturduğu doğrulandı. Basış bırakıldığında yüzey eski konumuna döner.

**Görsel kontrol:** Unity tarafından oluşturulan **1280×720 yatay** ve **390×844 dikey** önizlemeler incelendi. Başlık ve etiketler görünür, üç buton ekran sınırları içindedir. Bunlar gerçek cihaz ekran görüntüsü değildir; cihaz performansı ve gerçek dokunma gecikmesi henüz ölçülmedi.

- Test sonucu: `TestResults/phase1.xml`
- Günlük: `Logs/phase1-tests.log`
- Yatay önizleme: `docs/previews/main-menu-landscape.png`
- Dikey önizleme: `docs/previews/main-menu-portrait.png`

## Sonraki faz

Ana prompttaki faz onay akışı gereği **Faz 2 başlatılmadı**. Kullanıcı onayından sonra yalnızca Music Volume ve SFX Volume kaydırıcıları, gerçek zamanlı ses kontrolü, PlayerPrefs kalıcılığı ve geri dönüş butonu uygulanacak.
