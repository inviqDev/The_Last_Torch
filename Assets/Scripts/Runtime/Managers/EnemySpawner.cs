using System.Collections.Generic;
using UnityEngine;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [DefaultExecutionOrder(-998)]
    public class EnemySpawner : Singleton<EnemySpawner>
    {
        [SerializeField] private EnemyConfig_NavMesh[] configs;
        private EnemyConfig_NavMesh _currentConfig;

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

            _currentConfig = configs[0];

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
            var enemy = TryGetEnemyFromPool();
            MyAsserts.IsNotNull(enemy, "enemy is not spawned");

            var redImprovedEnemySpawnInterval = 2.5f;
            if (counter % redImprovedEnemySpawnInterval == 0)
            {
                _currentConfig = configs[1];
            }
            else
            {
                _currentConfig = configs[0];
            }
            
            enemy.SetConfig(_currentConfig);
        }

        private EnemyModel_NavMesh TryGetEnemyFromPool()
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
            enemy.transform.SetParent(null);
            enemy.OnEnemyDeath += MoveEnemyToPool;

            _spawnedEnemies.Add(enemy);

            return enemy;
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
}