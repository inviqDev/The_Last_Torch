using System.Collections.Generic;
using UnityEngine;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [DefaultExecutionOrder(-998)]
    public class EnemySpawner : Singleton<EnemySpawner>
    {
        [System.Serializable]
        public struct prefabBuild
        {
            public EnemyType enemyType;
            public EnemyModel enemyPrefab;
        }
        
        [SerializeField] private SpawnWaveConfig[] waveConfigs;
        [SerializeField] private int currentWaveIndex;
        
        [SerializeField] private Transform[] nonBossPoints;
        [SerializeField] private Transform[] bossPoints;
        [SerializeField] private Transform superBossSpawnPoint;
        
        [SerializeField] private prefabBuild[] prefabBuilds;
        private Dictionary<EnemyType, EnemyModel> _enemyDictionary;
        
        private readonly Queue<EnemyConfig> _bossWaveQueue = new();
        private readonly Queue<EnemyConfig> _waveQueue = new();

        private EnemyType _enemyType;
        private Timer _timer;
        
        private SpawnWaveConfig _currentWaveConfig;
        private float _spawnInterval;
        
        private int _bossSpawnPointIndex;
        private int _extraWaveLocalBossIndex;

        protected override void Awake()
        {
            base.Awake();

            _timer = new Timer(this);
            _bossSpawnPointIndex = 0;
            
            _enemyDictionary = new Dictionary<EnemyType, EnemyModel>(prefabBuilds.Length);
            foreach (var e in prefabBuilds)
            {
                MyAsserts.IsNotNull(e.enemyPrefab, "Prefab build is invalid");
                _enemyDictionary[e.enemyType] = e.enemyPrefab;
            }
        }

        public void SpawnNextWave()
        {
            if (currentWaveIndex < 0 || currentWaveIndex >= waveConfigs.Length)
            {
                MyAsserts.IsTrue(currentWaveIndex < 0 || currentWaveIndex >= waveConfigs.Length, 
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

            _timer.StopTimer();
            _timer.OnTicked += SpawnEnemy;
            _timer.TimerIsOver += WaveIsFullyReleased;


            var totalAmount = _waveQueue.Count + _bossWaveQueue.Count;
            _timer.StartTimerTicker(_spawnInterval, totalAmount);
            // при желании: UI-событие "WaveStarted(currentWaveIndex)"
        }

        private void WaveIsFullyReleased()
        {
            _timer.StopTimer();
            
            _timer.OnTicked -= SpawnEnemy;
            _timer.TimerIsOver -= WaveIsFullyReleased;

            print($"[EnemySpawner] Wave #{currentWaveIndex} released.");
            
            currentWaveIndex++;
            SpawnNextWave();
        }

        private void SpawnEnemy(int _)
        {
            if (_bossWaveQueue.Count == 0 && _waveQueue.Count == 0) return;

            var config = _bossWaveQueue.Count > 0 
                ? _bossWaveQueue.Dequeue() 
                : _waveQueue.Dequeue();
            
            if (!config)
            {
                MyAsserts.IsNotNull(config, "config is invalid");
            }
            
            Transform spawnPoint;
            if (IsSuperBossConfig(config))
            {
                MyAsserts.IsNotNull(superBossSpawnPoint, "super boss spawn point is not set");
                spawnPoint = superBossSpawnPoint;
            }
            else if (IsNormalBossConfig(config))
            {
                MyAsserts.IsTrue(bossPoints.Length > 0, "bossPoints array is not set");
                MyAsserts.IsTrue(_bossSpawnPointIndex < bossPoints.Length, "boss spawn point index is out of range");

                if (_currentWaveConfig.willSpawnExtraWave)
                {
                    spawnPoint = bossPoints[_extraWaveLocalBossIndex % bossPoints.Length];
                    _extraWaveLocalBossIndex++;
                }
                else
                {
                    spawnPoint = bossPoints[_bossSpawnPointIndex % bossPoints.Length];
                    _bossSpawnPointIndex++;
                }
            }
            else
            {
                MyAsserts.IsTrue(nonBossPoints.Length > 0, "nonBossPoints array is not set");
                spawnPoint = nonBossPoints[Random.Range(0, nonBossPoints.Length)];
            }
            
            var prefab = GetEnemyTypePrefab(config);
            MyAsserts.IsNotNull(prefab, $"There is no prefab mapped for {config.EnemyType}");
            
            var enemy = Pool.Instance?.TryGetObjectFromPool(prefab);
            MyAsserts.IsNotNull(enemy, "enemy is not spawned");
            
            MyAsserts.IsNotNull(spawnPoint, "point is not set properly");
            enemy.Mover.WarpTo(spawnPoint.position);

            enemy.OnCharacterDeath += MoveEnemyToPool;
            enemy.SetConfig(config);
        }

        private EnemyModel GetEnemyTypePrefab(EnemyConfig config)
        {
            if (!config) return null;
            MyAsserts.IsNotNull(_enemyDictionary, "enemy dictionary is not set");
            return _enemyDictionary.GetValueOrDefault(config.EnemyType);
        }
        
        private void MoveEnemyToPool(Character enemy)
        {
            enemy.OnCharacterDeath -= MoveEnemyToPool;
            Pool.Instance?.ReturnToPool(enemy as EnemyModel);
        }
        
        private void BuildWaveQueue(SpawnWaveConfig currentConfig)
        {
            _bossWaveQueue.Clear();
            _waveQueue.Clear();

            // --- БОССЫ ---
            if (currentConfig.willSpawnExtraWave)
            {
                // ExtraWave: всегда один супербосс (если задан) + N обычных боссов
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
                // Обычная волна: опционально супербосс и/или N обычных боссов
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
            if (currentConfig.willSpawnRed)    EnqueueMany(currentConfig.redUnique,    currentConfig.redEnemiesAmount);
            if (currentConfig.willSpawnBlue)   EnqueueMany(currentConfig.blueUnique,   currentConfig.blueEnemiesAmount);
            if (currentConfig.willSpawnYellow) EnqueueMany(currentConfig.yellowUnique, currentConfig.yellowEnemiesAmount);

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
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            
            _waveQueue.Clear();
            foreach (var c in list) _waveQueue.Enqueue(c);
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
            

            // return _currentWaveConfig?.bossEnemy &&
            //                (_currentWaveConfig.willSpawnBoss || _currentWaveConfig.willSpawnExtraWave);
        }
    }
}