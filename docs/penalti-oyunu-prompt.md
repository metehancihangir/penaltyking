# Piksel Penaltı Oyunu — Geliştirme Promptu

## 1. Genel Bakış

2D, 16-bit SNES tarzı piksel-art görsellere sahip, mobil öncelikli (touch/dokunmatik) bir penaltı atma oyunu geliştir. Oyuncu asla ekranda görünmez; sadece topu, kaleciyi, kaleyi, çim sahayı, piksel taraftarları ve stadyum tasarımını görür. Oyun basit ama akıcı, tatmin edici animasyonlara ve atmosferik ses tasarımına sahip olmalı.

**Önerilen teknoloji:** Unity (C#). Gerekçe: 2D sprite animasyonu, dokunmatik girdi, fizik motoru ve Android/iOS'a export için endüstri standardı ve iyi dokümante edilmiş bir araç seti sunuyor. (Alternatif olarak tarayıcı öncelikli olunmak istenirse HTML5 + Phaser.js kullanılabilir, ancak mobil önceliği nedeniyle Unity tavsiye edilir.)

---

## 2. Ana Menü

Giriş menüsü üç seçenekten oluşur:

1. **Singleplayer** — Botlara (AI kaleciye) karşı penaltı atma modu.
2. **Multiplayer** — Şimdilik menüde yer almasın veya "Yakında" (Coming Soon) etiketiyle pasif görünsün. Şu an için işlevsel olmasına gerek yok.
3. **Options** — Sadece ses ayarları içerir (bkz. Bölüm 6). Başka hiçbir ayar (grafik, dil, kontrol vb.) bu aşamada eklenmeyecek.

Singleplayer'a tıklandığında oyuncu önce **zorluk seviyesi** (Kolay / Orta / Zor) ve ardından **oyun modu** (bkz. Bölüm 4) seçer.

Menüdeki tüm butonlar piksel-art'a uygun görsel geri bildirim state'lerine sahip olmalı (normal / hover / pressed) — dokunulduğunda buton görsel olarak tepki vermeli.

---

## 3. Temel Oynanış Mekaniği

- Oyuncunun tek yapması gereken şey, kalede **Sol / Orta / Sağ** bölgelerinden birine dokunmak (touch).
- Kontrol şeması **tamamen dokunmatik/touch tabanlı** olmalı — mobil öncelikli tasarım. Ekranın alt kısmında veya kale üzerinde üç dokunma bölgesi (sol köşe, orta, sağ köşe) tanımlanmalı; oyuncu hangi bölgeye dokunursa şut o yöne gider.
- **Güç çubuğu veya nişan alma gibi ekstra bir mekanik YOK.** Sadece yön seçimi var — basit ve hızlı bir oynanış hedefleniyor.
- Kaleci, oyuncunun dokunmasıyla **aynı anda** bir yöne (sol/orta/sağ) rastgele/AI mantığıyla karar verip dalar. Bu klasik penaltı mekaniğidir: oyuncu kaleciyi göremeden/bilmeden karar verir.
- **Şut sonucu minimal tutulacak:** Sadece iki olası sonuç var — (a) top kaleciye çarpar ve kaleci kurtarır, (b) top kalecinin seçmediği yöne gider ve **gol** olur. Direkten dönme, üstten auta gitme gibi ek varyasyonlar bu aşamada YOK.
- Oynanış ekranında basit bir **skor göstergesi** (UI text) her zaman görünür olmalı; oyuncu güncel gol/şut sayısını (moda göre) anlık takip edebilmeli.

---

## 4. Oyun Modları (Singleplayer içinde)

Oyuncu Singleplayer'a girdiğinde iki modu seçebilmeli:

1. **Sabit Round Modu:** Belirli sayıda penaltı (örn. 5 şut) çekilir, sonunda kaç gol atıldığı skor olarak gösterilir.
2. **Endless (Süresiz) Modu:** Kaleci ilk kez kurtarana kadar oyun devam eder; oyuncu art arda kaç gol attıysa o sayı final skoru olur (yüksek skor/rekor takibi eklenebilir).

Her iki modun sonunda da bir **özet/skor ekranı** gösterilmeli:

- **Sabit Round Modu sonunda:** Toplam gol sayısı gösterilir; **"Tekrar Oyna"** ve **"Ana Menüye Dön"** butonları bulunmalı.
- **Endless Modu sonunda:** Mevcut skor ve varsa (cihazda kalıcı olarak saklanan) yüksek skor gösterilir; aynı şekilde "Tekrar Oyna" ve "Ana Menüye Dön" seçenekleri sunulmalı.

---

## 5. Zorluk Seviyesi

- Oyuna başlamadan önce oyuncu **Kolay / Orta / Zor** seçeneklerinden birini seçer.
- Zorluk seviyesi, kalecinin doğru yönü tahmin etme **olasılığını** değiştirir (örn. Kolay: kaleci %20 ihtimalle doğru yöne gider, Orta: %35, Zor: %50 gibi — kesin oranlar geliştirme sırasında dengelenebilir).
- **Dinamik/artan zorluk YOK.** Yani oyun ilerledikçe (art arda gol atıldıkça) kaleci daha akıllı hale gelmeyecek; seçilen zorluk seviyesi oyun/round boyunca sabit kalacak.

---

## 6. Options Menüsü

Sadece iki ayar kaydırma çubuğu (slider) bulunacak:

1. **Müzik Sesi (Music Volume)**
2. **Efekt Sesi (SFX Volume)**

Başka hiçbir ayar (grafik kalitesi, dil, kontrol düzeni, titreşim vb.) bu aşamada eklenmeyecek.

Slider değerleri gerçek zamanlı olarak ilgili ses kanallarına yansımalı ve uygulama kapatılıp tekrar açıldığında **kalıcı olarak korunmalı** (cihazda saklanan ayar verisi).

---

## 7. Görsel Stil ve Sahne Kompozisyonu

- **Piksel art seviyesi:** 16-bit SNES tarzı — sade 8-bit'ten daha detaylı, gölgelendirme ve renk paleti zenginliği olan bir piksel stil.
- **Kamera açısı:** Sabit, arkadan/kaleye doğru bakan görünüm (topun arkasından kaleye bakan klasik penaltı kamerası). Perspektif oynamaları veya geniş açı stadyum çekimleri YOK.
- **Sahne öğeleri (ekranda görünmesi gerekenler):**
  - Top
  - Kaleci (AI kontrollü)
  - Kale (file/direkler)
  - Çim saha
  - Piksel taraftarlar (arka planda, tribünlerde)
  - Stadyum tasarımı (tribün yapısı, ışıklandırma direkleri vb.)
- **Şutu çeken oyuncu KESİNLİKLE ekranda görünmeyecek.** Ne karakter modeli ne de ayak/bacak gibi bir parçası gösterilmeyecek — sadece topun hareketi görünür olacak.
- **Atmosfer/zaman dilimi:** Akşam/gece maçı, **floodlight (stadyum aydınlatma direkleri)** ile aydınlatılmış bir sahne. Gündüz/güneşli versiyon bu aşamada YOK.
- **Katman sıralaması (z-order):** Sahnedeki görsel öğeler şu sıraya göre dizilmeli (arkadan öne): arka plan → tribün/stadyum → çim saha → kale → kaleci → top → UI (skor, dokunma bölgeleri vb.).
- **Ekran geçişleri:** Menüler ve sahneler arası geçişlerde basit bir fade-in/fade-out (veya benzeri) geçiş efekti kullanılmalı; ani/sert sahne değişimlerinden kaçınılmalı.

---

## 8. Animasyonlar

Animasyon kalitesi oyunun en kritik parçalarından biri olarak ele alınmalı:

1. **Şut Animasyonu:** Oyuncu görünmediği için "şut çekme" animasyonu topun hareketiyle temsil edilir — topun sekme, dönme (spin) efekti, hıza bağlı gölge/blur (motion blur) ve kaleye doğru gerçekçi bir yörünge (trajectory) izlemesi gerekir. Şut anında ekranda kısa bir "vuruş" hissi yaratacak görsel/ses senkronizasyonu (örn. hafif ekran sarsıntısı/screen shake, toz efekti) eklenebilir.
2. **Kaleci Kurtarış Animasyonu:** Kaleci seçtiği yöne akıcı bir dalış (dive) animasyonu yapmalı — havada uzanma, topu tutma/tokatlama pozu, yere düşüş ve toparlanma kareleri dahil çok karakterli bir sprite animasyonu olmalı. **Sol / Orta / Sağ yönleri için ayrı ayrı üç animasyon varyasyonu** olmalı (kaleci gittiği yöne göre farklı dalış görüntüsü sergilemeli).
3. **Piksel Taraftar Kutlama Animasyonu (Gol Anında):** Gol olduğunda arka plandaki piksel taraftarlar basit ama fark edilir bir kutlama animasyonu yapmalı (örn. kolları havaya kaldırma, zıplama, renkli parçacık/konfeti efektleri). Karmaşık bireysel animasyonlara gerek yok — grup halinde senkronize, tekrar eden basit bir "cheer" döngüsü yeterli.
4. **Animasyon–Ses Senkronizasyonu:** Tüm animasyonlar (şut, dalış, kutlama), ilgili ses efektleriyle (bkz. Bölüm 9) zamanlama açısından senkronize olacak şekilde tetiklenmeli (örn. animasyon event/callback mekanizmasıyla).

---

## 9. Ses Tasarımı

- **Şuttan önce (bekleme anında):** Stadyumda düşük seviyeli, normal/sürekli tezahürat/kalabalık gürültüsü (ambient crowd noise) çalmalı — gerginlik hissi yaratmak için.
- **Gol olduğunda:** Yüksek enerjili bir "cheer" / kutlama sesi efekti devreye girmeli (kalabalığın patlaması gibi).
- **Kaleci kurtardığında:** Kalabalığın hayal kırıklığı sesi ("ohh" tepkisi gibi) veya daha sessiz/düşük tonlu bir tepki sesi çalmalı.
- **Vuruş anı:** Topa vurulma sesi (kick sound effect).
- Tüm bu efektler **SFX Volume** kaydırma çubuğuyla kontrol edilirken, varsa arka plan müziği **Music Volume** ile ayrı kontrol edilmeli.

---

## 10. Kapsam Dışı (Bu Aşamada Eklenmeyecekler)

Netlik için, aşağıdakilerin bu ilk versiyonda **eklenmeyeceği** özellikle belirtilmelidir:

- Multiplayer modu (ileride eklenecek, şimdilik yok/pasif)
- Güç çubuğu, nişan alma gibi ekstra şut mekanikleri
- Artan/dinamik zorluk sistemi
- Direkten dönme, üstten auta gitme gibi şut sonucu çeşitliliği
- Gündüz/gece seçimi (sadece gece/floodlight sabit)
- Grafik kalitesi, dil, kontrol düzeni gibi ek options ayarları
- Oyuncu karakterinin görsel olarak gösterilmesi

---

## 11. Performans ve Kalite Kriterleri

- Oyun, hedef mobil cihazlarda (orta seviye Android/iOS) **düşük touch input gecikmesiyle** çalışmalı — dokunma ile kalecinin/topun tepki vermesi arasında fark edilir bir gecikme olmamalı.
- Animasyonlar (şut, dalış, kutlama) **akıcı frame rate** ile oynamalı, takılma/donma yaşanmamalı.
- Yayına almadan önce gerçek cihazda veya emulator üzerinde bu iki kriter test edilmeli.

---

## 12. Özet Teknik Gereksinim Listesi

| Özellik | Karar |
|---|---|
| Platform | Mobil öncelikli, touch kontrol |
| Teknoloji önerisi | Unity (C#) |
| Görsel stil | 16-bit SNES piksel art |
| Kamera | Sabit, kaleye bakan arka açı |
| Şut mekaniği | Sadece yön (sol/orta/sağ) |
| Kaleci AI | Oyuncuyla eş zamanlı, rastgele/AI karar |
| Zorluk | Kolay/Orta/Zor (sabit, artmıyor) |
| Oyun modları | Sabit round + Endless (ikisi de) |
| Şut sonucu | Minimal (gol veya kurtarış) |
| Atmosfer | Akşam/gece, floodlight |
| Ses (Options) | Music Volume + SFX Volume (ayrı) |
| Oyuncu karakteri | Görünmez |
| Ayar kalıcılığı | Ses seviyeleri cihazda kalıcı saklanır |
| Skor gösterimi | Oynanış sırasında sürekli görünür UI text |
| Round sonu ekranı | "Tekrar Oyna" + "Ana Menüye Dön" butonları |
| Buton geri bildirimi | Normal / hover / pressed state'leri |
| Kaleci dalış animasyonu | Yöne göre (sol/orta/sağ) 3 ayrı varyasyon |
| Sahne katman sırası | Arka plan → tribün → saha → kale → kaleci → top → UI |
| Ekran geçişleri | Fade-in/fade-out |
| Performans hedefi | Düşük touch gecikmesi, akıcı frame rate |
