using UnityEngine;

public class SmartEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player; 
    private Animator _anim; 
    private AudioSource _audioSource;
    private float _fireRate = 3.0f; 
    private float _nextFire = -1; 
    private bool _isSmartEnemyDead = false;

    [SerializeField]
    private GameObject _BackLaserPrefab; 
    public Transform firePointBack; 
    [SerializeField] private float _rayCastDistance = 10f;  

    [SerializeField]
    private GameObject _FrontLaserPrefab; 
    public Transform firePointFront; 
    [SerializeField] private float _frontRayCastDistance = 10f; 
    private float _frontFireRate = 3.0f;
    private float _nextFrontFire = -1f;

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
        if (_isSmartEnemyDead == true)
        {
            return;
        }

        CalculateEnemyMovement(); 
        CheckIfPlayerBehind();
        CheckIfPowerupInFront();
    }

    void CalculateEnemyMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }

    private void CheckIfPlayerBehind()
    {
        if (Time.time < _nextFire) return; 

        if (_player == null)
        {
            return;
        }
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 1.55f;

        Vector2 rayDirection = transform.up;

        float rayDistance = _rayCastDistance;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayDistance);

        Debug.DrawRay(rayOrigin, rayDirection * _rayCastDistance, Color.green);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.tag == ("Player"))
            {
                ShootBackLaser();
                _nextFire = Time.time + _fireRate;
                break; 
            }
        }
    }

    private void ShootBackLaser()
    {
        if (_BackLaserPrefab != null && firePointBack != null)
        {
            Vector2 direction = Vector2.up;
            GameObject laserObj = Instantiate(_BackLaserPrefab, firePointBack.position, Quaternion.identity);
            LaserSmart laser = laserObj.GetComponent<LaserSmart>();

            if (laser != null)
            {
                laser.AssignBackEnemyLaser();
            }
        }
    }

    private void CheckIfPowerupInFront()
    {
        if (Time.time < _nextFrontFire) return; 

        Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * 1.55f; 
        Vector2 rayDirection = -transform.up; 
        float rayDistance = _frontRayCastDistance;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayDistance);

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.tag == ("Powerup"))
            {
                ShootFrontLaser();
                _nextFrontFire = Time.time + _frontFireRate;
                break;
            }
        }
    }
    
    private void ShootFrontLaser()
    {
        if (_FrontLaserPrefab != null && firePointFront != null)
        {
            Vector2 direction = Vector2.down; 
            GameObject laserObj = Instantiate(_FrontLaserPrefab, firePointFront.position, Quaternion.identity);
            LaserSmart laser = laserObj.GetComponent<LaserSmart>();

            if (laser != null)
            {
                laser.AssignFrontEnemyLaser(); 
            }
        }
    }

    public void PlayEnemyDeath()
    {
        _anim.SetTrigger("OnSmartEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        _isSmartEnemyDead = true;    
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