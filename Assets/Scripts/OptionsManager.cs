using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    [Header("Slider Bağlantıları")]
    public Slider sensitivitySlider;
    public Slider menuMusicSlider;
    public Slider gameMusicSlider;

    void Start()
    {
        // Ayarları yükle (Varsayılan değerleri ata)
        if(sensitivitySlider) sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1.5f);
        if(menuMusicSlider) menuMusicSlider.value = PlayerPrefs.GetFloat("MenuMusicVol", 0.5f);
        if(gameMusicSlider) gameMusicSlider.value = PlayerPrefs.GetFloat("GameMusicVol", 0.5f);
    }

    // --- HASSASİYET (Anlık Değişim) ---
    public void ChangeSensitivity(float newValue)
    {
        PlayerPrefs.SetFloat("Sensitivity", newValue);

        // Sahnede oyuncuyu bul ve hızını ANINDA değiştir
        // (PlayerMovement scriptinin adının doğru olduğundan emin ol)
        var player = FindFirstObjectByType<PlayerMovement>(); // Veya senin scriptinin adı neyse (BallController vs.)
        if (player != null)
        {
            player.sensitivity = newValue; // Scriptindeki değişkenin 'public' olması lazım!
        }
    }

    // --- OYUN MÜZİĞİ (Anlık Değişim) ---
    public void ChangeGameMusicVolume(float newValue)
    {
        PlayerPrefs.SetFloat("GameMusicVol", newValue);

        // Sahnede "GameMusic" adındaki objeyi bulup sesini ANINDA değiştir
        // DİKKAT: Oyun sahnesindeki müzik objenin adı "GameMusic" olmalı!
        GameObject gameMusicObj = GameObject.Find("GameManager"); 
        
        if (gameMusicObj != null)
        {
            gameMusicObj.GetComponent<AudioSource>().volume = newValue;
        }
    }

    // --- MENÜ MÜZİĞİ (Anlık Değişim) ---
    public void ChangeMenuMusicVolume(float newValue)
    {
        PlayerPrefs.SetFloat("MenuMusicVol", newValue);

        // Menü müziği objesini bul (Adı BackgroundMusic ise)
        GameObject menuMusicObj = GameObject.Find("BackgroundMusic");
        
        if (menuMusicObj != null)
        {
            menuMusicObj.GetComponent<AudioSource>().volume = newValue;
        }
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}