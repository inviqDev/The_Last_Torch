using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig_NavMesh", menuName = "Scriptable Objects/EnemyConfig_NavMesh")]
public class EnemyConfig_NavMesh : ScriptableObject
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
    
    [SerializeField] private float angularSpeed;
    public float AngularSpeed => angularSpeed;
    
    [SerializeField] private float acceleration;
    public float Acceleration => acceleration;
    
    [SerializeField] private float stoppingDistance;
    public float StoppingDistance => stoppingDistance;
    
    
    // =================================== //
    // =================================== //
    // =================================== //
    
    [Header("Attack settings")]
    [SerializeField] private float damage;
    public float Damage => damage;
}
