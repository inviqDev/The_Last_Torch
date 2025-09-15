using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Spawner", fileName = "wave_config")]
    public class SpawnWaveConfig : ScriptableObject
    {
        public EnemyConfig basicEnemy;
        public int basicEnemiesAmount;
        
        public bool willSpawnRed;
        public EnemyConfig redUnique;
        public int redEnemiesAmount;
        
        public bool willSpawnBlue;
        public EnemyConfig blueUnique;
        public int blueEnemiesAmount;

        public bool willSpawnYellow;
        public EnemyConfig yellowUnique;
        public int yellowEnemiesAmount;

        public bool willSpawnBoss;
        public EnemyConfig bossEnemy;
        public int bossEnemiesAmount;
        
        public bool willSpawnSuperBoss;
        public EnemyConfig superBoss;

        public bool willSpawnExtraWave;
        
        public float spawningInterval;
    }
}