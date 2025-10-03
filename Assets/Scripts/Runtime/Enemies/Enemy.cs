using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class Enemy : Character, IPoolable
    {
        public Action<EnemyConfig> OnConfigLoaded;

        [Header("Unique Key in pool dictionary")] 
        [SerializeField] protected string uniquePoolKey;

        [SerializeField] private MeshRenderer meshRenderer;

        protected float damage;
        
        private EnemyDropConfig dropConfig;
        private EnemyDrop dropGO;
        
        private List<Particle> _affectedParticles;

        public string UniquePoolKey => uniquePoolKey;
        public EnemyMovement Movement { get; private set; }

        public void InitializeEnemy(Player player, EnemyConfig config, Vector3 startPoint, float multiplier = 1f)
        {
            maxHealth = config.maxHealth * multiplier;
            currentHealth = maxHealth;

            damage = config.damage * multiplier;

            meshRenderer.material = config.material;
            transform.localScale = Vector3.one * config.scaleModifier;

            dropConfig = config.dropConfig;
            dropGO = config.dropConfig.dropGO;

            healthBar.Initialize(this);

            Movement ??= GetComponent<EnemyMovement>();
            UnityEngine.Assertions.Assert.IsNotNull(Movement, "enemy movement component is missing");
            Movement.Initialize(player, startPoint, config);

            OnConfigLoaded?.Invoke(config);
        }

        protected override void LaunchOnCharacterDeathLogic()
        {
            GameManager.Instance?.UIManager.ChangeKillsBarInfo(this);
            
            var drop = Pool.Instance?.TryGet(dropGO);
            UnityEngine.Assertions.Assert.IsNotNull(drop, "drop game object is missing");
            if (!drop) return;
            
            var dropPos = new Vector3(transform.position.x, 1f, transform.position.z);
            drop.SetUpDropFromConfig(dropConfig, dropPos);

            base.LaunchOnCharacterDeathLogic();
        }

        protected virtual void PerformAttack(Character target)
        {
        }

        public void AddAffectedParticle(Particle particle)
        {
            _affectedParticles ??= new List<Particle>();
            _affectedParticles.Add(particle);
        }

        public void OnGetFromPool()
        {
            gameObject.SetActive(true);
            transform.SetParent(null);
            Movement?.ActivateNavMeshAgent();
        }

        public void OnReturnToPool()
        {
            if (_affectedParticles is { Count: > 0 })
            {
                foreach (var p in _affectedParticles)
                {
                    p.ReturnParticleToPool();
                }
                
                _affectedParticles.Clear();
            }
            
            Movement.DeactivateNavMeshAgent();
            transform.SetParent(Pool.Instance?.EnemiesRoot);
            
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _affectedParticles ??= new List<Particle>();
            _affectedParticles.Clear();
        }

        private void OnDisable()
        {
            _affectedParticles?.Clear();
        }

        private void OnDestroy()
        {
            _affectedParticles?.Clear();
        }
    }
}