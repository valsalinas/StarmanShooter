using UnityEngine;

public class AggressiveEnemyRadar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Radar Detected " + other.name);

        if (other.tag == "Player")
        {
            if (transform.position.y > other.transform.position.y)
            {
                transform.parent.GetComponent<AggressiveEnemy>().ChasePlayer(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            transform.parent.GetComponent<AggressiveEnemy>().ChasePlayer(false);
        }
    }
}
