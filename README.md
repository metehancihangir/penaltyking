# Penalty King

Unity 6000.4.4f1 / C# / Universal 2D. Mobil öncelikli piksel penaltı oyunu.

## Açılış

Unity Hub → Projects → Add → Add project from disk ile bu klasörü seçin. Unity **6000.4.4f1** ile açın. `Assets/Scenes/MainMenu.unity` sahnesini açıp **Play** tuşuna basın.

**Faz 0–8 tamamlandı.** Android test paketi: `Builds/Android/PenaltyKing-development.apk` (94,3 MiB, Android 8+, ARM64/x86_64). Son doğrulama: 33/33 Unity testi; Android 11 emülatöründe dikey/yatay iki mod ve tekrar/menü akışı başarılı, 59,8–60 FPS. Ayrıntılar `docs/faz-8-raporu.md` dosyasındadır.

## Güncelleme serisi — Faz 1

Yeni menü: **Play → Online (pasif) / 2 Kişilik → Sabit Round / Endless**. Ana menüde yalnızca Play ve Options vardır. Zorluk seçimi yeni akıştan ve derleme listesinden çıkarıldı; mod açıklamaları kaldırıldı.

Options ekranında **Music Volume**, **SFX Volume** ve kalıcı **Titreşim** anahtarı bulunur. Kalabalık dahil stadyum sesleri SFX seviyesini izler. Titreşim tercihi varsayılan açık olup bu aşamada yalnızca kaydedilir; gol titreşimi Güncelleme Faz 6'da eklenecek.

**Bu aşama menü güncellemesidir.** Aynı telefondaki iki oyuncunun şut/kurtarış ve sıra sistemi Güncelleme Faz 2'de uygulanacak. Şu anda mod seçimi sonrası yerel maçın henüz hazır olmadığı bilgisi ve ana menüye dönüş gösterilir; eski bot oyunu başlatılmaz. Eski bot kodu, animasyon/ses regresyonlarını koruyarak Faz 2'de değiştirilecek.

Mevcut Android APK eski sürümdür. Güncel menüleri Unity'de `Assets/Scenes/MainMenu.unity` sahnesinden Play ile inceleyebilirsiniz. Plan: `update-notes.md`. Yeni sahneleri yeniden üretmek için **Penalty King → Updates → Phase 1 - Build menus** kullanılır; eski faz üreticileri tek başlarına çalıştırılmamalıdır.

## Proje yapısı

- `Assets/Scenes`: MainMenu, PlayerSelect, ModeSelect, Gameplay, Options; DifficultySelect eski, derlemeye dahil değil.
- `Assets/Scripts/Core`: GameManager, AudioManager, RuntimeBootstrap, SceneNavigator ve geçici tanılama paneli.
- `Assets/Scripts/Input`: Input System tabanlı dokunma algılayıcısı.
- `Assets/Scripts/UI`: ana menü bağlantıları, piksel buton tepkisi ve ekran güvenli alanı.
- `Assets/Scripts/UI/OptionsController.cs`, `Assets/Scripts/Core/AudioPreferences.cs`: ses arayüzü, anlık kanal kontrolü ve kalıcı kayıt.
- `Assets/Scripts/UI/DifficultySelectController.cs`, `ModeSelectController.cs`: zorluk/mod akışı ve sahne bağlantıları.
- `Assets/Scripts/Core/GameRules.cs`, `Assets/Resources/GameRules.asset`: düzenlenebilir zorluk oranları ve sabit round şut sayısı.
- `Assets/Scripts/Gameplay/PenaltyRound.cs`: yön seçimi, olasılık, gol/şut sayacı ve iki modun bitiş kuralları.
- `Assets/Scripts/Gameplay/GameplayController.cs`, `Assets/Scripts/UI/ShotZone.cs`: dokunma anında şut, tekrar giriş kilidi, geçici geri bildirim ve skor ekranı.
- `Assets/UI/Menu`: menüye ait özgün geçici stadyum ve piksel başlık PNG'leri.
- `Assets/Sprites/Stadium`, `Characters`, `Props`: gece stadyumu, karakter pozları ve top/kale PNG varlıkları.
- `Assets/Scripts/Gameplay/ShotPresentation.cs`: ortak animasyon zaman çizelgesi, yörünge, kareler ve ses zamanlama olayları.
- `Assets/Editor`: Faz 0 sahne kurulumu, Faz 1 menü üretimi ve önizleme araçları. Faz 1 menü üreticisi MainMenu içindeki üretilmiş menüyü yeniden oluşturur; elle menü düzenlediyseniz yeniden çalıştırmayın.
- `Assets/Tests/PlayMode`: altyapı, menü yönlendirme, pasif Multiplayer ve buton görsel tepkisi testleri.
- `docs`: orijinal prompt kopyaları, kullanıcının güncel kararları ve faz raporları.

## Test

Unity menüsünde **Window → General → Test Runner → PlayMode → Run All**.

Komut satırı (PowerShell):

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.4f1\Editor\Unity.exe' -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults "$PWD/TestResults/phase8.xml" -logFile "$PWD/Logs/phase8-tests.log"
```

Test komutunda `-quit` kullanılmaz; test çalıştırıcısı tamamlanınca kapanır. Aynı proje Unity Editor'de açıkken ikinci bir Unity işlemiyle test çalıştırmayın.

## Kapsam ve onay

Güncel kararlar `docs/kararlar.md` dosyasındadır. Oyuncu görünür ve vuruş animasyonu Faz 6 kapsamındadır. Gece atmosferi sabittir; kalabalık ambiyansı SFX kanalındadır. Her faz sonunda kullanıcı onayı beklenir.
