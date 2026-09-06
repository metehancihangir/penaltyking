# Faz 6 — Animasyonlar

Tamamlandı. Görünür oyuncunun hazırlık/vuruş/son hareket/toparlanma kareleri, kalecinin sol ve sağ dalışı ile ayrı merkez kurtarışı, top yörüngesi ve gol kutlaması Gameplay sahnesine bağlandı.

- Dokunma anında şut ve kaleci yönü tek seferde belirlenir; oyuncu hazırlık hareketine hemen başlar.
- 0,24 saniyede ayak–top teması gerçekleşir. Top bu ana kadar yerinde kalır, sonra dönerek ve perspektifte küçülerek kavisli bir yol izler. Gölge, kısa iz, toz ve hafif ekran sarsıntısı vuruşu destekler.
- 0,90 saniyede top kaleye varır. Skor ve GOL/KURTARIŞ yazısı bu anda güncellenir. Kaleci dalış, temas, yere iniş ve toparlanma karelerini oynar. Sol hareket sağ atlasın yatay aynasıdır; merkez ayrı dört poz kullanır.
- Golde taraftar sırası senkronize zıplar ve kısa konfeti döngüsü oynar. Kurtarışta kutlama olmaz.
- 2,20 saniyede animasyon biter; oyun devam ediyorsa yeni şut hazırlanır, round bittiyse sonuç paneli açılır. Giriş tüm hareket boyunca kilitlidir.

`ShotPresentation` tek zaman çizelgesi kullanır. `Kick`, `Impact(ShotResult)` ve `Completed` olayları Faz 7 sesleri için hazırdır. Gerçek ses dosyaları henüz eklenmedi. Kalabalık dahil SFX Volume kararı korunur. Zaman çizelgesi unscaled time kullanır; kesilen sahneye ait gecikmiş ses olayı kalmaz.

Doğrulama: **27/27 Unity PlayMode testi geçti** (`TestResults/phase6.xml`). Önceki mod/menü/ayar/dokunma testlerine ek olarak vuruştan önce topun yerinde kalması, olayların doğru sırada tek sefer tetiklenmesi, timeScale=0 davranışı, üç kurtarış varyasyonu, gol/kurtarış kutlama ayrımı ve animasyon ortasında sahneden ayrılma test edildi. Yatay/dikey sahne ve temas/sol/orta/sağ/kutlama kareleri Unity kamerasıyla render edilip incelendi.

Varlıklar: `Assets/Sprites/Characters/ShooterActions.png`, `KeeperSide.png`, `KeeperCenter.png`. Üretim istemleri `docs/art/phase6-imagegen-prompts.md` içindedir. Animasyon önizlemeleri `docs/previews/phase6-*.png`.

Unity'de MainMenu sahnesinden Play → Singleplayer → zorluk → mod → kaledeki SOL/ORTA/SAĞ ile denenebilir. Yeniden sahne üretmek gerekirse `Penalty King/Phase 6/Create animated gameplay` menüsü kullanılır; bu işlem Gameplay sahnesini yeniden kurar. Faz 5 üreticisi animasyon katmanını içermez.

Gerçek cihaz performansı ve yayın kontrolleri Faz 8 kapsamındadır. Faz 7 ses entegrasyonu için kullanıcı onayı beklenir.
