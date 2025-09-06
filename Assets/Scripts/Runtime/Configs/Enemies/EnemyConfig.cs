using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Configs/Characters/Enemy", fileName = "EnemyConfig")]
    public class EnemyConfig : CharacterBaseConfig
    {
        [Header("NavMesh settings")]
        public float angularSpeed;
        public float acceleration;
        public float stoppingDistance;
    }
}