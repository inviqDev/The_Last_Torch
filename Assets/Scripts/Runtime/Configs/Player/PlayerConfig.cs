using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "My Scriptable Objects/Characters/Player/Base Config", fileName = "player_config")]
    public class PlayerConfig : CharacterBaseConfig
    {
        public float startExp;
        
        [Header("Dash settings")]
        public float dashSpeed;
        public float dashDuration;
        public float dashCooldown;
    }
}