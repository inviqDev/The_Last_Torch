using System;
using System.Collections;
using UnityEngine;


namespace Runtime
{
    [System.Serializable]
    public struct ProjectileSettings
    {
        public float sizeMultiplier;
        public float moveSpeed;
    }
    
    public class BossEnemy : Enemy
    {
        [SerializeField] private ProjectileSettings projectileSettings;
        
        [SerializeField] private EnemyAttackTrigger attackTrigger;
        [SerializeField] private Projectile projectile;
        [SerializeField] private float cooldown = 2.0f;
        
        private Player _player;
        private Coroutine _attackLoop;
        
        private void OnEnable()
        {
            UnityEngine.Assertions.Assert.IsNotNull(attackTrigger, "attackTrigger is not set");
            if (!attackTrigger) return;
            
            attackTrigger.OnPlayerEnter += OnPlayerEnter;
            attackTrigger.OnPlayerExit  += OnPlayerExit;
        }
        
        private void OnPlayerEnter(Player player)
        {
            _player = player;
            
            _player.OnCharacterDeath += StopAttackingOnPlayerDeath;
            StartAttackingProcess();
        }

        private void StopAttackingOnPlayerDeath(Character player)
        {
            if (!_player)
            {
                return;
            }

            _player.OnCharacterDeath -= StopAttackingOnPlayerDeath;
            _player = null;
            
            StopAttackLoop();
        }

        private void OnPlayerExit(Player player)
        {
            _player = null;
            StopAttackLoop();
        }
        
        private void StartAttackingProcess()
        {
            if (_attackLoop != null) return;
            
            _attackLoop = StartCoroutine(AttackLoop());
        }

        private void StopAttackLoop()
        {
            if (_attackLoop == null) return;
            
            StopCoroutine(_attackLoop);
            _attackLoop = null;
        }
        
        private IEnumerator AttackLoop()
        {
            var wait = new WaitForSeconds(cooldown);

            while (true)
            {
                if (!_player || !_player.gameObject.activeInHierarchy)
                {
                    _attackLoop = null;
                    yield break;
                }

                PerformAttack(_player);
                yield return wait;

                if (_player) continue;
                
                _attackLoop = null; 
                yield break;
            }
        }

        protected override void PerformAttack(Character target)
        {
            var proj = Pool.Instance?.TryGet(projectile);
            UnityEngine.Assertions.Assert.IsNotNull(proj, "projectile is not spawned");

            proj.ChangeProjectileBasicSettings(projectileSettings);
            
            proj.OnMoveToPool += OnMoveToPool;
            proj.OnPlayerDamaged += OnPlayerDamaged;
            
            var offset = transform.forward * (transform.localScale.x * 0.5f) + transform.forward;
            
            var origin = transform.position + offset;
            var targetPos = target.transform.position;
            
            proj.LaunchProjectile(origin, targetPos);
        }

        private void OnPlayerDamaged(Player player)
        {
            player.TakeDamage(damage);
        }

        private void OnMoveToPool(Projectile proj)
        {
            proj.OnPlayerDamaged -= OnPlayerDamaged;
            proj.OnMoveToPool -= OnMoveToPool;
        }

        private void OnDisable()
        {
            if (attackTrigger)
            {
                attackTrigger.OnPlayerEnter -= OnPlayerEnter;
                attackTrigger.OnPlayerExit  -= OnPlayerExit;
            }
            
            StopAttackLoop();
        }
    }
}