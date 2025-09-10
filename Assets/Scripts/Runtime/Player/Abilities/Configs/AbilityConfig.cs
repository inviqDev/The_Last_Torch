using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(fileName = "AbilityConfig", menuName = "Player/Abilities/AbilityConfig")]
    public class AbilityConfig : ScriptableObject
    {
        public string abilityName;
        public Sprite abilityIcon;
        
        public float cooldown;
        public GameObject abilityPrefab;

        public float damage;
    }
}
