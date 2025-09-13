using System;
using UnityEngine;

namespace Runtime
{
    public class EnemyModel : Character, IPoolable
    {
        public event Action<EnemyModel> OnEnemyDeath;
        
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
        private float _stoppingDistance;
        
        protected float damage;
        
        public EnemyMovement Mover => mover;

        private void Start()
        {
            Debug.Assert(GameManager.Instance, "GameManager has not been found");
        }
        
        public void SetConfig(EnemyConfig config)
        {
            var player = GameManager.Instance.Player;

            health = config.maxHealth;
            currentHealth = health;
            moveSpeed = config.moveSpeed;
            
            dropConfig = config.dropConfig;
            dropGO = config.dropConfig.dropGO;
            
            transform.localScale *= config.localScaleModifier;
            dropMaterial = config.material;
            meshRenderer.material = dropMaterial;
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

            var drop = Pool.Instance?.TryGetObjectFromPool(dropGO);
            UnityEngine.Assertions.Assert.IsNotNull(drop, "drop object not found");
            
            drop.transform.position = transform.position;
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
            gameObject.SetActive(false);
            transform.SetParent(EnemySpawner.Instance?.transform);
        }
    }
}