# Güncelleme Faz 3 — Modlar ve maç yaşam döngüsü

Tarih: 7 Eylül 2026

## Tamamlanan davranış

Sabit Round, oyuncu başına beş şutla toplam on atış sonunda biter; erken bitiş veya uzatma yoktur. Sonuç ekranı PLAYER 1 ve PLAYER 2 adlarını, iki skoru ve kullanılan şut haklarını gösterir. Yüksek skorlu oyuncu kazanır; eşit skorda BERABERE yazar.

Tekrar Oyna aynı modla yeni maç açar. İki skor, atış sayıları, geçmişler, gizli yön seçimi, son sonuç ve oyuncu sırası sıfırlanır. PLAYER 1 şutçu olarak başlar. Pozlar sıfırlanır ve önceki sonuç sesi kesilir; ambiyans üst üste başlatılmaz. Gizli Tekrar Oyna eylemi maç sürerken oturumu sıfırlayamaz.

Endless golde ve kurtarışta sonraki oyuncuya geçer, on atışta veya ilk kurtarışta bitmez. Oyuncu toplamları sürer; geçmiş saklama sınırı toplamları etkilemez.

Sol üstteki küçük Menü düğmesi iki modda da görünürdür. İlk sıra kartında, şut seçiminde, telefon devrinde, kaleci seçiminde ve animasyon sırasında çalışır. Maçtan doğrudan ana menüye döner; bekleyen yönler ve oturum seçimleri temizlenir, animasyon durdurulur ve stadyum sesleri hemen kesilir. Sonuç ekranındaki Ana Menüye Dön aynı temizleme yolunu kullanır.

## Doğrulama

Unity PlayMode testlerine iki oyuncunun ayrı ayrı kazanması, on şuttan önce bitmeme, tam sıfırlama sonrası yeniden şut, on iki atışlık Endless ve beş farklı aşamada çıkış senaryoları eklendi. Mevcut beraberlik uçtan uca testine sonuç başlığı kontrolü eklendi.

**36/36 Unity PlayMode testi geçti; başarısız test yok.** Test çıktısı: `TestResults/update3.xml`. Günlük: `Logs/update3-tests.log`.

Sonuç paneli 390×844 ve 1280×720 yerleşim önizlemelerinde incelendi. Devir kartının üzerindeki Menü düğmesinin dikey önizlemesi de kontrol edildi. Bunlar editör yerleşim önizlemeleridir; çalışan maçtan ekran görüntüleri değildir.

- `docs/previews/update3-result-portrait.png`
- `docs/previews/update3-result-landscape.png`
- `docs/previews/update3-exit-portrait.png`

## Kapsam ve inceleme

Yeni APK üretilmedi; mevcut APK eski sürümdür. Unity'de `Assets/Scenes/MainMenu.unity` sahnesinden başlayarak iki modu inceleyebilirsiniz. Fiziksel telefon testi bu fazda yapılmadı.

Tam ekran saha, final skor tabelası ve sağ üst oyun içi ayarlar Faz 4 kapsamındadır. Buradaki Menü düğmesi maçtan çıkıştır, ayarlar düğmesi değildir.

Yeniden üretim: **Penalty King → Updates → Phase 3 - Build match controls**. Araç mevcut animasyon, ses ve telefon devri ekranını koruyarak sonuç panelini ve Menü düğmesini düzenler.
