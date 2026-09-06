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
