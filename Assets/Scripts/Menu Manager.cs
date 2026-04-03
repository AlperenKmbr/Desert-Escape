using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    // 1. PLAY BUTONU
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // Oyun sahnenin adı
    }

    // 2. OPTIONS BUTONU (Ayrı Sahneye Geçiş)
    public void OpenOptions()
    {
        // Ayarlar için oluşturduğun sahnenin adını buraya yazıyorsun
        SceneManager.LoadScene("Options"); 
    }

    // 3. QUIT BUTONU
    public void QuitGame()
    {
        
        Application.Quit(); 
    }
}