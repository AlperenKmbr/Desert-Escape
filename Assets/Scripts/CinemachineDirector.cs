using UnityEngine;
using Unity.Cinemachine;

public class CinemachineDirector : MonoBehaviour
{
    [Header("Kameralar")]
    [SerializeField] CinemachineCamera startCam; 
    [SerializeField] CinemachineCamera gameCam;

    [Header("Zamanlama")]
    [Tooltip("Oyun başında kaç saniye beklesin? (Örn: 2.0)")]
    [SerializeField] float initialWaitTime = 2.5f; // <-- YENİ: Bekleme süresi

    void Start()
    {
        // Başlangıç kamerası aktif
        startCam.Priority = 20;
        gameCam.Priority = 10;

        // Belirlenen süre kadar bekle, sonra geçiş yap
        Invoke("SwitchToGameplay", initialWaitTime);
    }

    void SwitchToGameplay()
    {
        gameCam.Priority = 30;
    }
}