using System.Collections.Generic;
using UnityEngine;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [DefaultExecutionOrder(-998)]
    public class EnemySpawner : Singleton<EnemySpawner>
    {
        [SerializeField] private EnemyConfig[] configs;
        private EnemyConfig _currentConfig;

        [SerializeField] private EnemyModel[] enemyPrefabs;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float _enemySpawnInterval;
        [SerializeField] private int _waveEnemiesAmount;

        private Timer _timer;

        // currently unused collection of launched enemies
        private List<EnemyModel> _spawnedEnemies;


        protected override void Awake()
        {
            base.Awake();

            _currentConfig = configs[0];
            _spawnedEnemies = new List<EnemyModel>();

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
            _timer.StopTimer();
            
            _timer.OnTicked -= SpawnEnemy;
            _timer.TimerIsOver -= WaveIsFullyReleased;
            
            print("Current wave is fully released => launch next wave of something ??");
        }

        private void SpawnEnemy(int counter)
        {
            var enemy = Pool.Instance?.TryGetObjectFromPool(enemyPrefabs[0]);
            MyAsserts.IsNotNull(enemy, "enemy is not spawned");
            
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy.Mover.WarpTo(spawnPoint.position);
            enemy.transform.SetParent(null);
            
            var redImprovedEnemySpawnInterval = 2.5f;
            if (counter % redImprovedEnemySpawnInterval == 0)
            {
                _currentConfig = configs[1];
            }
            else
            {
                _currentConfig = configs[0];
            }
            
            enemy.OnEnemyDeath += MoveEnemyToPool;
            enemy.SetConfig(_currentConfig);
        }

        private void MoveEnemyToPool(EnemyModel enemy)
        {
            enemy.OnEnemyDeath -= MoveEnemyToPool;

            _spawnedEnemies.Remove(enemy);
            Pool.Instance?.ReturnToPool(enemy);
        }
    }
}