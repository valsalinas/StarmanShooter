using UnityEngine;
public class LaserSmart : MonoBehaviour
{
    private float _speed = 7.0f;
    private bool _isBackEnemyLaser = false;
    private bool _isFrontEnemyLaser = false; 

    void Update()
    {
        if (_isBackEnemyLaser)
        {
            MoveUp();
        }
        if (_isFrontEnemyLaser)
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 8f || transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }
    public void AssignBackEnemyLaser()
    {
        _isBackEnemyLaser = true;
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y > 8f || transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }

    public void AssignFrontEnemyLaser()
    {
        _isFrontEnemyLaser = true;     
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isBackEnemyLaser == true) 
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            Destroy(gameObject);
        }

        if (other.tag == "Powerup" && _isFrontEnemyLaser == true) 
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
