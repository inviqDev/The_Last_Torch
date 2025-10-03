using UnityEngine;
using UnityEngine.AI;

namespace Runtime
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Face while stopped (optional)")] 
        [SerializeField] private bool faceTargetWhenStopped = true;
        [SerializeField] private float faceTurnSpeed = 720f;
        
        [SerializeField] private ObstacleAvoidanceType obstacleAvoidanceType;

        private NavMeshAgent _agent;
        private readonly float _repathInterval = 0.25f;
        
        private Transform _target;
        private float _nextRepath;
        
        private bool _activated;

        public void Initialize(Player player, Vector3 startPoint,EnemyConfig config)
        {
            _agent = GetComponent<NavMeshAgent>();
            
            ActivateNavMeshAgent();
            ApplyConfig(player, startPoint, config);
        }
        
        public void ActivateNavMeshAgent()
        {
            _agent ??= GetComponent<NavMeshAgent>();
            
            if (!_agent.enabled) _agent.enabled = true;
            _activated = true;
            
            _agent.updatePosition = true;
            _agent.updateRotation = true;
            _agent.updateUpAxis = true;
            _agent.isStopped = false;
            _agent.obstacleAvoidanceType = obstacleAvoidanceType;
            
            WarpTo(transform.position);
            if (_agent.isOnNavMesh) _agent.ResetPath();
            
            if (!_target) return;
            _agent.SetDestination(_target.position);
            _nextRepath = Time.time + _repathInterval;
        }
        
        public void DeactivateNavMeshAgent()
        {
            _activated = false;
            
            if (!_agent) return;

            if (!_agent.enabled) _agent.enabled = true;
            if (_agent.isOnNavMesh) _agent.ResetPath();
            
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _agent.isStopped = true;
            
            _agent.enabled = false;
        }

        private void ApplyConfig(Player player, Vector3 startPoint, EnemyConfig config)
        {
            UnityEngine.Assertions.Assert.IsNotNull(player, "player is missing");
            _target = player.transform;

            _agent.speed = config.moveSpeed;
            _agent.angularSpeed = config.angularSpeed;
            _agent.acceleration = config.acceleration;
            _agent.stoppingDistance = config.stoppingDistance;
            
            _agent.enabled = true;
            _agent.updatePosition = true;
            _agent.isStopped = false;
            WarpTo(startPoint);

            _agent.SetDestination(_target.position);
            _nextRepath = Time.time + _repathInterval;
        }

        private void Update()
        {
            if (!_activated) return;
            if (!_agent.isOnNavMesh) return;
            if (!_target || _agent.isStopped) return;

            if (Time.time < _nextRepath || _agent.pathPending) return;

            _agent.SetDestination(_target.position);
            _nextRepath = Time.time + _repathInterval;
        }

        private void LateUpdate()
        {
            if (!_activated) return;
            if (!_agent.isOnNavMesh) return;
            if (!_target || !_agent.isStopped || !faceTargetWhenStopped) return;

            var lookDirection = _target.position - transform.position;
            lookDirection.y = 0f;

            var lookAt = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, faceTurnSpeed * Time.deltaTime);
        }

        private void WarpTo(Vector3 spawnPos)
        {
            _agent.Warp(NavMesh.SamplePosition(spawnPos, out var hit, 2f, NavMesh.AllAreas)
                ? hit.position
                : spawnPos);
        }
    }
}