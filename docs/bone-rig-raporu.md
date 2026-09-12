# Kemik animasyonu, tutuş ve tribün düzeltmesi

Kullanıcı tercihi: piksel görünümü korunarak 2D kemik animasyonuna geçiş. Yeni APK oluşturulmaz.

## Yapı

Şutçu ve kaleci artık hareket sırasında tam gövde resimlerini değiştirmez. Her karakterde 15 eklemden oluşan bir Transform hiyerarşisi ve eklemlere bağlı görsel parçalar bulunur. Omuz–dirsek–el ve kalça–diz–ayak zincirleri iki kemikli IK hesabıyla hareket eder. Hazırlanma, destek ayağı, gerilme, vuruş devamı, dalış ve iniş aynı duraklatılabilir zaman çizelgesini kullanır.

Bu proje Canvas tabanlı olduğu için uygulama, Canvas ile çalışan özel `FootballRig` ve `RigMotion` bileşenleridir; Unity 2D Animation/SpriteSkin paketi kurulmuş gibi değerlendirilmemelidir. Kamera eksenine dönük 2D parçalar kullanılır, serbest 3D vücut dönüşü sağlanmaz. Yeni parça atlasları imagegen ile üretilmiştir; açık mavi ve yeşil forma seçenekleri mevcut renk materyaliyle çalışır.

## Kurtarış

- Altı bölgenin tamamında iki elin orta noktası top hedefiyle eşleşir.
- Temasta topun dönüşü durur ve top kalecinin iki elini takip eder.
- Tutulan top gövdenin önünde, eldivenlerin arkasında çizilir. Uçuş topunun kopyası görünmez.
- Kaleci yere inerken top elinde kalır. Atış sıfırlandığında tutulan top gizlenir ve penaltı topu yerine döner.

## Tribün ve arayüz

- Hareketli tribün parçaları artık yatayda boşluk bırakmaz; iki taraftar katmanının tüm genişliği kapsanır. Daha önce kaldırılan büyük yüzlü modeller geri eklenmez.
- Kalede hedef işaretleri ve seçim parlamaları kaldırılır. Altı görünmez dokunma alanı ve altı bölgeli ilk oyun rehberi çalışmaya devam eder.

## Doğrulama

- İlk tam PlayMode çalıştırması: 59 testin 58'i geçti. Tek hata, hedef konumunda kayan noktalı sayıları birebir karşılaştıran testteydi; konum kontrolü 0,001 birim toleransla düzeltildi (`TestResults/bone-rig.xml`).
- Son değişikliklerden sonra altı hedef, kemik tutuşu, şut, gol sonrası top konumu ve yatay kadraj testlerinin **10/10'u geçti** (`TestResults/bone-rig-final.xml`).
- Altı kurtarış yönünde temastan sonraki beş farklı anda topun iki elin merkezini takip ettiği doğrulandı. Vuruş anında ayak ucu ile topun hizası ve 120 örnekte kesintisiz diz hareketi denetlendi.
- Tribündeki 12 parçanın iki katmanı yatay boşluk bırakmadan kapladığı, tamamının hareket ettiği ve sahnede hedef belirteci kalmadığı doğrulandı.
- Son Unity yakalamalarında başlangıç, ayak–top teması, altı tutuş ve ikinci oyuncu renkleri kontrol edildi. Önizlemeler doğrudan Unity sahnesinden alınmıştır; GIF dosyaları sessizdir.

[Şut ve kurtarış önizlemesi](previews/bone-rig-save.gif) · [Gol ve sevinç önizlemesi](previews/bone-rig-goal.gif) · [Başlangıç görüntüsü](previews/bone-rig-ready.png)

Fiziksel telefon performansı ölçülmedi ve APK üretilmedi. Hareketler artık sürekli kemik dönüşleriyle çalışır; kesilmiş 2D parçalardan oluşan görselin estetik değerlendirmesi için önizleme ve Editor'de oynama önerilir.
