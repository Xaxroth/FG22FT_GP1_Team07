using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public void ResumeGame()
    {
        GameManager.Instance.TogglePauseGame();
    }
    
    public void RestartGame()
    {
        EventManager.InvokeOnRestartGame();
    }

    public void ExitGame()
    {
        EventManager.InvokeOnGameQuit();
    }

    public void MainMenu()
    {
        EventManager.InvokeOnLoadScene("MainMenu");
    }
}

