# Güncelleme Faz 2 — Aynı telefonda iki oyuncu

Tarih: 7 Eylül 2026

## Uygulanan davranış

Play → 2 Kişilik → mod seçimi artık yerel maçı açar. PLAYER 1 şutçu, PLAYER 2 kaleci olarak başlar. Şutçu yön seçince tam ekran telefon devri kartı açılır. Kaleci kartı yeni bir dokunuşla kapatır, ardından ayrı bir dokunuşla kurtarış yönünü seçer. İki yön tamamlanınca mevcut şut ve kaleci animasyonları oynar. Aynı yön kurtarış, farklı yön goldür.

Her sonuçtan sonra roller değişir. Kalecilik yapan kişi sonraki şutçu olur; bu noktada kart yalnızca yeni rolü anlatır, gereksiz telefon devri istemez. Kart küçük bir ölçek geçişiyle açılır. PLAYER adı ve Şut/Kurtarış sırası açıkça görünür.

## Mantık ve gizlilik

- Botun rastgele yön üretimi, kurtarış olasılıkları, Difficulty türü, zorluk oturum alanları, zorluk kontrolcüsü ve sahnesi kaldırıldı.
- `PenaltyRound` ilk yönü özel alanda saklar. İlk seçim skor veya atış sayısını değiştirmez; kaleci seçimiyle sonuç kaydedilir.
- Oyuncuların gol sayısı, atış sayısı ve geçmişi ayrıdır. Endless için geçmiş oyuncu başına son 64 sonucu tutar; toplamlar bundan bağımsızdır.
- Şut seçimi sonrası top/oyuncu/kaleci hareket etmez, vuruş sesi çalmaz ve seçilen bölge vurgulanmaz. Telefon devri perdesi tüm ekranı örter.
- Eski parmağın bırakılması, aynı karedeki tıklama ve başka parmağın tıklama olayı devri kapatamaz. Devir dokunuşu yön seçimine aktarılmaz.
- Animasyon sırasında yön girişleri engellenir. Yeni maçta gizli seçim, sıra, skorlar, geçmiş ve pozlar sıfırlanır.

## Faz sınırı

Çalışan maçın tutarlı olması için kişi başına beş şut, on atış sonunda karşılaştırılan skor ve kurtarışta devam eden Endless temel kuralları yeni modele uyarlandı. Faz 3'ün çıkış ve maç yaşam döngüsü düzenlemeleri ayrıca ele alınacak; tamamlandı sayılmadı.

Mevcut saha ve animasyon varlıkları korunur. Tam ekran saha, final skor tabelası, oyun içi ayarlar Faz 4 kapsamındadır. Ses değişimi ve fiziksel gol titreşimi Faz 6 kapsamındadır. Bu fazda yeni APK üretilmedi; önceki APK eski sürümdür.

## Doğrulama dosyaları

- `TestResults/update2.xml`: ilk tam test turu; 32 testten 31'i geçti. Sonuç panelinin etkinleştiği karede tıklamaya çalışan uçtan uca test için arayüzün bir kare hazırlanması beklenecek şekilde test düzeltildi.
- `TestResults/update2-final.xml`: **17/17 geçti**; ilgili oyun, menü seçimi, ses ve animasyon testlerinin tekrar sonucu.
- `TestResults/update2-touch-final.xml`: son dokunma korumasıyla **5/5 geçti**; iptal edilen dokunmanın toparlanması ve gerçek sanal Touchscreen girişleri dahil. Kalan başarısız test yoktur.
- `docs/previews/update2-handoff-portrait.png` ve `update2-handoff-landscape.png`: 390×844 ve 1280×720 telefon devri önizlemeleri; ikisi de görsel olarak incelendi.
- Gerçek Input System Touchscreen olaylarıyla basılı parmağın bırakılması ve yeni dokunuşla devam etme test edilir. Fiziksel telefon testi bu fazda yapılmadı.

## İnceleme

Unity'de `Assets/Scenes/MainMenu.unity` sahnesini açıp Play'e basın. Play → 2 Kişilik → mod seçimi yapın. Sıra kartını kapatıp bir yön seçin, telefonu devredin; ikinci oyuncu kartı kapatıp kurtarış yönünü seçsin.

Devir ekranını yeniden üretme: **Penalty King → Updates → Phase 2 - Build turn handoff**. Bu araç mevcut Gameplay sahnesindeki animasyon ve sesleri koruyarak devir arayüzünü yeniler.
