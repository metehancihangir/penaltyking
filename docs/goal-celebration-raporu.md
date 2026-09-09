# 9 Eylül — Beş saniyelik gol kutlaması

Bu belge önceki sürümü anlatır. Kullanıcı geri bildirimiyle süre 3 saniyeye indirildi; yeni şut/sevinç pozları ve top/tribün düzenlemeleri `action-revision-raporu.md` içindedir.

Golün ağla buluştuğu andan itibaren 5 saniye boyunca piksel **GOAL!** yazısı gösterilir. Hafif büyüme, lime çerçeveli açık renk harfler ve dört küçük altın/açık mavi havai fişek patlaması kullanılır. Ekran flaşı veya yeni ses eklenmez; gerçek taraftar gol sesi ve ambiyans korunur.

Golü atan oyuncu aynı forma ve beden ölçeğiyle küçük zafer zıplamaları ve gövde salınımı yapar. Yazı, oyuncu, tribünler ve mevcut atış aynı zaman çizelgesini kullanır. Ayarlar veya çıkış onayı açılınca kutlama da durur. Kapatınca kaldığı yerden sürer. Kutlama sonunda oyuncu ve top sıfırlanır, telefon devri başlar. Sabit Round'un son atışında normal şekilde sonuç ekranı açılır. Kurtarışlarda GOAL! gösterilmez ve eski 2,2 saniyelik atış süresi korunur.

Higgsfield/Seedance üretimi “Requires plus plan or higher” hatasıyla reddedildi; herhangi bir Higgsfield çıktısı oluşmadı. Kullanıcının “Unity animasyonuyla devam et” yanıtıyla harfler ve parçacıklar Unity UI mesh olarak hazırlandı. Harici video, indirme veya ücretli yeni servis kullanılmadı.

## İnceleme

- `previews/goal-celebration.gif`: gerçek Unity sahnesinden 10 fps örneklenmiş animasyon önizlemesi. Skor tablosu sahne örnekleme durumundadır; canlı maç kaydı değildir.
- Unity menüsü: **Penalty King → Preview Goal Celebration (no APK)**. Ana sahne: **Assets/Scenes/MainMenu**; Game görünümü **16:9**.
- Farklı şut/kurtarış yönleri seçerek gol, aynı yönleri seçerek kurtarış deneyin. Kutlama sırasında Menü'ye basıp “Maça devam et” ile zamanın korunmasını kontrol edin.

APK oluşturulmadı. Telefon performansı ve fiziksel cihazdaki görsel/ses kabulü ayrıca kullanıcı testi gerektirir.

## Doğrulama sonucu

Unity 6000.4.4f1 PlayMode: **20/20 geçti**, `TestResults/goal-celebration.xml`, günlük `Logs/goal-celebration-tests.log`. Kapsam: tam 5 saniyelik gol kutlaması, duraklatma/iptal, sıradaki oyuncuya geçiş, gol/kurtarış sesleri, tek temas/sonuç olayı, iki oyuncunun da kazanabildiği 10 atışlık maçlar, tekrar oynama, 12 atışlık Endless, menüye çıkış, kaleci ölçeği, ağ ve saha geometrisi. Önceki yarım kalan test grubu ayrıca **9/9 geçti** (`TestResults/match-polish-resume.xml`). Önizleme Unity sahnesinden üretildi ve kontrol edildi; derleme başarılı. `git diff --check` temiz.
