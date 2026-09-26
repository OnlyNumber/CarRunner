using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CarController _playerCar;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private FloorCreator _floorCreator;

    [SerializeField] private float LevelDistance;

    public Window StartGameWindow;
    public ProgressWindow GameplayWindow;

    public Window WinGameWindow;
    public Window LoseGameWindow;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _enemySpawner.SetTargetForEnemies(_playerCar.transform);

        StartGameWindow.ShowWindow();
        StartGameWindow.Button.onClick.AddListener(StartGame);

        WinGameWindow.Button.onClick.AddListener(WinGameWindow.HideWindow);
        LoseGameWindow.Button.onClick.AddListener(LoseGameWindow.HideWindow);

        WinGameWindow.Button.onClick.AddListener(RestartGame);
        LoseGameWindow.Button.onClick.AddListener(RestartGame);

        _playerCar.OnDeath += LoseGame;
    }

    public void Update()
    {
        if (_playerCar.transform.position.z >= LevelDistance && GameplayWindow.gameObject.activeInHierarchy)
        {
            WinGame();
        }
        else
            GameplayWindow.ProgressSlider.value = _playerCar.transform.position.z / LevelDistance;

    }

    private void StartGame()
    {
        StartGameWindow.HideWindow();
        GameplayWindow.ShowWindow();

        _playerCar.StartCar();
    }

    private void StopGame()
    {
        GameplayWindow.HideWindow();
        _playerCar.StopCar();
    }

    private void LoseGame()
    {
        StopGame();
        LoseGameWindow.ShowWindow();
    }

    private void WinGame()
    {
        StopGame();
        WinGameWindow.ShowWindow();
    }

    private void RestartGame()
    {
        _enemySpawner.ReturnAllEnemies();
        _floorCreator.MovePlatformsToStart();
        _playerCar.ResetHealth();
        _playerCar.transform.position = Vector3.zero;
        StartGameWindow.ShowWindow();
    }
}
