using UnityEngine;

namespace Runtime
{
    public class BasicEnemy : Enemy
    {
        [SerializeField] private EnemyAttackTrigger _attackTrigger;
        
        protected override void PerformAttack(Character target)
        {
            if (target is not Player player) return;
            player.TakeDamage(damage);
        }
        
        private void OnEnable()
        {
            _attackTrigger.OnPlayerEnter -= PerformAttack;
            _attackTrigger.OnPlayerEnter += PerformAttack;
        }
        
        private void OnDisable()
        {
            _attackTrigger.OnPlayerEnter -= PerformAttack;
        }
    }
}