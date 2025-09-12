using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-9999)]
    public class Pool : Singleton<Pool>
    {
        private readonly Dictionary<string, Stack<IPoolable>> _pool = new();

        /// <summary>
        /// Выдать префаб из пула, если в пуле пусто — инстанциируем prefab.
        /// </summary>
        public T TryGetObjectFromPool<T>(T prefab) where T : Object, IPoolable
        {
            if (!_pool.TryGetValue(prefab.UniquePoolKey, out var stack))
            {
                stack = new Stack<IPoolable>();
                _pool[prefab.UniquePoolKey] = stack;
            }
            
            var item = stack.Count > 0 ? stack.Pop() : Instantiate(prefab);
            return (T)item;
        }

        // /// <summary>
        // /// Удобный перегруз — ключ берём с prefab.PoolKey, чтобы не дублировать строки руками.
        // /// </summary>
        // public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        //     where T : BasePoolable
        // {
        //     return Get(prefab.PoolKey, prefab, position, rotation, parent);
        // }
        //
        // /// <summary>
        // /// Вернуть объект в пул.
        // /// </summary>
        // public void Return(BasePoolable item)
        // {
        //     if (item == null) return;
        //
        //     var key = item.PoolKey;
        //     if (string.IsNullOrEmpty(key))
        //     {
        //         Debug.LogWarning($"[Pool] {item.name} has empty PoolKey.");
        //         return;
        //     }
        //
        //     EnsureBucket(key);
        //
        //     item.transform.SetParent(transform);
        //     item.ReturnToPool();
        //     _pool[key].Push(item);
        // }
        //
        // /// <summary>
        // /// Предзагрузка N объектов (по желанию).
        // /// </summary>
        // public void Prewarm<T>(T prefab, int count) where T : BasePoolable
        // {
        //     var key = prefab.PoolKey;
        //     EnsureBucket(key);
        //
        //     for (int i = _pool[key].Count; i < count; i++)
        //     {
        //         var inst = Instantiate(prefab, transform);
        //         inst.ReturnToPool(); // деактивировать
        //         _pool[key].Push(inst);
        //     }
        // }
        //
        
        private void EnsureBucket(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            
            if (!_pool.ContainsKey(key))
                _pool[key] = new Stack<IPoolable>();
        }
    }
}
