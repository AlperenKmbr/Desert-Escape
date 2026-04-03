using UnityEngine;

public class FallingObjectController : MonoBehaviour
{
    [Tooltip("Kutu ekranın ne kadar altına inince yok olsun?")]
    [SerializeField] float destroyY = -25f;

    void Update()
    {
        // 1. YÜKSEKLİK KONTROLÜ
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObj = collision.gameObject;

        // A) OYUNCUYA ÇARPARSA -> Yok ol ve (varsa) hasar ver
        if (hitObj.CompareTag("Player"))
        {
            // Buraya ilerde can azaltma kodu eklenebilir:
            // hitObj.GetComponent<Health>().TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        // B) BAŞKA BİR KUTUYA ÇARPARSA -> Seksin (Yok olma)
        if (hitObj.CompareTag("FallingObject"))
        {
            return;
        }

        // C) DUVARA ÇARPARSA
        // (Duvar objelerinin Tag'inin "Untagged" veya "Wall" olduğundan emin ol)
        // Eğer duvarların özel bir Tag'i yoksa bu kontrol çalışır, varsa CompareTag("Wall") kullan.
        
        // Şimdilik "Player" ve "FallingObject" değilse DUVAR varsayıyoruz:
        bool isWall = !hitObj.CompareTag("Player") && !hitObj.CompareTag("FallingObject");

        if (isWall)
        {
            // Eğer oyun şu an "FallingObjects" (Geniş Alan) modundaysa
            if (MeshWallManager.IsFallingModeActive)
            {
                // Duvara çarpsa bile yok olmasın, sekip devam etsin.
                // Fizik materyali (Bounciness) varsa sekecektir.
                return; 
            }
            else
            {
                // Ama eğer Yılan veya Engel moduna geçtiysek
                // Yolu tıkamamak için duvara değdiği an patlasın.
                Destroy(gameObject);
            }
        }
    }
}