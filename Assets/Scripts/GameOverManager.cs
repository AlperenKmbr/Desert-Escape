using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Bağlantıları")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    
    [Header("Skor Bağlantısı")]
    public ScoreManager scoreManager;

    public void ShowGameOver()
    {
        // 1. Paneli Görünür Yap
        gameOverPanel.SetActive(true);

        // 2. Skoru Durdur ve Ekrana Yazdır
        if (scoreManager != null)
        {
            scoreManager.StopScore(); // ScoreManager'daki durdurma fonksiyonu
            int finalScore = Mathf.FloorToInt(scoreManager._currentScore);
            finalScoreText.text = "SCORE: " + finalScore.ToString();
        }

        // 3. OYUNU DONDUR (Zamanı durdurur, her şey hareketsiz kalır)
        Time.timeScale = 0f; 
    }

    // RESTART BUTONU İÇİN
    public void RestartGame()
    {
        Time.timeScale = 1f; // Zamanı normale döndür (Çok önemli!)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Mevcut sahneyi baştan yükle
    }

    // MAIN MENU BUTONU İÇİN
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene("Main Menu"); // Ana menü sahnenin adını tam yaz
    }
}