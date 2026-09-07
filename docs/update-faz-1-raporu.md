# Güncelleme Faz 1 — Menü ve gezinme

Tarih: 7 Eylül 2026

## Uygulanan değişiklikler

- Ana menüde Play ve Options kaldı. İstenen sloganlar ve YAKINDA etiketi kaldırıldı; PENALTY KING başlığı ile buton metinleri ortalandı.
- Play, yeni PlayerSelect sahnesini açar. Online pasiftir ve piksel dünya ikonu taşır. 2 Kişilik, iki kullanıcı ikonu ve Tek Telefon alt yazısıyla mod seçimini açar.
- Mod seçimi zorluk istemez; Sabit Round ve Endless altında açıklama bulunmaz. Geri düğmesi oyuncu seçimine döner ve oturum seçimini temizler.
- Options'tan PENALTY KING ve SES AYARLARI kaldırıldı. İki ses kaydırıcısı korunarak Titreşim anahtarı eklendi. Varsayılan açık; tercih PlayerPrefs ile anında kaydedilir ve servisler yeniden oluşturulunca yüklenir. Ses seviyelerinden bağımsızdır.
- Derleme sahneleri MainMenu, PlayerSelect, ModeSelect, Gameplay ve Options olarak güncellendi. DifficultySelect eski kaynak olarak korunur, derlemeye girmez.

## Faz sınırı

İki kişilik şut/kurtarış ve telefon devri Güncelleme Faz 2 kapsamındadır. Yeni yerel seçim eski bot maçını başlatmaz: mod seçimi sonrası henüz hazır olmadığı bilgisi ve ana menüye dönüş gösterilir. Eski bot/olasılık sınıfları animasyon ve ses regresyonlarını doğrulamak için bu aşamada korunmuştur; Faz 2'de değiştirilecektir.

Titreşim tercihi hazırdır; fiziksel gol titreşimi Güncelleme Faz 6'da bağlanacaktır. Oyun içi ayarlar paneli Faz 4, iki kişilik mod bitiş kuralları Faz 3 kapsamındadır.

Yeni APK üretilmedi. `Builds/Android/PenaltyKing-development.apk` önceki sürümdür.

## Doğrulama

- Unity 6000.4.4f1 PlayMode: **33 test geçti, 0 başarısız**. Menü/yerel seçim testleri yeni davranışa uyarlandı. Titreşimin dokunmayla açılıp kapanması, seslerden bağımsızlığı ve servis yeniden yüklemesi doğrulandı.
- Mevcut şut animasyonu, ses ve eski round davranışları ayrıca regresyon olarak test edildi; bunlar iki kişilik maçın tamamlandığı anlamına gelmez.
- Dört ekran için 390×844 ve 1280×720 önizlemeler üretildi. Önizleme üreticisi tüm Selectable sınırlarının ekran içinde olduğunu kontrol eder.
- Son ikon düzenlemesi ve sahne üretiminden sonra ilgili menü/ayar testleri yeniden çalıştırıldı: **10/10 geçti** (`TestResults/update1-final-menus.xml`).
- Test çıktısı: `TestResults/update1.xml`; Unity test günlüğü: `Logs/update1-tests.log`.
- Önizlemeler: `docs/previews/update1-{MainMenu,PlayerSelect,Options,ModeSelect}-{portrait,landscape}.png`.

## Unity'de inceleme ve yeniden üretim

`Assets/Scenes/MainMenu.unity` sahnesini açıp Play'e basın. Play → 2 Kişilik → mod seçimi ve geri dönüşleri; Options → Titreşim ve ses kaydırıcılarını inceleyin.

Sahne üreticisi: **Penalty King → Updates → Phase 1 - Build menus** (`UpdatePhaseOneSetup.Build`). Bu araç ilgili menü sahnelerini yeniden üretir; elle yapılan menü düzenlemelerini üzerine yazabilir. Eski faz üreticilerini tek başına çalıştırmak eski menü tasarımını geri getirir.
