# Karakter anatomisi ve hareket revizyonu

Kaynak istek: `docs/karakter-duzeltme-prompt.md`. Piksel görünümü ve mevcut 2D kemik yapısı korunur. APK oluşturulmaz.

## Oranlar ve birleşimler

- Omuz eklemleri arası 52, kalça eklemleri arası 35 birim: oran yaklaşık 1,49. İki bacak kemiği 48 + 48 birim; 210 birimlik tasarım boyunun yaklaşık %46'sı.
- Torso alt sınırı uzatıldı. Aynı forma atlasının alt orta bölümünden kesilen ayrı `Waist Overlap` katmanı kalça üzerinde 29 birim yüksekliğinde örtüşme sağlar. Renk değiştirme materyali bu katmana da uygulanır.
- Kol ve bacak hedefleri gerçek erişim uzaklığına kısıtlanır. Kemik uzunlukları dönüş sırasında sabittir. Dizler 8–145°, dirsekler 12–145° bükülme aralığındadır; düz kilitlenme ve ters IK çözümüne geçiş engellenir.
- Şutçunun dizleri üç çeyrek arka görünümde şut yönüne; önden görünen kalecinin dizleri hafif içe bükülür. Ayak bileği dönüşü ±28° ile sınırlıdır.

## Hazır duruşlar

Şutçuda destek bacağı daha düz, diğer diz hafif kırık; kalça −2°, omuzlar +2° eğimlidir. İki el farklı konumlarda ve gövdeye daha yakındır. Kalecinin elleri bel–göğüs arasında, dirsekleri gövdeye yakın, ayakları omuzlardan biraz geniştir. Gövdesi daha alçak ve hafif eğimlidir. Öne eğilme, sabit 2D kamera düzleminde alçalan göğüs ve bükülen dizlerle ifade edilir; gerçek 3D derinlik dönüşü değildir.

## Hareket zamanlaması

| Bölüm | Zaman / davranış |
|---|---|
| Yaklaşma ve yüklenme | İlk 0,50 saniyede ağırlık aktarımı, geriye çekilen ayak ve denge kolları |
| Kinetik zincir | Kalça dönüşü yaklaşık 0,45 sn, dizin ileri salınımı 0,50 sn, ayak bileği açılması 0,60 sn civarında başlar |
| Top teması | 0,70 sn; ayak ucu topa hizalı |
| Takip | Temastan sonra bacak ileri/yukarı devam eder, gövde hafif sıkışır ve destek ayağında küçük yükselme olur |
| Kaleci hazırlığı | Kısa ters yön ağırlık aktarımı, ardından dalış / merkez çöküşü / üst bölge sıçraması |
| Top yakalama | 1,36 sn; iki elin merkezinde sabit tutuş |
| İniş ve toparlanma | Temastan 0,05–0,48 sn sonra iniş, 0,38–0,68 sn arasında küçük sönümlenen sekme; 0,62–1,22 sn arasında doğrulma |

Kurtarışta top doğrulma boyunca ellerle birlikte hareket eder. Şutçu atış sonunda başlangıç konumuna yumuşakça döner. Gol sevinci hâlâ 3 saniyedir; son bölümünde kollar iner ve oyuncu yerine döner. Kemikler ekranın her karesinde örneklenir; 12 FPS sınırlaması yoktur. Önizleme 30 FPS'tir.

## Referans ve doğrulama

Kalecinin alçak hazırlığı, bacağın serbest bırakılması ve iki elle güvenli tutuş için [FIFA Training Centre — Learning to dive](https://www.fifatrainingcentre.com/en/environment/fifa-goalkeeper-training/goalkeeping-fundamentals/learning-to-dive.php) içindeki aşamalı egzersiz açıklamaları kullanıldı. Kaynak video üzerinden milisaniyelik hareket ölçümü yapılmadı; yukarıdaki süreler oyunun mevcut top teması ve uçuş süresine göre tasarlanmıştır.

İlk ilgili PlayMode çalıştırması **12/12 geçti** (`TestResults/anatomy.xml`). Altı kurtarış 30 FPS örneklenerek kemik uzunlukları, eklem sınırları, bükülme yönü ve bel katmanı materyali kontrol edildi. Top teması, tutuş, toparlanma konumu ve mevcut maç/ses olayları da test kapsamındadır. Son görsel düzenlemenin tekrar doğrulaması aşağıya eklenecektir.
