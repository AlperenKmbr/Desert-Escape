using UnityEngine;
using TMPro; // TextMeshPro'yu kullanmak için bu kütüphane ŞART!

public class ScoreManager : MonoBehaviour
{
    [Header("UI Bağlantısı")]
    [Tooltip("Canvas altındaki Text (TMP) objesini buraya sürükle")]
    [SerializeField] TextMeshProUGUI scoreText;

   public float _currentScore = 0f;
    private bool _isScoreRunning = true;

    void Start()
    {
        // Oyun başında skoru sıfırla
        _currentScore = 0f;
    }

    void Update()
    {
        // Oyun devam ettiği sürece skor artsın
        if (_isScoreRunning)
        {
            // Time.deltaTime kullanarak her saniye tam olarak 1 artmasını sağlıyoruz
            _currentScore += Time.deltaTime;

            // UI'ı güncelle
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // float olan süreyi tam sayıya (int) çevirip yazdırıyoruz.
            // Örn: 1.25 saniye -> Ekranda "1" yazar.
            int scoreInt = Mathf.FloorToInt(_currentScore);
            
            // Ekranda sadece sayı yazsın istiyorsan:
            scoreText.text = scoreInt.ToString();
            
            // Eğer "SCORE: 15" gibi yazsın istersen alt satırı aç, üsttekini sil:
            // scoreText.text = "SCORE: " + scoreInt.ToString();
        }
    }

    // İLERİDE KULLANMAN İÇİN:
    // Oyun bittiğinde (Game Over) bu fonksiyonu çağırıp skoru durdurabilirsin.
    public void StopScore()
    {
        _isScoreRunning = false;
        
        // İstersen burada High Score kaydetme işlemleri de yapılabilir.
        // PlayerPrefs.SetInt("HighScore", Mathf.FloorToInt(_currentScore));
    }
}