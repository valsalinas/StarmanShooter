using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyprefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    private float _spaceBombSpawnChance = 0.10f;
    [SerializeField]
    private GameObject _horizonEnemyPrefab;
    [SerializeField]
    private GameObject _laserEnemyParent;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private int _enemiesPerSpawn = 1;
    [SerializeField]
    private GameObject _aggressiveEnemyPrefab;
    [SerializeField]
    private GameObject _smartEnemyPrefab;
    [SerializeField]
    private GameObject _dodgeEnemyPrefab;
    [SerializeField]
    private GameObject _homingMissilePowerup;
    [SerializeField]
    private GameObject _bossEnemyPrefab;

    private bool _stopEnemySpawning = false;
    private bool _stopPowerupSpawning = false;

    public void StartSpawning()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        yield return new WaitForSeconds(5f); 
        StartWave(1);

        yield return new WaitForSeconds(10f); 
        StartWave(2);

        yield return new WaitForSeconds(15);  
        StartWave(3);

        yield return new WaitForSeconds(20f); 
        StartBossWave();
    }

    void StartWave(int waveNumber)
    {
        int _currentWave = 0;
        _currentWave = waveNumber;
        _enemiesPerSpawn = waveNumber + 1;

        if (_uiManager != null)
        {
            _uiManager.WaveMessage($"Wave {waveNumber} Incoming");
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(4.0f);

        while (_stopEnemySpawning == false)
        {

            for (int i = 0; i < _enemiesPerSpawn; i++)
            {
                if (_stopEnemySpawning)
                {
                    yield break;
                }

                float enemySpawnChance = Random.Range(0f, 1f);

                if (enemySpawnChance < 0.40f)
                {
                    SpawnEnemy(); 
                }
                else if (enemySpawnChance < 0.55f)
                {
                    SpawnAggressiveEnemy();
                }
                else if (enemySpawnChance < 0.70f)
                {
                    SpawnHorizontalEnemy();
                }
                else if (enemySpawnChance < 0.90f)
                {
                    SpawnDodgeEnemy();
                }
                else if (enemySpawnChance < 0.95f)
                {
                    SpawnSmartEnemy();
                }
                else
                {
                    SpawnLaserBeamEnemy(); 
                }

                yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
            }

            yield return new WaitForSeconds(3.0f);

            if (_stopEnemySpawning)
            {
                yield break;
            }
            yield return new WaitForSeconds(3.0f);

        }
    }

    void SpawnEnemy()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        GameObject newEnemy = Instantiate(_enemyprefab, posToSpawn, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
    }
    void SpawnAggressiveEnemy()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0f);
        GameObject aggressiveEnemy = Instantiate(_aggressiveEnemyPrefab, posToSpawn, Quaternion.identity);
        aggressiveEnemy.transform.parent = _enemyContainer.transform;
    }

    void SpawnSmartEnemy()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        GameObject smartEnemy = Instantiate(_smartEnemyPrefab, posToSpawn, Quaternion.identity);
        smartEnemy.transform.parent = _enemyContainer.transform;
    }

    void SpawnHorizontalEnemy()
    {
        Vector3 horizonEnemySpawnPos = new Vector3(-12.0f, Random.Range(1f, 4f), 0);
        GameObject horizonEnemy = Instantiate(_horizonEnemyPrefab, horizonEnemySpawnPos, Quaternion.identity);
        horizonEnemy.transform.parent = _enemyContainer.transform;
    }

    void SpawnDodgeEnemy()
    {
        Vector3 dodgeEnemySpawnPos = new Vector3(Random.Range(-8f, 8f), 7, 0);
        GameObject dodgeEnemy = Instantiate(_dodgeEnemyPrefab, dodgeEnemySpawnPos, Quaternion.identity);
        dodgeEnemy.transform.parent = _enemyContainer.transform;
    }

    void SpawnLaserBeamEnemy()
    {
        float randomPosX = Random.Range(-8f, 7f);
        GameObject laserEnemy = Instantiate(_laserEnemyParent, new Vector3(randomPosX, 7, 0), Quaternion.identity);
        laserEnemy.transform.parent = _enemyContainer.transform;

    }

    void StartBossWave()
    {
        StartCoroutine(BossWaveRoutine());
    }

    IEnumerator BossWaveRoutine()
    {
        _stopEnemySpawning = true;

        if (_uiManager != null)
        {
            _uiManager.BossWaveMessage("Boss Incoming!");
        }

        StopCoroutine(SpawnEnemyRoutine());
        yield return new WaitForSeconds(5.0f);

        foreach (Transform child in _enemyContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Vector3 spawnPos = new Vector3(0, 7, 0);
        GameObject boss = Instantiate(_bossEnemyPrefab, spawnPos, Quaternion.identity);
        boss.transform.parent = _enemyContainer.transform;

    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopPowerupSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            float randomFloat = Random.Range(0f, 1.0f);

            if (randomFloat < _spaceBombSpawnChance)
            {
                Instantiate(_powerups[5], posToSpawn, Quaternion.identity);
            }
            else
            {
            float powerupSpawnChance = Random.Range(0f, 1.0f);

            if (powerupSpawnChance < 0.4f)
            {
                Instantiate(_powerups[3], posToSpawn, Quaternion.identity);
            }
            else if (powerupSpawnChance < 0.5f)
            {
                Instantiate(_powerups[4], posToSpawn, Quaternion.identity);
            }
            else if (powerupSpawnChance < 0.65f)
            {
                Instantiate(_powerups[0], posToSpawn, Quaternion.identity);
            }
            else if (powerupSpawnChance < 0.80f)
            {
                Instantiate(_powerups[1], posToSpawn, Quaternion.identity);
            }
            else if (powerupSpawnChance < 0.95f)
            {
                Instantiate(_powerups[2], posToSpawn, Quaternion.identity);
            }
            else if (powerupSpawnChance < 0.995f)
            {
                Instantiate(_powerups[6], posToSpawn, Quaternion.identity);
            }
            else
            {
                Instantiate(_powerups[7], posToSpawn, Quaternion.identity);
            }
        }
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        _stopEnemySpawning = true;
        _stopPowerupSpawning = true;
    }
}