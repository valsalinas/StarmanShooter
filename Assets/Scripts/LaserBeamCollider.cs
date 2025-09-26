using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamCollider : MonoBehaviour
{
    private LaserEnemy _laserEnemy;

    void Start()
    {
        _laserEnemy = GetComponentInParent<LaserEnemy>();

        if (_laserEnemy == null)
        {
            Debug.LogError("LaserEnemy script not found in parent!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _laserEnemy.PlayerEnteredLaser(other);
            }
            else if (other.CompareTag("PlayerLaser") || other.CompareTag("SpaceBomb")) 
            {
                // Forward hit detection to LaserEnemy
                _laserEnemy.OnHitByPlayerAttack(other);
            }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _laserEnemy.PlayerExitedLaser(other);
        }
    }
}
