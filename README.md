Sonsuz koşu oyunu readme

Unity ile geliştirilmiş 3 şeritli 3D sonsuz koşu oyunudur. Oyuncu hayatta kaldıkça ve puan topladıkça hızlanan, dinamik engellere ve toplanabilir eşyalara sahip bir yapıya sahiptir. Yüksek performans için sonsuz yol üretiminde mantığı kullanılmıştır.

-Kontroller

A / D veya Sol / Sağ Ok: Şerit değiştirme

W / Yukarı Ok / Boşluk: Zıplama

S / Aşağı Ok: Eğilme (Havada basılırsa hızlı düşüş özelliği var)

-Oyun Mekanikleri ve Özellikler

Sonsuz Yol Üretimi: Oyun, bilgisayarı yormamak için yeni yollar oluşturup silmek yerine, arkada kalan yolları alıp oyuncunun önüne yerleştirir.

Gelişmiş Engel Fiziği:

-Küçük engellere çarpmak 1 can götürür.

-Büyük küplerin üstüne zıplayarak koşmaya devam edilebilir. Ancak küplere tam önden çarpmak öldürür.

-Büyük engellere yandan çarpıldığında karakter 1 can kaybeder, kısa bir ölümsüzlük kazanır ve otomatik olarak geldiği şeride geri seker.

Dinamik Hızlanma: Puan 1.000'i geçtiğinde ilk hızlanma başlar. Daha sonra her 10.000 puanda bir karakterin koşu hızı kademeli olarak artarak zorluk seviyesini yükseltir.

Can Sistemi: Oyuncunun toplam 3 canı vardır. Hasar alındığında karakter kısa bir süreliğine yanıp sönerek hasar almazlık kazanır.

Skor ve Toplanabilirler

-Hayatta kalınan her saniye otomatik olarak skor artar.

-Altın: +10 Puan

-Elmas: +100 Puan

Son not:Border(hitbox) özelliğini büyük kutulara entegre edemediğim için kodda ön yüzün alt kısımdan %90 lık kısmına çarpınca ölecek şekilde yaptım. Böylece üstünde yürüyebiliyor.)

Gereksiz dosya boyutu artmaması için herhangi bir asset kullanmadım ama nasıl ekleneceğini araştırdım.

İlk defa unity kullandığım için hatalar olabilir. Sadece bağlangıç kısımlarını öğrenebildim.
  


