using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Characters/Enemy", fileName = "enemy_config")]
    public class EnemyConfig : CharacterBaseConfig
    {
        public float localScaleModifier;
        public Material material;
        public EnemyDropConfig dropConfig;
        
        [Header("NavMesh settings")]
        public float angularSpeed;
        public float acceleration;
        public float stoppingDistance;
    }
}