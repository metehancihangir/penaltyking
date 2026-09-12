# Penalty King — Karakter Modeli ve Animasyon Düzeltme Prompt'u

Aşağıdaki prompt'u kodlama asistanına (Claude Code, Cursor vb.) veya animasyon/rig üzerinde çalışan yapay zekaya doğrudan verebilirsin.

---

## PROMPT

Projedeki 2D iskelet tabanlı (bone-based) karakterlerin (futbolcu ve kaleci) hem statik duruşlarını hem de animasyonlarını gözden geçirip düzelt. Şu anki halleri anatomik olarak bozuk ve doğal durmuyor; aşağıdaki sorunları teker teker çöz:

### 1. Anatomik Oranlar ve Pivot Yapısı
- Her iki karakterin de bel bölgesinde kopukluk/ayrışma var — üst gövde (torso) ile kalça (hips) bone'u arasındaki pivot noktasını ve sprite kesim sınırlarını kontrol et, katmanlar arasında boşluk kalmayacak şekilde üst üste bindirme (overlap) payı bırak.
- İnsan iskelet oranlarını referans al: omuz genişliği kalça genişliğinin yaklaşık 1.3–1.5 katı olmalı, bacak uzunluğu toplam boyun ~%45-48'i civarında olmalı.
- Her bone'un pivot noktasının gerçek eklem konumunda olduğunu doğrula (omuz, dirsek, kalça, diz, ayak bileği); pivot eklemden kaymışsa dönüşlerde (rotation) uzuv "kayıyormuş" gibi görünür.
- IK (Inverse Kinematics) kullanılıyorsa bacak ve kol zincirlerindeki hedef (target) noktalarının anatomik olarak mümkün açı sınırları içinde kısıtlandığından emin ol (ör. diz sadece öne bükülebilir, geriye bükülmemeli).

### 2. Futbolcunun Duruşu (Şut Öncesi Bekleme Pozu)
- Ağırlık merkezini iki ayağa dengeli dağıt; şu an omuzlar çok gergin/kare duruyor, hafif bir "contrapposto" (ağırlığın bir bacakta, omuzların hafif çapraz) duruşu ver ki daha doğal görünsün.
- Kollar vücuttan gereğinden fazla ayrık ve düz durmasın; dirsekte hafif kırılma olsun, eller tamamen simetrik ve gergin durmasın.
- Bel/kalça bölgesindeki boşluk sprite'ı: gövde-alt gövde arasına ince bir "bel" katmanı (mid-torso overlap sprite) ekleyerek görünen boşluğu kapat.

### 3. Kalecinin Duruşu
- Şu anki poz çok yapay ve "T-pose'a yakın" duruyor; gerçek kalecilerin hazırlık duruşunu referans al: dizler hafif bükük, ağırlık ayak parmaklarına yakın, kollar öne-yana açık ama dirsekten hafif kırık, eller omuz-bel arası yükseklikte ve avuç içi öne dönük.
- Omurga tamamen dik değil, hafif öne eğik olmalı (atlamaya hazır bir denge pozisyonu).
- Bacaklar omuz genişliğinden biraz daha açık, dizler hafif içe/öne bükük olmalı; şu anki gibi bacaklar dümdüz ve kilitli durmasın.

### 4. Şut Çekme Animasyonu
- Animasyonu şu fazlara ayır ve her fazda ara kareler (in-between frames) ekleyerek akıcılık sağla:
  1. **Yaklaşma/hazırlık**: gövde hafif geriye yaslanır, vurucu bacak geriye çekilir, kollar denge için açılır.
  2. **Vuruş öncesi tepe nokta**: vurucu bacak maksimum geriye çekilmiş, karşı kol öne uzanmış (doğal denge refleksi).
  3. **Vuruş anı**: kalça dönüşü önce başlar, sonra diz, en son ayak bileği tepki verir (kinetik zincir/"kinetic chain" mantığı — güç kalçadan ayağa doğru aktarılmalı, tüm bacak tek parça gibi hareket etmemeli).
  4. **Takip (follow-through)**: vuruş sonrası bacak yukarı-öne devam eder, gövde öne eğilir, denge ayağı hafif zıplar.
- Şu anki animasyonda muhtemelen ara kare sayısı az ve kalça-diz-bilek aynı anda hareket ediyor; bunları ayrı zamanlamalarla (staggered timing) hareket ettir.
- Squash & stretch prensibini hafifçe uygula: vuruş anında gövdede minimal bir sıkışma/esneme efekti animasyona doğallık katar.

### 5. Kaleci Kurtarış Animasyonları
- Üç ayrı kurtarış tipi tasarla: **merkez kurtarış** (öne çökme), **yana uçma** (dive — sağ/sol), **üst köşe kurtarışı** (sıçrama).
- Yana uçma animasyonunda: önce kısa bir hazırlık (anticipation) karesi olsun — ağırlık dive yönünün tersine hafif kayar, sonra ana harekete geçilir; gerçek kalecilerde bu "geri çekilme sonra patlama" hissi olur.
- Uçuş sırasında gövde ve bacaklar tek düz çizgi gibi kaskatı durmamalı; omurga hafif kavis yapsın, uzanan kol/el topa doğru gerilsin, diğer kol dengeleme için ters yöne açılsın.
- İniş karesinde ani "donma" olmasın; 2-3 karelik yumuşak bir yere çarpma/sekme (impact + settle) ekle.
- Kurtarış sonrası kalkış/toparlanma animasyonu da ekle ki kaleci havada asılı kalmış gibi görünmesin.

### 6. Genel Kalite Kontrol
- Tüm animasyonları 12 FPS yerine en az 18-24 FPS ara kare yoğunluğuyla dene, pixel art tarzını bozmadan hareket akıcılığını artır.
- Her animasyonun ilk ve son karesinin bir sonraki animasyona (idle'a) sprite pivotları açısından uyumlu şekilde bitmesini sağla ki geçişlerde ışınlanma/zıplama olmasın.
- Referans olarak gerçek penaltı çekimi ve kaleci kurtarış videolarından kare kare hareket analizi yap (kalça-diz-bilek zaman aralıkları, denge kolu açıları).

---

**Not:** Bunu tek seferde büyük bir istekle vermek yerine, kodlama asistanına önce sadece "1. Anatomik Oranlar" ve "3. Kaleci Duruşu" bölümünü verip sonucu görüp onayladıktan sonra animasyon kısımlarına geçmeni öneririm — bone yapısı değişirse tüm animasyonların yeniden ayarlanması gerekir.
