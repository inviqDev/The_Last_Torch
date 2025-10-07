using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime
{
    public class FootStomp : AbilityVfx
    {
        [SerializeField] private float pushSpeed;
        
        private Player _player;
        private Ability _ability;
        private EnemiesDetector _detector;

        private Vector3 _startPoint;
        private float _radius;

        private List<Enemy> _affectedEnemies = new();
        private IEnumerator _pushingRoutine;
        
        protected override void OnPlay(AbilityContext ctx)
        {
            ApplyAbilityContext(ctx);
            
            if (_pushingRoutine != null)
            {
                ReactivateMissedAffectedEnemies();
                StopCoroutine(_pushingRoutine);
                _pushingRoutine = null;
            }
            
            var detector = _player.Detector;
            if (!detector.EnemyExists) return;
            CollectAffectedEnemies(detector);
            
            if (_affectedEnemies.Count == 0) return;

            var particle = GameManager.Instance?.ParticlesManager.PlayParticleAt(
                    _ability.HitVfxPoolKey, _startPoint, 2f * _radius);
            
            _pushingRoutine = PushAway();
            StartCoroutine(_pushingRoutine);
        }
        
        private void ApplyAbilityContext(AbilityContext ctx)
        {
            _player = ctx.Player;
            _ability = ctx.Ability;
            _startPoint = _player.transform.position;
            _radius = ctx.Ability.AttackRange;
        }

        private void CollectAffectedEnemies(EnemiesDetector detector)
        {
            _affectedEnemies ??= new List<Enemy>();
            _affectedEnemies?.Clear();
            
            foreach (var e in detector.AvailableEnemies)
            {
                if ((e.transform.position - _startPoint).sqrMagnitude > _radius * _radius) continue;
                
                e.Movement.DeactivateNavMeshAgent();
                _affectedEnemies?.Add(e);
            }
        }
        
        private IEnumerator PushAway()
        {
            UnityEngine.Assertions.Assert.IsTrue(_affectedEnemies.Count > 0, 
                "pushing enemies collection is empty");
            if (_affectedEnemies.Count == 0) yield break;
            
            while (_affectedEnemies.Count > 0)
            {
                foreach (var e in _affectedEnemies.ToList())
                {
                    if (NeedToSkipInactiveEnemy(e)) continue;
                    PushEnemyAwayFromStartPoint(e);
                    RemoveEnemyOnMaxDistanceReached(e);
                }
                
                yield return null;
            }

            ReactivateMissedAffectedEnemies();
            _pushingRoutine = null;
            
            RaiseFinished(_ability);
        }

        private bool NeedToSkipInactiveEnemy(Enemy e)
        {
            if (e && e.gameObject.activeInHierarchy) return false;
            
            _affectedEnemies.Remove(e);
            return true;
        }

        private void PushEnemyAwayFromStartPoint(Enemy e)
        {
            var pushDirection = (e.transform.position - _startPoint).normalized;
            pushDirection.y = 0f;
            
            e.transform.Translate(pushDirection * (pushSpeed * Time.deltaTime), Space.World);
        }
        
        private void RemoveEnemyOnMaxDistanceReached(Enemy e)
        {
            if ((e.transform.position - _startPoint).sqrMagnitude < _radius * _radius) return;
                    
            e.Movement.ActivateNavMeshAgent();
            _affectedEnemies.Remove(e);
        }
        
        private void ReactivateMissedAffectedEnemies()
        {
            if (_affectedEnemies.Count > 0)
            {
                foreach (var e in _affectedEnemies)
                {
                    if (!e) continue;
                    e.Movement.ActivateNavMeshAgent();
                }
            }
            
            _affectedEnemies.Clear();
        }
        
        private void OnDisable()
        {
            if (_pushingRoutine == null) return;
            
            ReactivateMissedAffectedEnemies();
            StopCoroutine(_pushingRoutine);
            _pushingRoutine = null;
        }
    }
}
