# Faz 5 — Görsel varlıklar ve sahne kompozisyonu

Tamamlandı. Geçici K/O/kare işaretlerinin yerine gece stadyumu, projektörler, dolu tribünler, perspektif çim, beyaz direk/file, sarı kaleci, kırmızı şut oyuncusu ve futbol topu eklendi. Kullanıcının güncel kararı doğrultusunda şutu çeken oyuncu görünür.

Sabit kamera topun arkasından kaleye bakar. Sky/Floodlights → Stands → Crowd → Pitch → Goal → Keeper → Shooter → Ball → Touch/UI sırası korunur. Dekorasyon dokunma olaylarını engellemez. Dikey görünümde çim alan uzar; karakterler ve kale oranlarını korur. Arka plandaki beyaz penaltı noktası, dikey uzatmada ikinci bir top gibi görünmemesi için imagegen ile kaldırıldı.

Varlıklar `Assets/Sprites/Stadium`, `Assets/Sprites/Characters`, `Assets/Sprites/Props` klasörlerindedir. Kaynak PNG'ler korunur; Unity Point filtre ve sıkıştırmasız import kullanır. Üretim istemleri `docs/art/imagegen-prompts.md` içindedir. Referans görsellerden sprite/logo kopyalanmadı.

Doğrulama: 24/24 Unity PlayMode testi geçti (`TestResults/phase5.xml`). Yeni test, iki ekran oranında karakterlerin ekranda kaldığını ve üç hedefin dekorasyonun önünde dokunma aldığını kontrol eder. Yatay/dikey Unity kamera çıktıları `docs/previews/phase5-gameplay-*.png` olarak incelendi.

Kullanıcı 7 Eylül 2026'da Faz 6'ya geçişi de onayladı.
