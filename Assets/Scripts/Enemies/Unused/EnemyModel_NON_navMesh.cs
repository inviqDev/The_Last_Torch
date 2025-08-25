using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyModel_NON_navMesh : MonoBehaviour
{
    public event Action<EnemyModel_NON_navMesh> OnEnemyDeath;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private EnemyMovement_NON_NavMesh movementNonNavMeshComponent;
    
    
    #region REMOVE SERIALIZE FIELDS => MAKE IT PRIVATE

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float moveSpeed;
    [SerializeField] private bool needToMove;

    [SerializeField] private float damage;

    #endregion


    private PlayerModel _player;

    private void Start()
    {
        Debug.Assert(PlayerManager.Instance, "PlayerManager has not been found");
    }

    public void SetCurrentConfig(EnemyConfig_non_NavMesh config)
    {
        if (!PlayerManager.Instance) return;

        _player = PlayerManager.Instance.CurrentPlayer;

        maxHealth = config.MaxHealth;
        currentHealth = config.MaxHealth;

        moveSpeed = config.MoveSpeed;
        needToMove = config.NeedToMove;

        damage = config.Damage;

        meshFilter.mesh = config.EnemyMesh;
        movementNonNavMeshComponent.SetMovementSettings(_player, moveSpeed, needToMove);
    }

    public void TakeDamage(float incomingDamage)
    {
        currentHealth = Mathf.Clamp(currentHealth - incomingDamage, 0, maxHealth);

        if (currentHealth <= 0)
        {
            // Add dropping items logic here

            PrepareToPool();
            OnEnemyDeath?.Invoke(this);
        }
    }

    private void PrepareToPool()
    {
        gameObject.SetActive(false);
    }
}