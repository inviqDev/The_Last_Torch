using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Ability_Configs/Characters/Enemy", fileName = "EnemyConfig")]
    public class EnemyWaveConfig : ScriptableObject
    {
        public EnemyConfig[] enemyConfigs;
    }
}