# Ses kaynakları — Faz 7

7 Eylül 2026'da kaynak sayfalarındaki lisans bilgileri doğrulandı. Aşağıdaki dört kayıt **CC0 1.0** olarak sunuluyor. Kaynak MP3'ler Freesound'un halka açık HQ önizleme bağlantılarından alındı ve `Assets/Audio/Source` altında korundu. Hesapla erişilen orijinal WAV indirmeleri kullanılmadı.

| Oyun dosyası | Kayıt / üretici | Kaynak ve lisans |
|---|---|---|
| CrowdLoop.wav | Crowd Ambience — FlatHill | https://freesound.org/people/FlatHill/sounds/324757/ — CC0 |
| Kick.wav | Soccer Kick.wav — musita182 | https://freesound.org/s/261267/ — CC0 |
| GoalCheer.wav | goalloop.wav — huubjeroen | https://freesound.org/people/huubjeroen/sounds/113698/ — CC0 |
| SaveReaction.wav | crowd oh disappointed — mrrap4food | https://freesound.org/people/mrrap4food/sounds/619007/ — CC0 |

Lisans metni: https://creativecommons.org/publicdomain/zero/1.0/

İndirme bağlantıları:

- https://cdn.freesound.org/previews/324/324757_3839718-hq.mp3
- https://cdn.freesound.org/previews/261/261267_2399519-hq.mp3
- https://cdn.freesound.org/previews/113/113698_190760-hq.mp3
- https://cdn.freesound.org/previews/619/619007_781461-hq.mp3

Uyarlamalar `Assets/Editor/PhaseSevenSetup.cs` ile yeniden üretilebilir: ambiyansın 10. saniyesinden 24,5 saniye alınır, 0,5 saniyelik geçişle 24 saniyelik döngü yapılır. Tek seferlik kayıtların başındaki sessizlik çıkarılır; vuruş 0,40 saniye, tepkiler 1,18 saniye tutulur. Başlangıç/bitiş yumuşatma ve seviye dengeleme uygulanır. Orijinal kayıtlar değişmez; türetilmiş PCM WAV'lar `Assets/Audio/Stadium` içindedir.

Ambiyans tepe seviyesi 0,16, vuruş 0,80, gol 0,665, kurtarış 0,60. Ambiyans ve tek olay aynı anda çaldığında tam SFX seviyesinde bile sayısal kırpılma payı korunur. Seslerin düşük gecikme için import sırasında açılması ve önceden yüklenmesi sağlanır.

## Güncelleme Faz 6 — 8 Eylül 2026

Eski CrowdLoop ve GoalCheer artık Gameplay sahnesine bağlı değildir. Kick ve SaveReaction korunur. Yeni iki kaynağın CC0 bilgisi 8 Eylül 2026'da Freesound kaynak sayfalarından doğrulandı; yalnız halka açık HQ MP3 önizlemeleri indirildi.

| Aktif dosya | Kaynak | Lisans |
|---|---|---|
| ChantDrumLoop.wav | [Soccer fans screaming and playing drums in a small stadium of Chile — felix.blume](https://freesound.org/people/felix.blume/sounds/500250/) | CC0 1.0 |
| GoalOuff.wav | [Crowd Oooh.wav — deleted_user_2104797](https://freesound.org/people/deleted_user_2104797/sounds/324890/) | CC0 1.0 |

İndirme adresleri:

- https://cdn.freesound.org/previews/500/500250_1661766-hq.mp3
- https://cdn.freesound.org/previews/324/324890_2104797-hq.mp3

Uyarlama: davullu kaydın 30–54,5 saniyesi, 0,5 saniyelik çapraz geçişle 24 saniye döngüye çevrildi. Tepe 0,19; RMS 0,042. Toplu tepkinin 6,25–7,43 saniyesi alındı, başlangıç/bitiş zarfı ve düşük seviyeli özgün gürültüden üretilmiş nefesli “ff” kuyruğu eklendi. Tepki tepe 0,65; RMS 0,120. Eski “aaah” kaydı gol olayından çıkarıldı. İşitsel uygunluk kullanıcı dinlemesini bekler; yalnız kaynak açıklaması ve sayısal analiz dinleme doğrulaması değildir.

Yeniden üretim: Unity `PenaltyKing.Editor.UpdateAudioSource.Decode`, ardından `Tools/build_update6_audio.py` (Python + NumPy), ardından Unity `PenaltyKing.Editor.UpdatePhaseSixSetup.Build`. Ara çözülmüş WAV'lar yeniden üretilebilir, Git'e eklenmez; kaynak MP3 ve final WAV'lar korunur. Sayısal sonuç: `update6-metrics.json`. 52 saniyelik örnek iki döngü birleşimini ve gol/kurtarış olaylarını içerir; gerçek çalışma zamanı kaydı değildir.

## 8 Eylül 2026 — Faz 7 öncesi yeni ses düzeni

Önceki davullu miks/golde off tercihi, kullanıcının yeni talebiyle değişti. Aktif sesler CalmDrumLoop, GoalVictory, SaveOff ve MenuPixelTheme'dir. Kick korunur.

- CalmDrumLoop: CC0 felix.blume maç kaydı (yukarıdaki 500250) düşük ve filtrelenmiş katman; baskın davul/tom bu proje için matematiksel sentezle üretildi.
- SaveOff: CC0 Crowd Oooh (324890), daha kısık seviye ve özgün nefesli kuyruk.
- GoalVictory ve MenuPixelTheme: proje için özgün sentez, melodi ve düzen; dış müzik örneği kullanılmadı.
- `Tools/build_pre7_audio.py` ile yeniden üretilebilir. Girdi, önceki Faz 6 kaynaklarından üretilen ChantDrumLoop ve GoalOuff'tur. Bu kaynaklar aktif çalınmaz ancak yeniden üretim için korunur.
- Sayısal sonuçlar: `pre7-metrics.json`. İşitsel kabul ve uzun süreli rahatlık kullanıcı kontrolünü bekler.

## 8 Eylül 2026 — Yalnız gerçek tribün kayıtları

Son kullanıcı geri bildirimiyle sentetik davul/tom katmanı ve GoalVictory melodisi oynanıştan çıkarıldı. Aktif ambiyans `CrowdChant.wav`, felix.blume'nin yukarıda belgelenen gerçek maç kaydından üretilen ChantDrumLoop'un dengelenmiş kopyasıdır. Kayıttaki doğal tribün/davul sesleri korunur; üzerine ritim veya müzik eklenmez. Aktif gol sesi `GoalCrowd.wav`, huubjeroen'nin yukarıdaki gerçek GoalCheer kaydından gelir. Kick ve SaveOff korunur. Menü müziği yalnız menülerde çalar.

Yeniden üretim: `Tools/build_crowd_revision.py`, ardından `PenaltyKing.Editor.CrowdRevision.Setup`. Ölçümler: `crowd-revision-metrics.json`. Önceki dosyalar tarihsel üretimi korumak için bulunur ancak Gameplay sahnesine bağlı değildir. Ses uygunluğu dinlenmiş gibi varsayılmaz; kaynak türü ve sayısal çıkış doğrulanır.
