using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("MaxHealth settings")]
    [SerializeField] private float health;
    public float Health => health;
    
    [Header("Movement settings")]
    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    
    [Header("Dash settings")]
    [SerializeField] private float dashSpeed;
    public float DashSpeed => dashSpeed;
    [SerializeField] private float dashDuration;
    public float DashDuration => dashDuration;
    
    [Header("Attack settings")]
    [SerializeField] private float damage;
    public float Damage => damage;
}
