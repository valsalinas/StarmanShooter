using UnityEngine;

public class DodgeEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    private Player _player; 
    private Animator _anim; 
    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 3.0f; 
    private float _canFire = -1; 
    private bool _isDodgeEnemyDead = false;

    private bool _isDodging = false;
    [SerializeField] 
    private float _dodgeSpeed = 5f;
    private int _dodgeDir = 1; 
    [SerializeField] 
    private float _dodgeDuration = 0.5f; 
    private float _dodgeTimer = 0f; 

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
    void CalculateEnemyMovement()
    {
        if (_isDodging)
        {
            transform.Translate(Vector2.right * _dodgeDir * _dodgeSpeed * Time.deltaTime);
            _dodgeTimer -= Time.deltaTime;
            if (_dodgeTimer <= 0f) _isDodging = false;
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
    }

    public void DodgeLaser(bool dodging)
    {
        if (dodging)
        {
            if (_player == null)
            {
                _isDodging = false;
                return;
            }

            _isDodging = true;
            _dodgeTimer = _dodgeDuration;

            if (transform.position.x < _player.transform.position.x)
            {
                _dodgeDir = -1;
            }
            else
            {
                _dodgeDir = 1;
            }
        }
        else
        {
            _isDodging = false;
        }
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

    public void PlayEnemyDeath()
    {
        _anim.SetTrigger("OnDodgeEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        _isDodgeEnemyDead = true;
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

            PlayEnemyDeath();
            Destroy(gameObject, 2.8f);
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
