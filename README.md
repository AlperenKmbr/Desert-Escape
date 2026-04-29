#  Desert Escape
### Prosedürel Mesh Üretimi ve Dinamik Zorluk Tabanlı Hayatta Kalma Oyunu

![Unity Version](https://img.shields.io/badge/Unity-2022.3%2B-blue?logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-green?logo=c-sharp)
![Architecture](https://img.shields.io/badge/Architecture-Modular_State_Machine-orange)

**Desert Escape**, oyuncunun reflekslerini ve hayatta kalma becerilerini test eden; organik mağara yapılarını matematiksel olarak üreten (Perlin Noise) ve dinamik bir zorluk eğrisine sahip 2D survival oyunudur.

---

## ⚙️ Teknik Sistem Analizi ve Mantık (Core Systems)

### ⛰️ Prosedürel Dünya Üretimi (Mesh Generation)
Oyunun "beyni" olan `MeshWallManager`, dünyayı statik objelerle değil, çalışma zamanında (Runtime) oluşturulan **Dynamic Mesh** yapılarıyla kurgular.
* **Perlin Noise Mantığı:** Duvarların "organik" ve pürüzlü görünmesi için Perlin Noise fonksiyonu kullanılmıştır.
* **Dinamik Collider :** Mesh değiştikçe `PolygonCollider2D` verileri her karede güncellenerek fiziksel doğruluğu sağlar.

### 🌌 Parallax ve Arka Plan Yönetimi
Oyun atmosferini güçlendirmek amacıyla optimize edilmiş bir arka plan sistemi kullanılmıştır.
* **UV Offset Kaydırma:** `BackgroundLoop` sistemi, `SetTextureOffset` metodunu kullanarak arka plan dokusunu (texture) matematiksel bir döngüye sokar.
* **Sonsuz Derinlik Algısı:** Zaman bazlı (Time.deltaTime) koordinat kaydırma yöntemiyle, minimum bellek tüketimiyle sonsuz bir hareket ve derinlik hissi (Parallax Effect) oluşturulur.

### 🎮 Fizik Tabanlı Karakter Kontrolü
Karakter hareketi, basit bir pozisyon değişikliğinden ötesine geçerek fizik motoruyla entegre çalışır.
* **Rigidbody2D MovePosition:** Karakterin duvarların içinden geçmesini engelleyen, engellere çarptığında durmasını sağlayan fizik tabanlı bir hareket sistemi kullanılmıştır.
* **Görsel Geri Bildirim (Tilt):** Karakter sağa sola hareket ederken hareket hızına bağlı olarak görsel bir eğim (tilt) efekti kazanır.

---

## 🕹️ Oyun Modları ve Algoritmik Akış
Sistem, `GameStage` enum yapısı ile üç farklı fazı yönetir:

1.  **🧱 Obstacle Mode:** Orta genişlikte, pürüzlü ve "Spike" (sivri uç) içeren organik duvar yapısı.
2.  **🐍 Snake Mode:** Çok dar bir koridorda yüksek hız ve hassasiyet gerektiren kıvrımlı yol yapısı.
3.  **☄️ Falling Objects Mode:** Duvarların genişlediği ve yukarıdan rastgele boyutlarda objelerin yağdığı kaos modu. Bu modda spawner, mod aktifleşene kadar `WaitUntil` ile uyku modunda kalır.

---

## 🛠️ Mimari Özellikler

* **Singleton & Persistence:** Menü müziği ve arka plan, `DontDestroyOnLoad` ile sahneler arası korunurken, oyun sahnesine girişte kendilerini otomatik imha ederler.
* **Bellek Yönetimi (Memory Management):** Ekrandan çıkan tüm platformlar ve düşen nesneler belli bir koordinatın altına indiklerinde `Destroy` edilerek bellek şişmesi önlenir.
* **Kalıcı Ayarlar (PlayerPrefs):** Hassasiyet ve ses seviyeleri `PlayerPrefs` ile kaydedilir ve sahne geçişlerinde anında uygulanır.

---

## 📋 Veri ve Skor Mantığı
* **Time-Based Score:** Skor, `Time.deltaTime` üzerinden saniye bazlı hesaplanır ve UI güncellemeleri performansı korumak adına `UpdateScoreUI` içinde tamsayıya yuvarlanarak (Mathf.FloorToInt) yansıtılır.
* **Hasar & Ölümsüzlük:** Karakter hasar aldığında `ImmunityRoutine` (Coroutines) devreye girer ve oyuncuya 2 saniyelik geçici bir ölümsüzlük kalkanı tanır.
 
## 🖼️ Oyun Galerisi 



<div style="display: flex; flex-wrap: wrap; justify-content: center; gap: 10px; margin-bottom: 20px;">
    <img src="https://github.com/user-attachments/assets/7bb9d02a-d24d-4328-9bfa-6c0133bd358e" width="220" alt="Karakter Etkileşimi" style="margin: 5px;" />
    <img src="https://github.com/user-attachments/assets/b2770676-2e71-4cb3-bb92-9a9ea706fbc6" width="220" alt="Unity Editor Geliştirme Ortamı" style="margin: 5px;" />
    <img src="https://github.com/user-attachments/assets/6b479aee-2240-42e0-a693-e2a234ecd0bb" width="220" alt="Çöl Atmosferi Uzak Çekim" style="margin: 5px;" />
</div>

<div style="display: flex; flex-wrap: wrap; justify-content: center; gap: 10px; margin-bottom: 20px;">
    <img src="https://github.com/user-attachments/assets/b3e9e8dd-c3db-4b0f-b26f-a8074186c0bc" width="220" alt="Yapılandırma ve Mekanik Ayarlar" style="margin: 5px;" />
    <img src="https://github.com/user-attachments/assets/1a5f93f5-a981-44c6-848f-4bc825ccfd26" width="220" alt="Low-Poly Karakter Modeli" style="margin: 5px;" />
    <img src="https://github.com/user-attachments/assets/9e28e7c3-bb99-4ee4-af94-0902a47330fb" width="220" alt="Proje Hiyerarşisi ve Düzen" style="margin: 5px;" />
</div>

<div style="display: flex; justify-content: center; margin-top: 20px;">
    <img src="https://github.com/user-attachments/assets/3c924c3a-80ea-4cc7-8bdd-e1bd4391fe29" width="460" alt="Geniş Açılı Ekosistem Görünümü" />
</div>

<p style="text-align: center; font-style: italic;">

</p>




**Geliştirici:** [Alperen Kamber](https://github.com/AlperenKmbr)  
**Eğitim Kaynağı:** Udemy - Unity 2D Course
---

**Geliştirici:** [Alperen Kamber](https://github.com/AlperenKmbr)  
**Teknoloji:** Unity 2022.3+, C#, 2D Physics, Procedural Geometry
