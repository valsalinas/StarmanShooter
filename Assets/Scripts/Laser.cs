using UnityEngine;

public class Laser : MonoBehaviour 
{
    [SerializeField]
    private float _speed = 7.0f;
    private bool _isEnemyLaser = false;
    private bool _isSpaceBomb = false;

    void Update()
    {
        if (_isSpaceBomb == true)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
        
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }

        else 
        {
            MoveDown();
        }
    }

    void MoveUp() 
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void MoveDown() 
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true) 
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }

        if ((_isSpaceBomb == true) && (other.tag == "Enemy"))
        {
            Destroy(other.gameObject); 
            Destroy(this.gameObject);  
        }
    }
}