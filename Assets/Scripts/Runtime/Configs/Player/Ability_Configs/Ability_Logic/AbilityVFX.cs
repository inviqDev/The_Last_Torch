using System;
using UnityEngine;

namespace Runtime
{
    public readonly struct AbilityContext
    {
        public readonly PlayerModel Player;
        public readonly EnemyModel InitialTarget;
        public readonly EnemiesCollector EnemiesCollector;
        public readonly Ability Ability;

        public AbilityContext(PlayerModel player, EnemyModel initialTarget,
            EnemiesCollector enemiesCollector, Ability ability)
        {
            Player = player;
            InitialTarget = initialTarget;
            EnemiesCollector = enemiesCollector;
            Ability = ability;
        }
    }

    public abstract class AbilityVFX : MonoBehaviour, IPoolable
    {
        /// Сигнал «визуал полностью закончился» (ровно один раз).
        public event Action<Ability> Finished;
        
        [Header("Unique Key in pool dictionary")]
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;

        public void Play(AbilityContext ctx)
        {
            OnPlay(ctx);
        }

        protected void RaiseFinished(Ability a) => Finished?.Invoke(a);

        /// Реализуют наследники (корутина/логика эффекта внутри)
        protected abstract void OnPlay(AbilityContext ctx);
        
        public void OnGetFromPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(true);
        }
        
        public void OnReturnToPool()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            transform.SetParent(Pool.Instance?.transform);
        }
    }
}