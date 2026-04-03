using UnityEngine;

public class StartPlatform : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Platformun aşağı düşme hızı")]
    [SerializeField] float dropSpeed = 10f;
    
    [Tooltip("Kaç saniye bekleyip düşmeye başlasın?")]
    [SerializeField] float startDelay = 2.5f; 

    [Tooltip("Hangi Y konumuna gelince yok olsun? (Ekranın altı)")]
    [SerializeField] float destroyY = -30f; // <-- YENİ: Yok olma sınırı

    void Update()
    {
        // 1. Bekleme Süresi
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        // 2. Aşağı Hareket
        transform.Translate(Vector3.down * dropSpeed * Time.deltaTime);

        // 3. YOK OLMA KONTROLÜ (Eksik olan kısım buydu)
        // Eğer platform -30'dan daha aşağıindiyse, kendini yok et.
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}