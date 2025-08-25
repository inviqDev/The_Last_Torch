using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig_non_NavMesh", menuName = "Scriptable Objects/EnemyConfig_non_NavMesh")]
public class EnemyConfig_non_NavMesh : ScriptableObject
{
    [SerializeField] private Mesh enemyMesh;
    public Mesh EnemyMesh => enemyMesh;
    
    [Header("Health settings")] 
    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;
    
    // =================================== //
    // =================================== //
    // =================================== //

    [Header("Movement settings")]
    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;

    [SerializeField] private bool needToMove;
    public bool NeedToMove => needToMove;
    
    // =================================== //
    // =================================== //
    // =================================== //
    
    [Header("Attack settings")]
    [SerializeField] private float damage;
    public float Damage => damage;
}
