# Onaylanmış kararlar — 6 Eylül 2026

Bu dosyadaki kullanıcı kararları, kaynak promptların ilgili eski maddelerini geçersiz kılar. Kaynak dokümanların kopyaları değiştirilmeden saklanır.

- Motor: Unity (C#). Kurulu 6000.4.4f1 sürümü kullanılır.
- Görsel atmosfer: gece + stadyum projektörleri. Referanslar atmosfer/kompozisyon ilhamıdır; özgün varlıklar hazırlanır.
- **Şutu çeken oyuncu sahnede görünür.** Eski “oyuncu görünmez / ayak veya gölge dahi yok” kısıtı kaldırılmıştır.
- Faz 5'e şutu çeken oyuncunun özgün piksel sprite'ı eklenir.
- Faz 6'ya oyuncunun hazırlık, vuruş ve vuruş sonrası hareketi eklenir; ayağın topla temas anı top yörüngesi ve kick SFX ile senkronize edilir.
- Kalabalık ambiyansı dahil tüm stadyum efektlerini **SFX Volume** kontrol eder. Music Volume yalnızca müzik içindir.
- Faz 0–7 tamamlandı. Faz 5 sonunda 24/24, Faz 6 sonunda 27/27, Faz 7 sonunda 31/31 Unity testi geçti. İki mod gece stadyumu, şut/kurtarış animasyonları ve senkronize seslerle oynanabilir. Kullanıcı Faz 7'ye geçişi onayladı; Faz 8 için ayrı kullanıcı onayı beklenir.
- Diğer kapsam kısıtları korunur: multiplayer pasif; güç/nişan mekaniği, dinamik zorluk, gol/kurtarış dışında sonuç, gündüz seçimi veya ek options ayarı eklenmez.

## Faz 0 sınırı

Beş boş 2D sahne, sahne geçiş altyapısı, kalıcı oturum yöneticileri ve dokunma testi. Tanılama paneli yalnızca Editor/Development Build'de görünür; ürünün ana menüsü değildir. Final görseller, oyun mekanikleri, ses dosyaları ve ayarların PlayerPrefs'e kaydı sonraki ilgili fazlardadır.
