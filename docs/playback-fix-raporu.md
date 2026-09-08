# Dikey oynanış ve ses başlangıcı — 8 Eylül 2026

Kullanıcı dikey oynanışın devam etmesini seçti. Yatay yön zorlanmıyor.

## Uygulama

- Dikey ekranda kale ve top artık ekranın karşı uçlarına yerleşmiyor. Uzun ekranlarda kale 1,45 kat, şutçu 310 tasarım birimine büyüyor. Kale, kaleci, üç dokunma hedefi ve topun hedefleri aynı oranı kullanıyor.
- Penaltı noktası topun zemin konumunu izliyor. Kale alanı çizgileri büyüyen kalenin dışında kalacak şekilde uyarlanıyor. Yatay kadraj korunuyor.
- Menü müziği servislerin `Start` aşamasında da başlatılıyor; ilk sahnenin ses dinleyicisi oluşmadan çalmaya çalışma kaldırıldı. İlk açılış için başka bir sahneye geçiş gerekmiyor.
- `runInBackground` etkin: Unity Game penceresine ilk kez tıklanmaması veya pencerenin odağını kaybetmesi oyunun ses güncellemelerini durdurmuyor. Android'in uygulamayı arka plana alma davranışı işletim sistemine ait.
- Mevcut taraftar ve vuruş klipleri bağlı ve çalışıyor. Kullanıcının ses tercihleri sıfırlanmadı. Music menüyü, SFX taraftarı ve olay seslerini kontrol etmeye devam ediyor.
- Development paketinde kısa `[MobileAudio]` ölçümleri kaynakların dijital çıkışını kaydediyor; oyuncu arayüzünde görünmüyor.

## Doğrulama

- İlk açılış, taraftar/vuruş/sonuç sesi, sıra devrinde kesilmeyen ambiyans, tribün animasyonu, kadraj ve dokunma alanları: **9/9 PlayMode testi geçti** (`TestResults/playback-fix.xml`).
- Son kale alanı çizgisi düzeltmesinden sonra ilgili iki kadraj/dokunma testi yeniden geçti (`TestResults/playback-fix-pitch.xml`).
- Android 11 emülatöründe hiçbir giriş göndermeden beş saniye beklendi: menü müziği başladı. Gerçek dokunmalarla bir gol ve bir kurtarış, oyun içi ayarlar ve menüye dönüş doğrulandı.
- Müzik, taraftar, Kick, GoalVictory ve SaveOff için sıfırdan büyük dijital ses çıkışı ölçüldü. Rapor: `playback-fix-android-results.json`. Bu ölçüm fiziksel telefon hoparlöründe dinleme yerine geçmez.
- Android dikey görüntü: `previews/playback-fix-android-portrait.png` (720×1280). Daha uzun ekran için Unity görüntüsü: `previews/playback-fix-portrait.png` (390×844).

## Paket ve tekrar üretim

`Builds/Android/PenaltyKing-update7-fix.apk` — 0.2.1 / kod 3. Unity metodu: `PenaltyKing.Editor.PlaybackFix.Android`. Android kontrolü: `Tools/playback_fix_android_smoke.py` (yalnız ayrılmış yerel test emülatörü).

Nihai derleme: başarı, 0 hata; 123.547.047 bayt (117,8 MiB). SHA-256: `DD6B015C3F940177083B9FCF2E6D73CB2AF51F3E060F50BD58E465DC5CB66F95`. Derleme kaydı: `Logs/playback-fix-final-build.log`.

Unity Editor'de `Assets/Scenes/MainMenu.unity` açılıp Play'e basılır. Müzik için ayrıca menü düğmesine basmak gerekmez. Test edilen dijital çıkışa rağmen bilgisayarda ses yoksa Game görünümündeki hoparlör/sessiz düğmesi ve Windows'un Unity ses çıkışı kontrol edilmelidir.
