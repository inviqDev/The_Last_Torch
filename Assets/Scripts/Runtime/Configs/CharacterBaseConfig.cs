using UnityEngine;

namespace Runtime
{
    public class CharacterBaseConfig : ScriptableObject
    {
        [Header("MaxHealth settings")]
        public float maxHealth;
        
        [Header("Movement settings")]
        public float moveSpeed;
    
        [Header("Attack settings")]
        public float damage;
    }
}