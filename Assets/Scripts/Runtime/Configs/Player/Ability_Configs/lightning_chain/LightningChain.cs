using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class LightningChain : AbilityVFX
    {
        [Header("Chain")]
        [SerializeField] private int   maxBounces = 4;      // доп. прыжки ПОСЛЕ первого удара
        [SerializeField] private float hopRadius  = 5f;     // максимум до следующей цели
        [SerializeField] private float hopDelay   = 0.03f;  // маленькая пауза между звеньями (0..0.06)
        [SerializeField] private float hopSpeed   = 0f;     // если >0, задержка = dist / speed

        [Header("Segment")]
        [SerializeField] private LightningBolt boltPrefab;
        [SerializeField] private float segmentDuration = 0.12f; // жизни одного звена
        [SerializeField] private bool  waitSegmentsToFinish = false; // ждать окончания визуала всех звеньев перед КД

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
            var current = ctx.InitialTarget;

            if (!boltPrefab)
            {
                Debug.LogError("[LightningChain] boltPrefab not set");
                RaiseFinished(ability);
                yield break;
            }

            affected.Clear();

            int linksLeft = 1 + maxBounces;       // общее число звеньев
            Transform from = player.transform;

            // Для опционального ожидания окончания всех сегментов:
            // float lastSegmentTotalTime = segmentDuration + boltPrefab.GetComponent<LightningBolt>() ? 0.06f : 0f;

            while (current && linksLeft > 0)
            {
                if (!current.gameObject.activeInHierarchy || current.CurrentHealth <= 0) break;

                // 1) визуал звена — спавним и не ждём
                var seg = Instantiate(boltPrefab, Vector3.zero, Quaternion.identity);

                float duration;
                bool needToFollow;
                
                if (from == ctx.Player.transform)
                {
                    needToFollow = false;
                    duration = 0.25f;
                }
                else
                {
                    duration = segmentDuration;
                    needToFollow = true;
                }
                
                seg.Play(from, current.transform, duration, needToFollow);

                // 2) урон — сразу
                current.TakeDamage(ability.Damage);
                affected.Add(current);

                // 3) готовим следующее звено
                linksLeft--;
                if (linksLeft <= 0) break;

                var next = FindNextEnemy(current, ctx.EnemiesCollector.AttackableEnemies, hopRadius);
                if (!next) break;

                // задержка между прыжками: либо фикс, либо «время перелёта»
                if (hopSpeed > 0f)
                {
                    float dist = (next.transform.position - current.transform.position).magnitude;
                    float travel = dist / hopSpeed;
                    if (travel > 0f) yield return new WaitForSeconds(travel);
                }
                else if (hopDelay > 0f)
                {
                    yield return new WaitForSeconds(hopDelay);
                }

                from = current.transform;
                current = next;
            }

            // хотим ли ждать полного затухания последнего сегмента?
            if (waitSegmentsToFinish)
                yield return new WaitForSeconds(segmentDuration /* + fade, если нужно */);

            RaiseFinished(ctx.Ability);
            gameObject.SetActive(false);
        }

        private EnemyModel FindNextEnemy(EnemyModel from, List<EnemyModel> list, float radius)
        {
            if (list == null || list.Count == 0) return null;

            EnemyModel best = null;
            float bestSqr = float.MaxValue;
            float maxSqr = radius * radius;

            for (int i = 0; i < list.Count; i++)
            {
                var e = list[i];
                if (!e) continue;
                if (ReferenceEquals(e, from)) continue;
                if (affected.Contains(e)) continue;
                if (!e.gameObject.activeInHierarchy || e.CurrentHealth <= 0) continue;

                float sqr = (e.transform.position - from.transform.position).sqrMagnitude;
                if (sqr > maxSqr) continue;
                if (sqr < bestSqr) { bestSqr = sqr; best = e; }
            }
            return best;
        }
    }
}
