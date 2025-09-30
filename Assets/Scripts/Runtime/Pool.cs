using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class Pool : Singleton<Pool>
    {
        [SerializeField] private Transform enemiesRoot;
        [SerializeField] private Transform dropRoot;
        [SerializeField] private Transform soundsRoot;
        [SerializeField] private Transform particlesRoot;
        [SerializeField] private Transform vfxRoot;

        private readonly Dictionary<string, Stack<IPoolable>> _pool = new();
        private readonly Dictionary<string, Type> _keyOwnerType = new();
        private readonly HashSet<IPoolable> _activeItems = new();

        public Transform EnemiesRoot => enemiesRoot;
        public Transform DropRoot => dropRoot;
        public Transform ParticlesRoot => particlesRoot;
        public Transform SoundsRoot => soundsRoot;
        public Transform VFXRoot => vfxRoot;

        public T TryGet<T>(T prefab) where T : MonoBehaviour, IPoolable
        {
            var key = prefab.UniquePoolKey;
            EnsureBucket(key, typeof(T));

            var stack = _pool[key];
            var pooled = stack.Count > 0 ? stack.Pop() : Instantiate(prefab);

            UnityEngine.Assertions.Assert.IsTrue(pooled is T,
                $"Pooled instance under key '{key}' is {pooled?.GetType().Name}, " +
                $"but expected {typeof(T).Name}");

            var item = (T)pooled;
            UnityEngine.Assertions.Assert.IsTrue(_activeItems.Add(item),
                $"Item with key '{key}' is already active (double get?).");

            item.OnGetFromPool();
            return item;
        }

        public void ReturnToPool(IPoolable item)
        {
            if (item == null) return;

            EnsureBucket(item.UniquePoolKey);

            if (!_activeItems.Remove(item))
                return;

            item.OnReturnToPool();
            _pool[item.UniquePoolKey].Push(item);
        }

        public void RestartPool()
        {
            if (_activeItems.Count == 0) return;

            var copy = new IPoolable[_activeItems.Count];
            _activeItems.CopyTo(copy);

            foreach (var p in copy)
            {
                ReturnToPool(p);
            }
        }

        private void EnsureBucket(string key) => EnsureBucket(key, null);
        private void EnsureBucket(string key, Type ownerType)
        {
            UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrEmpty(key),
                "poolable item's unique pool key is not set");
            if (string.IsNullOrEmpty(key)) return;

            if (!_pool.ContainsKey(key))
                _pool[key] = new Stack<IPoolable>();

            if (ownerType == null) return;

            if (_keyOwnerType.TryGetValue(key, out var existing))
            {
                UnityEngine.Assertions.Assert.IsTrue(existing == ownerType,
                    $"Pool key '{key}' bound to {existing.Name}, " +
                    $"but requested {ownerType.Name}.");
            }
            else
            {
                _keyOwnerType[key] = ownerType;
            }
        }
    }
}

// public T TryGet<T>(T prefab) where T : MonoBehaviour, IPoolable [METHOD]
// var uniquePoolKey = prefab.UniquePoolKey;
// EnsureBucket(uniquePoolKey);
//
// var stack = _pool[uniquePoolKey];
// var item = stack.Count > 0 ? (T)stack.Pop() : Instantiate(prefab);
//
// _activeItems.Add(item);
// item.OnGetFromPool();
//
// return item;


// private void EnsureBucket(string key)
// {
//     var keyIsValid = !string.IsNullOrEmpty(key);
//     UnityEngine.Assertions.Assert.IsTrue(keyIsValid,
//         "poolable item's unique pool key is not set");
//     if (string.IsNullOrEmpty(key)) return;
//
//     if (!_pool.ContainsKey(key))
//         _pool[key] = new Stack<IPoolable>();
// }