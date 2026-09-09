# 9 Eylül — Şut ve gol sonrası hareket güncellemesi

- GOAL! kutlaması **3 saniye**. Yeni ses eklenmedi; mevcut gerçek taraftar ve top sesleri korunuyor.
- En öndeki taraftar şeridi ve alt tribün sırası sabit. Yalnız üst sıradaki altı tribün bölümü hafif kutlama hareketini sürdürüyor.
- Yeni sekiz pozlu oyuncu atlası: hazır duruş, gerilme, ayağın topa temas ettiği vuruş, devam hareketi ve dört ayrı kolları kaldırma/yumruk sevinci pozu. Eski duruşu zıplatma kaldırıldı. Aynı beden ölçeği korunur; oyuncuya göre kırmızı/açık mavi forma uygulanır.
- Şut hazırlığı 0,70 saniye; top temastan önce hareket etmez. Vuruş ayağının atlas üzerindeki temas noktası topa hizalanır. Topun ağla buluşması 1,36 saniyede olur; ses ve kaleci hareketi aynı atış zaman çizelgesindedir.
- Gol sonrası top ağın arka tabanına yerleşir. Konumu ve dönüşü 0,5 saniyede sabitlenir; kutlama boyunca dönmez. Sahanın önünde kalan eski top gölgesi gol sonrası gizlenir.
- Ceza yayının yatay yarıçapı 130'dan 235'e, perspektifteki derinliği 25'ten 78'e çıktı. Geniş yay ekranın altına doğru doğal olarak devam eder; yatay kadraj korunur.

## Performans kapsamı

Kullanıcı dizüstü bilgisayarının fişinin çekili olabileceğini belirtti. Kasma oyundan kaynaklanmış gibi kabul edilmedi; cihazdaki FPS sorununun çözüldüğü iddia edilmiyor. Kodda doğrudan görülen gereksiz işler azaltıldı: duran ağ artık her kare mesh yenilemesi istemez; hareket sırasında 49×21 yerine 33×15 köşe kullanılır. GOAL efekti ayrı alt Canvas'tadır ve piksel efekt güncellemesi 30 fps ile sınırlıdır. Oyuncu materyalinin UV parametresi yalnız poz değişiminde yazılır; tekrarlı eski konfeti katmanı kapalıdır.

## Varlık ve yeniden üretim

`Assets/Sprites/Characters/ShooterPerformance.png`, mevcut ShooterActions referans alınarak imagegen ile üretildi. İlk çıktının saydamlık yerine dama zemini üretmesi üzerine arka plan aynı araçla magentaya çevrildi. Unity PlayerKit shader'ı magentayı çizmez; yeni kaynak dosya korunur. Kareler ayak dayanak noktalarıyla bölünür, vuruş noktası atlas üzerinden hesaplanır.

Unity menüsü: **Penalty King → Apply Action Revision (no APK)**. Yöntem `PenaltyKing.Editor.ActionRevision.Setup`; yalnız sahne/varlık bağlama ve önizleme yapar. Mevcut sahne hazırdır. Eski sahne üreticileri tekrar çalıştırılırsa bu adım en son uygulanmalıdır.

APK oluşturulmadı. Görsel kabul için MainMenu'yu açıp 16:9 Game görünümünde iki farklı yönden gol/kurtarış deneyin; kutlama sırasında ayarlar veya çıkış onayını açıp devam edin.
