using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] 
    private float _moveSpeed = 5.0f;
    [SerializeField] 
    private Vector3 _stopPosition = new Vector3(0, 2.66f, 0);
    [SerializeField] 
    private GameObject _laserPrefab;
    [SerializeField] 
    private int _lasersPerBurstPhase1 = 1;
    [SerializeField] 
    private float _shotDelayPhase1 = 0.04f;
    [SerializeField] 
    private float _burstDelayPhase1 = 0.3f;
    [SerializeField] 
    private float _rotationStepPhase1 = 10f;
    [SerializeField] 
    private int _lasersPerBurstPhase2 = 2;
    [SerializeField] 
    private float _shotDelayPhase2 = 0.03f;
    [SerializeField] 
    private float _burstDelayPhase2 = 0.15f;
    [SerializeField] 
    private float _rotationStepPhase2 = 15f;
    private bool _isPhase2 = false;
    [SerializeField] 
    private GameObject _enemyShieldVisualizer;
    [SerializeField] 
    private int _enemyShieldPower = 3;
    private bool _isEnemyShieldsActive = false;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private bool _isAtCenter = false;
    private bool _isEnemyDead = false;
    private bool _rotatingRight = true;
    [SerializeField] 
    private float _rotationAngleLimit = 60f;
    private float _currentRotation = 0f;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();

        if (_enemyShieldVisualizer != null)
        {
            _enemyShieldVisualizer.SetActive(true); 
        }
    }

    void Update()
    {
        if (!_isAtCenter)
        {
            MovetoCenter();
        }
    }

    void MovetoCenter()
    {
        transform.position = Vector3.MoveTowards(transform.position, _stopPosition, _moveSpeed * Time.deltaTime);

        if (!_isAtCenter && Vector3.Distance(transform.position, _stopPosition) < 2.67f)
        {
            _isAtCenter = true;
            StartCoroutine(InitializeBoss());
        }
    }

    IEnumerator InitializeBoss()
    {
        yield return new WaitForSeconds(0.5f); 
        ActivateShields();
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine() 
    { 
        while (!_isEnemyDead)
        {
            int lasersPerBurst;
            float shotDelay;
            float burstDelay;
            float rotationStep;

            if (_isPhase2)
            {
                lasersPerBurst = _lasersPerBurstPhase2;
                shotDelay = _shotDelayPhase2;
                burstDelay = _burstDelayPhase2;
                rotationStep = _rotationStepPhase2;
            }
            else
            {
                lasersPerBurst = _lasersPerBurstPhase1;
                shotDelay = _shotDelayPhase1;
                burstDelay = _burstDelayPhase1;
                rotationStep = _rotationStepPhase1;
            }

            for (int i = 0; i < lasersPerBurst; i++)
            {
                if (_isEnemyDead) 
                    yield break;

                Instantiate(_laserPrefab, transform.position, transform.rotation);

                if (_rotatingRight)
                {
                    _currentRotation += rotationStep;
                    if (_currentRotation >= _rotationAngleLimit)
                        _rotatingRight = false;
                }
                else
                {
                    _currentRotation -= rotationStep;
                    if (_currentRotation <= -_rotationAngleLimit)
                        _rotatingRight = true;
                }

                transform.rotation = Quaternion.Euler(0, 0, _currentRotation);

                yield return new WaitForSeconds(shotDelay);
            }

            yield return new WaitForSeconds(burstDelay);
        }
    }

    public void ActivateShields()
    {
        _isEnemyShieldsActive = true;
        _enemyShieldPower = 3;
        if (_enemyShieldVisualizer != null)
        {
            _enemyShieldVisualizer.SetActive(true);
        }
    }

    public void EnemyShieldDamage()
    {
        if (!_isAtCenter && !_isEnemyShieldsActive)
        {
            return;
        }

        if (_isEnemyShieldsActive == true)
        {
            _enemyShieldPower -= 1;

            if (!_isPhase2 && _enemyShieldPower == 1)
            {
                _isPhase2 = true;
            }

            if (_enemyShieldPower <= 0)
            {
                _isEnemyShieldsActive = false;
                _enemyShieldVisualizer.SetActive(false);
            }
        }
        else
        {
            PlayEnemyDeath();
        }
    }

    public void PlayEnemyDeath()
    {
        if (_isEnemyDead == true) return;

        _isEnemyDead = true;
        StopAllCoroutines();
        _anim.SetTrigger("OnBossEnemyDeath");
        _moveSpeed = 0;
        _audioSource.Play();
        StartCoroutine(WinnerDisplay());
    }

    private IEnumerator WinnerDisplay()
    {
        yield return new WaitForSeconds(2.8f);

        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.WinnerSequence();
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEnemyDead)
        {
            return;
        }

        if (!_isAtCenter && !_isEnemyShieldsActive)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null) player.Damage();

            if (_isEnemyShieldsActive)
            {
                EnemyShieldDamage();
                return;
            }
            PlayEnemyDeath();

        }

        if (other.CompareTag("PlayerLaser") || other.CompareTag("SpaceBomb") || other.CompareTag("HomingMissile"))
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(50);
            }
            if (_isEnemyShieldsActive)
            {
                EnemyShieldDamage();
                return;
            }
            PlayEnemyDeath();
            Destroy(GetComponent<Collider2D>());
        }
    }
}

