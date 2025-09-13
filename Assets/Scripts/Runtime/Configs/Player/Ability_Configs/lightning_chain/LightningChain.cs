using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class LightningChain : AbilityVFX
    {
        [Header("Chain")] 
        [SerializeField] private int bouncesAmount = 5;
        [SerializeField] private float maxBounceDistance = 5f;

        [Header("Segment")] 
        [SerializeField] private LightningBolt boltPrefab;
        [SerializeField] private float boltAnimDuration = 0.12f;
        [SerializeField] private bool waitBoltAnimFinish;

        private readonly List<EnemyModel> affected = new();

        protected override void OnPlay(AbilityContext ctx)
        {
            StopAllCoroutines();
            StartCoroutine(ChainRoutine(ctx));
        }

        private IEnumerator ChainRoutine(AbilityContext ctx)
        {
            var player = ctx.Player;
            var ability = ctx.Ability;
            var currentEnemy = ctx.InitialTarget;

            if (!boltPrefab)
            {
                UnityEngine.Assertions.Assert.IsNotNull(boltPrefab, "[LightningChain] boltPrefab not set");

                RaiseFinished(ability);
                yield break;
            }

            affected.Clear();

            var bouncesLeft = bouncesAmount;
            var from = player.transform;

            while (currentEnemy && bouncesLeft > 0)
            {
                var bolt = Pool.Instance?.TryGetObjectFromPool(boltPrefab);
                UnityEngine.Assertions.Assert.IsNotNull(bolt, "[Lightning Bolt] is not spawned");

                float duration;
                bool needToFollow;
                if (from == ctx.Player.transform)
                {
                    needToFollow = false;
                    duration = 0.15f;
                }
                else
                {
                    duration = this.boltAnimDuration;
                    needToFollow = true;
                }

                bolt.Launch(from, currentEnemy.transform, duration, needToFollow);

                currentEnemy.TakeDamage(ability.Damage);
                affected.Add(currentEnemy);

                bouncesLeft--;
                if (bouncesLeft <= 0) break;

                var nextTarget = FindClosestEnemy(currentEnemy, ctx.EnemiesCollector.AttackableEnemies);
                if (!nextTarget) break;

                from = currentEnemy.transform;
                currentEnemy = nextTarget;
            }

            if (waitBoltAnimFinish)
            {
                yield return new WaitForSeconds(boltAnimDuration);
            }

            RaiseFinished(ctx.Ability);
        }

        private EnemyModel FindClosestEnemy(EnemyModel from, List<EnemyModel> attackableEnemies)
        {
            if (attackableEnemies == null || attackableEnemies.Count == 0) return null;

            EnemyModel closestEnemy = null;
            var maxSqrMag = maxBounceDistance * maxBounceDistance;

            foreach (var e in attackableEnemies)
            {
                if (!e || ReferenceEquals(e, from) || affected.Contains(e)) continue;
                if (!e.gameObject.activeInHierarchy || e.CurrentHealth <= 0) continue;

                var sqr = (e.transform.position - from.transform.position).sqrMagnitude;
                if (sqr > maxSqrMag) continue;
                
                closestEnemy = e;
            }

            return closestEnemy;
        }
    }
}