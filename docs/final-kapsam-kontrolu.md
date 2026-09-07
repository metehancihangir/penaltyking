# Son kapsam kontrolü

Kaynak promptlar, `docs/kararlar.md` içindeki kullanıcı değişiklikleriyle birlikte kontrol edildi.

| Madde | Son durum / doğrulama |
|---|---|
| Singleplayer | Ana menü → zorluk → mod → maç → sonuç → tekrar/ana menü akışı; iki mod için uçtan uca test |
| Multiplayer | Görünür, pasif; tıklamayla sahne açılmaz |
| Zorluk | Kolay/Orta/Zor; %20/%35/%50 varsayılan; maç başında olasılık kopyalanır |
| Modlar | Sabit Round varsayılan 5 şut; Endless ilk kurtarışta biter |
| Sonuçlar | Yalnızca gol veya kurtarış; doğru yön kurtarış |
| Giriş | Sol/orta/sağ; dokunma başlangıcında tek şut; animasyon ve sahne geçişi boyunca tekrar giriş kilidi |
| Güç/nişan çubuğu | Yok |
| Dinamik zorluk | Yok; skor arttıkça olasılık değişmez |
| Gündüz/gece seçimi | Yok; gece ve projektörler sabit |
| Oyuncu figürü | Kullanıcının değiştirdiği karar gereği görünür, vuruş animasyonu var |
| Options | Yalnızca Music Volume ve SFX Volume; kalıcı kayıt |
| Sesler | Kalabalık dahil tüm stadyum sesleri SFX kanalında; müzik kanalı ayrı |
| Sonuç paneli | Her iki modda mevcut gol skoru, Tekrar Oyna, Ana Menüye Dön |
| Yüksek skor | Daha önce tanımlanmış yüksek skor sistemi yok; fazdaki “varsa” maddesi yeni bir mekanik olarak eklenmedi |
| Geçişler | Beş ekran arasında kısa fade; geçişte ikinci dokunma engeli |
| Ürün arayüzü | Geliştirme ölçümleri yeni ayar veya ekran oluşturmaz; yalnızca development logları |

Otomatik testler: `EndToEndTests`, `SelectionTests`, `GameplayTests`, `MainMenuTests`, `OptionsTests`, `PenaltyRoundTests`, `GameplayAudioTests`, `GameplayArtTests`, `ShotAnimationTests`, `InfrastructureTests`.
