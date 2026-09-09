# 9 Eylül 2026 — Maç görünümü ve çıkış onayı

APK oluşturulmadan Unity test sürümü üzerinde çalışıldı.

- Ceza alanının ön çizgisine, kutunun dışında kalan perspektifli yarım çember eklendi.
- Top ölçeği 1,5'ten 1,2'ye indirildi: önceki sürüme göre %20 küçük. Uçuşta da aynı oran korunur.
- Ağın en yüksek hareket miktarı artırıldı; direkler sabit, hareket kısa ve sönümlenen yapıdadır.
- Kaleci yaklaşık %11 büyütüldü, ayakların zemin konumu korundu. Ten renklerinde hafif aydınlatma yapıldı; piksel filtreleme korunur.
- Player 1: kırmızı şutçu / yeşil kaleci. Player 2: açık mavi şutçu / sarı kaleci. Renkler rol devriyle değişir; animasyon karelerinde de aynı materyal kullanılır. Renk değişimi forma ve çoraplara uygulanır; özgün sprite dosyaları değiştirilmez.
- Ön ve arka tribünlerin zıplama hızı 15'ten 7'ye indirildi; genlik korunur.
- Menü düğmesi artık çıkış onayı açar: “Ana menüye dönülsün mü? Mevcut maçın ilerlemesi kaybolacak.” Devam düğmesi maçı aynı noktadan sürdürür. Onay penceresinde atış, ağ ve kutlama zaman çizelgesi durur; dokunmalar maça geçmez. Ambiyans devam eder.

## Ses döngüsü

Önceki uçları sessizleştirme kaldırıldı. Kaynağın ilk 10 saniyesinde, 0,75 saniyelik son/ilk bölüm eşit güçle çapraz birleştirildi. Net döngü 9,25 saniyedir; içerik hâlâ yalnız ilk 10 saniyeden gelir. Ses eklenmedi veya perde/hız değiştirilmedi. Birleşim örnek farkı yaklaşık 0,00059; tepe 0,20. Üç döngülük dinleme örneği: `previews/seamless-crowd-demo.wav`. Sayısal doğrulama fiziksel hoparlörde dinleme iddiası değildir.

## Yeniden üretim ve test

Ses: `Tools/build_user_stadium.py`. Sahne: `Penalty King → Apply Match Polish (no APK)` / `PenaltyKing.Editor.MatchPolish.Setup`. Önceki sahne üreticileri çalıştırılırsa bu adım en son uygulanmalıdır. Bu yöntem build çağırmaz.

Unity'de MainMenu sahnesini açıp Game görünümünü 16:9 / 1280×720 seçin. İki atış yaparak rol renklerini, devam eden atış sırasında Menü'ye basarak duraklatma/iptal/çıkışı deneyebilirsiniz.

Devam oturumunda yarım kalan çıkış onayı ve maç akışı testleri **9/9 geçti** (`TestResults/match-polish-resume.xml`). Yeni 5 saniyelik gol kutlamasıyla birlikte yürütülen genişletilmiş ilgili test grubu da **20/20 geçti** (`TestResults/goal-celebration.xml`). Kutlama ayrıntıları `goal-celebration-raporu.md` içindedir. APK oluşturulmadı.
