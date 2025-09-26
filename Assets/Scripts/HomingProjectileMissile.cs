using UnityEngine;

public class HomingProjectileMissile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;             
    [SerializeField] private float _rotationSpeed = 200f;    
    [SerializeField] private float _destroyLimitX = 11.3f;   
    [SerializeField] private float _destroyLimitY = 8f;

    private Rigidbody2D _rigidbody;
    private Transform _target;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody2D is missing on PlayerHomingMissile!");
        }

        RotateTowardsTargetInstantly();
    }

    void FixedUpdate()
    {
        AcquireClosestEnemy();

        if (_target != null)
        {
            HomeTowardsTarget();
        }

        MoveForward();
        CheckBounds();
    }

    private void AcquireClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        _target = closestEnemy;
    }
    private void HomeTowardsTarget()
    {
        if (_target == null)
        {
            return;
        }

        Vector2 toTarget = _target.position - transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, toTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetAngle), _rotationSpeed * Time.fixedDeltaTime);
    }

    private void RotateTowardsTargetInstantly()
    {
        if (_target == null)
        {
            return; 
        }

        Vector2 toTarget = _target.position - transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, toTarget);
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private void MoveForward()
    {
        _rigidbody.velocity = transform.up * _speed;
    }

    private void CheckBounds()
    {
        if (transform.position.y > 8f || transform.position.y < -8f)
        {
            Destroy(gameObject);
        }

        if (transform.position.x > 11.3f || transform.position.x < -11.3f)
        {
            Destroy(gameObject);
        }
    }
}
