using UnityEngine;

namespace Runtime
{
    public class SkeletonEnemy : EnemyModel
    {
        [SerializeField] private EnemyTriggerDetector _triggerDetector;

        private void OnEnable()
        {
            _triggerDetector.OnTriggerWithPlayer += PerformAttack;
        }

        protected override void PerformAttack(CharacterBase target)
        {
            // play anim
            if (target is PlayerModel player)
            {
                player.TakeDamage(damage);
            }
            
        }

        private void OnDisable()
        {
            _triggerDetector.OnTriggerWithPlayer -= PerformAttack;
        }
    }
}