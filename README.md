# Penalty King

Unity 6000.4.4f1 / C# / Universal 2D. Mobil öncelikli piksel penaltı oyunu.

## Açılış

Unity Hub → Projects → Add → Add project from disk ile bu klasörü seçin. Unity **6000.4.4f1** ile açın. `Assets/Scenes/MainMenu.unity` sahnesini açıp **Play** tuşuna basın.

MainMenu artık Faz 1 ana menüsünü gösterir: **Singleplayer → DifficultySelect**, **Options → Options**. **Multiplayer / Yakında** pasiftir. Etkin butonlarda üzerine gelme ve basma görsel tepkileri vardır. Gece stadyumu arka planı özgün, geçici piksel çizimidir; final görseller Faz 5 kapsamındadır.

Options ekranında **Music Volume** ve **SFX Volume** kaydırıcıları bulunur. Değişiklikler ses kanallarına anında yansır; kalabalık ambiyansı SFX seviyesini izler. Ayarlar PlayerPrefs ile kaydedilir ve sonraki açılışta yüklenir. **Geri** ana menüye döner. Gerçek ses klipleri Faz 7'de eklenecek.

Singleplayer akışı artık **Kolay / Orta / Zor → Sabit Round / Endless → Gameplay** şeklindedir. Mod ekranı seçilen zorluğu gösterir. Geri ile zorluk değiştirilebilir veya ana menüye dönülebilir. Zorluk seçilmeden moddan oyuna geçilemez.

Gameplay henüz Faz 0 altyapı panelini içerir; penaltı mekaniği Faz 4'te hazırlanacak. Panel seçilen zorluk ve modu gösterir, MainMenu düğmesiyle ana menüye dönülebilir. Altyapı paneli Editor/Development Build dışında görünmez.

## Zorluk yapılandırması

Unity Project panelinde `Assets/Resources/GameRules.asset` dosyasını seçin. Inspector'da Easy/Medium/Hard Save Probability değerleri varsayılan **0.20 / 0.35 / 0.50**, Fixed Round Shots **5** değerindedir. Oranlar 0–1 aralığındadır. Bu değerler oyunun Options ekranına yeni ayarlar eklemez; geliştirme yapılandırmasıdır.

GameManager bu dosyayı yükler ve seçim anındaki olasılık/şut sayısını oturuma kopyalar. Maç sırasında yapılandırma değiştirilse bile seçilmiş olasılık kendiliğinden değişmez. Zorluk yeniden seçildiğinde yeni yapılandırma kullanılır.

## Proje yapısı

- `Assets/Scenes`: MainMenu, DifficultySelect, ModeSelect, Gameplay, Options.
- `Assets/Scripts/Core`: GameManager, AudioManager, RuntimeBootstrap, SceneNavigator ve geçici tanılama paneli.
- `Assets/Scripts/Input`: Input System tabanlı dokunma algılayıcısı.
- `Assets/Scripts/UI`: ana menü bağlantıları, piksel buton tepkisi ve ekran güvenli alanı.
- `Assets/Scripts/UI/OptionsController.cs`, `Assets/Scripts/Core/AudioPreferences.cs`: ses arayüzü, anlık kanal kontrolü ve kalıcı kayıt.
- `Assets/Scripts/UI/DifficultySelectController.cs`, `ModeSelectController.cs`: zorluk/mod akışı ve sahne bağlantıları.
- `Assets/Scripts/Core/GameRules.cs`, `Assets/Resources/GameRules.asset`: düzenlenebilir zorluk oranları ve sabit round şut sayısı.
- `Assets/UI/Menu`: menüye ait özgün geçici stadyum ve piksel başlık PNG'leri.
- `Assets/Sprites`, `Audio`, `Prefabs`, `UI`: sonraki fazların varlık klasörleri.
- `Assets/Editor`: Faz 0 sahne kurulumu, Faz 1 menü üretimi ve önizleme araçları. Faz 1 menü üreticisi MainMenu içindeki üretilmiş menüyü yeniden oluşturur; elle menü düzenlediyseniz yeniden çalıştırmayın.
- `Assets/Tests/PlayMode`: altyapı, menü yönlendirme, pasif Multiplayer ve buton görsel tepkisi testleri.
- `docs`: orijinal prompt kopyaları, kullanıcının güncel kararları ve faz raporları.

## Test

Unity menüsünde **Window → General → Test Runner → PlayMode → Run All**.

Komut satırı (PowerShell):

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.4f1\Editor\Unity.exe' -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults "$PWD/TestResults/phase3.xml" -logFile "$PWD/Logs/phase3-tests.log"
```

Test komutunda `-quit` kullanılmaz; test çalıştırıcısı tamamlanınca kapanır. Aynı proje Unity Editor'de açıkken ikinci bir Unity işlemiyle test çalıştırmayın.

## Kapsam ve onay

Güncel kararlar `docs/kararlar.md` dosyasındadır. Oyuncu görünür ve vuruş animasyonu Faz 6 kapsamındadır. Gece atmosferi sabittir; kalabalık ambiyansı SFX kanalındadır. Her faz sonunda kullanıcı onayı beklenir.
