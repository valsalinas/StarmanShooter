using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;
    private float _speedMultiplier = 2.0f;
    private float _slowSpeed = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _nextFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldsActive = false;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _explosionSoundClip;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private float _thrusterVolume = 1.0f;
    private float _nextThruster;
    private float _thrusterRate = 0.1f;
    [SerializeField]
    private int _shieldPower = 3;
    [SerializeField]
    private int _ammo = 16;
    [SerializeField]
    private GameObject _ammoPrefab;
    private bool _isAmmoPowerUpActive = false;
    [SerializeField]
    private GameObject _healthPrefab;
    private bool _isHealthPowerUpActive = false;
    [SerializeField]
    private bool _isSpaceBombPowerUpActive = false;
    [SerializeField]
    private GameObject _spacebombPrefab;
    private ShakeCamera _shakeCamera;
    [SerializeField]
    private bool _isSlowDownPowerUpActive = false;

    [SerializeField]
    private GameObject _homingMissilePrefab;
    [SerializeField] private float _homingMissileDuration = 5.0f; 
    private bool _isHomingMissilePowerupActive = false;

    private bool _canTakeDamage = true;
    [SerializeField] private float _hitCooldown = 0.2f;
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _shakeCamera = Camera.main.GetComponent<ShakeCamera>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is Null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on the Player is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        DetermineMovement(horizontalInput, verticalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
            {
                ShootLaser();
            }
        }
            ThrusterMovement();
    }

    private void ThrusterMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _thrusterVolume <= 0 && Time.time < _nextThruster)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _thrusterVolume > 0)
        {
            _speed *= 3f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || _thrusterVolume <= 0)
        {
            _speed = 6;
        }

        if (Time.time < _nextThruster)
        {
            return;
        }

        _nextThruster = Time.time + _thrusterRate;

        if (Input.GetKey(KeyCode.LeftShift) && _thrusterVolume > 0)
        {
            _thrusterVolume = Mathf.Clamp(_thrusterVolume - 0.05f, 0f, 1f);

            if (_thrusterVolume <= 0)
            {
                _nextThruster = Time.time + 3f;
            }
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && _thrusterVolume < 1f)
        {
            _thrusterVolume = Mathf.Clamp(_thrusterVolume + 0.025f, 0f, 1f);
        }

        _uiManager.UpdateThrusters(_thrusterVolume);
    }

    void DetermineMovement(float horizontalInput, float verticalInput)
    {
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        float currentSpeed = _speed;

        if (_isSlowDownPowerUpActive == true)
        {
            currentSpeed = _slowSpeed;
        }
        else if (_isSpeedBoostActive == true)
        {
            currentSpeed = _speed * _speedMultiplier;
        }
       
        transform.Translate(direction * currentSpeed * Time.deltaTime); 
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.2f)
        {
            transform.position = new Vector3(-11.2f, transform.position.y, 0);
        }

        else if (transform.position.x < -11.2f)
        {
            transform.position = new Vector3(11.2f, transform.position.y, 0);
        }
    }

    void ShootLaser()
    {
        _nextFire = Time.time + _fireRate;

        if (_ammo >= 1)
        {
            if (_isSpaceBombPowerUpActive == true)
            {
                Instantiate(_spacebombPrefab, transform.position, Quaternion.identity);
                _ammo -= 1;
            }
            else if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
                _ammo -= 1;
            }
            else if (_isHomingMissilePowerupActive == true)
            {
                Instantiate(_homingMissilePrefab, transform.position, Quaternion.identity);
                _ammo -= 1;
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
                _ammo -= 1;
            }

            _audioSource.Play();
            _uiManager.UpdateAmmo(_ammo);
        }
    }
    
    public void Damage()
    {
        if (!_canTakeDamage)
            return; 

        _canTakeDamage = false; 
        StartCoroutine(HitCooldown());

        SpriteRenderer spriteRenderer = _shieldVisualizer.transform.GetComponent<SpriteRenderer>();

        if (_isShieldsActive == true)
        {
            _shieldPower -= 1;
            UpdateShieldPower(spriteRenderer);
        }

        else if (_isShieldsActive == false)
        {
            _lives -= 1; 
            _shakeCamera.StartCameraShake();
        }

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1 && _isShieldsActive == false)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void UpdateShieldPower(SpriteRenderer spriteRenderer)
    {
        if (_shieldPower == 2)
        {
            spriteRenderer.color = Color.green;
        }

        if (_shieldPower == 1)
        {
            spriteRenderer.color = Color.magenta;
        }

        if (_shieldPower == 0)
        {
            _isShieldsActive = false;
            _shieldVisualizer.SetActive(false);
        }
    }
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;

    }
    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
    }
    public void ShieldsActive()
    {
        SpriteRenderer spriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.white;
        _shieldPower = 3;
        _isShieldsActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AmmoPowerUpActive()
    {
        if (_ammo < 15)
        {
        _isAmmoPowerUpActive = true;
        _ammo = 15;
        StartCoroutine(AmmoPowerDownRoutine()); 
        }

        _uiManager.UpdateAmmo(_ammo);
    }
    IEnumerator AmmoPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isAmmoPowerUpActive = false;
    }
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void HealthPowerUpActive()
    {
        _isHealthPowerUpActive = true;

        if (_lives == 3)
        {
            return;
        }

        if (_lives == 2)
        {
            _lives += 1;
            _leftEngine.SetActive(false);
            StartCoroutine(HealthPowerDownRoutine());
        }
        else if (_lives == 1)
        {
            _lives += 1;
            _rightEngine.SetActive(false);
            StartCoroutine(HealthPowerDownRoutine());
        }

        _uiManager.UpdateLives(_lives);
    }
    private IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(_hitCooldown);
        _canTakeDamage = true; 
    }

    IEnumerator HealthPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isHealthPowerUpActive = false;
    }

    public void SpaceBombPowerUpActive()
    {
        _isSpaceBombPowerUpActive = true;
        StartCoroutine(SpaceBombPowerDownRoutine());
    }

    private IEnumerator SpaceBombPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpaceBombPowerUpActive = false;
    }

    public void SlowdownPowerUpActive()
    {
        _isSlowDownPowerUpActive = true;
        StartCoroutine(SlowdownPowerDownRoutine());
    }

    private IEnumerator SlowdownPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSlowDownPowerUpActive = false;
    }

    public void HomingMissilePowerUpActive()
    {
        _isHomingMissilePowerupActive = true;
        StartCoroutine(HomingMissilePowerDownRoutine());
    }

    private IEnumerator HomingMissilePowerDownRoutine()
    {
        yield return new WaitForSeconds(_homingMissileDuration);
        _isHomingMissilePowerupActive = false;
    }
}
