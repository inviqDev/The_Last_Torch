using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class ParticlesManager : MonoBehaviour
    {
        [SerializeField] private List<Particle> particles;
        
        private Dictionary<string, Particle> _particlesDictionary;

        public void Initialize()
        {
            _particlesDictionary = new Dictionary<string, Particle>();
            foreach (var p in particles)
            {
                var key = p.UniquePoolKey;
                _particlesDictionary.Add(key, p);
            }
        }

        public Particle PlayParticle(string uniquePoolKey, Character owner = null)
        {
            if (!_particlesDictionary.TryGetValue(uniquePoolKey, out var prefab))
            {
                UnityEngine.Assertions.Assert.IsNotNull(
                    prefab, $"particle {uniquePoolKey} is not found");
                
                return null;
            }

            var particle = Pool.Instance?.TryGet(prefab);
            particle?.PlayParticleEffect(owner);
            
            return particle;
        }
        
        public Particle PlayParticleAt(string uniquePoolKey, Vector3 worldPos, float worldScale = 1f, Character owner = null)
        {
            if (!_particlesDictionary.TryGetValue(uniquePoolKey, out var prefab))
            {
                UnityEngine.Assertions.Assert.IsNotNull(prefab, $"particle {uniquePoolKey} is not found");
                return null;
            }

            var particle = Pool.Instance?.TryGet(prefab);
            if (!particle) return null;

            // ВАЖНО: позиция и масштаб ДО PlayParticleEffect
            var t = particle.transform;
            t.SetParent(null);
            t.position = worldPos;
            t.localScale = Vector3.one * worldScale;

            particle.PlayParticleEffect(owner); // owner=null => масштаб останется, если применил правку выше
            return particle;
        }
    }
}
