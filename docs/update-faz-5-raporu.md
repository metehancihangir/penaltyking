# Güncelleme Faz 5 — İlk kullanım rehberi ve saha

Tarih: 7 Eylül 2026

## İlk kullanım

İlk yerel maçta, ilk şutun telefon devri ekranının üzerinde üç adımlı rehber açılır: Şutunu Seç, Telefonu Devret, Kurtarışını Seç. Üç hedef ve piksel oklar dokunulabilir yönleri, telefon/oyuncu simgeleri gizli seçimden sonraki devri anlatır. Mevcut lacivert, yeşil ve açık metin paleti kullanıldı.

Rehber boyunca şut ve devir girişleri engellenir. Aynı karede gelen ikinci ilerleme olayı yoksayılır. Başlayalım düğmesi rehberi kapatır; kullanıcı alttaki devir ekranını ayrı bir dokunuşla kapatır. Ayarlar rehberin üzerinde açılabilir; ayarlar açıkken rehber ilerlemez. Menüden çıkmak mümkündür.

Yalnızca üçüncü adım tamamlanınca `PenaltyKing.Guide.Completed.v1` PlayerPrefs anahtarı kaydedilir. Yarım bırakılan rehber sonraki maçta baştan açılır; tamamlanan rehber sonraki normal girişlerde gösterilmez. Tekrar denemek için Editor Console üzerinden değil, geçici bir geliştirme betiğinden `PlayerPrefs.DeleteKey(FirstPlayGuide.CompletedKey); PlayerPrefs.Save();` çağrısı yapılabilir. Bu işlem yalnız rehber tercihini sıfırlar.

## Saha ve animasyon

Kale genişliği 600'den 480'e, yüksekliği 216'dan 172,8'e indi (%20). Kaleci %15 küçültüldü. Üç dokunma alanı ve uçuş/kurtarış hedefleri yeni kaleye hizalandı. Şutçu yüksekliği eski 160 birimden yatayda 210, uzun dikey ekranda 240 birime çıkarıldı; bütün vuruş kareleri aynı yükseklik ölçeğini kullanır. Ayak-top teması ve kaleci el konumları yeniden ayarlandı.

Çime gömülü eski çizgiler kaldırıldı. `PixelPitch` Unity UI ağı; yatay çim şeritleri, küçük doku pikselleri, iki basamaklı perspektif ceza alanı ve topun gölgesiyle hizalı penaltı noktası çizer. Ceza alanı çizgisi penaltı noktasından önce biter. Gece tribünleri ve projektörler korunur. Yeni raster varlığı veya dış lisans gerektiren kaynak eklenmedi.

## Doğrulama

Unity derleme ve sahne üretimi başarılı: `Logs/update5-final-build.log`.

44/44 Unity PlayMode testi geçti (`TestResults/update5.xml`, `Logs/update5-tests.log`). Rehber giriş engelleme, aynı karede çift ilerleme, kalıcı kayıt, yarım rehber, ayarlardan dönüş ve iki ekran oranındaki hedef hizası dahil mevcut maç testleri başarılı.

Dikey (390×844) ve yatay (1280×720) sahne/rehber önizlemeleri incelendi. Yatay temas karesinde ayak topla, sağ kurtarış karesinde eller topla hizalıdır. Üç hedefin yeni kale içinde kalması otomatik olarak da sınanır. Dikey görünüm sahayı uzatır; oyuncu ve kale oranları korunur.

- `docs/previews/update5-gameplay-landscape.png`
- `docs/previews/update5-gameplay-portrait.png`
- `docs/previews/update5-gameplay-contact.png`
- `docs/previews/update5-gameplay-save.png`
- `docs/previews/update5-guide-step1.png`
- `docs/previews/update5-guide-step2.png`
- `docs/previews/update5-guide-step3.png`
- `docs/previews/update5-guide-landscape.png`

Sahne üreticisi: `PenaltyKing.Editor.UpdatePhaseFiveSetup.Build`. Faz 4 veya daha eski sahne üreticisi çalıştırılırsa Faz 5 üreticisini ardından çalıştırın.

Yeni APK üretilmedi. Mevcut APK önceki sürümdür; değişiklikler Unity projesindedir. Telefon üzerinde gerçek dokunma ve görünüm kontrolü yeni Android paketinde yapılmalıdır. Davullu tezahürat, ouuffff tepkisi ve fiziksel gol titreşimi Güncelleme Faz 6 kapsamındadır.
