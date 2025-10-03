using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime
{
    [DefaultExecutionOrder(-998)]
    public class Spawner : MonoBehaviour
    {
        [System.Serializable]
        public struct prefabBuild
        {
            public EnemyType enemyType;
            public Enemy enemyPrefab;
        }

        // public Action OnSuperBossSpawned;

        [SerializeField] private SpawnWaveConfig[] waveConfigs;

        [SerializeField] private bool testMode;
        [SerializeField] private int currentWaveIndex;

        [SerializeField] private bool spawnWavesContinuously;

        [SerializeField] private float statsMultiplier;
        [SerializeField] private float multiplierIncrement;

        [SerializeField] private Transform[] nonBossPoints;
        [SerializeField] private Transform[] bossPoints;
        [SerializeField] private Transform superBossSpawnPoint;

        [SerializeField] private prefabBuild[] prefabBuilds;

        private Dictionary<EnemyType, Enemy> _enemyDictionary;
        private Queue<EnemyConfig> _bossWaveQueue = new();
        private Queue<EnemyConfig> _waveQueue = new();
        private List<Enemy> _spawnedEnemies;

        private Player _player;
        private EnemyType _enemyType;
        private Timer _timer;

        private SpawnWaveConfig _currentWaveConfig;
        private float _spawnInterval;
        private int _bossSpawnPointIndex;
        private int _extraWaveLocalBossIndex;

        public void Initialize(Player player)
        {
            UnityEngine.Assertions.Assert.IsNotNull(player, "Player is not found");
            _player = player;

            InitializeCollections();
            ResetSpawnerSettings();
            StartSpawning();
        }

        private void InitializeCollections()
        {
            _enemyDictionary ??= new Dictionary<EnemyType, Enemy>(prefabBuilds.Length);
            foreach (var e in prefabBuilds)
            {
                UnityEngine.Assertions.Assert.IsNotNull(e.enemyPrefab, "Prefab build is invalid");
                _enemyDictionary[e.enemyType] = e.enemyPrefab;
            }

            _waveQueue = new Queue<EnemyConfig>();
            _bossWaveQueue = new Queue<EnemyConfig>();
            _spawnedEnemies = new List<Enemy>();
        }

        private void StartSpawning()
        {
            if (currentWaveIndex < 0 || currentWaveIndex >= waveConfigs.Length)
            {
                UnityEngine.Assertions.Assert.IsTrue(
                    currentWaveIndex < 0 || currentWaveIndex >= waveConfigs.Length,
                    "messed up spawner configs logic");

                return;
            }

            _currentWaveConfig = waveConfigs[currentWaveIndex];
            _spawnInterval = _currentWaveConfig.spawningInterval;

            if (_currentWaveConfig.willSpawnExtraWave)
            {
                _extraWaveLocalBossIndex = 0;
            }

            BuildWaveQueue(_currentWaveConfig);
            InitializeSpawnerTimer();

            var totalAmount = _waveQueue.Count + _bossWaveQueue.Count;
            _timer.StartTimerTicker(_spawnInterval, totalAmount);
        }

        private void InitializeSpawnerTimer()
        {
            _timer ??= new Timer(this);
            _timer.StopTimer();

            _timer.OnTicked += SpawnEnemy;
            if (spawnWavesContinuously)
            {
                _timer.TimerIsOver += SpawnNextWave;
            }
        }

        public void StopSpawningEnemies()
        {
            if (_spawnedEnemies is { Count: > 0 })
            {
                MoveSpawnedEnemiesToPool();
            }

            ResetSpawnerSettings();
            StopSpawnerTimer();
        }

        private void MoveSpawnedEnemiesToPool()
        {
            foreach (var e in _spawnedEnemies)
            {
                if (!e || !e.gameObject.activeInHierarchy) continue;

                e.Movement?.DeactivateNavMeshAgent();
                e.OnCharacterDeath -= MoveEnemyToPool;
                Pool.Instance?.ReturnToPool(e);
            }
        }

        private void ResetSpawnerSettings()
        {
            _waveQueue?.Clear();
            _bossWaveQueue?.Clear();
            _spawnedEnemies?.Clear();

            currentWaveIndex = 0;
            multiplierIncrement = 1;
            _bossSpawnPointIndex = 0;
            _extraWaveLocalBossIndex = 0;
        }

        private void StopSpawnerTimer()
        {
            if (_timer == null) return;
            _timer.OnTicked -= SpawnEnemy;
            _timer.TimerIsOver -= SpawnNextWave;
            _timer.StopTimer();
        }

        private void SpawnNextWave()
        {
            print($"[Spawner] Wave #{currentWaveIndex} released.");

            _timer.TimerIsOver -= SpawnNextWave;

            _waveQueue.Clear();
            _bossWaveQueue.Clear();

            _timer.StopTimer();
            _timer.OnTicked -= SpawnEnemy;

            currentWaveIndex++;
            statsMultiplier += multiplierIncrement;

            StartSpawning();
        }

        private void SpawnEnemy(int _)
        {
            if (_bossWaveQueue.Count == 0 && _waveQueue.Count == 0) return;

            var config = _bossWaveQueue.Count > 0
                ? _bossWaveQueue.Dequeue()
                : _waveQueue.Dequeue();

            if (!config)
            {
                UnityEngine.Assertions.Assert.IsNotNull(config, "invalid config");
            }

            Vector3 spawnPoint;
            if (IsSuperBossConfig(config))
            {
                UnityEngine.Assertions.Assert.IsNotNull(superBossSpawnPoint, "super boss spawn point is not set");
                spawnPoint = superBossSpawnPoint.position;
            }
            else if (IsNormalBossConfig(config))
            {
                UnityEngine.Assertions.Assert.IsTrue(bossPoints.Length > 0, "bossPoints array is not set");
                if (_currentWaveConfig.willSpawnExtraWave)
                {
                    spawnPoint = bossPoints[_extraWaveLocalBossIndex % bossPoints.Length].position;
                    _extraWaveLocalBossIndex++;
                }
                else
                {
                    spawnPoint = bossPoints[_bossSpawnPointIndex % bossPoints.Length].position;
                    _bossSpawnPointIndex++;
                }
            }
            else
            {
                if (currentWaveIndex != 0)
                {
                    spawnPoint = nonBossPoints[Random.Range(0, nonBossPoints.Length)].position;
                }
                else
                {
                    var length = nonBossPoints.Length;
                    UnityEngine.Assertions.Assert.IsTrue(length > 9, 
                        $"nonBossPoints is not valid for wave 0: need at least 9 points, but got {length}");
                    
                    spawnPoint = nonBossPoints[Random.Range(8, nonBossPoints.Length)].position;
                }
            }

            var prefab = GetEnemyTypePrefab(config);
            UnityEngine.Assertions.Assert.IsNotNull(prefab, $"There is no prefab mapped for {config.EnemyType}");

            var enemy = Pool.Instance?.TryGet(prefab);
            UnityEngine.Assertions.Assert.IsNotNull(enemy, "enemy is not spawned");
            enemy.InitializeEnemy(_player, config, spawnPoint, statsMultiplier);
            _spawnedEnemies.Add(enemy);

            enemy.OnCharacterDeath += MoveEnemyToPool;
        }

        private Enemy GetEnemyTypePrefab(EnemyConfig config)
        {
            if (!config) return null;
            UnityEngine.Assertions.Assert.IsNotNull(_enemyDictionary, "enemy dictionary is not set");
            return _enemyDictionary.GetValueOrDefault(config.EnemyType);
        }

        private void MoveEnemyToPool(Character enemy)
        {
            enemy.OnCharacterDeath -= MoveEnemyToPool;
            _spawnedEnemies.Remove(enemy as Enemy);
            Pool.Instance?.ReturnToPool(enemy as Enemy);
        }

        private void BuildWaveQueue(SpawnWaveConfig currentConfig)
        {
            _bossWaveQueue.Clear();
            _waveQueue.Clear();

            // --- БОССЫ ---
            if (currentConfig.willSpawnExtraWave)
            {
                // ExtraWave: всегда один супер босс (если задан) + N обычных боссов
                if (currentConfig.willSpawnSuperBoss && currentConfig.superBoss)
                    _bossWaveQueue.Enqueue(currentConfig.superBoss);

                if (currentConfig.bossEnemy && currentConfig.bossEnemiesAmount > 0)
                {
                    for (var i = 0; i < currentConfig.bossEnemiesAmount; i++)
                        _bossWaveQueue.Enqueue(currentConfig.bossEnemy);
                }
            }
            else
            {
                // Обычная волна: опционально супер босс и/или N обычных боссов
                if (currentConfig.willSpawnSuperBoss && currentConfig.superBoss)
                    _bossWaveQueue.Enqueue(currentConfig.superBoss);

                if (currentConfig.willSpawnBoss && currentConfig.bossEnemy && currentConfig.bossEnemiesAmount > 0)
                {
                    for (var i = 0; i < currentConfig.bossEnemiesAmount; i++)
                        _bossWaveQueue.Enqueue(currentConfig.bossEnemy);
                }
            }

            // --- ОСТАЛЬНЫЕ ---
            EnqueueMany(currentConfig.basicEnemy, currentConfig.basicEnemiesAmount);

            if (currentConfig.willSpawnRed)
                EnqueueMany(currentConfig.redUnique, currentConfig.redEnemiesAmount);

            if (currentConfig.willSpawnBlue)
                EnqueueMany(currentConfig.blueUnique, currentConfig.blueEnemiesAmount);

            if (currentConfig.willSpawnYellow)
                EnqueueMany(currentConfig.yellowUnique, currentConfig.yellowEnemiesAmount);

            ReshuffleWaveQueue(); // перемешиваем только «обычных» (не боссов)
        }

        private void EnqueueMany(EnemyConfig config, int count)
        {
            if (!config || count <= 0) return;
            for (var i = 0; i < count; i++)
            {
                _waveQueue.Enqueue(config);
            }
        }

        private void ReshuffleWaveQueue()
        {
            var list = new List<EnemyConfig>(_waveQueue);
            var amount = list.Count;
            for (var i = amount - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            _waveQueue ??= new Queue<EnemyConfig>();
            _waveQueue.Clear();

            foreach (var c in list)
            {
                _waveQueue.Enqueue(c);
            }
        }

        private bool IsSuperBossConfig(EnemyConfig cfg)
        {
            return _currentWaveConfig?.willSpawnSuperBoss == true && cfg == _currentWaveConfig.superBoss;
        }

        private bool IsNormalBossConfig(EnemyConfig cfg)
        {
            // обычный босс в двух случаях: обычная босс-волна или extra-волна
            return _currentWaveConfig?.bossEnemy == cfg &&
                   (_currentWaveConfig.willSpawnBoss || _currentWaveConfig.willSpawnExtraWave);
        }
    }
}