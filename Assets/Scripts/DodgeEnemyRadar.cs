using UnityEngine;

public class DodgeEnemyRadar : MonoBehaviour
{
    private DodgeEnemy _dodgeEnemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _dodgeEnemy = GetComponentInParent<DodgeEnemy>();

        Debug.Log("Radar Detected " + other.name);

        if (_dodgeEnemy != null)
        {
            if (other.tag == "PlayerLaser")
            {
                if (transform.position.y > other.transform.position.y)
                {
                    _dodgeEnemy.DodgeLaser(true);
                }
            }
        } 
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "PlayerLaser")
        {
            if (_dodgeEnemy != null) 
            {
                _dodgeEnemy.DodgeLaser(false);
            }
            
        }
    }
}