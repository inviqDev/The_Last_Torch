using UnityEngine;

namespace Runtime
{
    public class CharacterBaseConfig : ScriptableObject
    {
        [Header("Health settings")]
        public float maxHealth;
        
        [Header("Movement settings")]
        public float moveSpeed;
    
        [Header("Ability settings")]
        public Ability startAbility;
        
        [Header("Attack settings")]
        public float damage;
    }
}