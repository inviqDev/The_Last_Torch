using UnityEngine;

namespace Runtime
{
    public interface IPoolable
    {
        public string UniquePoolKey { get; }
        
        public void OnGetFromPool();
        public void OnReturnToPool();
    }
}