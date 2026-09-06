# Faz 0 — Tamamlandı, Faz 1 onayı bekleniyor

Tarih: 6 Eylül 2026. Motor: Unity 6000.4.4f1. Proje: `C:/Users/Metehan/Documents/ChatGPT/Penalty-King`.

## Oluşturulan altyapı

- Resmi Universal 2D şablonunun render ayarları, ortografik kameralar ve 2D editör ayarları.
- `Assets/Scenes/MainMenu.unity`, `DifficultySelect.unity`, `ModeSelect.unity`, `Gameplay.unity`, `Options.unity`: beş boş sahne, Build Settings içinde etkin ve MainMenu ilk sırada.
- `Assets/Scripts/Core/GameManager.cs`: tek örnek; zorluk, oyun modu, müzik/efekt ses değerlerini `DontDestroyOnLoad` ile sahneler arasında taşır.
- `Assets/Scripts/Core/AudioManager.cs`: tek örnek; ayrı Music ve SFX AudioSource'ları. Döngüsel kalabalık ambiyansı için ek kaynak SFX seviyesini izler. Henüz ses dosyaları eklenmedi.
- `Assets/Scripts/Core/RuntimeBootstrap.cs`: herhangi bir sahneden Play başlatıldığında yöneticileri doğru sırayla kurar.
- `Assets/Scripts/Core/SceneNavigator.cs`: asenkron sahne geçişi ve yinelenen geçiş isteği koruması.
- `Assets/Scripts/Input/TouchInputProbe.cs`: Input System ile birincil dokunmayı algılar; Editor/masaüstünde fareyle test edilebilir.
- `Assets/Scripts/Core/PhaseZeroDiagnostics.cs`: sahne geçişlerini ve durum korumasını gözle kontrol etmek için geçici geliştirme paneli.
- `Assets/Editor/PhaseZeroSetup.cs`: mevcut sahneleri ezmeden eksik altyapı sahnelerini kuran editör aracı.
- `Assets/Tests/PlayMode/InfrastructureTests.cs`: otomatik Play Mode testleri.
- `Assets/Sprites`, `Audio`, `Prefabs`, `UI`: varlık klasörleri.
- `Packages`, `ProjectSettings`, assembly tanımları ve `.gitignore`: yeniden açılabilir Unity projesi ve bağımlılık kilidi.
- `docs/kararlar.md`, orijinal prompt kopyaları ve `README.md`: kapsam değişiklikleri ve açılış yönergeleri.

## Çıktı / test kriteri

**Karşılandı:** Boş sahneler arası geçiş çalışıyor; GameManager veriyi sahneler arası koruyor.

Unity Test Runner / PlayMode / batchmode sonucunda **4 test geçti, 0 test başarısız**:

| Test | Sonuç |
|---|---|
| Beş sahnede gezinme; zorluk, mod, ses ve yönetici kimliklerinin korunması | Geçti |
| SFX susturulunca kalabalığın da susması; müzik kanalının bağımsızlığı | Geçti |
| Yinelenen GameManager ve AudioManager nesnelerinin çoğalmaması | Geçti |
| Sanal dokunmatik cihazda basışın bir kez algılanması; bırakışın yeni basış üretmemesi | Geçti |

Makine tarafından üretilen sonuç: `TestResults/phase0.xml`. Günlük: `Logs/phase0-tests.log`. Unity test süreci çıkış kodu: **0**.

Kurulumda şablondaki eski Input System 1.12.0 sürümü Unity 6.4 ile derleme hatası verdi. Proje Input System **1.19.0** ve Unity'nin yerleşik Test Framework **1.6.0** sürümlerine sabitlendi. Penceresiz testte Game View odağı bulunmadığından sanal dokunma testi giriş yönlendirmesini test süresince açıkça ayarlayıp sonunda geri yükler.

Bu sonuçlar Unity içindeki altyapıyı ve sanal giriş olaylarını doğrular. Gerçek Android/iOS dokunma gecikmesi, cihaz performansı, görsel kalite ve sesin işitsel kontrolü bu fazda test edilmedi; ilgili sonraki fazlarda yapılacak.

## Sonraki onay

Ana prompt Bölüm 2'deki faz onay akışı gereği Faz 1 başlatılmadı. Onay sonrası Singleplayer, pasif Multiplayer/Yakında ve Options butonları olan ana menü hazırlanacak.
