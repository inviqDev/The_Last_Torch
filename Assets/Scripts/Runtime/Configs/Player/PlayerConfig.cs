using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Configs/Characters/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : CharacterBaseConfig
    {
        public AbilityConfig defaultAbility;
        
        [Header("Dash settings")]
        public float dashSpeed;
        public float dashDuration;
    }
}