using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "My Scriptable Objects/Characters/Enemy/Drop Config", fileName = "enemy_drop")]
    public class EnemyDropConfig : ScriptableObject
    {
        public EnemyDrop dropGO;
        public Material dropMaterial;
        
        public float expGained;

        public float healthBoost;
        public float moveSpeedBoost;
        public float damageBoost;
    }
}
