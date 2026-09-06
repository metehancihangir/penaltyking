# Faz 3 — Zorluk ve mod seçimi tamamlandı

## Faz 2 kontrolü

Önceki oturumun dosyaları, test sonucu ve iki ayrı Unity işlemiyle yapılan PlayerPrefs kalıcılık kaydı incelendi. Mevcut proje üzerinde Faz 0–2'nin **10 testi tekrar geçti**. Faz 2'de tamamlanmamış bir gereksinim bulunmadı. Kullanıcının “her şey okey olduğunda faz3'e başlayalım” onayıyla Faz 3 uygulandı.

## Uygulanan kapsam

- `DifficultySelect`: **Kolay / Orta / Zor**, ardından `ModeSelect` geçişi.
- `ModeSelect`: seçilen zorluk etiketi, **Sabit Round / Endless**, seçim sonrası `Gameplay` geçişi.
- Geri butonlarıyla moddan zorluğa, zorluktan ana menüye dönülebilir. Zorluk değiştirildiğinde eski mod seçimi geçersizleşir.
- Yeni Singleplayer girişinde seçim akışı yeniden başlar. Mod sahnesi doğrudan açılmışsa zorluk seçilmeden oyun başlatılamaz.
- Hızlı art arda dokunmalarda ilk geçiş isteği korunur; ikinci dokunma zorluk/modu değiştirmez.
- GameManager, zorluk/mod ve seçilen olasılık/şut sayısını sahneler arasında korur.
- Varsayılan doğru yön tahmin olasılıkları **Kolay %20, Orta %35, Zor %50**. Sabit round **5 şut** olarak yapılandırıldı.
- `Assets/Resources/GameRules.asset` Inspector üzerinden düzenlenebilir. GameManager bunu yükleyerek seçim anındaki değerleri oturuma kopyalar. Artan/dinamik zorluk eklenmedi.
- Mevcut gece stadyumu, piksel buton durumları ve güvenli alan/ölçekleme altyapısı kullanıldı.

## Oluşturulan / değiştirilen dosyalar

| Dosya | Değişiklik |
|---|---|
| `Assets/Scenes/DifficultySelect.unity` | Zorluk seçim ekranı ve geri dönüş |
| `Assets/Scenes/ModeSelect.unity` | Mod seçim ekranı, zorluk etiketi ve geri dönüş |
| `Assets/Scripts/UI/DifficultySelectController.cs` | Üç zorluk seçimi ve mod sahnesine geçiş |
| `Assets/Scripts/UI/ModeSelectController.cs` | İki mod seçimi, seçim sırası koruması ve Gameplay'e geçiş |
| `Assets/Scripts/Core/GameRules.cs` | Düzenlenebilir oyun yapılandırmasının türü |
| `Assets/Resources/GameRules.asset` | Varsayılan olasılıklar ve şut sayısı |
| `Assets/Scripts/Core/GameManager.cs` | Yapılandırma yükleme, seçim bayrakları, sabit oturum değerleri |
| `Assets/Scripts/UI/MainMenuController.cs` | Yeni Singleplayer girişinde seçim akışını sıfırlama |
| `Assets/Editor/PhaseThreeSetup.cs` | İki seçim sahnesini ve eksikse yapılandırma varlığını kurma |
| `Assets/Editor/MenuPreview.cs` | Seçim ekranlarının yatay/dikey önizlemeleri |
| `Assets/Tests/PlayMode/SelectionTests.cs` | Altı birleşim, geri dönüş, seçim sırası, hızlı dokunma ve sabit oran testleri |
| `README.md`, `docs/kararlar.md`, `docs/faz-2-raporu.md` | Güncel kullanım, onay ve önceki faz kontrolü |

## Çıktı / test kriteri

**Karşılandı:** Zorluk ve mod seçimleri doğru kaydediliyor ve Gameplay'e aktarılıyor.

Unity 6000.4.4f1 Play Mode sonucu: **14 test geçti, 0 başarısız**. Önceki fazların 10 testine ek olarak:

1. Ana Menü → Zorluk → Mod → Gameplay akışı **üç zorluk × iki mod** için UI raycast/click üzerinden doğrulandı.
2. Geri dönüş, zorluğu değiştirme ve yeni Singleplayer girişinde seçimleri yeniden isteme doğrulandı.
3. Zorluksuz doğrudan mod girişinin engellenmesi ve hızlı çift dokunmada ilk seçimin korunması doğrulandı.
4. Varsayılan %20/%35/%50 ve 5 şut değerleri; yapılandırma değişse bile seçilmiş olasılığın yeniden seçime kadar sabit kalması doğrulandı.

**Görsel kontrol:** Her iki ekranın 1280×720 yatay ve 390×844 dikey Unity önizlemeleri incelendi. Metinler ve seçenekler görünür, butonlar ekran sınırları içinde.

- Faz 2 yeniden kontrol sonucu: `TestResults/phase2-recheck.xml`
- Faz 3 test sonucu: `TestResults/phase3.xml`
- Günlük: `Logs/phase3-tests.log`
- Önizlemeler: `docs/previews/DifficultySelect-landscape.png`, `DifficultySelect-portrait.png`, `ModeSelect-landscape.png`, `ModeSelect-portrait.png`

Gerçek mobil cihaz testi yapılmadı. Gameplay bu fazda hâlâ geliştirme amaçlı altyapı panelidir; gerçek penaltı mantığı, skor ve modların oynanış döngüsü Faz 4 kapsamındadır.

## Sonraki onay

**Faz 4 başlatılmadı.** Ana prompttaki faz onay akışı gereği sonraki faz için kullanıcı onayı beklenir.
