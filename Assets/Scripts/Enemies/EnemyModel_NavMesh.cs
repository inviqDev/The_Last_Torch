using System;
using UnityEngine;

public class EnemyModel_NavMesh : MonoBehaviour
{
    public event Action<EnemyModel_NavMesh> OnEnemyDeath;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private EnemyMovement_NavMesh navMeshMover;
    public EnemyMovement_NavMesh NavMeshMover => navMeshMover;
    
    private PlayerModel _player;
    
    
    #region REMOVE "FOR TESTING" SERIALIZED FIELDS => MAKE PRIVATE

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float stoppingDistance;
    
    [SerializeField] private float damage;

    #endregion
    

    private void Start()
    {
        Debug.Assert(PlayerManager.Instance, "PlayerManager has not been found");
    }

    public void SetCurrentConfig(EnemyConfig_NavMesh configNavMesh)
    {
        if (!PlayerManager.Instance) return;

        _player = PlayerManager.Instance.CurrentPlayer;

        meshFilter.mesh = configNavMesh.EnemyMesh;
        
        maxHealth = configNavMesh.MaxHealth;
        currentHealth = configNavMesh.MaxHealth;
        
        moveSpeed = configNavMesh.MoveSpeed;
        angularSpeed = configNavMesh.AngularSpeed;
        acceleration = configNavMesh.Acceleration;
        stoppingDistance = configNavMesh.StoppingDistance;
        
        damage = configNavMesh.Damage;

        navMeshMover.ApplyMovementConfig(
            _player, moveSpeed, angularSpeed, acceleration, stoppingDistance);
    }

    public void TakeDamage(float incomingDamage)
    {
        currentHealth = Mathf.Clamp(currentHealth - incomingDamage, 0, maxHealth);

        if (currentHealth <= 0)
        {
            // Add DROP item logic here

            PrepareToPool();
            OnEnemyDeath?.Invoke(this);
        }
    }

    private void PrepareToPool()
    {
        navMeshMover.StopAndReset();
        gameObject.SetActive(false);
    }
}