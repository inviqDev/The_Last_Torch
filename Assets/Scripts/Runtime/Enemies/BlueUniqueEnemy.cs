using UnityEngine;

namespace Runtime
{
    public class BlueUniqueEnemy : EnemyModel
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
            _attackTrigger.OnPlayerEnter += OnPlayerTriggered;
        }
        
        protected override void PerformAttack(Character target)
        {
            // play anim, particles, etc
            if (target is Player player)
            {
                player.TakeDamage(damage);
            }
        }
    }
}