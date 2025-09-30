using System;
using UnityEngine;

namespace Runtime
{
    public readonly struct AbilityContext
    {
        public readonly Player Player;
        public readonly Enemy InitialTarget;
        public readonly EnemiesDetector EnemiesDetector;
        public readonly Ability Ability;

        public AbilityContext(Player player, Enemy initialTarget,
            EnemiesDetector enemiesDetector, Ability ability)
        {
            Player = player;
            InitialTarget = initialTarget;
            EnemiesDetector = enemiesDetector;
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

        public void LaunchAblilityVFX(AbilityContext ctx)
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
            Finished = null;
            
            gameObject.SetActive(false);
            transform.SetParent(Pool.Instance?.VFXRoot);
        }
    }
}