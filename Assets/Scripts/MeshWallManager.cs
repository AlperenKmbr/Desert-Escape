using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStage1
{
    Obstacle, Snake, FallingObjects
}

public class MeshWallManager : MonoBehaviour
{
    // ========================================================================
    // AYARLAR
    // ========================================================================

    [Header("Kritik Ayar")]
    [SerializeField] float referenceCameraSize = 10f; 

    [Header("Zamanlama")]
    [SerializeField] float startDelay = 2.5f; 

    [Header("Falling Mode Gecikmesi")]
    [SerializeField] float fallingModeStartDelay = 1.0f; 

    [Header("--- ORGANİK MAĞARA AYARLARI ---")]
    [Tooltip("Duvar yüzeyi ne kadar pürüzlü olsun?")]
    [SerializeField] float caveRoughness = 1.5f; 
    [Tooltip("Duvar pürüzlerinin sıklığı")]
    [SerializeField] float caveNoiseFrequency = 0.15f; 
    [Tooltip("Yolun kıvrılma şiddeti")]
    [SerializeField] float pathWindingAmount = 3.0f; 
    [Tooltip("Spike'lar yuvarlak mı sivri mi?")]
    [SerializeField] bool roundSpikes = true;

    [Tooltip("Duvarlar birbirine EN FAZLA ne kadar yaklaşabilir? (Güvenlik Kilidi)")]
    [SerializeField] float minPlayableWidth = 4.0f; 

    [Header("Mesh References")]
    [SerializeField] MeshFilter leftWallFilter;
    [SerializeField] MeshFilter rightWallFilter;
    
    // --- TEXTURE AYARLARI GERİ GELDİ (World Space İçin) ---
    [Header("Texture Ayarları")]
    [Tooltip("Texture'ın tekrar etme sıklığı. X=0.5, Y=0.5 deneyebilirsin.")]
    [SerializeField] Vector2 textureTiling = new Vector2(0.5f, 0.5f); 
    // -----------------------------------------------------
    
    [SerializeField] PolygonCollider2D leftWallCollider;
    [SerializeField] PolygonCollider2D rightWallCollider;
    [SerializeField] float wallThickness = 25f; 

    [Header("Movement")]
    [SerializeField] float spawnPosY = 100f; 
    [SerializeField] float scrollSpeed = 8f; 
    [SerializeField] float destroyPosY = -25f; 
    [SerializeField] float segmentResolution = 0.25f;

    [Header("Stage Settings")]
    [Range(0f, 1f)] [SerializeField] float obstacleWidthPercent = 0.5f;
    [Range(0f, 1f)] [SerializeField] float snakeWidthPercent = 0.25f;
    [Range(0f, 1f)] [SerializeField] float wideWidthPercent = 0.85f;

    [Header("Intro")]
    [SerializeField] float startNarrowingSpeed = 0.8f; 
    [SerializeField] float startWidthMultiplier = 3.5f; 
    [SerializeField] float topNarrowingAmount = 60f; 
    [SerializeField] float entranceOpenSpeed = 0.3f; 

    [Header("Spike Settings")]
    [SerializeField] float firstSpikeDelay = 3.0f; 
    [Range(0f, 1f)] [SerializeField] float bigSpikeChance = 0.05f;
    [SerializeField] Vector2 bigSpikeLengthRange = new Vector2(4f, 8f);
    [SerializeField] float minSpikeGap = 3.0f;
    [SerializeField] float initialSafeDistance = 50f;

    [Header("Difficulty")]
    [SerializeField] float widthChangeSpeed = 2.0f;
    [SerializeField] float maxPathOffset = 2.5f;
    [SerializeField] float speedIncreaseRate = 0.1f;
    [SerializeField] float maxScrollSpeed = 15f;
    
    public float currentTurnSpeed = 2.0f; 
    public float maxTurnSpeed = 8.0f;     
    public float turnSpeedIncreaseRate = 0.1f; 

    public static bool IsFallingModeActive = false;
    
    Camera _mainCam;
    float _halfScreenWidth;
    float _currentPathCenter = 0f;
    float _targetPathCenter = 0f;
    float _currentPathWidth;
    float _targetPathWidth;
    float _entranceEffectMultiplier = 1.0f;
    float _firstSpikeTimer; 

    GameStage1 _currentStage;
    float _stageTimer = 10f; 
    bool _isFirstSnakeFrame = false;
    
    bool _isSpiking = false;
    int _spikeSide = 0;
    float _currentSpikeProgress = 0f;
    float _activeSpikeTotalLength = 0f;
    int _lastSpikeSide = 0;
    float _spikeCooldownDistance = 0f;

    // UV Listeleri Geri Geldi
    List<Vector3> _lVerts = new List<Vector3>();
    List<Vector2> _lUVs = new List<Vector2>(); // <---
    List<Vector3> _rVerts = new List<Vector3>();
    List<Vector2> _rUVs = new List<Vector2>(); // <---

    Mesh _leftMesh, _rightMesh;

    void Start()
    {
        _mainCam = Camera.main;
        UpdateDynamicAreaFixed(); 

        _leftMesh = new Mesh { name = "LeftMeshDynamic" }; _leftMesh.MarkDynamic();
        _rightMesh = new Mesh { name = "RightMeshDynamic" }; _rightMesh.MarkDynamic();
        
        if (leftWallFilter != null) leftWallFilter.mesh = _leftMesh;
        if (rightWallFilter != null) rightWallFilter.mesh = _rightMesh;

        SetStage(GameStage1.Obstacle);

        _targetPathWidth = _halfScreenWidth * 2f * obstacleWidthPercent;
        _currentPathWidth = _halfScreenWidth * startWidthMultiplier;
        _spikeCooldownDistance = initialSafeDistance;
        _entranceEffectMultiplier = 1.0f;
        _firstSpikeTimer = firstSpikeDelay; 

        PreFillScreen();
    }

    void FixedUpdate()
    {
        if (startDelay > 0)
        {
            startDelay -= Time.fixedDeltaTime;
            UpdateMeshes(); return; 
        }

        if (_firstSpikeTimer > 0) _firstSpikeTimer -= Time.fixedDeltaTime;

        if (_entranceEffectMultiplier > 0f)
        {
            _entranceEffectMultiplier -= Time.fixedDeltaTime * entranceOpenSpeed;
            if (_entranceEffectMultiplier < 0f) _entranceEffectMultiplier = 0f;
        }

        transform.position = new Vector3(0, 0, -1f); 
        MoveVerticesDown();

        float topMostY = (_lVerts.Count > 0) ? _lVerts[_lVerts.Count - 1].y : spawnPosY - segmentResolution;
        
        int safetyLoop = 0; 
        while (topMostY < spawnPosY && safetyLoop < 100)
        {
            float nextSpawnY = topMostY + segmentResolution;
            SpawnMeshSegment(nextSpawnY);
            topMostY = nextSpawnY; 
            safetyLoop++;
        }

        _currentPathWidth = Mathf.Lerp(_currentPathWidth, _targetPathWidth, startNarrowingSpeed * Time.fixedDeltaTime);
        RemoveOldVertices();
        UpdateMeshes(); 
        HandleDifficulty();
    }

    void SpawnMeshSegment(float y)
    {
        // 1. GENİŞLİK
        if (Mathf.Abs(_currentPathWidth - _targetPathWidth) < 0.5f)
        {
            if (_currentStage == GameStage1.Snake && _isFirstSnakeFrame)
            {
                _currentPathWidth = _targetPathWidth; _currentPathCenter = 0f; _targetPathCenter = 0f; _isFirstSnakeFrame = false;
            }
            else _currentPathWidth = Mathf.Lerp(_currentPathWidth, _targetPathWidth, widthChangeSpeed * Time.fixedDeltaTime);
        }

        // 2. KIVRIM
        float windingOffset = 0f;
        if (_currentStage == GameStage1.Obstacle)
        {
            windingOffset = Mathf.Sin(y * 0.1f) * pathWindingAmount;
        }

        if (_currentStage != GameStage1.FallingObjects)
        {
             float smooth = Mathf.Min(currentTurnSpeed * Time.fixedDeltaTime, 0.15f);
             _currentPathCenter = Mathf.Lerp(_currentPathCenter, _targetPathCenter, smooth);
             if (Mathf.Abs(_currentPathCenter - _targetPathCenter) < 0.2f) PickNewPath();
        }
        else _currentPathCenter = Mathf.Lerp(_currentPathCenter, 0f, Time.fixedDeltaTime);

        float finalCenter = _currentPathCenter + windingOffset;

        // 3. GİRİŞ
        float finalWidth = _currentPathWidth;
        if (y > 20f)
        {
            float hFactor = Mathf.Clamp01((y - 20f) / 70f);
            finalWidth -= topNarrowingAmount * (hFactor * hFactor) * _entranceEffectMultiplier;
            if (finalWidth < 3f) finalWidth = 3f;
        }

        // 4. DUVAR GEOMETRİSİ
        if (_firstSpikeTimer <= 0) 
        {
            if (_spikeCooldownDistance > 0) _spikeCooldownDistance -= segmentResolution;
            
            float fLeftX = finalCenter - (finalWidth / 2f); 
            float fRightX = finalCenter + (finalWidth / 2f);

            if (_currentStage == GameStage1.Obstacle) 
            {
                float noiseLeft = (Mathf.PerlinNoise(y * caveNoiseFrequency, 0f) - 0.5f) * caveRoughness;
                float noiseRight = (Mathf.PerlinNoise(y * caveNoiseFrequency, 100f) - 0.5f) * caveRoughness;

                fLeftX += noiseLeft;
                fRightX -= noiseRight; 

                if (!_isSpiking && _spikeCooldownDistance <= 0 && Random.value < bigSpikeChance)
                {
                    _isSpiking = true;
                    _spikeSide = (_lastSpikeSide == 0) ? ((Random.value > 0.5f) ? 1 : -1) : -_lastSpikeSide;
                    _lastSpikeSide = _spikeSide; _currentSpikeProgress = 0f;
                    _activeSpikeTotalLength = Random.Range(bigSpikeLengthRange.x, bigSpikeLengthRange.y);
                }

                if (_isSpiking)
                {
                    _currentSpikeProgress += segmentResolution;
                    if ((_currentSpikeProgress / _activeSpikeTotalLength) <= 1f)
                    {
                        float spikeShape = 0f;
                        float progress01 = _currentSpikeProgress / _activeSpikeTotalLength;

                        if (roundSpikes) spikeShape = Mathf.Sin(progress01 * Mathf.PI); 
                        else spikeShape = Mathf.PingPong(progress01, 0.5f) * 2f; 

                        float spikeDepth = (finalWidth * 0.6f) * spikeShape; 

                        if (_spikeSide == -1) fLeftX += spikeDepth; 
                        else fRightX -= spikeDepth;
                    }
                    else { _isSpiking = false; _spikeCooldownDistance = minSpikeGap; }
                }
            }
            else _isSpiking = false;

            // GÜVENLİK KİLİDİ
            float currentDistance = fRightX - fLeftX;
            if (currentDistance < minPlayableWidth)
            {
                float pushAmount = (minPlayableWidth - currentDistance) / 2f;
                fLeftX -= pushAmount;
                fRightX += pushAmount;
            }

            AddVertices(y, fLeftX, fRightX, true);
        }
        else
        {
            _isSpiking = false; 
            AddVertices(y, finalCenter - (finalWidth / 2f), finalCenter + (finalWidth / 2f), true);
        }
    }

    // --- UV MANTIĞI GERİ GELDİ (World Space - Esnemeyen Versiyon) ---
    void AddVertices(float y, float leftX, float rightX, bool separateCoords = false)
    {
        float lX, rX;
        if (separateCoords) { lX = leftX; rX = rightX; }
        else { float c = leftX; float w = rightX; lX = c - (w / 2f); rX = c + (w / 2f); }

        float extraWallWidth = wallThickness; 
        
        Vector3 v1 = new Vector3(-_halfScreenWidth - extraWallWidth, y, 0);
        Vector3 v2 = new Vector3(lX, y, 0);
        
        _lVerts.Add(v1); _lVerts.Add(v2);
        
        // DİKKAT: Burası UV Haritasını "Pozisyona Göre" oluşturur. Esnemeyi bu engeller.
        _lUVs.Add(new Vector2(v1.x * textureTiling.x, y * textureTiling.y)); 
        _lUVs.Add(new Vector2(v2.x * textureTiling.x, y * textureTiling.y)); 

        Vector3 v3 = new Vector3(rX, y, 0);
        Vector3 v4 = new Vector3(_halfScreenWidth + extraWallWidth, y, 0);

        _rVerts.Add(v3); _rVerts.Add(v4);  
        _rUVs.Add(new Vector2(v3.x * textureTiling.x, y * textureTiling.y)); 
        _rUVs.Add(new Vector2(v4.x * textureTiling.x, y * textureTiling.y)); 
    }

    void PreFillScreen()
    {
        for (float y = destroyPosY; y < spawnPosY; y += segmentResolution) SpawnMeshSegment(y);
        UpdateMeshes();
    }
    void MoveVerticesDown() {
        float move = scrollSpeed * Time.fixedDeltaTime;
        for (int i=0; i<_lVerts.Count; i++) {
            Vector3 vL=_lVerts[i]; vL.y-=move; _lVerts[i]=vL;
            Vector3 vR=_rVerts[i]; vR.y-=move; _rVerts[i]=vR;
        }
    }
    void RemoveOldVertices() {
        while (_lVerts.Count > 4 && _lVerts[0].y < destroyPosY) {
            _lVerts.RemoveRange(0, 2); _lUVs.RemoveRange(0, 2);
            _rVerts.RemoveRange(0, 2); _rUVs.RemoveRange(0, 2);
        }
    }
    void UpdateMeshes() {
        _leftMesh.Clear(); _leftMesh.SetVertices(_lVerts); _leftMesh.SetUVs(0, _lUVs);
        _leftMesh.SetTriangles(GenTris(_lVerts.Count), 0); _leftMesh.RecalculateNormals(); _leftMesh.RecalculateBounds();
        
        _rightMesh.Clear(); _rightMesh.SetVertices(_rVerts); _rightMesh.SetUVs(0, _rUVs);
        _rightMesh.SetTriangles(GenTris(_rVerts.Count), 0); _rightMesh.RecalculateNormals(); _rightMesh.RecalculateBounds();
        
        UpdCol();
    }
    void UpdCol() {
        if(leftWallCollider!=null && _lVerts.Count>=4){
            List<Vector2> p=new List<Vector2>(); for(int i=1;i<_lVerts.Count;i+=2)p.Add(_lVerts[i]); for(int i=_lVerts.Count-2;i>=0;i-=2)p.Add(_lVerts[i]); leftWallCollider.SetPath(0,p);
        }
        if(rightWallCollider!=null && _rVerts.Count>=4){
            List<Vector2> p=new List<Vector2>(); for(int i=0;i<_rVerts.Count;i+=2)p.Add(_rVerts[i]); for(int i=_rVerts.Count-1;i>=1;i-=2)p.Add(_rVerts[i]); rightWallCollider.SetPath(0,p);
        }
    }
    int[] GenTris(int cnt) {
        if(cnt<4)return new int[0]; int q=(cnt/2)-1; int[] t=new int[q*6];
        for(int i=0;i<q;i++){int r=i*2;int x=i*6;t[x]=r;t[x+1]=r+1;t[x+2]=r+2;t[x+3]=r+2;t[x+4]=r+1;t[x+5]=r+3;} return t;
    }
    void PickNewPath() {
        float maxO = maxPathOffset; if (_currentStage == GameStage1.Obstacle) maxO *= 0.7f;
        float safeL = Mathf.Max(0f, _halfScreenWidth - (_currentPathWidth / 2f) - 0.5f);
        float limit = Mathf.Min(maxO, safeL);
        float range = 1.5f; 
        float minT = Mathf.Max(-limit, _currentPathCenter - range);
        float maxT = Mathf.Min(limit, _currentPathCenter + range);
        if (minT >= maxT) _targetPathCenter = Random.Range(-limit, limit); else _targetPathCenter = Random.Range(minT, maxT);
    }
    void HandleDifficulty() {
        if(scrollSpeed < maxScrollSpeed) scrollSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        if(currentTurnSpeed < maxTurnSpeed) currentTurnSpeed += turnSpeedIncreaseRate * Time.fixedDeltaTime;
        _stageTimer -= Time.fixedDeltaTime;
        if(_stageTimer <= 0) { 
            float r = Random.value;
            if (r < 0.33f) SetStage(GameStage1.Obstacle); else if (r < 0.66f) SetStage(GameStage1.Snake); else SetStage(GameStage1.FallingObjects);
        }
    }

    void SetStage(GameStage1 s) {
        _currentStage = s; 
        IsFallingModeActive = false; 
        _isSpiking = false; _spikeCooldownDistance = 0f;

        if(s==GameStage1.Obstacle) { 
            _targetPathWidth=_halfScreenWidth*2f*obstacleWidthPercent; 
            _stageTimer=Random.Range(8f,12f); 
        }
        else if(s==GameStage1.Snake) { 
            _targetPathWidth=_halfScreenWidth*2f*snakeWidthPercent; 
            _stageTimer=Random.Range(5f,10f); 
            _isFirstSnakeFrame = true; 
        }
        else { 
            _targetPathWidth=_halfScreenWidth*2f*wideWidthPercent; 
            _targetPathCenter=0f; 
            StartCoroutine(EnableFallingModeWithDelay()); 
            _stageTimer=Random.Range(5f, 8f); 
        }
        if(_targetPathWidth<2.5f)_targetPathWidth=2.5f;
    }

    IEnumerator EnableFallingModeWithDelay()
    {
        yield return new WaitForSeconds(fallingModeStartDelay);
        if (_currentStage == GameStage1.FallingObjects)
        {
            IsFallingModeActive = true;
        }
    }
    
    void UpdateDynamicAreaFixed()
    {
        float screenHeight = referenceCameraSize * 2f;
        _halfScreenWidth = (screenHeight * _mainCam.aspect) / 2f;
    }
}