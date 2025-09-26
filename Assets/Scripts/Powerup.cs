using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _clip;
    private Player _player;
    public float moveSpeed = 5f; 
    private bool isMoving = false;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            isMoving = true; 
        }

        if (isMoving)
        { 
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _player.transform.position) < 0.1f) 
            {
                isMoving = false;
            }
        }
        if (transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoPowerUpActive();
                        break;
                    case 4:
                        player.HealthPowerUpActive();
                        break;
                    case 5:
                        player.SpaceBombPowerUpActive();
                        break;
                    case 6:
                        player.SlowdownPowerUpActive();
                        break;
                    case 7:
                        player.HomingMissilePowerUpActive();
                        break;
                    default:
                        break;
                }
            }
        }
        Destroy(this.gameObject);
    }
}