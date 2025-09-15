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
        
        private void OnPlayerTriggered(PlayerModel player)
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
            if (target is PlayerModel player)
            {
                player.TakeDamage(damage);
            }
        }
    }
}