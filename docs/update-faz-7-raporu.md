# Güncelleme Faz 7 — Entegrasyon ve Android teslimi

Tarih: 8 Eylül 2026

## Son referans düzeltmesi

Kullanıcının mavi çizgili görseli önceki yorumu netleştirdi: büyük ceza alanı çizgisi kaleye yakın değil, oyuncunun ayaklarına yakın görünmelidir. Büyük alan bu doğrultuda derinleştirildi; küçük kale alanı kaleye yakın kalır. Penaltı noktası büyük alanın içinde, topun zemindeki konumundadır. Topun üzerinde ikinci bir işaret çizilmez. Dikey/yatay uyarlanabilir kompozisyon korunur.

Önceki altı düzeltme de bu pakete dahildir: saha üzerinde kısa sıra kartı, kesintisiz devir ambiyansı, sakin davul ağırlıklı ses ve yeni gol/kurtarış efektleri, tüm tribünlerin kutlaması, etkileşimli ilk kullanım rehberi ve özgün piksel menü müziği. Son üretici `PreSevenPolish.Build`.

## Unity testleri

**48/48 PlayMode testi geçti.** Sonuç: `TestResults/update7.xml`, günlük: `Logs/update7-tests.log`. Son ceza alanı yerleşimi bu koşuya dahildir.

Test kapsamı: bot/zorluk bulunmaması, yerel seçimler, dokuz yön eşleşmesi, gizli seçim/devir, beşer şut ve beraberlik, Endless devamı ve sınırlı geçmiş, tekrar/çıkış, ilk kullanım kaydı ve pratik giriş izolasyonu, ekran oranları, bütün taraftar bölgeleri, ses kanalları, menü müziği geçişleri, gol titreşim isteği ve ayarlarda duraklatma.

## Android

Yeni paket `Builds/Android/PenaltyKing-update7.apk`; sürüm 0.2.0, kod 2, Android 8+ (API26), ARM64 ve x86_64, development/test derlemesi. Önceki APK ayrı dosyada korunur.

Android emülatöründe gerçek uygulama UI'sine ADB dokunmaları gönderilir; maçın iç fonksiyonları çağrılarak ekranlar atlanmaz. `Tools/update7_android_smoke.py` koşuyu tekrarlar. Geliştirme telemetrisi ekran kontrolü eklemez; yalnız logcat'e mevcut etkileşim hedeflerini, maç durumunu ve performans pencerelerini yazar.

Derleme sıfır hatayla 2 dakika 46 saniyede tamamlandı. APK: **104.225.588 bayt (99,4 MiB)**. SHA-256: `148C7EA143E78C6787F090D406DFCDFBEBD255DD102345960803FE6DC272D33C`.

Android 11/API30 x86_64 emülatöründe 720×1280 dikey / 1280×720 yatay test edildi. Temiz rehber, ayarlar ve devam, 10 atış sonunda 5–5 beraberlik, tekrar oynama, Options, ekran dönüşü, uygulama yeniden açılınca rehberin tekrar gelmemesi ve 12 atışlık Endless başarılı. 23 atışta toplam 46 şut/kaleci yön dokunması işlendi; kontrol edilen loglarda NullReference/AndroidJava/FATAL exception yok.

38 ölçüm penceresinde **59,4–60,0 FPS**; en yüksek pencere P95 16,70 ms, en uzun tek kare 50,04 ms. Dokunma kuyruğundan işleyiciye 5,203–23,153 ms. Bu fiziksel parmak-ekran gecikmesi değildir. Emülatör host GPU ve 1536 MB yapılandırılmış RAM kullanır. Ham sonuç: `docs/update7-mobile-results.json`, günlük: `Logs/update7-android-logcat.txt`.

Kurulan pakette sürüm 0.2.0/kod 2, minSdk26/targetSdk36 ve VIBRATE izni doğrulandı. Son oynanış: `docs/previews/update7-android-landscape-gameplay.png`; rehber, devir, beraberlik, seçenekler ve Endless görüntüleri aynı klasörde `update7-android-*` adlarıyla kayıtlı.

## Bilinen doğrulama sınırları

Fiziksel telefon titreşimi kullanıcı isteğiyle ertelendi; emülatör testi bu kontrolün yerine geçmez. Yeni seslerin uzun süreli rahatlığı ve işitsel beğenisi kullanıcı dinlemesini bekler. Bu oturumun aracı ses girdisini desteklemediği için dinlenmiş gibi bir sonuç raporlanmaz. Kaynak lisansları ve sayısal ses ölçümleri `docs/audio-source/README.md` dosyasındadır. Mağaza yayını/iOS paketi bu teslimde yoktur.


