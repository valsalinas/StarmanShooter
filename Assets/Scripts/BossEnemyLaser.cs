using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyLaser : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 10f;

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            Destroy(gameObject);
        }
    }
}
