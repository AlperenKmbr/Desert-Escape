using UnityEngine;
using UnityEngine.SceneManagement; // Sahne isimlerini okumak için ekledik

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Start()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MenuMusicVol", 0.5f);
    }
    void Awake()
    {
        // Çift ses çıkmasını engelle
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Menüler arası gezerken silinme
    }

    // --- YENİ EKLENEN KISIM BAŞLANGICI ---

    void OnEnable()
    {
        // Sahne değiştiğinde bana haber ver diyoruz
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Obje silinirken takibi bırak
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Sahne her yüklendiğinde bu fonksiyon otomatik çalışır
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Eğer yüklenen sahnenin adı "Game" ise (Buraya kendi oyun sahnenin adını tam yazmalısın)
        if (scene.name == "Game") 
        {
            // Menü müziğini yok et! (Böylece oyun müziğine yer açılır)
            Destroy(gameObject);
        }
    }
    
   
}