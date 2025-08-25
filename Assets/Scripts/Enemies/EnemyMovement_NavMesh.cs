using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement_NavMesh : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [Header("Repath")] 
    [SerializeField] private float repathInterval = 0.25f; // как часто обновляем цель

    [Header("Face while stopped (optional)")] 
    [SerializeField] private bool faceTargetWhenStopped = true;
    [SerializeField] private float faceTurnSpeed = 720f;

    private Transform _target;
    private float _nextRepath;

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        // Агент полностью рулит позицией и поворотом
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.updateUpAxis = true; // обычная 3D сцена

        // Базовые дефолты (при желании перенести в EnemyConfig_non_NavMesh)
        agent.speed = 3.5f;
        agent.angularSpeed = 720f; // град/сек: как быстро вертеться
        agent.acceleration = 16f;
        agent.stoppingDistance = 1.5f;
        // agent.autoBraking = true; // плавная остановка около цели
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }

    public void ApplyMovementConfig(PlayerModel player, float moveSpeed,
        float angularSpeed, float acceleration, float stoppingDistance)
    {
        Debug.Assert(player, "Player is not found");
        _target = player.transform;

        agent.speed = moveSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;

        if (_target && !agent.isStopped)
        {
            agent.SetDestination(_target.position);
            _nextRepath = Time.time + repathInterval;
        }
        else
        {
            agent.ResetPath();
        }
    }

    private void Update()
    {
        if (!_target || agent.isStopped) return;

        if (Time.time >= _nextRepath)
        {
            agent.SetDestination(_target.position);
            _nextRepath = Time.time + repathInterval;
        }
    }

    // Опционально: красиво повернуть лицом к игроку, когда агент стоит (например, в радиусе атаки)
    private void LateUpdate()
    {
        if (!faceTargetWhenStopped || !agent.isStopped || !_target) return;

        var dir = _target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 1e-4f) return;

        var lookAt = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, faceTurnSpeed * Time.deltaTime);
    }

    // Безопасный телепорт на сетку при спауне / реюзе из пула
    public void WarpTo(Vector3 spawnPos)
    {
        if (NavMesh.SamplePosition(spawnPos, out var hit, 2f, NavMesh.AllAreas))
            agent.Warp(hit.position); // телепорт без физ. рывков
        else
            agent.Warp(spawnPos);
    }

    // Вызывать при возврате в пул
    public void StopAndReset()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }
}