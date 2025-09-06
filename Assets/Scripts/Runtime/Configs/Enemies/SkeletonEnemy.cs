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
            
            var dam = Damage;
            var player = target as PlayerModel;

            player?.TakeDamage(dam);
        }

        private void OnDisable()
        {
            _triggerDetector.OnTriggerWithPlayer -= PerformAttack;
        }
    }
}