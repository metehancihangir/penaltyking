# 9 Eylül 2026 — Yatay oynanış, kullanıcı ambiyansı ve ağ tepkisi

Bu çalışma Unity test sürümüdür. Kullanıcının talebi doğrultusunda **APK oluşturulmadı**; mevcut 0.2.2 APK bu değişiklikleri içermez.

## Değişiklikler

- Yalnız iki yatay yön etkin. Dikey otomatik dönüş kapalı; çalışma zamanı da bu ayarı uygular.
- Referanstaki kale %5 küçültüldü; tabanı saha çizgisinde kaldı. Dokunma bölgeleri ve topun hedefleri birlikte daraltıldı.
- Penaltı noktası ve başlangıç topu kaleden 18 tasarım birimi uzaklaştırıldı (1280×720 görüntüde 24 piksel). Futbolcunun başlangıç/temas konumu buna uyarlandı. Önceki %50 top büyütmesi korunuyor.
- Çok geniş yatay ekranlarda saha kompozisyonu dikey olarak sıkışmıyor; 16:9 merkezdeki oyun mesafesi korunup arka plan yanlara genişliyor.
- Kalecinin dört yana atlama karesi atlasın ortak piksel ölçeğinde gösteriliyor. Toparlanma karesinin yüksekliğe göre yeniden büyütülmesi kaldırıldı; diz çöken pozun zemine teması korundu.
- GoalNetRipple, gol anında temas noktasından kısa ve sönümlenen bir ağ dalgası üretir. Direkler/üst çerçeve sabit kalır. Kurtarışta tetiklenmez; ayarlarda aynı zaman çizelgesiyle durur, iptal/yeni atışta sıfırlanır.

## Kullanıcının WAV dosyası

`638367__usbmed_ambiences_sound_library__colombian_soccer_stadium_crowd_ambix.wav` dosyasının yalnız 0–10 saniyesi kullanıldı. Kaynak 48 kHz, 24-bit, dört kanallıdır. İlk yönsüz kanal çift mono olarak hazırlandı; yönsel kanallar sıradan stereo kanalları gibi karıştırılmadı. Kaynağın aslı değiştirilmedi.

Sonuç `Assets/Audio/Stadium/UserStadiumLoop.wav`: tam 10 saniye, stereo PCM, iki uçta 10 ms yumuşatma, tepe 0,20. Üzerine ritim/müzik eklenmedi. SFX Volume kontrolünde döner. Golde GoalCrowd, kurtarışta SaveOff korunur. Kaynak kullanıcı tarafından sağlandı; önceki CC0 kayıtlarının lisans tespiti bu yeni kaynağa otomatik uygulanmaz.

Üretim: `Tools/build_user_stadium.py`; sayısal kayıt `audio-source/user-stadium-metrics.json`.

## Doğrulama ve Unity'de deneme

15/15 PlayMode testi geçti (`TestResults/landscape-revision.xml`). Kapsam: kaleci kareleri arasında piksel ölçeği, gol/kurtarış ağ davranışı ve sabit çerçeve, şut zamanlaması, ses kliplerinin sinyali ve 10 saniye uzunluğu, ses ayarları, duraklatma, üç hedefin kaleye oturması ve farklı ekran oranları.

`Assets/Scenes/MainMenu.unity` sahnesini açın. **Game** penceresinin çözünürlük listesinden **16:9** veya **1280×720** seçip Play'e basın. Unity Editor Game penceresinin oranı, telefon yön ayarından bağımsızdır. Telefonda yatay yön ayarı sonraki bir build'e dahil olacaktır.

Sahne kurulumunu yeniden üretmek için `Penalty King → Apply Landscape Revision (no APK)`; yalnız görüntüler için `PenaltyKing.Editor.LandscapeRevision.Capture`. Bu iki yöntem APK üretmez.
