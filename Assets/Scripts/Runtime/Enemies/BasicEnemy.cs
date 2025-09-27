using UnityEngine;

namespace Runtime
{
    public class BasicEnemy : EnemyModel
    {
        [SerializeField] private EnemyAttackTrigger _attackTrigger;
        
        private void OnEnable()
        {
            _attackTrigger.OnPlayerEnter += OnPlayerTriggered;
        }
        
        private void OnPlayerTriggered(Player player)
        {
            PerformAttack(player);
        }
        
        private void OnDisable()
        {
            _attackTrigger.OnPlayerEnter -= OnPlayerTriggered;
        }
        
        protected override void PerformAttack(Character target)
        {
            if (target is not Player player) return;
            player.TakeDamage(damage);
        }
    }
}