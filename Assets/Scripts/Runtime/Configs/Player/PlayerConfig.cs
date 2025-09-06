using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Configs/Characters/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : CharacterBaseConfig
    {
        [Header("Dash settings")]
        public float dashSpeed;
        public float dashDuration;
    }
}