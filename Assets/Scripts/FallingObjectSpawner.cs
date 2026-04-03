using System.Collections;
using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] GameObject fallingObjectPrefab;
    [SerializeField] float spawnRate = 2.0f; // Kutu atma hızı
    [SerializeField] float startDelay = 2.0f; // Mod açılınca İLK bekleme süresi

    // Pozisyon ayarları
    [SerializeField] float spawnY = 15f;
    [SerializeField] float spawnWidthX = 3.5f;

    void Start()
    {
        // Oyuna başlar başlamaz bu döngüyü başlat
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Sonsuz döngü
        while (true)
        {
            // 1. ADIM: Mod "Falling" olana kadar bekle.
            // (Snake veya Spike modundaysak kod burada donar kalır, işlemciyi yormaz)
            yield return new WaitUntil(() => MeshWallManager.IsFallingModeActive);

            // 2. ADIM: Mod açıldı! Ama hemen kutu atma.
            // Senin istediğin o "Gecikme" burası.
            yield return new WaitForSeconds(startDelay);

            // 3. ADIM: Mod aktif olduğu sürece kutu atmaya devam et
            while (MeshWallManager.IsFallingModeActive)
            {
                SpawnObject();
                
                // Bir sonraki kutu için bekle
                yield return new WaitForSeconds(1f / spawnRate);
            }

            // Mod kapandığı an (Snake'e geçince) içteki döngü biter,
            // başa döner ve tekrar 1. ADIM'da beklemeye başlar.
        }
    }

    void SpawnObject()
    {
        if (fallingObjectPrefab == null) return;

        float randomX = Random.Range(-spawnWidthX, spawnWidthX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0);

        GameObject newRock = Instantiate(fallingObjectPrefab, spawnPos, Quaternion.identity);
        
        float randomSize = Random.Range(0.8f, 1.3f);
        newRock.transform.localScale = Vector3.one * randomSize;
    }
}