using UnityEngine;

public class AggressiveEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f; 
    private float _canFire = -1; 
    private bool _isEnemyDead = false;
    public bool _isChasing = false;
    private Vector3 _attackDirection; 
    private bool _hasLockedOn = false;

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
        if (!_isChasing && _player != null)
        {
            Debug.Log("Enemy detects Player");
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            if (!_hasLockedOn && _player != null)
            {
                Debug.Log("Enemy locks onto Player");
                _attackDirection = (_player.transform.position - this.transform.position).normalized;
                _hasLockedOn = true;
            }

            transform.position += _attackDirection * Time.deltaTime * (_speed * 2);

            if (transform.position.y < -5f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ChasePlayer(bool chasing)
    {
        Debug.Log("Is detecting Player");
        _isChasing = chasing;
    }

    public void PlayEnemyDeath()
    {
        _isEnemyDead = true;
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit Something");

        if (other.tag == "Player")
        {

            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
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

            PlayEnemyDeath();
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.8f);
        }
    }
}
