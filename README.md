# Penalty King

Unity 6000.4.4f1 / C# / Universal 2D. Mobil öncelikli piksel penaltı oyunu.

## Açılış

Unity Hub → Projects → Add → Add project from disk ile bu klasörü seçin. Unity **6000.4.4f1** ile açın. `Assets/Scenes/MainMenu.unity` sahnesini açıp **Play** tuşuna basın.

**Faz 0–8 tamamlandı.** Android test paketi: `Builds/Android/PenaltyKing-development.apk` (94,3 MiB, Android 8+, ARM64/x86_64). Son doğrulama: 33/33 Unity testi; Android 11 emülatöründe dikey/yatay iki mod ve tekrar/menü akışı başarılı, 59,8–60 FPS. Ayrıntılar `docs/faz-8-raporu.md` dosyasındadır.

## Güncelleme serisi — Faz 1–3

Menü: **Play → Online (pasif) / 2 Kişilik → Sabit Round / Endless**. Bot ve zorluk kodu kaldırıldı.

Aynı telefonda iki oyuncu oynar. Önce PLAYER 1 şut yönünü seçer. Telefon devri ekranı seçimi gizler; PLAYER 2 ekrana dokunarak paneli kapatır ve yeni bir dokunuşla kaleci yönünü seçer. İki seçim tamamlanınca animasyon oynar. Aynı yön kurtarış, farklı yön goldür. Sonraki şutta roller değişir. Kalecilik yapan kişi bir sonraki şutçu olduğundan bu rol değişiminde telefonu geri vermek gerekmez.

Her oyuncunun skoru ve atış geçmişi ayrıdır. Sabit Round için temel kural kişi başı 5 şut; Endless kurtarışlarda da sürer. Sonuç ekranı iki oyuncunun skorlarını, kullanılan haklarını ve kazananı veya beraberliği gösterir. Tekrar Oyna aynı modda skorları, geçmişi ve sırayı sıfırlar. Sol üstteki Menü düğmesi devir ve animasyon dahil her aşamada maçı kapatır; sesler durur ve seçimler temizlenir. Skor tabelası ve tam ekran saha Faz 4'te yenilenecek.

Options: Music Volume, SFX Volume ve kalıcı Titreşim anahtarı. Fiziksel gol titreşimi ve yeni tezahürat Faz 6'da bağlanacak; oyun içi ayarlar Faz 4 kapsamındadır.

Mevcut Android APK eski sürümdür. Güncel oyunu Unity'de `Assets/Scenes/MainMenu.unity` sahnesinden Play ile inceleyebilirsiniz. Devir ekranını kapatıp ardından sol/orta/sağ bölgesine dokunun. Faz 3 için yeni APK üretilmedi.

Plan: `update-notes.md`. Rapor: `docs/update-faz-3-raporu.md`. Menü üreticisi **Penalty King → Updates → Phase 1 - Build menus**, devretme ekranı üreticisi **Phase 2 - Build turn handoff**. Maç kontrolleri üreticisi **Phase 3 - Build match controls**. Eski gameplay üreticileri çalıştırılırsa sonrasında animasyon, ses ve güncel devir ekranı tekrar bağlanmalıdır.

## Proje yapısı

- `Assets/Scenes`: MainMenu, PlayerSelect, ModeSelect, Gameplay, Options.
- `Assets/Scripts/Core`: oturum, ses, titreşim tercihi ve sahne geçişleri.
- `Assets/Scripts/Gameplay/PenaltyRound.cs`: iki insanın yönleri, oyuncu skorları, atış geçmişi ve rol sırası.
- `Assets/Scripts/Gameplay/GameplayController.cs`: gizli şut seçimi, devir, kaleci seçimi ve animasyon akışı.
- `Assets/Scripts/UI/TurnHandoff.cs`: tam ekran telefon devri ve yeni dokunuş gereksinimi.
- `Assets/Scripts/Gameplay/ShotPresentation.cs`, `GameplayAudio.cs`: animasyonlar ve olaylara bağlı sesler.
- `Assets/Tests/PlayMode`: iki oyuncu, dokunma, ses, animasyon ve menü testleri.

## Test

Unity menüsünde **Window → General → Test Runner → PlayMode → Run All**.

Komut satırı (PowerShell):

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.4f1\Editor\Unity.exe' -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults "$PWD/TestResults/phase8.xml" -logFile "$PWD/Logs/phase8-tests.log"
```

Test komutunda `-quit` kullanılmaz; test çalıştırıcısı tamamlanınca kapanır. Aynı proje Unity Editor'de açıkken ikinci bir Unity işlemiyle test çalıştırmayın.

## Kapsam ve onay

Güncel kararlar `docs/kararlar.md` dosyasındadır. Oyuncu görünür ve vuruş animasyonu Faz 6 kapsamındadır. Gece atmosferi sabittir; kalabalık ambiyansı SFX kanalındadır. Her faz sonunda kullanıcı onayı beklenir.
