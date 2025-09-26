using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private bool _isGameOver;

    [SerializeField]
    private bool _isWinner;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && (_isGameOver == true || _isWinner == true))
        {
            SceneManager.LoadScene(1); 
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
    public void Winner()
    {
        _isWinner = true;
    }
}