# Piksel Penaltı Oyunu — Uygulama Talimatı (Master Prompt)

Bu prompt, bir AI kodlama asistanına (örn. Claude Code) doğrudan verilmek üzere hazırlanmıştır. Ekte iki referans doküman/materyal daha bulunmaktadır:

1. `penalti-oyunu-prompt.md` — Oyunun tam tasarım dokümanı (mekanik, mod, zorluk, görsel stil, animasyon, ses tasarımı detayları).
2. `penalti-oyunu-fazli-prompt.md` — Yukarıdaki tasarımın ekran bazlı fazlara/modüllere bölünmüş hali.
3. **Referans görseller** (ekte ayrıca paylaşılacak) — Oyunun görsel atmosferinin, sahne kompozisyonunun ve piksel art tarzının nasıl hissettirmesi gerektiğine dair örnekler.

Aşağıdaki talimatları eksiksiz ve sırasıyla uygula.

---

## 1. İlk Adım: Teknoloji Sorusu (İŞE BAŞLAMADAN ÖNCE SOR)

Kod yazmaya veya proje kurmaya başlamadan **önce** bana şu soruyu sor:

> "Bu projeyi hangi teknoloji/motor ile geliştirmemi istersin? Şu anda **Unity (C#)** öneriliyor ve bende zaten kurulu — bu yönde ilerleyebiliriz. Ama sence bu proje için daha uygun, daha hızlı geliştirilebilir veya daha iyi sonuç verecek başka bir teknoloji/motor varsa (örneğin Godot, HTML5+Phaser, vb.), bunu gerekçeleriyle bana öner ve hangisini seçtiğimi bildirmemi bekle. Benim onayım olmadan varsayılan olarak Unity'e geçme."

Ben cevap vermeden **hiçbir proje dosyası, sahne veya kod oluşturma.** Bu, atlanabilecek bir adım değildir.

Teknoloji netleştikten sonra, o teknolojiye özgü proje kurulum adımlarını (Faz 0) uygula.

---

## 2. Fazları Sırasıyla ve Harfiyen Uygula

`penalti-oyunu-fazli-prompt.md` dosyasındaki fazları **sırasıyla, atlamadan** uygula (Faz 0 → Faz 1 → ... → Faz 8).

**Kritik kural — "harfiyen uygulama":**
- Her fazda yazan her madde (mekanik detayları, animasyon açıklamaları, ses tasarımı öğeleri, kapsam dışı bırakılan özellikler vb.) **birebir, atlanmadan veya basitleştirilmeden** uygulanacak.
- Özellikle **Faz 6 (Animasyonlar)** ve **Faz 7 (Ses Entegrasyonu)** için: dokümanda tarif edilen her animasyon (şut, kaleci dalışı, taraftar kutlaması) ve her ses efekti (bekleme ambiyansı, vuruş sesi, gol kutlaması, kurtarış hayal kırıklığı sesi) eksiksiz şekilde eklenecek. "Basit bir versiyon yeterli" gibi bir yaklaşımla eksik/sığ bir uygulama yapma — dokümanda ne yazıyorsa o seviyede detay hedeflenmeli.
- `penalti-oyunu-prompt.md` (ana tasarım dokümanı) ile `penalti-oyunu-fazli-prompt.md` (fazlı doküman) arasında bir çelişki görürsen, bana sorup netleştir; kendi başına yorumlayıp karar verme.
- Bölüm 10'daki (ana tasarım dokümanı) **"Kapsam Dışı"** listesi bağlayıcıdır — oradaki hiçbir özellik (multiplayer aktivasyonu, güç çubuğu, dinamik zorluk, ekstra şut sonucu çeşitliliği, gündüz/gece seçimi, ekstra options ayarı, oyuncu karakterinin görünmesi) hiçbir fazda "iyi bir fikir olur" diyerek eklenmeyecek.

**Faz onay akışı:**
- Her fazı bitirdiğinde:
  1. Hangi dosyaları/script'leri/sahneleri oluşturduğunu veya değiştirdiğini özetle.
  2. Faz dokümanındaki "Çıktı/Test kriteri"nin karşılanıp karşılanmadığını belirt.
  3. Bir sonraki faza geçmeden **önce benim onayımı bekle.** Onay almadan bir sonraki faza geçme.

---

## 3. Referans Görsellerin Kullanımı

Ekte paylaşacağım referans görseller, oyunun **görsel atmosferini, sahne kompozisyonunu ve piksel art hissini** anlaman için verilmektedir (Faz 5 — Görsel Varlıklar ve Sahne Kompozisyonu ile ilgili).

Bu görseller için şu kurallara uy:
- **Birebir kopya oluşturma.** Referans görsellerdeki spesifik sprite'ları, logoları veya varsa tanınabilir marka/lisanslı öğeleri doğrudan taklit etme veya yeniden üretme.
- Görsellerden **stil, renk paleti, kompozisyon mantığı, ışıklandırma hissi (floodlight/akşam atmosferi) ve piksel art detay seviyesi** gibi unsurları ilham al.
- Hedefin, referanslara **benzer ama özgün, hatta mümkünse daha kaliteli/cilalı** bir görsel sonuç üretmek. Yani referanslar bir "taban çizgi" (baseline) niteliğinde — onların altında kalmamalı, mümkünse görsel kalite ve detay olarak üstüne çıkmalısın.
- Eğer referans görsellerle `penalti-oyunu-prompt.md`'deki Bölüm 7 (Görsel Stil ve Sahne Kompozisyonu) arasında bir çelişki görürsen (örn. görsellerde gündüz sahne varken dokümanda gece isteniyor), **dokümandaki yazılı talimat önceliklidir** — bu durumda bana bildirip nasıl ilerlemek istediğimi sor.

---

## 4. Genel Çalışma Prensipleri

- Belirsiz veya dokümanlarda net olmayan bir konuyla karşılaşırsan, varsayım yapıp ilerlemek yerine **bana sor.**
- Kod/proje ilerledikçe karşılaştığın teknik kısıtlamaları (örn. "bu animasyon bu motorda şu şekilde daha iyi çalışır" gibi) benimle paylaş ve öneride bulun, ama dokümanın özünü (mekanik, kapsam, tasarım kararları) değiştirecek önerileri benim onayım olmadan uygulama.
- Her fazın sonunda kısa, teknik olmayan bir dille (ben geliştirici değilsem anlayabileceğim şekilde) özet ver; gerekirse ek teknik detayı isteğe bağlı olarak sonuna ekleyebilirsin.

---

## Özet Akış

1. Teknoloji sorusu sor → onay bekle.
2. Faz 0'dan başla, sırasıyla ilerle.
3. Her fazı dokümanlardaki her detayla birebir/harfiyen uygula.
4. Referans görselleri ilham kaynağı olarak kullan, birebir kopyalama, kalite hedefini referanslardan yukarı koy.
5. Her faz sonunda özet ver ve onay bekle.
6. Belirsizlik/çelişki durumunda varsayım yapma, sor.
