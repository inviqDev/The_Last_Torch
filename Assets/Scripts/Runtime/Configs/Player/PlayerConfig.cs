using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Ability_Configs/Characters/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : CharacterBaseConfig
    {
        public float startExp;
        
        [Header("Dash settings")]
        public float dashSpeed;
        public float dashDuration;
        public float dashCooldown;
    }
}