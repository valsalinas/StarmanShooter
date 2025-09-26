using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    private Player _player;
    private Animator _anim;
    public bool relocate = false;
    [SerializeField]
    private GameObject _parent;
    [SerializeField]
    private GameObject _laserBeamColPrefab;
    [SerializeField]
    private GameObject[] _laserbeams;
    [SerializeField]
    private bool spawnLaser = false;
    private bool _stopLaserMovement = false;
    [SerializeField]
    private GameObject _explosion;
    private bool _inLaser = false;
    private IEnumerator _inLaserRoutine;
    private Collider2D _coll;
    private Player _playerInLaser;
    [SerializeField]
    private Collider2D _laserBeamCollider;
    [SerializeField]
    private AudioClip _clip;

    void Start()
    {
        _coll = GetComponent<Collider2D>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        foreach (GameObject laser in _laserbeams)
        {
            laser.SetActive(false); 
        }
    }

    void Update()
    {

        if (_stopLaserMovement == false)
        { 
            CalculateMovement();
           
        }
    }

    void CalculateMovement()
    {
        if (relocate == true)
        {
            float _randomPosX = Random.Range(-6.5f, 6.5f);
            _parent.transform.position = new Vector3(_randomPosX, 7, 0);
            relocate = false;
        }
    }

    public void ActivateLasers()
    {
        Debug.Log("Activating lasers!");
        AudioSource.PlayClipAtPoint(_clip, transform.position);
        foreach (GameObject laser in _laserbeams)
        {
            laser.SetActive(true);
        }
        if (_laserBeamCollider != null)
            _laserBeamCollider.enabled = true;  // Enable laser detection collider
    }

    public void DeactivateLasers()
    {
        Debug.Log("Deactivating lasers!");
        foreach (GameObject laser in _laserbeams)
        {
            laser.SetActive(false);
        }
        if (_laserBeamCollider != null)
            _laserBeamCollider.enabled = false; // Disable laser detection collider
    }

    // This method runs when something enters the trigger collider of the laser
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit Something"); //Debug log to show someting collided

        if (other.tag == "Player") //Damage to player inside Laserbeam
        {
            _playerInLaser = other.GetComponent<Player>();
            _inLaser = true; // Set flag that player is in the laser

            // If a damage coroutine is already running, stop it first
            if (_inLaserRoutine != null)
            {
                StopCoroutine(_inLaserRoutine);
            }
            // Start the coroutine that applies damage over time
            _inLaserRoutine = InLaser();
            StartCoroutine(_inLaserRoutine);
        }
    }

    // This method runs when the player exits the laser beam
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player left laser");
            _inLaser = false; //Player is no longer in Laser

            //Stop damage coroutine if its running
            if (_inLaserRoutine != null)
            {
                StopCoroutine(_inLaserRoutine);
                _inLaserRoutine = null;
            }

            _playerInLaser = null; // Clear the reference to the player
        }
    }

    // Coroutine that applies damage every second while the player is in the laser
    private IEnumerator InLaser()
    {
        while (_inLaser == true)
        {
            if (_playerInLaser != null)
            {
                _playerInLaser.Damage(); // Damage the player once per second
            }

            yield return new WaitForSeconds(1f); // wait 1 second before next damage
        }
    }
    public void PlayerEnteredLaser(Collider2D playerCollider)
    {
        _playerInLaser = playerCollider.GetComponent<Player>();
        _inLaser = true;

        if (_inLaserRoutine != null)
            StopCoroutine(_inLaserRoutine);

        _inLaserRoutine = InLaser();
        StartCoroutine(_inLaserRoutine);
    }

    public void PlayerExitedLaser(Collider2D playerCollider)
    {
        _inLaser = false;

        if (_inLaserRoutine != null)
        {
            StopCoroutine(_inLaserRoutine);
            _inLaserRoutine = null;
        }
        _playerInLaser = null;
    }

    public void OnHitByPlayerAttack(Collider2D other)
    {
        Destroy(other.gameObject);

        if (_player != null)
        {
            _player.AddScore(10);
        }

        _coll.enabled = false;
        _stopLaserMovement = true;

        Destroy(this.gameObject, 2.1f);
    }
}