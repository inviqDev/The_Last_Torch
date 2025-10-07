using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "My Scriptable Objects/Characters/Player/Ability Config", fileName = "player_ability")]
    public class AbilityConfig : ScriptableObject
    {
        public string abilityName;
        public Sprite abilityIcon;

        public string HitParticlePoolKey;
        public string HitSoundPoolKey;

        public float progressTime;
        public float cooldownTime;
        
        public AbilityVFX abilityVFX;
        public float maxAttackDistance;

        public float damage;
    }
}
