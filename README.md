# Penalty King

Unity 6000.4.4f1 / C# / Universal 2D. Mobil öncelikli piksel penaltı oyunu.

## Açılış

Unity Hub → Projects → Add → Add project from disk ile bu klasörü seçin. Unity **6000.4.4f1** ile açın. `Assets/Scenes/MainMenu.unity` sahnesini açıp **Play** tuşuna basın.

MainMenu artık Faz 1 ana menüsünü gösterir: **Singleplayer → DifficultySelect**, **Options → Options**. **Multiplayer / Yakında** pasiftir. Etkin butonlarda üzerine gelme ve basma görsel tepkileri vardır. Gece stadyumu arka planı özgün, geçici piksel çizimidir; final görseller Faz 5 kapsamındadır.

Options ekranında **Music Volume** ve **SFX Volume** kaydırıcıları bulunur. Değişiklikler ses kanallarına anında yansır; kalabalık ambiyansı SFX seviyesini izler. Ayarlar PlayerPrefs ile kaydedilir ve sonraki açılışta yüklenir. **Geri** ana menüye döner. Gerçek ses klipleri Faz 7'de eklenecek.

Zorluk/mod ve oynanış sahneleri henüz Faz 0 altyapı test panelini içerir. Zorluk/mod ekranları Faz 3'te hazırlanacak. Bu geçici panellerde MainMenu düğmesiyle ana menüye dönülebilir. Altyapı paneli Editor/Development Build dışında görünmez.

## Proje yapısı

- `Assets/Scenes`: MainMenu, DifficultySelect, ModeSelect, Gameplay, Options.
- `Assets/Scripts/Core`: GameManager, AudioManager, RuntimeBootstrap, SceneNavigator ve geçici tanılama paneli.
- `Assets/Scripts/Input`: Input System tabanlı dokunma algılayıcısı.
- `Assets/Scripts/UI`: ana menü bağlantıları, piksel buton tepkisi ve ekran güvenli alanı.
- `Assets/Scripts/UI/OptionsController.cs`, `Assets/Scripts/Core/AudioPreferences.cs`: ses arayüzü, anlık kanal kontrolü ve kalıcı kayıt.
- `Assets/UI/Menu`: menüye ait özgün geçici stadyum ve piksel başlık PNG'leri.
- `Assets/Sprites`, `Audio`, `Prefabs`, `UI`: sonraki fazların varlık klasörleri.
- `Assets/Editor`: Faz 0 sahne kurulumu, Faz 1 menü üretimi ve önizleme araçları. Faz 1 menü üreticisi MainMenu içindeki üretilmiş menüyü yeniden oluşturur; elle menü düzenlediyseniz yeniden çalıştırmayın.
- `Assets/Tests/PlayMode`: altyapı, menü yönlendirme, pasif Multiplayer ve buton görsel tepkisi testleri.
- `docs`: orijinal prompt kopyaları, kullanıcının güncel kararları ve faz raporları.

## Test

Unity menüsünde **Window → General → Test Runner → PlayMode → Run All**.

Komut satırı (PowerShell):

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.4f1\Editor\Unity.exe' -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults "$PWD/TestResults/phase2.xml" -logFile "$PWD/Logs/phase2-tests.log"
```

Test komutunda `-quit` kullanılmaz; test çalıştırıcısı tamamlanınca kapanır. Aynı proje Unity Editor'de açıkken ikinci bir Unity işlemiyle test çalıştırmayın.

## Kapsam ve onay

Güncel kararlar `docs/kararlar.md` dosyasındadır. Oyuncu görünür ve vuruş animasyonu Faz 6 kapsamındadır. Gece atmosferi sabittir; kalabalık ambiyansı SFX kanalındadır. Her faz sonunda kullanıcı onayı beklenir.
