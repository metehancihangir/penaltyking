# Onaylanmış kararlar — 6 Eylül 2026

Bu dosyadaki kullanıcı kararları, kaynak promptların ilgili eski maddelerini geçersiz kılar. Kaynak dokümanların kopyaları değiştirilmeden saklanır.

- Motor: Unity (C#). Kurulu 6000.4.4f1 sürümü kullanılır.
- Görsel atmosfer: gece + stadyum projektörleri. Referanslar atmosfer/kompozisyon ilhamıdır; özgün varlıklar hazırlanır.
- **Şutu çeken oyuncu sahnede görünür.** Eski “oyuncu görünmez / ayak veya gölge dahi yok” kısıtı kaldırılmıştır.
- Faz 5'e şutu çeken oyuncunun özgün piksel sprite'ı eklenir.
- Faz 6'ya oyuncunun hazırlık, vuruş ve vuruş sonrası hareketi eklenir; ayağın topla temas anı top yörüngesi ve kick SFX ile senkronize edilir.
- Kalabalık ambiyansı dahil tüm stadyum efektlerini **SFX Volume** kontrol eder. Music Volume yalnızca müzik içindir.
- Faz 0–8 tamamlandı. Son doğrulamada 33/33 Unity testi geçti. Android emülatöründe iki ekran yönü, iki mod, tekrar oynama ve menü akışı doğrulandı; 59,8–60 FPS ölçüldü. Android test APK'sı `Builds/Android/PenaltyKing-development.apk` dosyasındadır. Yeni kapsam için kullanıcı talebi beklenir.
- Diğer kapsam kısıtları korunur: multiplayer pasif; güç/nişan mekaniği, dinamik zorluk, gol/kurtarış dışında sonuç, gündüz seçimi veya ek options ayarı eklenmez.

## Faz 0 sınırı

Beş boş 2D sahne, sahne geçiş altyapısı, kalıcı oturum yöneticileri ve dokunma testi. Tanılama paneli yalnızca Editor/Development Build'de görünür; ürünün ana menüsü değildir. Final görseller, oyun mekanikleri, ses dosyaları ve ayarların PlayerPrefs'e kaydı sonraki ilgili fazlardadır.

## 7 Eylül 2026 — Güncelleme serisi

Yeni kararların kaynağı kökteki update-notes.md dosyasıdır; eski kararların çelişen maddelerinin yerine geçer. Bot/zorluk kaldırılacak; aynı telefonda iki oyuncu, rol değişimi ve telefon devri olacak. Sabit Round oyuncu başına 5 şut ve beraberlikle bitiş; Endless otomatik bitişsiz olacak. Options'a kalıcı titreşim tercihi ve oyun içi ayarlara erişim eklenir. Golde kısa titreşim ve ouuffff taraftar tepkisi istenir.

Güncelleme Faz 1: Menü/oyuncu/mod seçimi ve titreşim tercihi uygulandı. Yerel maç sistemi Faz 2'yi bekler; yeni menü bot maçını başlatmaz. Önceki APK güncellenmedi. Rapor: docs/update-faz-1-raporu.md.

Güncelleme Faz 2 ile iki insanın yön seçimi, telefon devri ve rol değişimi bağlandı; eski bot/zorluk kodu ve sahnesi kaldırıldı. Oyuncu skorları ayrıdır. Sabit Round/Endless temel kuralları yerel modele uyarlandı; Faz 3 yaşam döngüsü işleri bekliyor. Rapor: docs/update-faz-2-raporu.md.
