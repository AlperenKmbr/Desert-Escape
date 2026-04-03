using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] int health = 50;
    [SerializeField] bool isPlayer;

    [Header("Ölümsüzlük Ayarı")]
    [Tooltip("Hasar aldıktan sonra kaç saniye hasar işlemesin?")]
    [SerializeField] float immunityTime = 2.0f; // 2 Saniye ölümsüzlük
    
    private bool isImmune = false; // Şu an ölümsüz müyüz?

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Eğer zaten ölümsüzsek, hiç kontrol etme bile
        if (isImmune) return;

        // Çarptığımız objede 'Damage' scripti var mı?
        if (collision.gameObject.TryGetComponent(out Damage damageDealer)) 
        {
            TakeDamage(damageDealer.GetDamage());
            Debug.Log(gameObject.name + " duvara çarptı! Kalan Can: " + health);
        }
    }

    public int GetHealth() => health;

    void TakeDamage(int damageValue)
    {
        // Canı azalt
        health -= damageValue;
        
        if (health <= 0)
        {
            OnCharacterDeath();
        }
        else
        {
            // Ölmediysek geçici ölümsüzlüğü başlat
            StartCoroutine(ImmunityRoutine());
        }
    }

    // Ölümsüzlük Sayacı
    IEnumerator ImmunityRoutine()
    {
        isImmune = true; // Korumayı aç
        Debug.Log("🛡️ Kalkan Aktif! (2 Saniye)");

        yield return new WaitForSeconds(immunityTime); // 2 saniye bekle

        isImmune = false; // Korumayı kapat
        Debug.Log("❌ Kalkan Bitti.");
    }

    
   void OnCharacterDeath()
    {
        if (isPlayer)
        {
            Debug.Log("☠️ OYUN BİTTİ - Karakter Öldü");

            // --- YENİ EKLENEN KISIM ---
            // Sahnede GameOverManager scriptini bul ve ekranı göster!
            GameOverManager gameOver =FindFirstObjectByType<GameOverManager>();
            if (gameOver != null)
            {
                gameOver.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("Sahnede GameOverManager bulunamadı!");
            }
            // --------------------------

            Destroy(gameObject); 
        }
    }
}