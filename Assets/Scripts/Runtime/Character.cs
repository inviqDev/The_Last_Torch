using System;
using UnityEngine;

namespace Runtime
{
    public abstract class Character : MonoBehaviour
    {
        public Action<float> OnHealthChanged;
        public Action<Character> OnCharacterDeath;
        
        [SerializeField] protected HealthBar healthBar;
        
        [Header("Layer masks")]
        [SerializeField] protected LayerMask playerLayerMask;
        
        protected float maxHealth;
        protected float currentHealth;
        protected float moveSpeed;
        
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        
        public virtual void TakeDamage(float incomingDamage)
        {
            currentHealth = Mathf.Clamp(CurrentHealth - incomingDamage, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}