using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
  [Header("Ayarlar")]
    [Tooltip("Hassasiyet ayarı")]
    [SerializeField] public float sensitivity = 1.5f; 
    
    [Tooltip("Sağ/Sol sınır (Bunu duvar genişliğine göre ayarla)")]
    [SerializeField] float xClamp = 4.5f; 
    
    [Tooltip("Hareket yumuşaklığı")]
    [SerializeField] float smoothSpeed = 10f;

    [SerializeField] float tiltAmount = -25f;

    private Rigidbody2D _rb;
    private Vector3 _lastMousePos;
    private float _targetX;
    private bool _isTouching = false;

    void Start()
    {
       _rb = GetComponent<Rigidbody2D>();
        _targetX = transform.position.x;
        
        // --- YENİ EKLENEN KISIM ---
        // Kaydedilmiş "Sensitivity" ayarını çek. Eğer daha önce hiç ayar yapılmadıysa varsayılan olarak 1.5f yap.
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.5f);
        
    }

    void Update()
    {
        // Girdileri (Parmak hareketini) Update'te alıyoruz
        HandleInput();
    }

    void FixedUpdate()
    {
        // Fiziksel hareketi FixedUpdate'te yapıyoruz (Duvara girmemek için şart!)
        ApplyMovement();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePos = Input.mousePosition;
            _isTouching = true;
            
            // ÖNEMLİ: Parmağı yeni koyduğumuzda, hedefi karakterin şu anki gercek konumuna eşitle.
            // (Eğer duvara takıldıysak hedef duvarın içinde kalmış olabilir, bunu sıfırlıyoruz)
            _targetX = transform.position.x;
        }
        else if (Input.GetMouseButton(0) && _isTouching)
        {
            Vector3 currentMousePos = Input.mousePosition;
            float moveDelta = currentMousePos.x - _lastMousePos.x;

            float moveAmount = (moveDelta / Screen.width) * sensitivity * 10f;
            
            _targetX += moveAmount;
            _targetX = Mathf.Clamp(_targetX, -xClamp, xClamp);

            _lastMousePos = currentMousePos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isTouching = false;
        }
    }

    void ApplyMovement()
    {
        // 1. Hedefe doğru yumuşak bir değer hesapla
        float newX = Mathf.Lerp(_rb.position.x, _targetX, smoothSpeed * Time.fixedDeltaTime);

        // 2. Mevcut Y pozisyonunu koru (Veya aşağı iniyorsa onu da buraya ekleyebilirsin)
        Vector2 targetPos = new Vector2(newX, _rb.position.y);

        // 3. FİZİK MOTORUYLA HAREKET ET
        // MovePosition: "Oraya gitmeye çalış, ama yolunda duvar varsa DUR." demektir.
        _rb.MovePosition(targetPos);

        // Görsel eğim (Tilt) efekti
        float tilt = (newX - transform.position.x) * tiltAmount;
        transform.rotation = Quaternion.Euler(0, 0, tilt);
    }
}