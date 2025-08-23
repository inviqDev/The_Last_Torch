using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerModel : MonoBehaviour
{
    [SerializeField] private PlayerMovement movementComponent;
    [SerializeField] private Dash dashComponent;

    [Header("Movement current stats")]
    [SerializeField] private float MoveSpeed;
    
    [Header("Dash current stats")]
    [SerializeField] private float DashSpeed;
    [SerializeField] private float DashDuration;

    private void Start()
    {
        PlayerManager.Instance.OnConfigChanged += ChangeConfig;
    }

    public void ChangeConfig(PlayerConfig config)
    {
        MoveSpeed = config.MoveSpeed;
        DashSpeed = config.DashSpeed;
        DashDuration = config.DashDuration;
        
        movementComponent.SetMoveSettingsFromConfig(config);
        dashComponent.SetDashSettingsFromConfig(config);
        
        print($"moveSpeed: {MoveSpeed} // DashSpeed: {DashSpeed} // DashDuration: {DashDuration}");
    }

    public void ChangeAllModifiers()
    {
        movementComponent.IncreaseMoveSpeedModifier();
        dashComponent.IncreaseDashSpeedModifier();
        dashComponent.IncreaseDashDurationModifier();
    }
}