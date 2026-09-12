# Altı hedef ve animasyon güncellemesi — 12 Eylül 2026

## Oynanış

- Şutçu ve kaleci **Sol Üst, Orta Üst, Sağ Üst, Sol Alt, Orta Alt, Sağ Alt** bölgelerinden birini seçer.
- Aynı sütun ve aynı yükseklik seçilirse kurtarış; başka bir bölge seçilirse gol olur. Örneğin Sol Üst şutu, Sol Alt seçimiyle kurtarılmaz.
- Şut seçimi telefon devrinde gizli kalır. Hedef işaretleri sadece seçim sırasında görünür.
- İki insan, tek telefon, dönüşümlü roller, oyuncu başına beş şut ve Endless kuralları korunur.
- Yeni altı bölgeli deneme rehberi bir kez gösterilir; rehberdeki seçim skoru etkilemez.

## Görseller ve hareket

- Büyük yüzlü ön taraftar modelleri kapatıldı. Stadyumun iki katındaki diğer taraftarların sevinci korunur.
- Ağ efekti Image bileşeninin gerçek çizim sınırlarını kullanır. Kale direkleri gol sırasında genişleyip daralmaz.
- Siyah top kopyası görünümündeki gölge, yumuşak oval gölgeyle değiştirildi; vuruşta ayrı parça gibi görünen küçük toz kareleri kaldırıldı.
- Yeni atlas: 12 şut karesi. Hazırlanma, yaklaşma, destek ayağı, geriye salınım, temas ve vuruş sonrası takip ayrı pozlardır. Temas karesinin krampon konumu topa hizalanır.
- Yeni kaleci atlası: üst köşeye uzanma, alçak yatış, orta üst sıçrama, orta alt kapanma ve toparlanma pozları. Sol dalışlar sağ dalışın aynasıdır. Top hedefi ile kurtarış pozunun eldiven temas noktası aynı koordinatı kullanır.
- Saha yeşilleri, perspektif çim dokusu, kesintisiz beyaz çizgiler ve eliptik penaltı noktası yenilendi.
- Üç saniyelik GOAL!, mevcut sevinç kareleri, forma renkleri, sesler ve menüye dönüş onayı korunur.

## Doğrulama

Unity Editor açık olduğundan doğrulama `.utmp/six-verification` içindeki ayrı proje kopyasında yapıldı. Tam PlayMode çalışması **57/57 geçti** (`TestResults/six-target.xml`). Son hedef konumu, rehber ve işaret görünürlüğü değişikliklerinden etkilenen **11/11 test tekrar geçti** (`TestResults/six-target-final.xml`). Doğrulanan Gameplay sahnesi ve iki yeni atlasın sprite kesimleri ana projeye aktarıldı; kod dosyalarıyla eşleşmeleri kontrol edildi.

Yeni kontroller 36 şut/kaleci eşleşmesini, altı gerçek dokunma bölgesini, eldiven/top temasını, hedef işaretlerinin tamamının dokunulabilir alan içinde kalmasını ve ağ hareketinde çizim sınırlarının sabitliğini kapsar. Mevcut ses, duraklatma, Endless, sabit maç ve telefon devri regresyonları da geçti.

Önizlemeler: `docs/previews/six-target-ready.png`, `six-target-guide.png`, `six-target-frame-14.png` ve `six-target-save-*.png`.

Unity açıkken eski sahne bellekte kalırsa Play modundan çıkıp **Assets/Scenes/MainMenu.unity** sahnesini yeniden açın ve Play → 2 Kişilik üzerinden başlayın. Altı bölgeli rehberin tamamlanması önceki üç bölgeli rehberden ayrı tutulur.

APK üretilmez. Önizlemeler Unity sahnesinin gerçek görselleriyle, aynı animasyon zaman çizelgesi örneklenerek alınır; telefon performans ölçümü değildir.
