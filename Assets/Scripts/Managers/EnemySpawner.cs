using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-998)]
public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private EnemyConfig_NavMesh[] configs;
    private EnemyConfig_NavMesh _currentConfigNavMesh;

    [SerializeField] private EnemyModel_NavMesh[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float _enemySpawnInterval;
    [SerializeField] private int _waveEnemiesAmount;

    private Stack<EnemyModel_NavMesh> _pool;
    private Timer _timer;
    
    // currently unused collection of launched enemies
    private List<EnemyModel_NavMesh> _spawnedEnemies;
    

    protected override void Awake()
    {
        base.Awake();

        _currentConfigNavMesh = configs[0];

        _pool = new Stack<EnemyModel_NavMesh>();
        _spawnedEnemies = new List<EnemyModel_NavMesh>();

        _timer = new Timer(this);
    }

    private void Start()
    {
        _timer.StopTimer();

        _timer.OnTicked += SpawnEnemy;
        _timer.TimerIsOver += WaveIsFullyReleased;
        
        _timer.StartTimerTicker(_enemySpawnInterval, _waveEnemiesAmount);
    }

    private void WaveIsFullyReleased()
    {
        print("Current wave is fully released => launch next wave of something ??");
    }

    private void SpawnEnemy(int counter)
    {
        var redImprovedEnemySpawnInterval = 2.5f;
        if (counter % redImprovedEnemySpawnInterval == 0)
        {
            _currentConfigNavMesh = configs[1];
        }
        else
        {
            _currentConfigNavMesh = configs[0];
        }
        
        GetEnemy(_currentConfigNavMesh);
    }

    private void GetEnemy(EnemyConfig_NavMesh configNavMesh)
    {
        EnemyModel_NavMesh enemy = null;
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

        enemy.NavMeshMover.WarpTo(spawnPoint.position);
        // enemy.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);
        enemy.transform.SetParent(null);

        enemy.SetCurrentConfig(configNavMesh);
        enemy.OnEnemyDeath += MoveEnemyToPool;

        _spawnedEnemies.Add(enemy);
    }

    private void MoveEnemyToPool(EnemyModel_NavMesh enemy)
    {
        enemy.OnEnemyDeath -= MoveEnemyToPool;

        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(transform);

        _spawnedEnemies.Remove(enemy);
        _pool.Push(enemy);
    }
}