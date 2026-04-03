using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentBackground : MonoBehaviour
{
    private static PersistentBackground instance;

    void Awake()
    {
        // Eğer sahnede zaten bir arka plan varsa (Örn: Options'tan Main Menu'ye dönünce), yenisini YOK ET.
        // Bu sayede eski arka plan hiç kesilmeden akmaya devam eder.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Sahneler arası geçerken beni silme!
    }

    // --- OYUNA GİRİNCE ARKA PLANI SİLME KISMI ---
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // "Game" yazan yere asıl oyun sahnenin adını yazmalısın.
        // Oyuna girince bu menü arka planı yok olur, oyunun kendi arka planı başlar.
        if (scene.name == "Game") 
        {
            Destroy(gameObject);
        }
    }
}