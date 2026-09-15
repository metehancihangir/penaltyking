# Altı ayrı kurtarış pozu ve hareket zinciri

Kaynak: kullanıcının 13 Eylül'de verdiği `animasyon-duzeltme-detayli-prompt.md` ve iki iskelet referansı. APK oluşturulmaz.

## Uygulama

- Unity 6000.4 için **2D Animation 14.0.4** kuruldu. Önce denenen 13 serisi bu Editor sürümüyle derlenmediği için kullanılmıyor.
- İki karakterde gerçek `SpriteSkin`, kaydedilmiş ağırlıklı Sprite meshleri ve 17 kemik bulunur. Canvas çizimi, PlayMode sırasında `SpriteSkin.GetDeformedVertexPositionData()` verisini kullanır. Editörün kare kare önizlemesinde aynı bind matrisleri ve ağırlıklarla eşdeğer örnekleme yapılır.
- Omurga `Hips → Lower Spine → Upper Spine → Chest → Head` hiyerarşisindedir. Gövde ağırlıkları omuz/kalça çevresinde yumuşatıldı; aşırı uzayan arka plan köprü üçgenleri çizilmez.
- `KeeperPoses.cs` altı bağımsız temas pozu içerir. Her pozun bel/göğüs açıları, iki ayak hedefi, iki el hedefi ve bilek açıları ayrıdır. Karakter kökü kurtarış yönünü üretmek için döndürülmez.
- Yan ve üst köşe kurtarışlarında uzanan kol temas sırasında tam açıklığa getirilir; ikinci el topu kavramak için yaklaşır. İtiş bacağı uzar, diğer bacak daha az gerili kalır. Merkez alt çöküş, merkez üst sıçrama kullanır.
- Kısa hazırlık, hedefe uzanma, iniş, sönümlenen temas ve top elindeyken toparlanma birlikte çalışır.
- Şutta kalça hareketi diz salınımından önce, bilek açılması daha sonra başlar. Karşı kol öne, aynı taraftaki kol geriye gider. Vuruştan sonra bacak ileri/yukarı devam eder.
- Gol sevincinde sol kol önce, sağ kol 0,14 saniye gecikmeyle kalkar; yükseklikleri de farklıdır. Kutlama hâlâ 3 saniyedir.

## Doğrulama

`TestResults/detailed-animation.xml`: **13/13 PlayMode testi geçti**. Altı hedefte el/top hizası, öndeki dirseğin açıklığı, itiş bacağı uzaması, üç parçalı omurga ve 17 kemikli gerçek Sprite Skin kullanımı denetlendi. Mevcut atış, kurtarış, gol, kadraj ve toparlanma kontrolleri de geçti.

Bu testler görsel beğeninin yerini tutmaz. Son pozlar ve hareket geçişleri ayrıca Unity yakalamaları üzerinden değerlendirilir. Telefon performans ölçümü veya APK üretimi yapılmadı.
