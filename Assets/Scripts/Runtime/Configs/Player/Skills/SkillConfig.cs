using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Player/Skills", fileName = "SkillConfig")]
    public class SkillConfig : ScriptableObject
    {
        [Header("Skill logo")]
        public Sprite logo;
        
        [Header("Cooldown in seconds")]
        public float cooldownTime;
        
        [Header("Skill damage")]
        public float damage;
    }
}
