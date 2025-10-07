using UnityEngine;

namespace Runtime
{
    public enum EnemyType
    {
        Basic, 
        Red, 
        Blue, 
        Yellow, 
        Boss, 
        SuperBoss
    }
    
    [CreateAssetMenu(menuName = "My Scriptable Objects/Characters/Enemy/Base Config", fileName = "enemy_config")]
    public class EnemyConfig : CharacterBaseConfig
    {
        public EnemyType EnemyType;
        
        public float scaleModifier;
        public Material material;
        public EnemyDropConfig dropConfig;
        
        [Header("NavMesh settings")]
        public float angularSpeed;
        public float acceleration;
        public float stoppingDistance;
    }
}