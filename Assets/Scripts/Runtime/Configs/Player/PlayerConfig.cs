using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Ability_Configs/Characters/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : CharacterBaseConfig
    {
        // [Header("Ability settings")]
        // public AbilityConfig[] abilities;
        
        public float startExp;
        
        [Header("Dash settings")]
        public float dashSpeed;
        public float dashDuration;
    }
}