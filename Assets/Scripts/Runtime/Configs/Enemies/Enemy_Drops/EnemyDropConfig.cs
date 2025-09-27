using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(fileName = "DropConfig", menuName = "Enemy/Drop")]
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
