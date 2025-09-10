using System;
using UnityEngine;

namespace Runtime
{
    public abstract class CharacterBase : MonoBehaviour
    {
        public Action<float> OnHealthChanged;
        
        [SerializeField] protected HealthBar healthBar;
        
        protected float health;
        protected float currentHealth;
        protected float moveSpeed;
        
        public float CurrentHealth => currentHealth;
        
        public virtual void TakeDamage(float incomingDamage)
        {
            currentHealth = Mathf.Clamp(CurrentHealth - incomingDamage, 0, health);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}