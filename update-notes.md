# Penalty King — Güncelleme planı

Tarih: 7 Eylül 2026

Durum: Faz 1–7 sonrasındaki 12 Eylül altı hedef ve görsel düzeltme güncellemesi tamamlandı. Aşağıdaki eski faz kayıtları tarihsel notlardır. Yeni APK üretilmedi.

## 12 Eylül — Altı hedef ve görsel düzeltmeler

- [x] Büyük yüzlü ön taraftar modellerini kaldır; stadyumdaki diğer taraftarların kutlamalarını koru.
- [x] Golde kalenin büyüyüp küçülmesini düzelt: ağ deformasyonu Image bileşeninin gerçek çizim sınırlarıyla çalışır.
- [x] Vuruşta siyah parça görünümünü gider: top kopyası gölge yerine oval gölge kullan, küçük toz karelerini kaldır.
- [x] Yeni 12 karelik şut atlasını ayak/top temasına hizala; mevcut sevinç pozlarını koru.
- [x] Çim renklerini, ince saha dokusunu, sürekli perspektif çizgilerini ve penaltı noktasını yenile.
- [x] Şutçu ve kaleci için altı bölge ekle: sol/orta/sağ × üst/alt. Yalnız aynı bölge kurtarıştır; aynı sütunda farklı yükseklik gol olur.
- [x] Üst/alt hedeflere uygun kaleci çizimleri, eldiven temas noktaları ve top uçuşları ekle. Hedef işaretleri seçim dışında gizlenir.
- [x] İlk oyun rehberini altı düğmeyle güncelle; eski rehber tamamlanmış olsa bile yeni rehberi bir kez göster.
- [x] Tam PlayMode kontrolleri 57/57; son hedef/rehber düzenlemelerinin tekrar kontrolleri 11/11 geçti. Test edilen sahne ve atlas kesimleri ana projeye aktarıldı.
- [x] APK üretme. Mevcut paketler değişmedi.

Ayrıntılar: `docs/six-target-raporu.md`. Unity önizlemesi: `docs/previews/six-target-ready.png`; animasyon: `docs/previews/six-target.gif`.

## Kapsam ve öncelik

Kullanıcının telefondaki APK testi sonrasında verdiği yeni kararlar, eski promptların çelişen maddelerinin yerine geçer. Özellikle bot, zorluk, pasif yerel multiplayer ve ilk kurtarışta biten Endless kuralları değişir. Kaynak promptlar tarihsel belge olarak korunur.

Gece atmosferi, piksel sanat dili, görünür şutçu ve vuruş animasyonu korunur. Kalabalık ve davul dahil stadyum seslerini SFX Volume kontrol eder. Online oyun bu güncellemede uygulanmaz.

Son kullanıcı kararlarıyla zorluk metinleri ve işlevi tamamen kaldırılır; iki insan tek telefonda sırayla oynar. Options kapsamına Titreşim aç/kapat kontrolü eklenir; eski yalnızca iki ses ayarı kısıtı bu kontrol için geçersizdir. Maç içinden ayarlara erişim sağlanır.

Kullanıcı aynı mesajın başında Multiplayer yazısının ortalanmasını istemiş, daha sonra ana menüyü yalnızca Play ve Options olarak değiştirmiştir. Son karar uygulanır: ana menüde Multiplayer butonu da YAKINDA etiketi de bulunmaz.

## Güncelleme Faz 1 — Menü ve gezinme

### Modül 1.1 — Ana menü

- [x] FLOODLIGHT FOOTBALL, ÜÇ YÖN. TEK ŞANS. ve PENALTI SENİN. metinlerini kaldır.
- [x] PENALTY KING başlığını sadeleşen yerleşime göre ortala.
- [x] Singleplayer ve Multiplayer butonlarını kaldır; yalnızca Play ve Options bırak.
- [x] Buton metinlerini ortala; mevcut piksel stilini koru.

### Modül 1.2 — Play sonrası oyuncu seçimi

- [x] Play → oyuncu seçimi ekranını oluştur.
- [x] Online butonuna piksel dünya küresi veya Wi-Fi ikonu ekle. Online işlevi pasif kalsın; bağlantı veya eşleştirme başlatmasın.
- [x] 2 Kişilik butonunu, Tek Telefon alt yazısını ve uygun iki kullanıcı/telefon ikonunu ekle.
- [x] 2 Kişilik → mod seçimi akışını bağla. Geri gezinmesini yeni akışa uyarla.
- [x] Zorluk seçimini gezinmeden çıkar; kullanıcı hiçbir aşamada bot/zorluk seçmesin.

### Modül 1.3 — Options ve mod seçimi

- [x] Options ekranındaki PENALTY KING ve SES AYARLARI yazılarını kaldır.
- [x] Music Volume ve SFX Volume kontrollerini ve kalıcı kayıtlarını koru.
- [x] Titreşim aç/kapat kontrolünü ekle; tercih uygulama kapatılıp açıldığında korunsun. Gol titreşimi bağlantısı Modül 6.3'te tamamlanacak.
- [x] Sabit Round ve Endless seçeneklerini koru; ikisinin alt açıklamalarını kaldır.
- [x] Mod ekranı dahil tüm ekranlardaki zorluk metinlerini kaldır; zorluk işlevini Modül 2.1 kapsamında tamamen kaldır.

Kabul: Ana menüde yalnızca iki ana eylem vardır. Play → 2 Kişilik → mod seçimi yolu çalışır; Online pasiftir. Geri dönüşler doğru ekrana gider ve kaldırılan metinler görünmez.

Faz 1 tesliminde menü ve seçim akışı tamamlandı. Faz 2 ile yerel maç bağlandı ve eski bot/zorluk kodu ile sahnesi kaldırıldı. Titreşim tercihi kaydedilir; fiziksel gol titreşimi Faz 6 kapsamındadır.

## Güncelleme Faz 2 — Aynı telefonda iki oyuncu ve sıra sistemi

### Modül 2.1 — İnsan kontrollü şut ve kurtarış

- [x] Botun kaleci yönü üretimini ve kurtarış olasılıklarını aktif oyun mantığından çıkar.
- [x] Zorluk seçimi zorunluluğunu, oturumdaki zorluk bağımlılığını ve ilgili aktif yapılandırmayı kaldır.
- [x] Player 1 ve Player 2 için ayrı kimlik, skor ve şut geçmişi tut.
- [x] İlk oyuncu şut yönünü, diğer oyuncu kurtarış yönünü seçsin.
- [x] Her iki seçim tamamlandığında mevcut şut/kaleci animasyonunu bu iki insanın seçimiyle oynat.
- [x] Aynı yön kurtarış, farklı yön gol kuralını koru; rastgele bot kararı verme.

### Modül 2.2 — Telefon devri ve rol değişimi

- [x] Şut seçimi → telefon devri → kaleci seçimi → animasyon/sonuç → sonraki sıra durumlarını açıkça yönet.
- [x] Şut seçimini devretme ekranında gizle; seçili yönü vurgulama veya topu önceden hareket ettirme yoluyla ikinci oyuncuya açıklama.
- [x] Sıra dışı girişleri, çift dokunmayı ve animasyon sırasında yeniden seçimi engelle.
- [x] Aktif şutçuyu ve kaleciyi kısa, okunaklı bir gösterimle belirt.
- [x] Rol değişimini ve yeni maçta sıfırlamayı uygula.
- [x] Telefon devrinde kısa, kullanıcı dostu bir piksel geçiş animasyonu göster. Örnek başlık: “Sıra PLAYER 2'de”; alt bilgi: “Kurtarış sırası” veya “Şut sırası” ve “Devam etmek için dokun”. Oyuncu numarası ve rol sıraya göre değişsin.
- [x] Devir ekranında herhangi bir yere dokunulduğunda panel kapansın; bu dokunuş tüketilsin ve yön seçmek için yeni bir dokunuş gereksin.
- [x] Önceki yön seçimini yapan parmağın basılı tutulması veya bırakılması devir panelini kapatmasın; panel yeni bir dokunuş beklesin.

Onaylanan tasarım: İlk şutta Player 1 şutçu, Player 2 kaleci olur; her tamamlanan şuttan sonra roller değişir. Telefon devrinde sıradaki oyuncu ekrana dokunarak devam eder. Kalecilik yapan oyuncu sonraki şutta şutçu olacağından aynı kişiye gereksiz telefon devri istenmez; yeni rol kısa bir bildirimle anlatılır. Aynı fiziksel ekranı izleyerek yapılan seçimi görmeyi yazılımla tamamen engellemek mümkün olmadığından devretme akışı seçimleri sonradan açığa çıkarmamalıdır.

Kabul: Her sonuç iki insanın seçimiyle oluşur. Şut yönü kaleci seçim ekranında sızmaz. Skor doğru oyuncuya yazılır ve roller belirlenen sırada değişir.

Faz 2 entegrasyon notu: Çalışan yerel maç için kişi başı 5 şut ve kurtarışta devam eden Endless temel kuralları da yeni modele uyarlandı. Bu temel Faz 3’te sonuç ekranı, güvenli çıkış ve tekrar oynama akışlarıyla tamamlandı.

## Güncelleme Faz 3 — Mod kuralları ve maç yaşam döngüsü

### Modül 3.1 — Sabit Round

- [x] Modu iki oyunculu sıraya uyarla; iki oyuncunun eşit sayıda şut hakkı olmasını sağla.
- [x] Sonuç ekranını iki skor ve maç sonucunu gösterecek biçimde düzenle.
- [x] Tekrar Oyna ve Ana Menüye Dön akışlarını yeni oturuma uyarla.

Onaylanan kural: Oyuncu başına 5 şut, toplam 10 şut. Tüm haklar tamamlanınca yüksek skor kazanır; eşitlik beraberliktir. Erken bitiş veya uzatma eklenmez.

### Modül 3.2 — Endless

- [x] İlk kurtarışta bitiş kuralını kaldır.
- [x] Golde de kurtarışta da sonraki oyuncunun sırasına geç; otomatik şut/round sınırı koyma.
- [x] İki oyuncunun skorlarını oyun sürdükçe tut.
- [x] Oyuncuların menüye dönebileceği kompakt bir çıkış eylemi sağla.

Kabul: Endless'ta art arda kurtarışlar oyunu bitirmez. Sabit Round'da iki oyuncu eşit hak kullanır. Tekrar oynama yönleri, skorları, geçmişi ve sırayı temizler.

## Güncelleme Faz 4 — Tam ekran saha ve piksel skor tabelası

### Modül 4.1 — Tam ekran kompozisyon

- [x] Stadyumu küçük bir oyun paneli yerine tüm ekranı kaplayacak şekilde düzenle.
- [x] Üstteki mevcut Endless, Gol ve Şut değerlerini kaldır; yerlerini yeni skor tabelası alsın.
- [x] Alttaki Bir yöne dokun ve SOL / ORTA / SAĞ yazılarını kaldır. Görünmez dokunma bölgeleri kullanılabilir kalmalı.
- [x] Gece stadyumunu ekran kenarlarına kadar uzat; skor ve kontrolleri çentik/güvenli alan içinde tut.
- [x] Dikey ve yatay ekranlarda top, şutçu, kaleci ve kale okunabilir kalsın; görselleri esnetme.

### Modül 4.2 — Skor tabelası

- [x] Referanstaki üst bant, karşılıklı oyuncu alanları, ortadaki skor ve atış işaretlerinden yararlanarak özgün piksel tabela oluştur.
- [x] Player 1 ve Player 2 adlarını ve ayrı skorlarını göster; aktif oyuncuyu ayırt et.
- [x] Sabit Round'da bekleyen atış/gol/kurtarış işaretlerini göster.
- [x] Endless için büyüyüp ekranı taşırmayan sınırlı bir son atış geçmişi tasarla; toplam skor görünür kalsın.
- [x] Referanstaki gerçek takım adlarını, logoları ve reklamları kullanma.

Kabul: Oyun sahnesi ekranı doldurur. Eski sayaç/yönerge metinleri yoktur. Tabela küçük telefon ekranında okunur ve dokunma bölgelerini kapatmaz.

### Modül 4.3 — Oyun içi ayarlar paneli

- [x] Sağ üst köşeye küçük, oyunun stiline uygun piksel dişli simgesi yerleştir; görünümü küçük olsa da dokunma alanı rahat kullanılabilir olsun ve skor tabelasıyla çakışmasın.
- [x] Simgeye dokunulduğunda mevcut oyun sahnesi üzerinde ayarlar paneli açılsın; Options sahnesine geçilmesin ve maçtan çıkılmasın.
- [x] Panelde Music Volume, SFX Volume ve Titreşim aç/kapat kontrolü bulunsun. Ana menü Options ekranıyla aynı tercihleri kullansın ve değişiklikleri anında uygulasın.
- [x] Panel açıkken alttaki yön seçimi, rehber ve telefon devri girişlerini engelle; panel açma/kapatma dokunuşu oyuna sızmasın.
- [x] Panel kapatılınca skor, şut hakları, roller, gizli seçimler ve sıra korunsun; maç sıfırlanmasın.
- [x] Önerilen davranış: panel açıkken şut animasyonu ve sıra ilerlemesi duraklasın; kapatıldığında aynı noktadan devam etsin. Ayar değişikliklerini duyabilmek için ambiyans ses ayarına göre çalmayı sürdürsün. Mevcut animasyonlar ölçeklenmemiş zaman kullandığından yalnızca zaman ölçeğini sıfırlamakla yetinilmesin.

Kabul: Oyunun herhangi bir aşamasında ayarlar açılıp kapatılabilir; maç durumu kaybolmaz, arkada seçim yapılamaz ve ses/titreşim tercihleri iki ayarlar arayüzünde tutarlıdır.

## Güncelleme Faz 5 — İlk kullanım rehberi ve saha görselleri

### Modül 5.1 — İlk kullanım rehberi

- [x] İlk oyunda, penaltı kullanılmadan önce kısa bir piksel rehber paneli göster.
- [x] Oklarla dokunulabilir yönleri anlat; az metin, okunaklı font ve anlaşılır görsel geri bildirim kullan.
- [x] Yeni iki kişilik düzene uygun olarak şut seçimi, telefon devri ve kaleci seçimini açıkla.
- [x] Rehber dokunuşunun yanlışlıkla şut/kurtarış seçmesine izin verme.
- [x] Rehber tamamlandı bilgisini sakla; sonraki normal girişlerde yeniden gösterme.

### Modül 5.2 — Kale, saha çizgileri ve şutçu

- [x] Kaleyi mevcut görünümüne göre küçült; kaleci, top hedefleri ve dokunma bölgelerini yeni kale ölçülerine uyarla.
- [x] Şutçu futbolcuyu büyüt; hazırlık, temas ve vuruş sonrası karelerinde aynı ölçeği koru.
- [x] Mevcut rahatsız edici penaltı/saha çizgisini, referanstaki ceza alanı geometrisi ve perspektifine uygun özgün piksel çizgilerle yeniden düzenle.
- [x] Penaltı noktasını ve topun konumunu saha perspektifiyle uyumlu tut.
- [x] Top yörüngesini, ayak-top temasını ve kalecinin kurtarış konumlarını yeni kompozisyona göre yeniden ayarla.

Kesin büyütme/küçültme oranları kullanıcı tarafından verilmedi; dikey/yatay önizlemeler üzerinden belirlenecek.

Kabul: Daha küçük kale ve daha büyük şutçu birlikte dengeli görünür. Oyuncunun ayağı topa temas eder; kurtarışlar ve gol hedefleri yeni kaleyle hizalıdır. Rehber yalnızca ilk kullanımda gösterilir.

## Güncelleme Faz 6 — Davullu taraftar tezahüratı ve gol titreşimi

Durum: Uygulama ve 10/10 hedefli Unity testi tamamlandı. İşitsel kabul ve gerçek telefon kontrolü bekliyor. Rapor: `docs/update-faz-6-raporu.md`.

### Modül 6.1 — Ambiyans değişimi

- [x] Kafe sohbetini andıran mevcut maç ambiyansını aktif kullanımdan kaldır.
- [x] Yerine maç hissi veren toplu taraftar tezahüratı ekle; davul sesi mutlaka bulunmalı.
- [x] Özgün veya uygun lisanslı ses kullan; kaynak ve lisans kaydını güncelle.
- [x] Döngü geçişini kesintisiz yap; çok gürültülü veya yorucu bir miks oluşturma.
- [x] Vuruş, gol ve kurtarış efektlerinin duyulabilirliğini koru.
- [x] Gol anındaki mevcut “aaahhhh” taraftar tepkisini kaldır; yerine kullanıcının istediği toplu “ouuffff” tepkisini koy. Bu değişiklik gol olayı içindir; kurtarışa taşınmasın.

### Modül 6.2 — Ses kontrolü ve doğrulama

- [x] Tezahürat ve davulu SFX Volume kanalına bağla; SFX sıfırken ikisi de susmalı.
- [x] Menüye dönüşte stadyum seslerini durdur; tekrar girişte üst üste döngüler başlatma.
- [ ] Gerçek dinleme ve telefon hoparlöründe kullanıcı kontrolüyle atmosferi değerlendir; yalnızca teknik ses ölçümleri yeterli sayılmasın.

Kabul: Davullu tezahürat duyulur, konuşma/kafe hissi giderilir, ses patlaması veya döngü kopması olmaz. SFX ayarı bütün stadyum seslerini birlikte kontrol eder.

### Modül 6.3 — Gol titreşimi

- [x] Golün görsel olarak gerçekleştiği sonuç anında kısa bir telefon titreşimi üret; yön seçilirken veya sonuç daha gösterilmeden titreştirme.
- [x] Her gol için yalnızca bir kez tetikle; kurtarışta titreşim verme. Duraklatıp devam etmek aynı golü tekrar titreştirmesin.
- [x] Titreşimi Options ve oyun içi paneldeki ortak aç/kapat tercihine bağla; kapalıyken golde titreşim oluşmasın.
- [x] Titreşim tercihi SFX Volume'dan bağımsız olsun; ses kısılması titreşim tercihini değiştirmesin.
- [x] Önerilen başlangıç tercihi: titreşim açık ve kısa, tek darbe. Desteklemeyen cihazlarda oyun normal devam etsin.
- [ ] Gerçek telefonda açık/kapalı durumunu ve hissedilen süreyi doğrula; emülatör sonucu fiziksel titreşim doğrulaması yerine geçmesin.

Kabul: Gol başına tek kısa titreşim vardır; kapalı ayarda ve kurtarışta titreşim yoktur. Tercih yeniden açılışta korunur.

## Güncelleme Faz 7 — Entegrasyon, test ve yeni APK

Uygulama, 48/48 Unity testi, Android emülatör testi ve yeni APK teslimi tamamlandı. Kullanıcıya bağlı dinleme/ertelenmiş fiziksel titreşim kontrolleri aşağıda açık tutuldu. Rapor: `docs/update-faz-7-raporu.md`.

- [x] Eski bot/zorluk ve ilk kurtarışta bitiş testlerini yeni kurallara uyarla.
- [x] Menü, pasif Online, yerel oyuncu seçimi ve geri gezinme akışlarını doğrula.
- [x] İki insanın tüm yön eşleşmelerinde gol/kurtarış hesabını doğrula.
- [x] Gizli seçim, telefon devri, rol değişimi, çift dokunma ve skor sahipliği senaryolarını test et.
- [x] Sabit Round'un eşit haklarını, beraberliği ve tekrar oynamayı test et.
- [x] Endless'ın çok sayıda gol/kurtarıştan sonra devam ettiğini ve geçmiş göstergesinin sınırlı kaldığını doğrula.
- [x] İlk kullanım rehberini temiz kayıtla ve sonraki açılışta test et.
- [x] Tam ekran yerleşimini, güvenli alanları ve animasyon hizasını dikey/yatay telefon ölçülerinde incele.
- [x] SFX sıfır/orta/yüksek seviyelerde ses davranışını kontrol et.
- [ ] Son kullanıcı isteğine göre yeni arcade gol sesini, kurtarıştaki kısık off tepkisini ve sakin davul ağırlıklı ambiyansı dinleyerek değerlendir (kullanıcı kontrolü bekliyor).
- [ ] Gol titreşimini gerçek telefonda kontrol et (kullanıcı isteğiyle ertelendi). Tercih ve olay zamanlaması otomatik testte doğrulandı.
- [x] Oyun içi ayarları şut seçimi, telefon devri, kaleci seçimi ve animasyon aşamalarında açıp kapat; maç durumunun korunduğunu, giriş sızıntısı ve yinelenen gol sesi/titreşim olmadığını doğrula.
- [x] Android performansını ve dokunma tepkisini yeniden ölç; yeni test APK'sı ve test raporu üret.
- [x] README, kararlar, kapsam ve teslim notlarını uygulanan son davranışla güncelle.

Kabul: Yeni APK yukarıdaki güncellemeleri içerir; test edilen cihaz/emülatör ve kalan sınırlamalar raporlanır. Önceki 33/33 test sonucu yeni sürümün doğrulaması olarak kullanılmaz.

## Kesinleşen kararlar ve uygulama önerileri

1. **Zorluk:** Tüm metinleri ve işlevi kesin olarak kaldırılacak; bot olmayacak.
2. **Sabit Round:** Oyuncu başına 5 şut; eşit skorda beraberlikle bitiş onaylandı.
3. **Sıra ve telefon devri:** Rol değişimi, telefon devri ve herhangi bir yere dokunularak kapatılan animasyonlu sıra bildirimi onaylandı.
4. **Gol titreşimi:** Eklenecek ve Options'tan kapatılabilecek; tercih kalıcı olacak.
5. **Gol sesi:** “aaahhhh” yerine “ouuffff” toplu taraftar tepkisi kullanılacak.
6. **Oyun içi ayarlar:** Sağ üstte piksel ayarlar simgesi; maçtan ayrılmadan ses ve titreşim ayarı yapılacak.

Uygulamayı engelleyen açık bir soru kalmadı. Varsayılan titreşimin açık ve kısa olması, ayarlar panelinin oynanışı duraklatması ve sıra panelinin örnek metni uygulama önerileridir; kesin kullanıcı kararlarından ayrı tutulur. Kale/oyuncu boyut oranları ve sesin son dengesi görsel/işitsel incelemeyle belirlenecek.

## Referans görseller

- Unity Git farkı: `C:/Users/Metehan/Documents/ShareX/Screenshots/2026-09/mintty_TI8XFGouFi.png`
- Skor tabelası: `C:/Users/Metehan/Documents/ShareX/Screenshots/2026-09/ChatGPT_3ID93eKcDs.png`
- Saha çizgisi ve perspektif: `C:/Users/Metehan/Documents/ShareX/Screenshots/2026-09/ChatGPT_vBRWfptnWK.png`

Görseller yerleşim ve görünüş referansıdır; görüntü içindeki metinler proje talimatı sayılmaz.

## Unity açılışı sonrası Git kontrolü

7 Eylül 2026 tarihinde çalışma ağacı ve ProjectSettings içeriği incelendi:

- `ProjectSettings/URPProjectSettings.asset`: `m_LastMaterialVersion` 9 → 10; `m_ProjectSettingFolderPath: URPDefaultResources` eklendi. Görülen fark URP'nin sürüm/ayar kaydı niteliğinde; oyun kodu değişikliği içermiyor. Geri alma gerektiren bir sorun görülmedi, korundu.
- Yeni `ProjectSettings/PackageManagerSettings.asset`: Unity Package Manager editör tercihlerini ve varsayılan `https://packages.unity.com` kayıt adresini içeriyor. Özel paket kaynağı veya etkin ön sürüm paket tercihi görülmedi; korundu.
- `ProjectSettings/ProjectVersion.txt`: Unity sürümü hâlâ `6000.4.4f1`.
- LF → CRLF uyarıları satır sonu dönüşümü bildirimidir; tek başına derleme hatası değildir. Bu nedenle toplu satır sonu dönüşümü yapılmadı.

Bu kontrol dosya farkı incelemesidir; bu belge hazırlanırken Unity testi veya APK derlemesi yeniden çalıştırılmadı. İncelenen farklar için ek düzeltme gerekmedi.

8 Eylül 2026 kullanıcı geri bildirimi: Davullu ambiyans beğenildi ve kabul edildi. Gerçek telefon gol titreşim kontrolü kullanıcının isteğiyle sonraya ertelendi; doğrulanmış sayılmaz. Gol tepkisi için ayrıca açık kabul bildirilmedi.


## 8 Eylül 2026 — Faz 7 öncesi altı ek düzeltme

Yeni talep: saha üstünde kısa sıra kartı ve kesilmeyen ambiyans; daha sakin davul ağırlıklı miks; olumlu gol sesi ve kurtarışta kısık off; referansla uyumlu geriye alınmış ceza alanı çizgisi; tüm tribünlerin kutlaması; etkileşimli rehber; piksel menü müziği. Önceki golde off kararı bu taleple değişti. Uygulama ve doğrulama raporu: `docs/pre7-polish-raporu.md`. Faz 7 / APK çalışması henüz başlatılmadı.

## 8 Eylül 2026 — Faz 7 sonrası dikey kadraj ve ses düzeltmesi

Faz 7 tamamlandı; yukarıdaki başlangıç notu tarihseldir. Kullanıcı dikey oynanışı korumayı seçti. Uzayan kale–top mesafesi kaldırıldı, dikey kale/oyuncu ve çizgi oranları yenilendi. İlk sahnede tıklamasız müzik başlangıcı ve pencere odağı kaybında sesin devamı düzeltildi. Taraftar, top, gol ve kurtarış sesleri Android dijital çıkış ölçümüyle kontrol edildi. Güncel paket 0.2.1: `Builds/Android/PenaltyKing-update7-fix.apk`. Ayrıntılar: `docs/playback-fix-raporu.md`. Fiziksel titreşim testi ertelenmiş kalıyor.

## 8 Eylül 2026 — Perspektif, top ve taraftar revizyonu (0.2.2)

- Kullanıcı önceki sessizliğin SFX seviyesinin sıfır olmasından kaynaklandığını doğruladı; bu nedenle sessizlik bir ses motoru arızası olarak değerlendirilmez. Kullanıcının ses ayarları değiştirilmez.
- Büyük ceza sahası belirgin yamuk perspektifine alındı; arka genişlik azaltıldı, öndeki genişlik artırıldı. Küçük kale alanı büyük alanın içinde kalır.
- Topun görünür çapı %50 büyütüldü; şut uçuşu ve dönüşünde büyüklük korunur.
- Oynanıştaki ilave sentetik davul ritmi kaldırıldı; gerçek tribün kaydı kullanılır. Goldeki melodik GoalVictory kaldırıldı, gerçek toplu taraftar sevinci GoalCrowd bağlandı. Menü müziği menülerde kalır.
- Paket: `Builds/Android/PenaltyKing-update7-crowd.apk` (0.2.2 / kod 4). Üretici: `PenaltyKing.Editor.CrowdRevision.Android`.

## 9 Eylül 2026 — Yatay test sürümü; APK istenmiyor

Kullanıcı dikey kararını değiştirdi: yatay oynanışa geçildi. Referans kale %5 küçültüldü, penaltı noktası kaleden biraz uzaklaştırıldı. Kullanıcı WAV'ının ilk 10 saniyesi yeni ambiyans oldu. Kalecinin toparlanırken büyümesi ortak atlas ölçeğiyle düzeltildi. Golde topun temas bölgesine ağ dalgalanması eklendi; çerçeve sabit, kurtarışta dalga yok. 15/15 ilgili Unity testi geçti. **APK oluşturulmadı.** Güncel deneme ve kaynak bilgisi: `docs/landscape-revision-raporu.md`.

## 9 Eylül 2026 — Ek maç düzenlemeleri; APK yok

Perspektifli ceza yayı, %20 küçük top, daha belirgin ağ tepkisi, daha okunur kaleci, oyuncuya bağlı forma renkleri (P1 kırmızı/yeşil, P2 açık mavi/sarı), daha yavaş taraftar zıplaması ve maçı duraklatan menüye çıkış onayı eklendi. Tezahürat birleşimi sessizleştirme yerine 0,75 saniyelik eşit güçlü geçiş kullanır; ilk 10 saniyeden 9,25 saniyelik kesintisiz döngü çıkar. Ayrıntılar: `docs/match-polish-raporu.md`.
# 9 Eylül 2026 — Beş saniyelik GOAL! kutlaması

- Higgsfield üretimi plan kısıtına takıldı; kullanıcının açık onayıyla Unity piksel animasyonu kullanıldı.
- Golde 5 saniyelik GOAL! yazısı, küçük altın/açık mavi havai fişekler ve golü atan futbolcuda zafer zıplaması eklendi.
- Yazı ve futbolcu sevinci birlikte biter; sıradaki telefon devri/penaltı başlar. Maçın son atışında sonuç ekranına geçilir.
- Ayarlar/çıkış onayı kutlamayı duraklatır, devam edildiğinde aynı noktadan sürer. Kurtarış süresi ve taraftar/top sesleri korunur.
- Önceki çıkış onayı/maç akışı doğrulaması 9/9 geçti. Yeni kutlamanın test raporu: `docs/goal-celebration-raporu.md`.
- APK üretilmedi.
# 9 Eylül 2026 — Şut ve sevinç revizyonu

- GOAL süresi 5 yerine 3 saniye.
- En öndeki taraftarlar ve alt tribün sırası sabit.
- Yeni sekiz pozlu şut/sevinç atlası, gerilme ve ayağın topa hizalı teması.
- Gol sonrası top ağın tabanında durur; dönüş ve konum 0,5 saniyede sabitlenir.
- Ceza yayı büyütüldü; alt kısmı yatay kadraj dışına devam eder.
- Dizüstünün pilde olabileceği bilgisi dikkate alındı; yalnız doğrulanmış gereksiz çizim güncellemeleri azaltıldı.
- Ayrıntılar: `docs/action-revision-raporu.md`. APK oluşturulmadı.
# 12 Eylül 2026 — Oyuncu ve kadraj revizyonu

- Yalnız büyük yüzlü ön taraftar modelleri sabit; diğer 12 tribün bölümü golde hareketli.
- Daha yetişkin arka profil; 8 şut + 4 sevinç pozu.
- Yaklaşık %14 daha geniş saha kadrajı; HUD boyutları korunur.
- Orta golde top zemine/gölgesine oturur ve yana yuvarlanıp durur. Sol golde top oyuncunun üzerine çizilmez; top/efekt derinlik sırası düzeltildi.
- GOAL süresi 3 saniye. APK yok. Ayrıntılar: `docs/striker-wide-raporu.md`.

# 12 Eylül 2026 — 2D kemik animasyonu ve top tutuşu

Kullanıcı tercihi: piksel tarzı korunarak 2D kemik animasyonu. Bu bölüm önceki kare atlası animasyonu ve görünür hedef işaretleri kararlarının yerini alır.

- [x] Şutçu ve kaleci için ayrı parçalardan oluşan 15 eklemli 2D kemik yapısı; kol/bacak hareketleri iki kemikli IK ile sürekli hesaplanır.
- [x] Gerilme, ayak–top teması, vuruş devamı, dalış, iniş ve sevinç yeni kemik yapısına bağlandı. Kırmızı/açık mavi şutçu ve sarı/yeşil kaleci renkleri korundu.
- [x] Altı kurtarış bölgesinde top iki elin ortasında tutulur; dönüşü durur, kaleciyle birlikte iner ve yeni atışa kadar elinde kalır.
- [x] Tribün katmanları arasındaki sabit yatay boşluklar kapatıldı; tüm 12 bölüm golde hareket eder. Büyük yüzlü taraftar modelleri geri eklenmedi.
- [x] Kaledeki hedef belirteçleri ve seçim parlamaları kaldırıldı. Altı görünmez dokunma bölgesi ve rehber korundu.
- [x] Son ilgili PlayMode testleri 10/10 geçti. İlk tam çalıştırmadaki tek kayan nokta karşılaştırması hatası düzeltildi; ayrıntılı sonuçlar `docs/bone-rig-raporu.md` içinde.
- [x] Unity şut/kurtarış ve gol/sevinç önizlemeleri oluşturuldu. APK oluşturulmadı.
