using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour 
{
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    private GameManager _gameManager;
    [SerializeField]
    private Scrollbar _thrusterVolume;
    [SerializeField]
    private TMP_Text _ammoText;
    [SerializeField] 
    private TMP_Text _waveMessageText;

    [SerializeField]
    private TMP_Text _bossWaveMessageText;

    [SerializeField]
    private TMP_Text _winnerMessageText;

    void Start()
    {
        _ammoText.text = "Ammo: " + 15;
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    public void WaveMessage(string message)
    {
        _waveMessageText.text = message;
        _waveMessageText.gameObject.SetActive(true);
        StartCoroutine(HideWaveMessage());
    }

    private IEnumerator HideWaveMessage()
    {
        yield return new WaitForSeconds(3f); 
        _waveMessageText.gameObject.SetActive(false);
    }

    public void BossWaveMessage(string message)
    {
        _bossWaveMessageText.text = message;
        _bossWaveMessageText.gameObject.SetActive(true);
        StartCoroutine(HideBossWaveMessage());
    }

    private IEnumerator HideBossWaveMessage()
    {
        yield return new WaitForSeconds(4f); 
        _bossWaveMessageText.gameObject.SetActive(false);
    }

    public void UpdateThrusters(float currentThrusterSize)
    {
        _thrusterVolume.size = currentThrusterSize;

        if (_thrusterVolume.size == 0)
        {
            _thrusterVolume.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            _thrusterVolume.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void UpdateAmmo(int ammoAmount)
    {
        if (ammoAmount < 0)
        {
            return;
        }
        _ammoText.text = "Ammo: " + ammoAmount.ToString();
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
        {
            return;
        }
        Debug.Log("_livesImg: " + _livesImg);
        Debug.Log("_liveSprites: " + _liveSprites);
        Debug.Log("currentLives: " + currentLives);

        if (currentLives < _liveSprites.Length)
        {
            _livesImg.sprite = _liveSprites[currentLives];
        }

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void WinnerSequence()
    {
        _gameManager.Winner();  
        _winnerMessageText.gameObject.SetActive(true);
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(WinnerAndGameOverFlickerRoutine());
    }

    IEnumerator WinnerAndGameOverFlickerRoutine()
    {
        while (true)
        {
            _winnerMessageText.text = "WINNER!!";
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);

            _winnerMessageText.text = "";
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}