using System.Collections;
using UnityEngine;

public enum GameState
{
    Playing // Tek bir mod yeterli, oyun direkt başlar
}

public class GameManager : MonoBehaviour
{
    public static GameState CurrentState = GameState.Playing;

    void Start()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("GameMusicVol", 0.5f);
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        Debug.Log("Oyun Başladı: Direkt Akış.");
    }
}
