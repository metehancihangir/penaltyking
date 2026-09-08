# Güncelleme Faz 6 — Davullu ambiyans, gol tepkisi ve titreşim

Tarih: 8 Eylül 2026

Uygulama ve Unity testleri tamamlandı. İşitsel kabul ve gerçek telefon titreşim kontrolü bekliyor; Faz 6'nın tüm kabul maddeleri kapanmış sayılmıyor.

## Ses

Gameplay artık ChantDrumLoop ve GoalOuff kullanır. Önceki CrowdLoop/GoalCheer kaynakları arşiv ve eski üretici için korunur ancak aktif sahneye bağlı değildir. Yeni ambiyans, gerçek maçtaki taraftar ve davul kaydından 24 saniyelik çapraz geçişli döngüdür. Gol tepkisi toplu “oooh” kaydının kısa, nefesli “ff” kuyruğuyla uyarlanmış halidir. Gol olayına bağlıdır; kurtarış eski SaveReaction kaydını kullanır.

Ambiyans (davul dahil) ve olaylar ortak SFX ayarını izler. Müzik ve titreşim tercihi bağımsızdır. SFX sıfıra inince döngü sessiz ilerler; yeniden ses açınca baştan başlamaz. Menü çıkışı kaynakları durdurur. Ses tepe seviyeleri ambiyans 0,19, gol 0,65; mevcut vuruşla toplam teorik tepe 0,99'u aşmaz. Döngü sınır farkı 0,000949'dur. Bunlar teknik ölçümlerdir; duyumsal kaliteyi kanıtlamaz.

Kaynaklar ve CC0 lisans bağlantıları `docs/audio-source/README.md` içindedir. Yeniden üretim: Unity `UpdateAudioSource.Decode` → Python `Tools/build_update6_audio.py` → Unity `UpdatePhaseSixSetup.Build`.

## Titreşim

GameplayHaptics, ShotPresentation'ın görsel Impact olayını dinler. Gol başına bir kez 65 ms tek darbe ister. Kurtarışta, kapalı tercihte veya gol gösterilmeden önce darbe istemez. Duraklatma aynı olayı yeniden üretmez. Yeni vuruşta koruma sıfırlanır. Options ve oyun içi ayarlar aynı kalıcı VibrationEnabled tercihini yönetir.

Android 8+ cihazda VibrationEffect.createOneShot ve vibrator servisi kullanılır; cihazda motor yoksa veya servis çağrısı kullanılamazsa oyun devam eder. Editor/masaüstünde fiziksel titreşim uygulanmaz. Android Gradle manifestine VIBRATE izni eklenir; aynı izin çoğaltılmaz. Android API kaynağı: https://developer.android.com/reference/android/os/VibrationEffect#createOneShot(long,int)

## Doğrulama

Faz 5: 44/44 PlayMode testi başarılı (`TestResults/update5.xml`).

Faz 6: ses, titreşim ve oyun içi ayarlar için 10/10 PlayMode testi başarılı (`TestResults/update6.xml`, `Logs/update6-tests.log`). Sessiz SFX ile gol darbesi, sonuç öncesi/sonrası duraklatma, kapalı tercih, kurtarışta sessizlik ve sonraki golde tekrar tetikleme doğrulandı. Test olayı donanım isteğini sayar; telefon motorunun çalıştığını iddia etmez.

Android oyuncu kodu derlemesi başarılı; VIBRATE manifest izninin bir kez eklendiği doğrulandı (`Logs/update6-android-check.log`). Kontrol sırasında eksik Android JNI yerleşik modülü bulundu ve `Packages/manifest.json` ile kilit dosyasına eklendi. Bu doğrulama APK veya cihaz testi değildir.

## Bekleyen gerçek kullanım kontrolü

Bu oturumda ses girdisi araç tarafından desteklenmedi; dosyalar dinlenmiş gibi değerlendirilmedi. Kullanıcıya `docs/previews/update6-audio-demo.wav` örneği sunuldu ve işitsel değerlendirme soruldu. Bu, tam SFX seviyesinde hazırlanmış 52 saniyelik varlık miksidir, çalışma zamanı kaydı değildir. Gol yaklaşık 4. ve 30. saniyelerde, kurtarış yaklaşık 10. saniyede; döngü birleşimleri 24. ve 48. saniyelerdedir.

Telefon hoparlöründe davul/tezahürat dengesi, gol tepkisinin istenen “ouuffff” hissi ve döngü birleşimi kontrol edilmeli. Gerçek telefonda titreşim açık/kapalı, SFX sıfır ve ayarlardan dönüş senaryoları denenmeli. Yeni APK Güncelleme Faz 7 kapsamındadır; bu fazda mevcut APK değiştirilmedi.

8 Eylül 2026 kullanıcı geri bildirimi: Davullu ambiyans beğenildi ve kabul edildi. Gerçek telefon gol titreşim kontrolü kullanıcının isteğiyle sonraya ertelendi; doğrulanmış sayılmaz. Gol tepkisi için ayrıca açık kabul bildirilmedi.

