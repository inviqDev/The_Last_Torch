using System;
using UnityEngine;

namespace Runtime
{
    public class EnemyModel : Character, IPoolable
    {
        public Action<EnemyConfig> OnConfigLoaded;
        
        [Header("Unique Key in pool dictionary")]
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;

        protected float damage;
        
        [SerializeField] private MeshRenderer meshRenderer;

        private float _angularSpeed;
        private float _acceleration;
        private float stoppingDistance;
        
        private EnemyDropConfig dropConfig;
        private EnemyDrop dropGO;
        
        private EnemyMovement _mover;
        public EnemyMovement Mover => _mover;

        private void Start()
        {
            MyAssertions.EnsureIsNotNull(GameManager.Instance);
        }

        public void SetEnemyConfig(EnemyConfig config, float multiplier = 1f)
        {
            var player = GameManager.Instance?.Player;
            MyAssertions.EnsureIsNotNull(player);
            
            if (!player)
            {
                print("Player is missing");
                
                _mover?.StopAndReset();
                return;
            }
            
            maxHealth = config.maxHealth * multiplier;
            currentHealth = maxHealth;
            
            moveSpeed = config.moveSpeed;
            _angularSpeed = config.angularSpeed;
            _acceleration = config.acceleration;
            stoppingDistance = config.stoppingDistance;

            damage = config.damage * multiplier;
            
            meshRenderer.material = config.material;
            transform.localScale = Vector3.one * config.scaleModifier;
            
            dropConfig = config.dropConfig;
            dropGO = config.dropConfig.dropGO;

            healthBar.Init(this);

            _mover ??= GetComponent<EnemyMovement>();
            MyAssertions.EnsureIsNotNull(_mover);
            _mover.ApplyMovementConfig(player, moveSpeed, 
                _angularSpeed, _acceleration, stoppingDistance);
                
            OnConfigLoaded?.Invoke(config);
        }

        public override void LaunchOnCharacterDeathLogic()
        {
            GameManager.Instance?.PrintEnemyDeathCounter();
            
            var drop = Pool.Instance?.TryGetObjectFromPool(dropGO);
            MyAssertions.EnsureIsNotNull(drop);
            if (!drop) return;
            
            var dropPos = new Vector3(transform.position.x, 1f, transform.position.z);
            drop.SetUpDropFromConfig(dropConfig, dropPos);
            
            base.LaunchOnCharacterDeathLogic();
        }

        protected virtual void PerformAttack(Character target)
        {
        }
        
        public void OnGetFromPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(true);
        }
        
        public void OnReturnToPool()
        {
            _mover.StopAndReset();
            transform.SetParent(Pool.Instance?.EnemiesRoot);
            
            gameObject.SetActive(false);
        }
    }
}