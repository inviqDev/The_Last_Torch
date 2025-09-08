using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    [CreateAssetMenu(fileName = "AbilityConfig", menuName = "ScriptableObjects/AbilityConfig")]
    public class AbilityConfig : ScriptableObject
    {
        public new string name;
        public Sprite icon;
        
        public GameObject prefab;
        
        public float attackRange;
        public float damage;
        
        public float cooldown;
    }
}

// public enum ActivationType
// {
//     AutoAttack,
//     Target,
// }
//
// public ActivationType activationType;