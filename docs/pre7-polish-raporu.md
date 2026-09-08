# Faz 7 öncesi — Oynanış, devir ve ses iyileştirmeleri

Tarih: 8 Eylül 2026

Kullanıcının altı yeni isteği uygulanmıştır. Bu çalışma Güncelleme Faz 7 değildir; yeni APK üretilmez. Son istekler, önceki gol/off ses yerleşimi kararının yerine geçer.

## 1. Saha üzerinde sıra bildirimi

Tam opak devir ekranı yerine %12 koyu saydam giriş katmanı ve kısa kart vardır. Görünen metin yalnız “Şut Sırası / PLAYER N'de” veya “Kurtarış Sırası / PLAYER N'de”. Eski açıklama ve “Devam etmek için dokun” gizlendi. Yeni dokunuşla devam etme, gizli seçim ve çift dokunma koruması sürer. Handoff sahne değiştirmez veya ses kaynağını durdurmaz; ambiyansın örnek sayacının iki devir aşamasında da ilerlediği test edilir.

## 2. Daha sakin sesler

CalmDrumLoop: önceki gerçek maç kaydı çok düşük seviyede ve 430 Hz üstü azaltılarak uzak arka plan katmanı yapıldı. Özgün bas davul/tom ritmi öne alındı; 24 saniyelik kesintisiz döngü. Eski bağırış ağırlıklı miks aktif sahneden çıkarıldı.

GoalVictory: özgün, kısa ve olumlu arcade arpeji + ağ benzeri hafif vuruş. Goldeki önceki ouff artık kullanılmıyor. SaveOff: toplu tepkinin kısık, nefesli off uyarlaması; önceki SaveReaction/ah kaydı devreden çıkarıldı. Seslerin işitsel kabulü kullanıcı dinlemesini bekler; bu oturumda dinleme desteklenmediği için sayısal ölçümlere dayanarak ses kalitesi doğrulanmış sayılmaz.

Yeni tepe/RMS: ambiyans 0,200/0,030; gol 0,553/0,130; kurtarış 0,272/0,050. Birleşimde kırpılma payı korunur. Kaynak/lisans ve yeniden üretim bilgisi `docs/audio-source/README.md` içinde.

## 3. Referans oynanış ve ceza alanı

Verilen görsel projenin yatay kompozisyonuyla örtüşür; kale, şutçu, tabela ve gece atmosferi korundu. Son mavi çizgili referansla ceza alanı yeniden derinleştirildi; ön çizgi oyuncunun ayaklarına yaklaşır. Küçük kale alanı geride kalır. Penaltı noktası topun altındaki zemin konumunda daha küçük bir piksel işareti oldu. Dikey görünüm saha boyunu uyarlamaya devam eder.

## 4. Tüm tribünler

Arka tribünün üst/alt katında 12 taraftar bölümü, mevcut resmin UV bölgelerinden Unity UI içinde ayrıldı. Merdivenler ve stadyum yapısı yerinde kalır. Her bölüm hafif farklı evrede zıplar; mevcut ön sıra da kutlar. Hareket aynı şut zaman çizelgesinden örneklenir; ayarlarda durur, kurtarışta çalışmaz, yeni atışta başlangıca döner. Kaynak raster dosyaları değiştirilmedi.

## 5. Etkileşimli ilk kullanım rehberi

Mini kale/saha, üç adım şeridi ve deneme hedefleri eklendi. Bir hedefe dokunmak seçimi vurgular ve kısa açıklama verir; gerçek maç yönü veya skoru değişmez. Ayarlar açıkken deneme engellenir. Tamamlanma tercihi korunur; daha önce bitirenlerin rehberi zorla yeniden açılmaz. Temiz kayıtla test edilir.

## 6. Menü müziği

MenuPixelTheme: bu proje için özgün 16 ölçülük, 120 BPM chiptune; üçgen/yumuşak kare sesler, bas ve hafif perküsyon. 32 saniye döngü, başka bir eserden melodi alınmadı. MainMenu, seçim ve Options gezinmesinde sürer; Gameplay'de durur, dönüşte tekrar başlar. Music Volume yönetir; SFX bağımsızdır. Resources altında önceden yüklenir.

## Doğrulama

Unity sahne üretimi ve yatay/dikey önizleme başarılı (`Logs/pre7-build-final.log`).

Son yerleşim dahil 48/48 test Güncelleme Faz 7 koşusunda geçti: `docs/update-faz-7-raporu.md`.

Önizlemeler: `docs/previews/pre7-gameplay-landscape.png`, `pre7-gameplay-portrait.png`, `pre7-handoff-landscape.png`, `pre7-handoff-portrait.png`, `pre7-guide-portrait.png`, `pre7-guide-landscape.png`, `pre7-gameplay-goal.png`.

Ses örneği: `docs/previews/pre7-audio.wav` (24 saniye; gol yaklaşık 4., kurtarış yaklaşık 10. saniye). Menü müziği: `Assets/Resources/Audio/MenuPixelTheme.wav`. Örnek miks çalışma zamanı kaydı değildir.

Üretici: `PenaltyKing.Editor.PreSevenPolish.Build`. Önceki sahne üreticilerini çalıştırdıktan sonra bunu en son çalıştırın. Unity testleri ve önizlemeler gerçek telefonda 10 dakikalık dinleme rahatlığı ya da fiziksel titreşim kontrolünün yerine geçmez. Kullanıcının titreşim kontrolünü sonraya bırakma kararı korunur.
