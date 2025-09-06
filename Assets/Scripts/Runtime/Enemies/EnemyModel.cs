using System;
using UnityEngine;

namespace Runtime
{
    public class EnemyModel : MonoBehaviour
    {
        public event Action<EnemyModel> OnEnemyDeath;

        // [SerializeField] private MeshFilter meshFilter;
        
        [SerializeField] private EnemyMovement mover;
        public EnemyMovement Mover => mover;
    
        #region REMOVE "FOR TESTING" SERIALIZED FIELDS => MAKE PRIVATE

        [field: SerializeField] public float MaxHealth { get; private set; }
        [field: SerializeField] public float CurrentHealth { get; private set; }

        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float AngularSpeed { get; private set; }
        [field: SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public float StoppingDistance { get; private set; }

        
        [field: SerializeField] public float Damage { get; private set; }

        #endregion
    
        private void Start()
        {
            Debug.Assert(GameManager.Instance, "GameManager has not been found");
        }

        public void SetConfig(EnemyConfig config)
        {
            var player = GameManager.Instance.Player;
        
            MaxHealth = config.maxHealth;
            CurrentHealth = MaxHealth;
        
            MoveSpeed = config.moveSpeed;
            AngularSpeed = config.angularSpeed;
            Acceleration = config.acceleration;
            StoppingDistance = config.stoppingDistance;
        
            Damage = config.damage;

            mover.ApplyMovementConfig(player, 
                MoveSpeed, AngularSpeed, Acceleration, StoppingDistance);
            
            
        }

        public void TakeDamage(float incomingDamage)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - incomingDamage, 0, MaxHealth);

            if (CurrentHealth <= 0)
            {
                // Add DROP item logic here
                
                // Move from here to pool
                PrepareToPool();
                
                OnEnemyDeath?.Invoke(this);
            }
        }

        private void PrepareToPool()
        {
            mover.StopAndReset();
            gameObject.SetActive(false);
        }

        protected virtual void PerformAttack(CharacterBase target)
        {
        }
    }
}