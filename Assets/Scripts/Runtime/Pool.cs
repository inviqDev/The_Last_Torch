using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-995)]
    public class Pool : Singleton<Pool>
    {
        [SerializeField] private Transform enemiesRoot;
        [SerializeField] private Transform dropRoot;
        [SerializeField] private Transform soundsRoot;
        [SerializeField] private Transform particlesRoot;
        [SerializeField] private Transform vfxRoot;
        
        private readonly Dictionary<string, Stack<IPoolable>> _pool = new();
        private readonly List<IPoolable> _activeItems = new();
        
        public Transform EnemiesRoot => enemiesRoot;
        public Transform DropRoot => dropRoot;
        public Transform ParticlesRoot => particlesRoot;
        public Transform SoundsRoot => soundsRoot;
        public Transform VFXRoot => vfxRoot;

        
        public T TryGet<T>(T prefab) where T : MonoBehaviour, IPoolable
        {
            var uniquePoolKey = prefab.UniquePoolKey;
            EnsureBucket(uniquePoolKey);
            
            if (!_pool.TryGetValue(uniquePoolKey, out var stack))
            {
                _pool[uniquePoolKey] = new Stack<IPoolable>();
            }
            
            var poolableObject = stack?.Count > 0 ? stack.Pop() : Instantiate(prefab);
            var item = (T)poolableObject;
            
            _activeItems.Add(item);
            item.OnGetFromPool();
            
            return item;
        }
        
        public void ReturnToPool(IPoolable item)
        {
            if (item == null) return;
        
            EnsureBucket(item.UniquePoolKey);
            _activeItems.Remove(item);
            item.OnReturnToPool();
            
            _pool[item.UniquePoolKey].Push(item);
        }

        public void RestartPool()
        {
            if (_activeItems.Count == 0) return;
            
            foreach (var p in _activeItems)
            {
                ReturnToPool(p);
            }
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
