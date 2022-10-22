public static class EventManager
{
    public delegate void GameOver();
    public static event GameOver OnGameOver;

    public delegate void GameWon();
    public static event GameWon OnGameWon;

    public delegate void GameLost();
    public static event GameLost OnGameLost;
    
    public delegate void TogglePauseGame();
    public static event TogglePauseGame OnTogglePauseGame;
    
    public delegate void GameStart();
    public static event GameStart OnGameStart;
    
    public delegate void RestartGame();
    public static event RestartGame OnRestartGame;
    
    public delegate void GameQuit();
    public static event GameQuit OnGameQuit;
    
    public delegate void LoadScene(string sceneName);
    public static event LoadScene OnLoadScene;
    

    public static void InvokeOnGameOver()
    {
        OnGameOver?.Invoke();
    }
    
    public static void InvokeOnTogglePauseGame()
    {
        OnTogglePauseGame?.Invoke();
    }
    
    public static void InvokeOnGameStart()
    {
        OnGameStart?.Invoke();
    }
    
    public static void InvokeOnRestartGame()
    {
        OnRestartGame?.Invoke();
    }
    
    public static void InvokeOnGameQuit()
    {
        OnGameQuit?.Invoke();
    }
    
    public static void InvokeOnLoadScene(string sceneName)
    {
        OnLoadScene?.Invoke(sceneName);
    }

    public static void InvokeOnGameWon()
    {
        OnGameWon?.Invoke();
    }
    
    public static void InvokeOnGameLost()
    {
        OnGameLost?.Invoke();
    }
}
