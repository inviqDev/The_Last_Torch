using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(fileName = "AbilityConfig", menuName = "Player/Abilities/AbilityConfig")]
    public class AbilityConfig : ScriptableObject
    {
        public string abilityName;
        public Sprite abilityIcon;

        public string SoundUniquePoolKey;
        public string ParticlesUniquePoolKey;

        public float progressTime;
        public float cooldownTime;
        
        public AbilityVFX abilityVFX;
        public float minAttackDistance;

        public float damage;
    }
}
