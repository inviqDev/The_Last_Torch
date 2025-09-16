using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-995)]
    public class Pool : Singleton<Pool>
    {
        [SerializeField] private Transform enemiesRoot;
        [SerializeField] private Transform soundsRoot;
        [SerializeField] private Transform particlesRoot;

        
        private readonly Dictionary<string, Stack<IPoolable>> _pool = new();
        
        public Transform EnemiesRoot => enemiesRoot;
        public Transform ParticlesRoot => particlesRoot;
        public Transform SoundsRoot => soundsRoot;

        
        public T TryGetObjectFromPool<T>(T prefab) where T : MonoBehaviour, IPoolable
        {
            var uniquePoolKey = prefab.UniquePoolKey;
            EnsureBucket(uniquePoolKey);
            
            if (!_pool.TryGetValue(uniquePoolKey, out var stack))
            {
                _pool[uniquePoolKey] = new Stack<IPoolable>();
            }
            
            var poolableObject = stack?.Count > 0 ? stack.Pop() : Instantiate(prefab);
            var item = (T)poolableObject;
            item.OnGetFromPool();
            
            return item;
        }
        
        public void ReturnToPool(IPoolable item)
        {
            if (item == null) return;
        
            var uniquePoolKey = item.UniquePoolKey;
            EnsureBucket(uniquePoolKey);
            item.OnReturnToPool();
            
            _pool[uniquePoolKey].Push(item);
        }
        
        private void EnsureBucket(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                UnityEngine.Assertions.Assert.IsTrue(!string.IsNullOrEmpty(key), 
                    "poolable item's unique pool key is not set");
                return;
            }
            
            if (!_pool.ContainsKey(key))
                _pool[key] = new Stack<IPoolable>();
        }
    }
}
