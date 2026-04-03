using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject pausePanel;
    public GameObject optionsPanel;

    // SAĞ ÜSTTEKİ PAUSE BUTONUNA BASINCA ÇALIŞACAK
    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Zamanı dondur! Oyun durur.
    }

    // RESUME BUTONU İÇİN
    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false); // Açıksa kapat
        Time.timeScale = 1f; // Zamanı normale döndür! Oyun devam eder.
    }

    // OPTIONS BUTONU İÇİN
    public void OpenOptions()
    {
        pausePanel.SetActive(false); // Pause menüsünü gizle
        optionsPanel.SetActive(true); // Ayarları aç
    }

    // OPTIONS İÇİNDEKİ "GERİ" BUTONU İÇİN
    public void CloseOptions()
    {
        optionsPanel.SetActive(false); // Ayarları gizle
        pausePanel.SetActive(true); // Pause menüsünü geri getir
    }

    // RESTART BUTONU İÇİN
    public void RestartGame()
    {
        Time.timeScale = 1f; // ÖNEMLİ: Sahne yenilenmeden önce zamanı düzelt!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    // MAIN MENU BUTONU İÇİN
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // ÖNEMLİ: Ana menüye donuk gitmemek için
        SceneManager.LoadScene("Main Menu");
    }
}