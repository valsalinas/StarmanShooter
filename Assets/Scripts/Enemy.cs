using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f; 
    private float _canFire = -1; 
    [SerializeField]
    private GameObject _enemyShieldVisualizer;
    [SerializeField]
    private bool _isEnemyShieldsActive = false;
    [SerializeField]
    private int _enemyShieldPower = 1;
    private Coroutine _shieldCoroutine;
    private bool _isEnemyDead = false;


    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        if (_enemyShieldVisualizer == null)
        {
            Debug.LogError("Enemy Shield Visualizer is NULL!");
        }
        else
        {
            _shieldCoroutine = StartCoroutine(ActivateEnemyShieldRoutine());
        }
    }

    void Update()
    {
        CalculateEnemyMovement();
        ShootEnemyLaser();
    }

    void ShootEnemyLaser()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(4f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }           
    }

    void CalculateEnemyMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }

    public void PlayEnemyDeath()
    {
        _isEnemyDead = true;
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();

        if (_shieldCoroutine != null)
        {
            StopCoroutine(_shieldCoroutine);
            _isEnemyShieldsActive = false;
            _enemyShieldVisualizer.SetActive(false);
        }
    }

    public void EnemyShieldDamage()
    {
        SpriteRenderer spriteRenderer = _enemyShieldVisualizer.transform.GetComponent<SpriteRenderer>();

        if (_isEnemyShieldsActive == true)
        {
            _enemyShieldPower -= 1;
            UpdateEnemyShieldPower(spriteRenderer);
        }
        else if (_isEnemyShieldsActive == false)
        {
            PlayEnemyDeath();
        }
    }
    public void EnemyShieldsActive()
    {
        SpriteRenderer spriteRenderer = _enemyShieldVisualizer.GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.green;
        _enemyShieldPower = 1;
        _isEnemyShieldsActive = true;
        _enemyShieldVisualizer.SetActive(true);
    }
    public void UpdateEnemyShieldPower(SpriteRenderer spriteRenderer)
    {
        if (_enemyShieldPower == 0)
        {
            _isEnemyShieldsActive = false;
            _enemyShieldVisualizer.SetActive(false);
        }
    }

    IEnumerator ActivateEnemyShieldRoutine()
    {
        float random = Random.Range(2f, 6f);
        yield return new WaitForSeconds(random);

        if (_isEnemyDead) yield break;

        EnemyShieldsActive(); 
        yield return new WaitForSeconds(2f);   

        if (_enemyShieldPower > 0 && !_isEnemyDead)
        {
            _isEnemyShieldsActive = false;
            _enemyShieldVisualizer.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            if (_isEnemyShieldsActive == true)
            {
                EnemyShieldDamage();
                return;
            }

            PlayEnemyDeath();
            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "PlayerLaser" || other.tag == "SpaceBomb" || other.tag == "HomingMissile")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }

            if (_isEnemyShieldsActive == true)
            {
                EnemyShieldDamage();
                return;
            }

            PlayEnemyDeath();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.10f);
        }
    }
}
