using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Ability_Configs/Characters/Enemy", fileName = "EnemyConfig")]
    public class EnemyConfig : CharacterBaseConfig
    {
        public EnemyDropConfig dropConfig;
        
        [Header("NavMesh settings")]
        public float angularSpeed;
        public float acceleration;
        public float stoppingDistance;
    }
}