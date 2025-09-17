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

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private EnemyMovement mover;

        private EnemyDropConfig dropConfig;
        private EnemyDrop dropGO;
        private Material dropMaterial;
        
        private float _angularSpeed;
        private float _acceleration;
        protected float stoppingDistance;
        
        protected float damage;
        
        public EnemyMovement Mover => mover;

        private void Start()
        {
            Debug.Assert(GameManager.Instance, "GameManager has not been found");
        }
        
        public void SetConfig(EnemyConfig config)
        {
            var player = GameManager.Instance?.Player;

            maxHealth = config.maxHealth;
            currentHealth = maxHealth;
            moveSpeed = config.moveSpeed;
            
            dropConfig = config.dropConfig;
            dropGO = config.dropConfig.dropGO;
            dropMaterial = config.material;
            meshRenderer.sharedMaterial = dropMaterial;
            
            transform.localScale = Vector3.one * config.scaleModifier;
            
            _angularSpeed = config.angularSpeed;
            _acceleration = config.acceleration;
            stoppingDistance = config.stoppingDistance;

            damage = config.damage;

            healthBar.Init(this);
            mover.ApplyMovementConfig(player,
                moveSpeed, _angularSpeed, _acceleration, stoppingDistance);
            
            OnConfigLoaded?.Invoke(config);
        }

        public override void TakeDamage(float incomingDamage)
        {
            base.TakeDamage(incomingDamage);
            if (!(currentHealth <= 0)) return;
            
            LaunchOnEnemyDeathLogic();
            OnCharacterDeath?.Invoke(this);
        }

        protected virtual void LaunchOnEnemyDeathLogic()
        {
            mover.StopAndReset();

            var drop = Pool.Instance?.TryGetObjectFromPool(dropGO);
            UnityEngine.Assertions.Assert.IsNotNull(drop, "drop object not found");
            
            drop.transform.position = new Vector3(transform.position.x, 0.65f, transform.position.z);;
            drop.MeshRenderer.material = dropMaterial;
            drop.SetUpConfigValues(dropConfig);
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
            transform.SetParent(Pool.Instance?.EnemiesRoot);
            mover.StopAndReset();
            gameObject.SetActive(false);
        }
    }
}