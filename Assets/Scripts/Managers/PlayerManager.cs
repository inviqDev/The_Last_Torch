using System;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class PlayerManager : Singleton<PlayerManager>
{
    public event Action<PlayerConfig> OnConfigChanged;

    [SerializeField] private PlayerConfig[] configs;
    [SerializeField] private int currentLevel;
    private PlayerConfig _currentConfig;
    
    [SerializeField] private PlayerModel playerPrefab;
    [SerializeField] private Transform _spawnPoint;
    private PlayerModel _currentPlayer;
    public PlayerModel CurrentPlayer => _currentPlayer;

    protected override void Awake()
    {
        Debug.Assert(currentLevel < configs.Length - 1, "PLAYER CURRENT LEVEL IS NOT VALID !");
        if (currentLevel > configs.Length - 1) return;
        
        _currentConfig = configs[currentLevel - 1];
    }
    
    private void Start()
    {
        _currentPlayer = Instantiate(playerPrefab, _spawnPoint.position, Quaternion.identity, null);
        _currentPlayer.ChangeConfig(_currentConfig);
    }

    public void IncreasePlayerLevel()
    {
        ++currentLevel;
        LoadNextLevelConfig();
    }

    private void LoadNextLevelConfig()
    {
        if (currentLevel > configs.Length)
        {
            print("OVER LIMIT");
            currentLevel = 1;
        }
        
        _currentConfig = configs[currentLevel - 1];
        OnConfigChanged?.Invoke(_currentConfig);
    }
}