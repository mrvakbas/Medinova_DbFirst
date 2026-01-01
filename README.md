# 🏥 Medinova - Akıllı Hastane Yönetim Sistemi

**Medinova**, hasta, doktor ve hastane yönetimi arasındaki süreçleri dijitalleştiren; **Yapay Zeka (Gemini)** ve **Makine Öğrenmesi (ML.NET)** teknolojileriyle güçlendirilmiş kapsamlı bir Hastane Yönetim Sistemi'dir.

Bu proje, **.NET Framework** altyapısı kullanılarak **Database First** yaklaşımı ile geliştirilmiştir. Klasik hastane yönetim fonksiyonlarının ötesine geçerek, hastalara semptom analizi yapan bir AI asistanı sunar ve yöneticiler için geleceğe yönelik randevu doluluk tahminlerinde bulunur.

---

## 🚀 Proje Hakkında

Medinova, modern sağlık ihtiyaçlarına cevap vermek üzere tasarlanmıştır. Sadece randevu almakla kalmaz, yönetimsel kararları desteklemek için veri analitiği ve yapay zeka kullanır.

### 🎯 Temel Özellikler

* **3 Farklı Kullanıcı Paneli:**
    * 👨‍💼 **Admin Paneli:** Tüm sistem yönetimi, doktor/bölüm ekleme, log takibi ve istatistikler.
    * 👨‍⚕️ **Doktor Paneli:** Randevu takibi, hasta geçmişi görüntüleme.
    * 👥 **Hasta Paneli:** Randevu alma, geçmiş randevular ve AI sağlık asistanı erişimi.
* **🤖 Gemini AI Entegrasyonu:** Hastaların şikayetlerini dinleyen, analiz eden ve ilgili bölümlere yönlendirme tavsiyesi veren yapay zeka destekli "AI Sağlık Asistanı".
* **📈 ML.NET ile Tahminleme (SSA):** Geçmiş randevu verilerini analiz ederek (Time Series Forecasting), gelecek dönemdeki randevu yoğunluğunu tahmin eden Makine Öğrenmesi modülü.
* **📊 Gelişmiş Dashboard:** Anlık istatistikler, grafiksel veriler ve trend analizleri.
* **📝 Loglama Sistemi:** Sisteme giriş-çıkışlar, ekleme ve silme işlemlerinin detaylı log kaydı.
* **📅 Randevu Sistemi:** Bölüm ve doktor seçimi ile kolay randevu oluşturma arayüzü.

---

## 🛠 Teknolojiler ve Mimari

Bu projede kullanılan temel teknolojiler ve kütüphaneler:

| Alan | Teknoloji |
| :--- | :--- |
| **Backend** | .NET Framework (C#) |
| **Veri Tabanı Yaklaşımı** | Entity Framework (Database First) |
| **Veri Tabanı** | MS SQL Server |
| **Yapay Zeka (LLM)** | Google Gemini AI API |
| **Makine Öğrenmesi** | Microsoft ML.NET (Time Series Forecasting - SSA) |
| **Frontend** | HTML5, CSS3, Bootstrap, JavaScript/jQuery |
| **Grafikler** | Chart.js |

---

## 📸 Proje Görselleri

### 1. Randevu Oluşturma
Kullanıcı dostu arayüz ile hızlı ve kolay randevu alma süreci.
<img width="1600" height="731" alt="1 2" src="https://github.com/user-attachments/assets/fd4dcb7e-0ce7-4f4f-af02-36570174b8e2" />

### 2. AI Sağlık Asistanı (Gemini Entegrasyonu)
Hastaların semptomlarına göre ön bilgi alabildiği, doğal dil işleme destekli akıllı sohbet arayüzü.
<img width="1600" height="731" alt="2defaultAI" src="https://github.com/user-attachments/assets/c1c475ba-3c8d-4a4e-9234-cbb5ae1a331f" />
<img width="1591" height="726" alt="3AIBekleme" src="https://github.com/user-attachments/assets/26a6d1d1-6c49-426e-b067-e376428fc3f0" />
<img width="1592" height="729" alt="4AsistanCevap" src="https://github.com/user-attachments/assets/f95dbf5e-19d7-415b-ad0b-582cb7e4fa5a" />

### 3. Admin Dashboard ve ML.NET Tahminleri
Yöneticiler için sisteme genel bakış. Alt kısımda ML.NET ile oluşturulan gelecek dönem randevu tahmin grafiği yer almaktadır.
<img width="1600" height="2661" alt="7Admin Dashboard" src="https://github.com/user-attachments/assets/2164919d-c2f8-4429-a2ef-767c77317d3a" />
<img width="1600" height="731" alt="8Admin_Testimonial" src="https://github.com/user-attachments/assets/e871c511-cede-40fc-9442-d6a4a88536f7" />

### 4. Sistem Logları
Güvenlik ve takip amacıyla; giriş/çıkış, ekleme ve silme işlemlerinin tutulduğu log ekranı.
<img width="1600" height="1156" alt="8 Loglama" src="https://github.com/user-attachments/assets/dd47511a-fa96-4272-890b-fe68a1fa0526" />

### 5.Doktor ve Hasta Panelleri
Sistemdeki doktorların ve hastaların ayrı ayrı panelleri
<img width="1600" height="862" alt="9Doctor paneli" src="https://github.com/user-attachments/assets/71b85d6a-9b8b-489e-bc57-269b2fcf1810" />
<img width="1600" height="731" alt="10Hasta_Panel," src="https://github.com/user-attachments/assets/f0f56cbb-490d-4d7c-8c63-926e7d29de7e" />

### 6.Login ve Yetkisiz Erişim Sayfaları
Giriş Sayfası ve 403(Yetkisi Olmayan) Sayfası
<img width="1600" height="731" alt="6loginSayfası" src="https://github.com/user-attachments/assets/ece6cce9-2b28-4429-a0ff-d8e3ef726b39" />
<img width="1600" height="731" alt="11Authorize" src="https://github.com/user-attachments/assets/0fbdb208-42a7-425a-8e34-f46eeeb3f838" />

### 👨‍💻 Geliştirici

**[Merve AKBAŞ]**
* LinkedIn: [https://www.linkedin.com/in/mrvakbass/]
* GitHub: [https://github.com/mrvakbas]
