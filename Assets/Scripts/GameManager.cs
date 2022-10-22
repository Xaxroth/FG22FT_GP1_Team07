using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject _player;
    public GameObject Player { get { return _player; } }
    [SerializeField] private GameObject _enemy;
    public GameObject Enemy { get { return _enemy; } }
    [SerializeField] private MasterSpawner _spawner;
    public MasterSpawner Spawner { get { return _spawner; } }
    [SerializeField] private UIManager _uiManager;
    public UIManager UIManager { get { return _uiManager; } }
    [SerializeField] private GameObject _mainCamera;
    public GameObject MainCamera { get { return _mainCamera; } }

    [SerializeField] private float _startTimer;
    public TimerSystem TimerSystem { get; private set; }
    
    //Dirty and (hopefully) temporary solution for state management 
    public enum GameState
    {
        PreGame,
        Playing,
        Paused,
        GameOver
    }
    public GameState CurrentGameState { get; private set; }

    public float HighScore { get; private set; }
    public float CurrentScore { get; private set; }
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        TimerSystem = new TimerSystem(_startTimer);
        CurrentGameState = GameState.PreGame;
        
        

        LoadHighScore();

        EventManager.OnRestartGame += RestartGame;
        EventManager.OnLoadScene += LoadScene;
        EventManager.OnGameQuit += QuitGame;
        EventManager.OnGameWon += GameWon;
        EventManager.OnGameLost += GameLost;
        TimerSystem.OnTimerEnd += TimerEnded;
    }

    private void Start()
    {
        Time.timeScale = 0;
        ShowCursor(false);
        AudioListener.pause = false;
        StartCoroutine(ListenForFirstInput());
        
    }

    private void OnDisable()
    {
        EventManager.OnRestartGame -= RestartGame;
        EventManager.OnLoadScene -= LoadScene;
        EventManager.OnGameQuit -= QuitGame;
        EventManager.OnGameWon -= GameWon;
        EventManager.OnGameLost -= GameLost;
        TimerSystem.OnTimerEnd -= TimerEnded;
    }

    private void Update()
    {
        TimerSystem.Update();
        CurrentScore += Time.deltaTime;
        if(CurrentGameState == GameState.PreGame && Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.M))
        {
            ResetHighScore();
        }
    }

    private void GameOver()
    {
        ShowCursor(true);
        Time.timeScale = 0;
        CurrentGameState = GameState.GameOver;
        EventManager.InvokeOnGameOver();
    }
    
    private void GameWon()
    {
        SaveHighScore();
        GameOver();
    }

    private void GameLost()
    {
        GameOver();
    }
    
    private void TimerEnded()
    {
        EventManager.InvokeOnGameLost();
    }
    
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UnPauseGame();
    }
    
    private IEnumerator ListenForFirstInput()
    {
        yield return new WaitUntil(() => _player.GetComponent<InputHandler>().LeftIsPressed || _player.GetComponent<InputHandler>().RightIsPressed);
        UnPauseGame();
        EventManager.InvokeOnGameStart();
    }
    
    
    public void TogglePauseGame()
    {
        if(CurrentGameState == GameState.Playing)
        {
            PauseGame();
        }
        else if(CurrentGameState == GameState.Paused)
        {
            UnPauseGame();
        }
    }

    private void PauseGame()
    {
        ShowCursor(true);
        Time.timeScale = 0;
        AudioListener.pause = true;
        CurrentGameState = GameState.Paused;
        EventManager.InvokeOnTogglePauseGame();
    }

    private void UnPauseGame()
    {
        ShowCursor(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
        CurrentGameState = GameState.Playing;
        EventManager.InvokeOnTogglePauseGame();
    }
    
    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SaveHighScore()
    {
        var score = CurrentScore;
        if(score < PlayerPrefs.GetFloat("HighScore"))
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }
    
    private void LoadHighScore()
    {
        if(PlayerPrefs.HasKey("HighScore"))
        {
            HighScore = PlayerPrefs.GetFloat("HighScore");
        }else{
            ResetHighScore();
        }
    }

    private void ResetHighScore()
    {
        HighScore = 300f;
        PlayerPrefs.SetFloat("HighScore", HighScore);
    }
    
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void ShowCursor(bool toggle)
    {
        Cursor.visible = toggle;
    }


}

