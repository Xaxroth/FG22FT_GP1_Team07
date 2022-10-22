using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Play UI")]
    [SerializeField] private TextMeshProUGUI timertext;
    [SerializeField] private Slider _specialSlider, _distanceSlider;
    [SerializeField] private Image _distanceBarImage, specialBarImage;
    
    [Header("Pause UI")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _firstPauseButton;

    [Header("Game Over UI")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _firstGameOverButton;
    [SerializeField] private TextMeshProUGUI _gameOverHighScoreText;
    [SerializeField] private string _gameOverHighScoreTextPrefix = "High Score: ";
    
    [Header("Win UI")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _firstWinButton;
    [SerializeField] private TextMeshProUGUI _winScoreText;
    [SerializeField] private string _winScoreTextPrefix = "Score: ";
    [SerializeField] private TextMeshProUGUI _winHighScoreText;
    [SerializeField] private string _winHighScoreTextPrefix = "High Score: ";

    [Header("Pre Game UI")]
    [SerializeField] private GameObject _gameStartControlsOverLay;

    [Header("Boundary Warning UI")] 
    [SerializeField] private Image _warningPanel;

    
    private FuelSystem _fuelSystem;
    private EnemyScript _enemyScript;

    private void Awake()
    {
         _specialSlider.maxValue = 1;
         _distanceSlider.maxValue = 1;
        
        _gameStartControlsOverLay.SetActive(true);
        
        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameLost += OnGameLost;
        EventManager.OnGameWon += OnGameWon;
        EventManager.OnTogglePauseGame += OnGamePause;
    }

    private void Start()
    {
        _enemyScript = GameManager.Instance.Enemy.GetComponent<EnemyScript>();
        _fuelSystem = GameManager.Instance.Player.GetComponent<PlayerState>().FuelSystem;
        SetSpecial();
        _fuelSystem.OnFuelChanged += SetSpecial;
    }

    private void OnDestroy()
    {
        EventManager.OnGameStart -= OnGameStart;
        EventManager.OnGameLost -= OnGameLost;
        EventManager.OnGameWon -= OnGameWon;
        EventManager.OnTogglePauseGame -= OnGamePause;
        _fuelSystem.OnFuelChanged -= SetSpecial;
    }

    private void Update()
    {
        UpdateTimerUI();
    }

    private void FixedUpdate()
    {
        SetDistanceToGoalBar(_enemyScript.GetDistanceToPlayerPercentage());
    }

    private void OnGamePause()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused)
        {
            ActivateMenu();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstPauseButton);
        }else
        {
            DeactivateMenu();
        }
    }
    
    void ActivateMenu()
    {
        _pausePanel.SetActive(true);
        _distanceSlider.gameObject.SetActive(false);
        _specialSlider.gameObject.SetActive(false);
    }

    public void DeactivateMenu()
    {
        _pausePanel.SetActive(false);
        _distanceSlider.gameObject.SetActive(true);
        _specialSlider.gameObject.SetActive(true);
    }

    private void OnGameStart()
    {
        _gameStartControlsOverLay.SetActive(false);
    }
    
    private void OnGameLost()
    {
        _gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstGameOverButton);
        var formattedHighScoreNumber = FormatTime(GameManager.Instance.HighScore, true);
        _winHighScoreText.SetText(_winHighScoreTextPrefix + formattedHighScoreNumber);
    }
    
    private void OnGameWon()
    {
        _winPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstWinButton);
        var formattedScoreNumber = FormatTime(GameManager.Instance.CurrentScore, true);
        var formattedHighScoreNumber = FormatTime(GameManager.Instance.HighScore, true);
        _winScoreText.SetText(_winScoreTextPrefix + formattedScoreNumber);
        _winHighScoreText.SetText(_winHighScoreTextPrefix + formattedHighScoreNumber);
    }

    private void SetSpecial()
    {
        specialBarImage.material.SetFloat("_ProgressValue", _fuelSystem.GetFuelPercent());
    }

    public void SetDistanceToGoalBar(float value)
    {
        _distanceSlider.value = 1 - value;
        _distanceBarImage.fillAmount = 1 - value;
        _distanceBarImage.material.SetFloat("_ProgressValue", 1 - value);
    }

    private void UpdateTimerUI()
    {
        var time = GameManager.Instance.TimerSystem.Timer;
        var timeString = FormatTime(time);
        timertext.SetText(timeString);
    }
    
    public void SetBoundaryEffect(Vector4 offset)
    {
        _warningPanel.material.SetVector("_Offset_per_edge", offset);
    }

    
    private string FormatTime(float time, bool includeMilliseconds = false)
    {
        var min = Mathf.Floor(time / 60).ToString("00");
        var sec = Mathf.Floor(time % 60).ToString("00");
        var ms = Mathf.Floor((time * 100) % 100).ToString("00");
        return min + ":" + sec + (includeMilliseconds ? ":" + ms : "");
    }



}
