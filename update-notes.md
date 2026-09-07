# Penalty King — Güncelleme planı

Tarih: 7 Eylül 2026

Durum: Güncelleme Faz 1 uygulandı; sonraki güncelleme fazları bekliyor. Mevcut APK önceki Faz 0–8 sürümüdür; bu aşamada yeni APK üretilmedi. Aşağıdaki fazlar yeni güncelleme serisidir.

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

Faz 1 sınırı: Menü ve seçim akışı tamamlandı. Yerel maçın sıra/şut sistemi Faz 2’de bağlanacak; bu arada mod seçimi sonrasında bot maçı başlatılmaz. Eski zorluk sahnesi derleme listesinden çıkarıldı; eski oyun mantığının koddan tamamen kaldırılması Faz 2 kapsamındadır. Titreşim tercihi kaydedilir; fiziksel gol titreşimi Faz 6 kapsamındadır.

## Güncelleme Faz 2 — Aynı telefonda iki oyuncu ve sıra sistemi

### Modül 2.1 — İnsan kontrollü şut ve kurtarış

- [ ] Botun kaleci yönü üretimini ve kurtarış olasılıklarını aktif oyun mantığından çıkar.
- [ ] Zorluk seçimi zorunluluğunu, oturumdaki zorluk bağımlılığını ve ilgili aktif yapılandırmayı kaldır.
- [ ] Player 1 ve Player 2 için ayrı kimlik, skor ve şut geçmişi tut.
- [ ] İlk oyuncu şut yönünü, diğer oyuncu kurtarış yönünü seçsin.
- [ ] Her iki seçim tamamlandığında mevcut şut/kaleci animasyonunu bu iki insanın seçimiyle oynat.
- [ ] Aynı yön kurtarış, farklı yön gol kuralını koru; rastgele bot kararı verme.

### Modül 2.2 — Telefon devri ve rol değişimi

- [ ] Şut seçimi → telefon devri → kaleci seçimi → animasyon/sonuç → sonraki sıra durumlarını açıkça yönet.
- [ ] Şut seçimini devretme ekranında gizle; seçili yönü vurgulama veya topu önceden hareket ettirme yoluyla ikinci oyuncuya açıklama.
- [ ] Sıra dışı girişleri, çift dokunmayı ve animasyon sırasında yeniden seçimi engelle.
- [ ] Aktif şutçuyu ve kaleciyi kısa, okunaklı bir gösterimle belirt.
- [ ] Rol değişimini ve yeni maçta sıfırlamayı uygula.
- [ ] Telefon devrinde kısa, kullanıcı dostu bir piksel geçiş animasyonu göster. Örnek başlık: “Sıra PLAYER 2'de”; alt bilgi: “Kurtarış sırası” veya “Şut sırası” ve “Devam etmek için dokun”. Oyuncu numarası ve rol sıraya göre değişsin.
- [ ] Devir ekranında herhangi bir yere dokunulduğunda panel kapansın; bu dokunuş tüketilsin ve yön seçmek için yeni bir dokunuş gereksin.
- [ ] Önceki yön seçimini yapan parmağın basılı tutulması veya bırakılması devir panelini kapatmasın; panel yeni bir dokunuş beklesin.

Onaylanan tasarım: İlk şutta Player 1 şutçu, Player 2 kaleci olur; her tamamlanan şuttan sonra roller değişir. Telefon devrinde sıradaki oyuncu ekrana dokunarak devam eder. Kalecilik yapan oyuncu sonraki şutta şutçu olacağından aynı kişiye gereksiz telefon devri istenmez; yeni rol kısa bir bildirimle anlatılır. Aynı fiziksel ekranı izleyerek yapılan seçimi görmeyi yazılımla tamamen engellemek mümkün olmadığından devretme akışı seçimleri sonradan açığa çıkarmamalıdır.

Kabul: Her sonuç iki insanın seçimiyle oluşur. Şut yönü kaleci seçim ekranında sızmaz. Skor doğru oyuncuya yazılır ve roller belirlenen sırada değişir.

## Güncelleme Faz 3 — Mod kuralları ve maç yaşam döngüsü

### Modül 3.1 — Sabit Round

- [ ] Modu iki oyunculu sıraya uyarla; iki oyuncunun eşit sayıda şut hakkı olmasını sağla.
- [ ] Sonuç ekranını iki skor ve maç sonucunu gösterecek biçimde düzenle.
- [ ] Tekrar Oyna ve Ana Menüye Dön akışlarını yeni oturuma uyarla.

Onaylanan kural: Oyuncu başına 5 şut, toplam 10 şut. Tüm haklar tamamlanınca yüksek skor kazanır; eşitlik beraberliktir. Erken bitiş veya uzatma eklenmez.

### Modül 3.2 — Endless

- [ ] İlk kurtarışta bitiş kuralını kaldır.
- [ ] Golde de kurtarışta da sonraki oyuncunun sırasına geç; otomatik şut/round sınırı koyma.
- [ ] İki oyuncunun skorlarını oyun sürdükçe tut.
- [ ] Oyuncuların menüye dönebileceği kompakt bir çıkış eylemi sağla.

Kabul: Endless'ta art arda kurtarışlar oyunu bitirmez. Sabit Round'da iki oyuncu eşit hak kullanır. Tekrar oynama yönleri, skorları, geçmişi ve sırayı temizler.

## Güncelleme Faz 4 — Tam ekran saha ve piksel skor tabelası

### Modül 4.1 — Tam ekran kompozisyon

- [ ] Stadyumu küçük bir oyun paneli yerine tüm ekranı kaplayacak şekilde düzenle.
- [ ] Üstteki mevcut Endless, Gol ve Şut değerlerini kaldır; yerlerini yeni skor tabelası alsın.
- [ ] Alttaki Bir yöne dokun ve SOL / ORTA / SAĞ yazılarını kaldır. Görünmez dokunma bölgeleri kullanılabilir kalmalı.
- [ ] Gece stadyumunu ekran kenarlarına kadar uzat; skor ve kontrolleri çentik/güvenli alan içinde tut.
- [ ] Dikey ve yatay ekranlarda top, şutçu, kaleci ve kale okunabilir kalsın; görselleri esnetme.

### Modül 4.2 — Skor tabelası

- [ ] Referanstaki üst bant, karşılıklı oyuncu alanları, ortadaki skor ve atış işaretlerinden yararlanarak özgün piksel tabela oluştur.
- [ ] Player 1 ve Player 2 adlarını ve ayrı skorlarını göster; aktif oyuncuyu ayırt et.
- [ ] Sabit Round'da bekleyen atış/gol/kurtarış işaretlerini göster.
- [ ] Endless için büyüyüp ekranı taşırmayan sınırlı bir son atış geçmişi tasarla; toplam skor görünür kalsın.
- [ ] Referanstaki gerçek takım adlarını, logoları ve reklamları kullanma.

Kabul: Oyun sahnesi ekranı doldurur. Eski sayaç/yönerge metinleri yoktur. Tabela küçük telefon ekranında okunur ve dokunma bölgelerini kapatmaz.

### Modül 4.3 — Oyun içi ayarlar paneli

- [ ] Sağ üst köşeye küçük, oyunun stiline uygun piksel dişli simgesi yerleştir; görünümü küçük olsa da dokunma alanı rahat kullanılabilir olsun ve skor tabelasıyla çakışmasın.
- [ ] Simgeye dokunulduğunda mevcut oyun sahnesi üzerinde ayarlar paneli açılsın; Options sahnesine geçilmesin ve maçtan çıkılmasın.
- [ ] Panelde Music Volume, SFX Volume ve Titreşim aç/kapat kontrolü bulunsun. Ana menü Options ekranıyla aynı tercihleri kullansın ve değişiklikleri anında uygulasın.
- [ ] Panel açıkken alttaki yön seçimi, rehber ve telefon devri girişlerini engelle; panel açma/kapatma dokunuşu oyuna sızmasın.
- [ ] Panel kapatılınca skor, şut hakları, roller, gizli seçimler ve sıra korunsun; maç sıfırlanmasın.
- [ ] Önerilen davranış: panel açıkken şut animasyonu ve sıra ilerlemesi duraklasın; kapatıldığında aynı noktadan devam etsin. Ayar değişikliklerini duyabilmek için ambiyans ses ayarına göre çalmayı sürdürsün. Mevcut animasyonlar ölçeklenmemiş zaman kullandığından yalnızca zaman ölçeğini sıfırlamakla yetinilmesin.

Kabul: Oyunun herhangi bir aşamasında ayarlar açılıp kapatılabilir; maç durumu kaybolmaz, arkada seçim yapılamaz ve ses/titreşim tercihleri iki ayarlar arayüzünde tutarlıdır.

## Güncelleme Faz 5 — İlk kullanım rehberi ve saha görselleri

### Modül 5.1 — İlk kullanım rehberi

- [ ] İlk oyunda, penaltı kullanılmadan önce kısa bir piksel rehber paneli göster.
- [ ] Oklarla dokunulabilir yönleri anlat; az metin, okunaklı font ve anlaşılır görsel geri bildirim kullan.
- [ ] Yeni iki kişilik düzene uygun olarak şut seçimi, telefon devri ve kaleci seçimini açıkla.
- [ ] Rehber dokunuşunun yanlışlıkla şut/kurtarış seçmesine izin verme.
- [ ] Rehber tamamlandı bilgisini sakla; sonraki normal girişlerde yeniden gösterme.

### Modül 5.2 — Kale, saha çizgileri ve şutçu

- [ ] Kaleyi mevcut görünümüne göre küçült; kaleci, top hedefleri ve dokunma bölgelerini yeni kale ölçülerine uyarla.
- [ ] Şutçu futbolcuyu büyüt; hazırlık, temas ve vuruş sonrası karelerinde aynı ölçeği koru.
- [ ] Mevcut rahatsız edici penaltı/saha çizgisini, referanstaki ceza alanı geometrisi ve perspektifine uygun özgün piksel çizgilerle yeniden düzenle.
- [ ] Penaltı noktasını ve topun konumunu saha perspektifiyle uyumlu tut.
- [ ] Top yörüngesini, ayak-top temasını ve kalecinin kurtarış konumlarını yeni kompozisyona göre yeniden ayarla.

Kesin büyütme/küçültme oranları kullanıcı tarafından verilmedi; dikey/yatay önizlemeler üzerinden belirlenecek.

Kabul: Daha küçük kale ve daha büyük şutçu birlikte dengeli görünür. Oyuncunun ayağı topa temas eder; kurtarışlar ve gol hedefleri yeni kaleyle hizalıdır. Rehber yalnızca ilk kullanımda gösterilir.

## Güncelleme Faz 6 — Davullu taraftar tezahüratı ve gol titreşimi

### Modül 6.1 — Ambiyans değişimi

- [ ] Kafe sohbetini andıran mevcut maç ambiyansını aktif kullanımdan kaldır.
- [ ] Yerine maç hissi veren toplu taraftar tezahüratı ekle; davul sesi mutlaka bulunmalı.
- [ ] Özgün veya uygun lisanslı ses kullan; kaynak ve lisans kaydını güncelle.
- [ ] Döngü geçişini kesintisiz yap; çok gürültülü veya yorucu bir miks oluşturma.
- [ ] Vuruş, gol ve kurtarış efektlerinin duyulabilirliğini koru.
- [ ] Gol anındaki mevcut “aaahhhh” taraftar tepkisini kaldır; yerine kullanıcının istediği toplu “ouuffff” tepkisini koy. Bu değişiklik gol olayı içindir; kurtarışa taşınmasın.

### Modül 6.2 — Ses kontrolü ve doğrulama

- [ ] Tezahürat ve davulu SFX Volume kanalına bağla; SFX sıfırken ikisi de susmalı.
- [ ] Menüye dönüşte stadyum seslerini durdur; tekrar girişte üst üste döngüler başlatma.
- [ ] Gerçek dinleme ve telefon hoparlöründe kullanıcı kontrolüyle atmosferi değerlendir; yalnızca teknik ses ölçümleri yeterli sayılmasın.

Kabul: Davullu tezahürat duyulur, konuşma/kafe hissi giderilir, ses patlaması veya döngü kopması olmaz. SFX ayarı bütün stadyum seslerini birlikte kontrol eder.

### Modül 6.3 — Gol titreşimi

- [ ] Golün görsel olarak gerçekleştiği sonuç anında kısa bir telefon titreşimi üret; yön seçilirken veya sonuç daha gösterilmeden titreştirme.
- [ ] Her gol için yalnızca bir kez tetikle; kurtarışta titreşim verme. Duraklatıp devam etmek aynı golü tekrar titreştirmesin.
- [ ] Titreşimi Options ve oyun içi paneldeki ortak aç/kapat tercihine bağla; kapalıyken golde titreşim oluşmasın.
- [ ] Titreşim tercihi SFX Volume'dan bağımsız olsun; ses kısılması titreşim tercihini değiştirmesin.
- [ ] Önerilen başlangıç tercihi: titreşim açık ve kısa, tek darbe. Desteklemeyen cihazlarda oyun normal devam etsin.
- [ ] Gerçek telefonda açık/kapalı durumunu ve hissedilen süreyi doğrula; emülatör sonucu fiziksel titreşim doğrulaması yerine geçmesin.

Kabul: Gol başına tek kısa titreşim vardır; kapalı ayarda ve kurtarışta titreşim yoktur. Tercih yeniden açılışta korunur.

## Güncelleme Faz 7 — Entegrasyon, test ve yeni APK

- [ ] Eski bot/zorluk ve ilk kurtarışta bitiş testlerini yeni kurallara uyarla.
- [ ] Menü, pasif Online, yerel oyuncu seçimi ve geri gezinme akışlarını doğrula.
- [ ] İki insanın tüm yön eşleşmelerinde gol/kurtarış hesabını doğrula.
- [ ] Gizli seçim, telefon devri, rol değişimi, çift dokunma ve skor sahipliği senaryolarını test et.
- [ ] Sabit Round'un eşit haklarını, beraberliği ve tekrar oynamayı test et.
- [ ] Endless'ın çok sayıda gol/kurtarıştan sonra devam ettiğini ve geçmiş göstergesinin sınırlı kaldığını doğrula.
- [ ] İlk kullanım rehberini temiz kayıtla ve sonraki açılışta test et.
- [ ] Tam ekran yerleşimini, güvenli alanları ve animasyon hizasını dikey/yatay telefon ölçülerinde incele.
- [ ] SFX sıfır/orta/yüksek seviyelerde ses davranışını kontrol et.
- [ ] Gol tepkisinin “ouuffff” olduğunu, ambiyansta davul bulunduğunu ve seslerin doğru olayda çaldığını dinleyerek doğrula.
- [ ] Gol titreşimini gerçek telefonda, tercih açık/kapalıyken ve SFX sıfırken kontrol et; ayarın yeniden açılışta korunduğunu doğrula.
- [ ] Oyun içi ayarları şut seçimi, telefon devri, kaleci seçimi ve animasyon aşamalarında açıp kapat; maç durumunun korunduğunu, giriş sızıntısı ve yinelenen gol sesi/titreşim olmadığını doğrula.
- [ ] Android performansını ve dokunma tepkisini yeniden ölç; yeni test APK'sı ve test raporu üret.
- [ ] README, kararlar, kapsam ve teslim notlarını uygulanan son davranışla güncelle.

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
