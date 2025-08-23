using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerModel : Singleton<PlayerModel>
{
    [SerializeField] private Movement movementComponent;
    [SerializeField] private Dash dashComponent;

    [Header("Movement current stats")]
    [field: SerializeField] public float MoveSpeed;
    
    [Header("Dash current stats")]
    [field: SerializeField] public float DashSpeed;
    [field: SerializeField] public float DashDuration;

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