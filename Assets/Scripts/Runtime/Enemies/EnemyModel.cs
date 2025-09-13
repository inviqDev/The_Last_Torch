using System;
using UnityEngine;

namespace Runtime
{
    public class EnemyModel : CharacterBase, IPoolable
    {
        public event Action<EnemyModel> OnEnemyDeath;
        
        [Header("Unique Key in pool dictionary")]
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;
        

        [SerializeField] private EnemyMovement mover;

        protected float damage;
        
        private float _angularSpeed;
        private float _acceleration;
        private float _stoppingDistance;
        
        private EnemyDropConfig dropConfig;
        
        public EnemyMovement Mover => mover;

        private void Start()
        {
            Debug.Assert(GameManager.Instance, "GameManager has not been found");
        }
        public void SetConfig(EnemyConfig config)
        {
            var player = GameManager.Instance.Player;

            dropConfig = config.dropConfig;
            
            health = config.maxHealth;
            currentHealth = health;
            moveSpeed = config.moveSpeed;
            
            healthBar.Init(this);

            _angularSpeed = config.angularSpeed;
            _acceleration = config.acceleration;
            _stoppingDistance = config.stoppingDistance;

            damage = config.damage;

            mover.ApplyMovementConfig(player,
                moveSpeed, _angularSpeed, _acceleration, _stoppingDistance);
        }

        public override void TakeDamage(float incomingDamage)
        {
            base.TakeDamage(incomingDamage);
            if (!(currentHealth <= 0)) return;
            
            LaunchOnEnemyDeathLogic();
            OnEnemyDeath?.Invoke(this);
        }

        protected virtual void LaunchOnEnemyDeathLogic()
        {
            mover.StopAndReset();
            
            var drop = Instantiate(dropConfig.dropGO, transform.position, Quaternion.identity);
            drop.GetComponent<EnemyDrop>().SetExpGainedAmount(dropConfig.expGained);
        }

        protected virtual void PerformAttack(CharacterBase target)
        {
        }
        
        public void OnGetFromPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            gameObject.SetActive(false);
            transform.SetParent(EnemySpawner.Instance?.transform);
        }
    }
}