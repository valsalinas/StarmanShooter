using UnityEngine;

public class HorizontalEnemy : MonoBehaviour //handles horizonenemy movement and collision with enemy
{
    [SerializeField]
    private float _speed = 3.0f;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

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
        CalculateMovement();
    }
    void CalculateMovement()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if (transform.position.x > 13.0f)
        {
            Destroy(gameObject);
        }
    }

    public void PlayEnemyDeath()
    {
        _anim.SetTrigger("OnHorizonDeath");
        _speed = 0;
        _audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit Something");

        if (other.tag == "Player")
        {
            Debug.Log("Hit Player");

            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            PlayEnemyDeath();

            Destroy(this.gameObject, 2.10f);
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
            Destroy(this.gameObject, 2.10f);
        }
    }
}
