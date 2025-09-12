using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(fileName = "DropConfig", menuName = "Enemy/Drop")]
    public class EnemyDropConfig : ScriptableObject
    {
        public GameObject dropGO;
        public float expGained;
    }
}
