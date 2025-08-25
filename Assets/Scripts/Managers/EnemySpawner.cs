using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-998)]
public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private EnemyConfig[] configs;
    private EnemyConfig _currentConfig;

    [SerializeField] private EnemyModel[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int _waveEnemiesAmount;

    private Stack<EnemyModel> _pool;
    private List<EnemyModel> _spawnedEnemies;
    private Timer _timer;

    protected override void Awake()
    {
        base.Awake();

        _currentConfig = configs[0];

        _pool = new Stack<EnemyModel>();
        _spawnedEnemies = new List<EnemyModel>();

        _timer = new Timer(this);
    }

    private void Start()
    {
        _timer.StopTimer();

        _timer.OnTicked += SpawnEnemy;
        _timer.TimerIsOver += zaglyshka;
        
        _timer.StartTimerTicker(1.25f, _waveEnemiesAmount);
    }

    private void zaglyshka()
    {
        print("zaglyshka");
        // Debug.Break();
    }

    private void SpawnEnemy(int counter)
    {
        var redImprovedEnemySpawnInterval = 2.5f;
        if (counter % redImprovedEnemySpawnInterval == 0)
        {
            _currentConfig = configs[1];
        }
        else
        {
            _currentConfig = configs[0];
        }
        
        GetEnemy(_currentConfig);
    }

    private void GetEnemy(EnemyConfig config)
    {
        EnemyModel enemy = null;
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        switch (_pool.Count)
        {
            case > 0:
                enemy = _pool.Pop();
                enemy.gameObject.SetActive(true);
                break;
            case 0:
                enemy = Instantiate(enemyPrefabs[0]);
                break;
        }

        Debug.Assert(enemy, "enemy is not spawned");

        enemy.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);
        enemy.transform.SetParent(null);

        enemy.SetCurrentConfig(config);
        enemy.OnEnemyDeath += MoveEnemyToPool;

        _spawnedEnemies.Add(enemy);
    }

    private void MoveEnemyToPool(EnemyModel enemy)
    {
        enemy.OnEnemyDeath -= MoveEnemyToPool;

        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(transform);

        _spawnedEnemies.Remove(enemy);
        _pool.Push(enemy);
    }
}