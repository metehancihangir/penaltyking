# 12 Eylül 2026 — Yetişkin oyuncu, geniş kadraj ve top derinliği

- Yalnız **03 Crowd** katmanındaki büyük yüzlü ön taraftar modelleri sabit kalır. Arkadaki iki tribün sırasının toplam 12 bölümü golde yeniden hareket eder.
- Oyuncu daha küçük kafa oranı, geniş ve köşeli omuzlar, güçlü sırt ve daha yetişkin vücut oranlarıyla yeniden üretildi. Kırmızı ve açık mavi formalar korunur.
- Şut dizisi 4 pozdan **8 poza** çıktı: hazır duruş, iki yaklaşma adımı, ayağı dikme, gerilme, temas, vuruş devamı ve toparlanma. Dört sevinç pozu ile atlas toplam 12 karedir. Kareler aynı piksel ölçeğinde, ayak dayanak noktalarıyla hizalanır. Sevinç elleri üst satır boşluğuna taştığı için sprite sınırları görsele göre ayarlanmıştır.
- Saha görüş alanı 960×540 yerine 1120×630 mantıksal alanı kapsar: 16:9 görünümde yaklaşık %14 daha uzaktan görünür. Skor tabelası ve düğmelerin okunabilir boyutu korunur. Yatay yön devam eder.
- Top gol sonrası kalenin kırpılmış sprite'ındaki gerçek arka zemin çizgisine, kendi yarıçapı hesaba katılarak yerleşir. Altına temas gölgesi eklenir. Orta gol hafifçe yana yuvarlanıp kalecinin yanında durur; 0,5 saniye sonunda konum ve dönüş sabittir.
- Topun derinlik sırası uçuşta şutçunun, sonuçta kalecinin de arkasına geçer; artık oyuncunun gövdesinin üzerine çizilmez. Gölge/iz efektleri aynı derinliğe uyar. Yeni atışta özgün çizim sırası geri gelir.
- GOAL! ve oyuncu sevinci 3 saniye; ayarlarda/çıkış onayında duraklatma korunur. Seslere değişiklik yapılmadı. APK üretilmedi.

## Kaynaklar ve kontrol

`Assets/Sprites/Characters/StrikerAdult.png` mevcut karakter referansıyla imagegen kullanılarak üretilmiştir. Magenta zemin Unity shader'ında saydam olarak çizilir; özgün çıktı dosyası değiştirilmeden projeye kopyalandı.

Sahne hazırdır. Yeniden kurulum gerekirse en son **Penalty King → Apply Adult Striker and Wide View (no APK)** / `PenaltyKing.Editor.StrikerRevision.Setup` çalıştırılır. Bu yöntem build almaz.

`docs/previews/striker-wide.gif`: gerçek Unity sahnesinin zaman çizelgesinden 20 fps örneklenmiş şut ve sevinç önizlemesi; canlı maç kaydı değildir, skor sahnenin örnek durumundadır. `striker-wide-goal-Left.png`, `striker-wide-goal-Center.png`, `striker-wide-goal-Right.png` üç son yerleşimi; `striker-wide-wide.png` geniş telefon oranını gösterir.

## Doğrulama

Unity 6000.4.4f1 PlayMode testlerinin tamamı **55/55 geçti** (`TestResults/striker-wide.xml`, `Logs/striker-wide-tests.log`). Yeni test, sekiz ayrı şut pozunu, sol/orta/sağ golde gölgeye oturan ve dönmeden duran topu, topun iki oyuncunun da arkasında çizilmesini ve yeni atışta özgün sıralamanın geri gelmesini kontrol eder. Mevcut ses, duraklatma, sabit maç, Endless, rol devri ve farklı ekran oranı testleri de geçti. 16:9 şut teması, orta/sol gol yerleşimi ve geniş ekranda ikinci oyuncunun forması Unity render'ları üzerinden incelendi. Fiziksel telefon testi/APK build yapılmadı.
