# Faz 4 — Temel oynanış tamamlandı

## Uygulanan kapsam

- Gameplay sahnesinde üç dokunma bölgesi: **Sol / Orta / Sağ**.
- Şut dokunma başlangıcında işlenir; parmağın kalkması beklenmez. Kaleci yönü aynı callback içinde belirlenir, ardından sonuç gösterilir.
- Kalecinin doğru yönü seçme ihtimali, Faz 3'te seçilen sabit olasılıktır. Doğru tahmin dışındaki durumda diğer iki yön eşit olasılıkla seçilir. Yanlış tahmin dalında oyuncunun yönü tekrar seçilmez.
- Yönler aynıysa **kurtarış**, farklıysa **gol**. Başka şut sonucu yoktur.
- Gol ve şut sayacı sürekli görünür; sonuç dokunma anında sayaca yansır.
- **Sabit Round:** yapılandırılmış şut sayısı sonunda biter (varsayılan 5); kurtarış olsa da kalan şutlar devam eder.
- **Endless:** ilk kurtarışta biter; o ana kadarki gol sayısı skordur. Sabit roundun şut sınırı uygulanmaz.
- Her iki modda sonuç paneli, **Tekrar Oyna** ve **Ana Menüye Dön** bulunur. Tekrar oynamak skor/sayacı sıfırlar; aynı mod ve zorluk korunur.
- Sonuç 0,8 saniye okunabilir biçimde gösterilirken diğer dokunmalar engellenir. Biten rounda ilave şut eklenemez.
- Seçimler eksikse doğrudan Gameplay açılması oyun başlatmaz; ana menüye dönüş sunulur.
- Geçici çizimlerde sarı K kutusu kaleciyi, beyaz kare topu, kırmızı O kutusu şutu çeken oyuncuyu temsil eder. Final sprite ve animasyonlar eklenmedi.

## Oluşturulan / değiştirilen dosyalar

| Dosya | Görev |
|---|---|
| `Assets/Scenes/Gameplay.unity` | Üç dokunma bölgesi, geçici saha/karakterler, skor ve sonuç paneli |
| `Assets/Scripts/Gameplay/PenaltyRound.cs` | İki sonuç, ağırlıklı kaleci seçimi, sayaçlar, sabit round/Endless bitiş kuralları |
| `Assets/Scripts/Gameplay/GameplayController.cs` | Girdi kilidi, anlık sonuç, geçici geri bildirim, tekrar oynama ve menü dönüşü |
| `Assets/Scripts/UI/ShotZone.cs` | Pointer-down ile şut; pointer-click/release ile ikinci şutu engelleme |
| `Assets/Editor/PhaseFourSetup.cs` | Gameplay sahnesini oluşturan editör aracı |
| `Assets/Editor/MenuPreview.cs` | Geçici oynanış ve skor paneli yerleşim önizlemeleri |
| `Assets/Tests/PlayMode/PenaltyRoundTests.cs` | Karışık sonuçlar, sınırlar, olasılıklar ve Endless serisi testleri |
| `Assets/Tests/PlayMode/GameplayTests.cs` | UI, sanal touch cihazı, sayaç, tekrar oynama ve menüye dönüş testleri |
| `README.md`, `docs/kararlar.md` | Oynama yönergeleri ve faz durumu |

## Çıktı / test kriteri

**Karşılandı:** Her iki mod geçici görsellerle uçtan uca oynanabilir; gol/şut sayacı ve final skorları doğru hesaplanır.

Unity 6000.4.4f1 Play Mode: **23 test geçti, 0 başarısız**. Önceki fazların 14 testi korunmuştur. Dokuz yeni test şunları kapsar:

- Beş şutta karışık gol/kurtarış dizisi, doğru toplam ve altıncı şutun reddi.
- Endless'ta gol serisi ardından ilk kurtarışta bitiş; sabit round sınırından etkilenmeme.
- Üç zorluk oranı × üç şut yönü için sabit rastgelelik tohumu ile toplam 90.000 örnek; hedef olasılıklar ve yanlış yönlerin dengeli dağılımı.
- %0/%100 sınırları, özel round uzunluğu ve geçersiz yönün sayaç tüketmemesi.
- UI üzerinden beş şut, hızlı/çift basışın sayılmaması, skor paneli ve temiz tekrar oynama.
- Endless'ta ilk şut kurtarılırsa sıfır skor; menüye dönüş.
- Endless'ta altı golle beş şut sınırını aşarak devam etme.
- Sanal Touchscreen cihazının gerçek Input System → UI yolundan basışta şut üretmesi; bırakışın ikinci şut üretmemesi.
- Zorluk/mod seçimi olmadan şutun engellenmesi.

Yatay 1280×720 ve dikey 390×844 oynanış/sonuç paneli yerleşimleri Unity renderer'ından alınarak incelendi. Önizlemeler statik yerleşim örnekleridir; skor davranışı ayrıca Play Mode testleriyle doğrulandı.

- Test sonucu: `TestResults/phase4.xml`
- Günlük: `Logs/phase4-tests.log` — çıkış kodu 0
- Önizlemeler: `docs/previews/gameplay-placeholder-landscape.png`, `gameplay-placeholder-portrait.png`, `round-summary-placeholder-landscape.png`, `round-summary-placeholder-portrait.png`

Gerçek mobil cihaz performansı henüz ölçülmedi. Nihai görseller Faz 5, oyuncu/top/kaleci/taraftar animasyonları Faz 6, gerçek ses klipleri Faz 7, son cilalama ve cihaz testi Faz 8 kapsamındadır. Faz 4'teki kısa sonuç beklemesi nihai animasyonların yerine geçmez.

## Sonraki onay

**Faz 5 başlatılmadı.** Ana prompttaki faz onay akışı gereği görsel varlıklar ve sahne kompozisyonuna geçmeden kullanıcı onayı beklenir.
