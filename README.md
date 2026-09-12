# Penalty King

Unity 6000.4.4f1 / C# / Universal 2D. Gece stadyumunda, aynı telefonda iki oyunculu piksel penaltı oyunu.

**12 Eylül çalışma sürümü:** piksel parçalarla sürekli 2D kemik animasyonu, altı bölgede kalecinin iki eliyle top tutması ve boşluksuz hareketli tribünler. Kale işaretleri kaldırıldı; altı görünmez dokunma bölgesi korundu. Yeni APK üretilmedi. Ayrıntılar ve doğrulama: [kemik animasyonu raporu](docs/bone-rig-raporu.md). Unity önizlemeleri: [şut ve kurtarış](docs/previews/bone-rig-save.gif), [gol ve sevinç](docs/previews/bone-rig-goal.gif).

## Son Android paketi — 9 Eylül değişikliklerini içermez

`Builds/Android/PenaltyKing-update7-crowd.apk` — sürüm 0.2.2 (kod 4), Android 8+, ARM64/x86_64, development/test paketi. Belirgin ceza sahası perspektifi, %50 büyük top ve gerçek taraftar seslerini içerir. Önceki `PenaltyKing-update7.apk` sürüm 0.2.0'dır.

Güncelleme Faz 7: 48/48 Unity PlayMode testi geçti. Android doğrulama ayrıntıları ve paket özeti `docs/update-faz-7-raporu.md` dosyasındadır. Fiziksel titreşim testi kullanıcı isteğiyle ertelendi; yeni seslerin uzun süreli rahatlığı kullanıcı dinlemesini bekler.

## Unity'de açılış

Unity Hub ile bu proje klasörünü açın. Project panelinde **Assets → Scenes → MainMenu** sahnesine çift tıklayın, üstte **▶ Play** düğmesine basın. Boş Untitled sahnesi oyunu içermez.

Yatay test için Game çözünürlük listesinden **16:9 / 1280×720** seçin. Telefonun yön ayarı Editor Game penceresinin oranını otomatik değiştirmez. Mevcut sahneler hazırdır; tekrar üretmeniz gerekmez. Eski sahne üreticileri yeni düzeni geri alabilir. Güncel kemik/tribün kurulumu `PenaltyKing.Editor.BoneRigRevision.Setup` ile mevcut Gameplay sahnesine uygulanır.

**Play → 2 Kişilik → Sabit Round / Endless**. Online pasiftir; bot/zorluk yoktur. İlk maçta etkileşimli rehber gösterilir. Saha üzerindeki sıra kartını kapatıp kalede sol/orta/sağ × üst/alt bölgelerinden birine dokunun. Şut seçimi gizlidir; telefonu kaleci oyuncuya verin, sıra kartını kapatıp kurtarış bölgesini seçin. Aynı bölge kurtarış, farklı bölge gol. Her atışta roller değişir.

Sabit Round: oyuncu başına 5 şut; beraberlikte biter. Endless kurtarışlarda da devam eder. Tekrar Oyna aynı modla yeni maç başlatır. Menü düğmesi maçı duraklatıp çıkış onayı sorar; “Maça devam et” aynı noktadan sürdürür. Üst tabelada oyuncu skorları ve beşer son atış görünür.

Music Volume menü müziğini, SFX Volume davul/tezahürat ve olay seslerini yönetir. Sağ üst dişli maçı terk etmeden ayarları açar; animasyon/girişler durur. Sıra kartı ise ambiyansı kesmez. Titreşim tercihi kalıcı ve sesten bağımsızdır; Android gol anında 65 ms tek darbe ister.

## Güncel görsel ve sesler

Büyük ceza alanı kullanıcının mavi çizgili referansı doğrultusunda oyuncunun ayaklarına kadar uzanır. Küçük kale alanı geride, penaltı noktası topun zemin konumundadır. Tüm tribünler golde kutlar. Rehberdeki denemeler maça işlemez. Menüde özgün chiptune, sahada ek sentetik ritim içermeyen gerçek tribün kaydı, golde gerçek taraftar sevinci ve kurtarışta kısık off uyarlaması kullanılır.

Ses kaynakları/lisanslar: `docs/audio-source/README.md`. Değişiklik planı: `update-notes.md`. Tarihsel faz 0–8 raporları önceki sürümü anlatır; güncel doğrulama yerine kullanılmaz.

## Geliştirme ve test

**Window → General → Test Runner → PlayMode → Run All**.

Son sahne üreticisi `PenaltyKing.Editor.PreSevenPolish.Build`; önceki üreticileri çalıştırırsanız bunu en son çalıştırın. Güncel APK üreticisi `PenaltyKing.Editor.CrowdRevision.Android`. Test emülatörü için `Tools/update7_android_smoke.py` gerçek Android dokunmaları gönderir; geliştirme loglarından durum ve performans toplar. `Tools/playback_fix_android_smoke.py` tıklama öncesi müziği, beş sesin dijital çıkışını ve dikey kadrajı kontrol eder.

Aynı proje Unity Editor'de açıkken ikinci bir batch Unity başlatmayın. Fiziksel cihazın verileri otomatik test tarafından temizlenmez; temiz kayıt testi yalnız ayrılmış emülatörde yapılır.
