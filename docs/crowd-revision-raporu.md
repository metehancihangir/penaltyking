# Perspektif, top ve gerçek taraftar sesi — 0.2.2

- Ceza alanının arka kenarı öne göre daraltıldı; yan çizgiler belirgin yamuk perspektifi oluşturuyor. Küçük kale alanı onun içinde kalıyor.
- Topun çapı %50 büyüdü. Uçuş ve iz efektleri aynı ölçeği kullanır; şut yönü ve sonuç hesaplaması değişmedi.
- CalmDrumLoop'taki sonradan eklenen sentez ritim devreden çıkarıldı. CrowdChant, belgelenmiş CC0 gerçek tribün kaydını kullanır; kayıt içindeki doğal davullar dışında ilave davul veya müzik yoktur.
- GoalVictory melodisi çıkarıldı; GoalCrowd gerçek toplu taraftar sevinci kullanır. Menü müziği yalnız menülerde kalır. Top teması ve kurtarış sesi korunur.
- Önceki sessizliğin nedenini kullanıcı SFX=0 olarak doğruladı. Kullanıcının kaydedilmiş ses tercihleri sıfırlanmadı.

Unity doğrulaması: **9/9 PlayMode testi geçti** (`TestResults/crowd-revision.xml`): gerçek klipler ve sinyal seviyesi, ses kanalı kontrolü, şut/kurtarış animasyonları, dokunma hedefleri ve ekran oranları.

Sahne üretimi: `PenaltyKing.Editor.CrowdRevision.Setup`; APK: `PenaltyKing.Editor.CrowdRevision.Android`; ses üretimi: `Tools/build_crowd_revision.py`. Kaynaklar ve lisans geçmişi `audio-source/README.md` içinde.

Paket: `Builds/Android/PenaltyKing-update7-crowd.apk`, 0.2.2 / kod 4.

Android 11 emülatöründe 720×1280 dikey görüntü, gerçek dokunmalarla gol/kurtarış ve ayarlardan dönüş geçti. Beş ses kaynağında dijital çıkış ölçüldü; golde çalan kayıt GoalCrowd. Rapor: `crowd-revision-android-results.json`; görüntü: `previews/crowd-revision-android-portrait.png`. Fiziksel hoparlörde dinleme otomatik doğrulama kapsamında değildir.

Derleme 0 hata ile tamamlandı. APK 104.620.700 bayt. SHA-256: `10A839DDFCE6C0D78B7D0F02CC3779A2C1A09A0BB72DDE22333D9CA5DE87119A`.
