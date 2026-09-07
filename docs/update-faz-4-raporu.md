# Güncelleme Faz 4 — Tam ekran saha, tabela ve oyun içi ayarlar

Tarih: 7 Eylül 2026

## Önceki fazın kontrolü

Faz 3 test dosyası yeniden kontrol edildi: 36/36 test geçmişti. Sonuç `docs/update-faz-3-raporu.md` dosyasına işlendi; Faz 3'te eksik uygulama kalmadı.

## Görsel düzen

Stadyum artık güvenli alanın içindeki küçük bir panel yerine ekranın tamamını doldurur. Saha boyu ekran oranına göre uzar; kale, oyuncu ve top aynı oranda ölçeklenir. Dikey ekranda tabelaya ve futbolcunun alt kenarına yer bırakılır. Eski Endless/Gol/Şut sayaçları ve sabit yön yönergeleri gizlenmiştir; görünmez dokunma bölgeleri korunur.

Üstte lacivert zemin, yeşil aktif oyuncu vurgusu, PLAYER 1 / PLAYER 2 adları, ortada skor ve beşer atış işareti vardır. Yeşil artı golü, mercan çarpı kurtarışı gösterir; kullanılmamış atışlar nötrdür. Endless'ta yalnızca son beş sonuç görünür, toplam skor korunur. Aktif oyuncu ve şut/kurtarış rolü belirtilir. Tabela çentik/güvenli alan içinde kalır.

Menü kontrolü tablanın solunda, piksel dişli ayarlar düğmesi sağındadır. Ayarlar ve sonuç panelleri mevcut lacivert/yeşil tasarım dilini sürdürür. Kale boyutunun küçültülmesi, futbolcunun büyütülmesi ve saha çizgilerinin yeniden çizimi Faz 5 kapsamındadır.

## Oyun içi ayarlar

Dişli simgesi mevcut sahne üzerinde Music Volume, SFX Volume ve Titreşim kontrollerini açar. Ana menü Options ekranıyla aynı kayıt ve oturum verilerini kullanır. Değişiklikler anında uygulanır; Devam Et paneli kapatır.

Panel açıkken şut ve devir girişleri engellenir. Şut animasyonunun kendi zaman çizelgesi duraklar; yalnızca zaman ölçeği değiştirilmez. Vuruş/sonuç sesleri gerekiyorsa duraklatılır, ambiyans ayar değişikliklerini duymak için çalmayı sürdürür. Kapatınca aynı maç, gizli yön seçimi, skorlar, sıra ve animasyon noktası korunur; olaylar yeniden tetiklenmez. Fiziksel gol titreşimi Faz 6 kapsamındadır.

## Higgsfield taslak görseli

Higgsfield ile bir UI konsepti üretildi: `7b464000-823a-41b2-a681-d9bc6d0e3998`. İstek mevcut gece/piksel stile göre lacivert, teal, lime renkler, iki oyuncu, beş atış işareti ve tam ekran saha olarak tanımlandı. İstenen model `nano_banana_pro`, servisin sonuç kaydındaki model `nano_banana_2` oldu.

[Higgsfield konsept görseli](https://d8j0ntlcm91z4.cloudfront.net/user_3J0rn8sL7j9J79zvJ5HteKImCIJ/hf_20260907_192711_7b464000-823a-41b2-a681-d9bc6d0e3998.png)

Bu görsel konsept çıktısıdır; oyuna sabit bir ekran resmi olarak eklenmedi. Gerçek arayüz mevcut proje paletiyle Unity Image/Text/Slider/Toggle öğelerinden oluşturuldu; skorlar ve etkileşimler canlıdır. Konseptin araç çıktısı kullanıcıya gösterildi; yerel Unity önizlemeleri ayrıca görsel olarak incelendi.

## Doğrulama ve teslim

**40/40 Unity PlayMode testi geçti.** Test dosyası: `TestResults/update4.xml`; günlük: `Logs/update4-tests.log`.

Son işaret okunurluğu ve güvenli alan düzenlemesinden sonra ilgili UI testleri yeniden çalıştırıldı: **5/5 geçti** (`TestResults/update4-final-ui.xml`). Kalan başarısız test yoktur.

Yeni testler ayarların dört seçim aşamasında açılması, gizli yönün korunması, animasyonun iki ayrı noktada duraklatılması, tek vuruş/sonuç sesi, Options ile ortak tercihler, sınırlı atış geçmişi ve üç ekran oranında sahayı doldurma davranışını kapsar.

Önizlemeler editör yerleşim görüntüleridir:

- `docs/previews/update4-gameplay-portrait.png`
- `docs/previews/update4-gameplay-landscape.png`
- `docs/previews/update4-settings-portrait.png`
- `docs/previews/update4-settings-landscape.png`

Unity'de MainMenu → Play → 2 Kişilik → mod seçimiyle inceleyebilirsiniz. Sahne üreticisi **Penalty King → Updates → Phase 4 - Build full screen HUD** menüsündedir. Önceki faz üreticileri tek başına çalıştırılırsa bu üreticiyle son UI yeniden bağlanmalıdır.

Bu aşamada yeni APK üretilmedi; telefondaki önceki APK eski sürümdür. Fiziksel telefon testi yapılmadı.
