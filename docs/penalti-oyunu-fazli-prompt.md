# Piksel Penaltı Oyunu — Fazlı / Modüler Geliştirme Promptu

Bu doküman, `penalti-oyunu-prompt.md` içindeki oyun tasarımını **ekran bazlı fazlara** böler. Her faz bağımsız bir modül olarak ele alınmalı; bir faz tamamlanıp test edilmeden bir sonrakine geçilmemeli. Bir AI kodlama asistanına (örn. Claude Code) bu dokümanı verirken "Faz 1'den başla, her fazı bitirince bana rapor ver ve onay bekle" şeklinde yönlendirebilirsin.

**Teknoloji:** Unity (C#), 2D, mobil öncelikli (touch input).
**Genel görsel stil:** 16-bit SNES tarzı piksel art, sabit kaleye-bakan kamera, akşam/gece + floodlight atmosferi.

---

## FAZ 0 — Proje Kurulumu ve Altyapı

**Ekran:** Yok (proje iskeleti)

**Kapsam:**
- Unity 2D projesi oluştur, klasör yapısını kur (`Scenes/`, `Scripts/`, `Sprites/`, `Audio/`, `Prefabs/`, `UI/`).
- Sahne (Scene) listesini oluştur: `MainMenu`, `DifficultySelect`, `ModeSelect`, `Gameplay`, `Options` (Options bir overlay/popup olarak da tasarlanabilir).
- Global bir `GameManager` (singleton, `DontDestroyOnLoad`) oluştur — seçilen zorluk, mod ve ses ayarlarını sahneler arası taşımak için.
- Basit bir `AudioManager` iskeleti oluştur (Music ve SFX için ayrı `AudioSource` kanalları — ileride Faz 5'te dolduracağız).
- Touch input sistemini test etmek için placeholder bir dokunma algılayıcı scripti kur.

**Çıktı/Test kriteri:** Boş sahneler arası geçiş çalışıyor, GameManager veriyi sahneler arası koruyor.

---

## FAZ 1 — Ana Menü Ekranı

**Ekran:** `MainMenu`

**Referans (md dosyasından):** Bölüm 2 — Ana Menü

**Kapsam:**
- Üç buton: **Singleplayer**, **Multiplayer**, **Options**.
- **Multiplayer** butonu pasif/disabled olmalı veya üzerinde "Yakında" etiketiyle gösterilmeli — tıklanabilir olmamalı, herhangi bir sahneye yönlendirmemeli.
- **Singleplayer** butonu → `DifficultySelect` sahnesine geçiş yapmalı.
- **Options** butonu → Options ekranını (popup veya ayrı sahne, Faz 2'de detaylandırılacak) açmalı.
- Arka planda stadyum/menü teması olan basit bir piksel art görsel (bu aşamada placeholder/basit renkli kutu olabilir, final asset'ler sonradan eklenir).
- Buton geçişlerinde basit hover/press görsel feedback (piksel art buton state'leri: normal/hover/pressed).

**Çıktı/Test kriteri:** Üç buton görünür, Singleplayer ve Options tıklanabilir ve doğru yerlere yönlendiriyor, Multiplayer tıklanamıyor.

---

## FAZ 2 — Options Ekranı

**Ekran:** `Options` (popup veya ayrı sahne)

**Referans (md dosyasından):** Bölüm 6 — Options Menüsü

**Kapsam:**
- Sadece **iki** kaydırma çubuğu (slider):
  1. Music Volume
  2. SFX Volume
- Başka hiçbir ayar eklenmeyecek (dil, grafik, kontrol vb. YOK — bu kısıt önemli, ekstra ayar önerilmemeli).
- Slider değerleri `AudioManager`'daki ilgili `AudioSource` seviyelerine gerçek zamanlı bağlanmalı.
- Ayarlar `PlayerPrefs` ile kalıcı olarak kaydedilmeli (uygulama kapatılıp açıldığında korunmalı).
- "Geri/Kapat" butonu ile Ana Menü'ye dönüş.

**Çıktı/Test kriteri:** Slider'lar hareket ettirildiğinde ses seviyesi anlık değişiyor, uygulama yeniden açıldığında son ayarlar korunuyor.

---

## FAZ 3 — Zorluk ve Mod Seçim Ekranı

**Ekran:** `DifficultySelect` → `ModeSelect` (tek ekranda birleştirilebilir ya da iki ayrı ekran olabilir — karar geliştirme sırasında verilebilir, ama akış sırası: önce zorluk, sonra mod)

**Referans (md dosyasından):** Bölüm 4 (Oyun Modları) ve Bölüm 5 (Zorluk Seviyesi)

**Kapsam:**
- **Zorluk seçimi:** Kolay / Orta / Zor — üç buton. Seçim `GameManager`'a kaydedilir.
  - Kolay: kaleci doğru yönü tahmin etme olasılığı düşük (örn. %20)
  - Orta: orta seviye (örn. %35)
  - Zor: yüksek (örn. %50)
  - (Kesin oranlar `GameManager` içinde kolayca değiştirilebilir bir config olarak tutulmalı.)
- **Mod seçimi:** Sabit Round (örn. 5 şut) / Endless — iki buton. Seçim `GameManager`'a kaydedilir.
- Her iki seçim tamamlandıktan sonra `Gameplay` sahnesine geçiş.
- **Not:** Dinamik/artan zorluk sistemi YOK — seçilen zorluk oyun boyunca sabit kalmalı, bu ekranda veya sonraki fazlarda buna dair bir mekanik eklenmemeli.

**Çıktı/Test kriteri:** Zorluk ve mod seçimleri doğru şekilde kaydediliyor ve Gameplay sahnesine aktarılıyor.

---

## FAZ 4 — Oynanış Ekranı: Temel Mekanik (Görselsiz/Placeholder)

**Ekran:** `Gameplay`

**Referans (md dosyasından):** Bölüm 3 — Temel Oynanış Mekaniği

**Kapsam (bu fazda görsel kalite önemli değil, sadece mantık/mekanik çalışsın):**
- Ekranda üç dokunma bölgesi tanımla: Sol / Orta / Sağ (placeholder kutular yeterli).
- Oyuncu bir bölgeye dokunduğunda:
  - Kaleci aynı anda (rastgele, zorluk seviyesine göre ağırlıklandırılmış) bir yön seçer.
  - Oyuncunun seçimi ile kalecinin seçimi karşılaştırılır.
  - Aynıysa → **Kurtarış**. Farklıysa → **Gol**.
- **Güç çubuğu veya nişan alma mekaniği eklenmemeli** — sadece üç yönlü seçim.
- **Şut sonucu minimal tutulmalı** — sadece gol/kurtarış, direkten dönme veya auta gitme gibi ek sonuçlar YOK.
- Sabit Round modunda: her şuttan sonra sayaç artmalı, belirlenen sayıya (örn. 5) ulaşınca skor ekranı/özet gösterilmeli.
- Endless modunda: ilk kurtarışta oyun biter, o ana kadarki gol sayısı skor olarak gösterilmeli.
- Basit bir skor göstergesi (UI text) ekle.

**Çıktı/Test kriteri:** Her iki mod da placeholder görsellerle uçtan uca oynanabilir; skor doğru hesaplanıyor.

---

## FAZ 5 — Görsel Varlıklar ve Sahne Kompozisyonu

**Ekran:** `Gameplay` (görsel katman)

**Referans (md dosyasından):** Bölüm 7 — Görsel Stil ve Sahne Kompozisyonu

**Kapsam:**
- 16-bit SNES tarzı piksel art sprite'ları ekle:
  - Top
  - Kaleci (idle/bekleme pozu)
  - Kale (direk + file)
  - Çim saha zemini
  - Piksel taraftar sprite'ları (arka plan, tribün)
  - Stadyum tasarımı (tribün yapısı, floodlight direkleri)
- Kamera açısını sabitle: topun arkasından kaleye bakan sabit görünüm.
- Atmosfer: akşam/gece + floodlight aydınlatma efekti (örn. lighting overlay, glow sprite'ları).
- **Oyuncu karakteri hiçbir şekilde sahnede yer almamalı** — ne tam figür ne de kısmi (ayak, gölge vb.) — bu kısıt kesin.
- Sahnedeki tüm görsel katmanları doğru z-order/sıralamayla yerleştir (arka plan → tribün → saha → kale → kaleci → top → UI).

**Çıktı/Test kriteri:** Sahne, md dosyasındaki "sahnede görünmesi gereken öğeler" listesiyle birebir eşleşiyor; oyuncu figürü hiçbir karede görünmüyor.

---

## FAZ 6 — Animasyonlar

**Ekran:** `Gameplay` (animasyon katmanı)

**Referans (md dosyasından):** Bölüm 8 — Animasyonlar

**Kapsam:**
1. **Şut Animasyonu:** Topun sekme/dönme (spin) efekti, kaleye doğru gerçekçi yörünge (trajectory), vuruş anında hafif screen shake ve/veya toz efekti.
2. **Kaleci Kurtarış Animasyonu:** Dalış (dive) sprite sekansı — havada uzanma, tutma/tokatlama pozu, yere düşüş, toparlanma. Yön bazlı (sol/orta/sağ) üç ayrı animasyon varyasyonu olmalı.
3. **Piksel Taraftar Kutlama Animasyonu:** Gol anında tribündeki taraftarlar için senkronize, döngüsel basit bir "cheer" animasyonu (kollar havada, zıplama, opsiyonel konfeti/parçacık efekti).
4. Tüm animasyonlar ses efektleriyle (Faz 7) zamanlama açısından senkronize edilecek şekilde tasarlanmalı (animasyon event'leri kullanılabilir).

**Çıktı/Test kriteri:** Her şut sonrası (gol veya kurtarış) ilgili animasyonlar sorunsuz, takılmadan oynuyor.

---

## FAZ 7 — Ses Entegrasyonu

**Ekran:** `Gameplay` (ses katmanı) + `AudioManager` genişletmesi

**Referans (md dosyasından):** Bölüm 9 — Ses Tasarımı

**Kapsam:**
- **Şuttan önce:** Düşük seviyeli, sürekli ambient kalabalık/tezahürat sesi (loop).
- **Vuruş anı:** Topa vurulma sesi (kick SFX), şut animasyonuyla senkronize.
- **Gol olduğunda:** Yüksek enerjili "cheer" kutlama sesi, taraftar animasyonuyla senkronize.
- **Kurtarış olduğunda:** Kalabalığın hayal kırıklığı sesi ("ohh" tepkisi).
- Tüm efektler `SFX Volume`, ambient/loop müzik varsa `Music Volume` slider'ına bağlı olmalı (Faz 2'deki Options ile entegre).

**Çıktı/Test kriteri:** Ses efektleri doğru anlarda tetikleniyor ve Options ekranındaki slider'lar bu sesleri gerçek zamanlı etkiliyor.

---

## FAZ 8 — Cilalama (Polish) ve Uçtan Uca Test

**Ekran:** Tüm ekranlar

**Kapsam:**
- Tüm ekranlar arası geçişlerde basit fade-in/fade-out veya transition efekti.
- Sabit Round modu sonunda özet/skor ekranı: "Tekrar Oyna" ve "Ana Menüye Dön" butonları.
- Endless modu sonunda skor ekranı: mevcut skor + (varsa) yüksek skor (PlayerPrefs ile kalıcı).
- Genel performans testi: mobil cihazda (veya emulator'de) touch input gecikmesi, animasyon akıcılığı (frame rate) kontrolü.
- Bölüm 10'daki (md dosyası) "Kapsam Dışı" listesindeki hiçbir özelliğin yanlışlıkla eklenmediğinin son kontrolü: Multiplayer aktif değil, güç çubuğu yok, dinamik zorluk yok, ekstra şut sonucu çeşitliliği yok, gündüz/gece seçimi yok, ekstra options ayarı yok, oyuncu karakteri görünmüyor.

**Çıktı/Test kriteri:** Oyun baştan sona (Ana Menü → Zorluk/Mod Seçimi → Oynanış → Skor Ekranı → Ana Menüye Dönüş) akıcı şekilde oynanabiliyor.

---

## Faz Uygulama Notu (AI Asistanına Talimat)

Bu dokümanı bir AI kodlama asistanına verirken şu talimatı ekle:

> "Bu dokümandaki fazları sırasıyla uygula. Her fazı bitirdiğinde bana hangi dosyaları/scriptleri oluşturduğunu özetle ve bir sonraki faza geçmeden önce onay iste. Bir fazda md dosyasındaki bir detayla çelişen bir karar vermen gerekirse, karar vermeden önce bana sor."
