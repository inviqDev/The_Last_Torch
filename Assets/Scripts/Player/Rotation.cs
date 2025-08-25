using UnityEngine;

public enum FacingMode
{
    None,
    Direction,        // к заданному мировому направлению (задаётся извне)
    TargetTransform,  // к Transform цели (обновляется каждый кадр)
    TargetPosition    // к фиксированной мировой точке
}

public class Rotation : MonoBehaviour
{
    [Header("Transform to rotate")]
    [SerializeField] private Transform rotateTarget;            // кого крутим (обычно корень префаба)

    [Header("Smoothing")]
    [SerializeField] private bool useSmoothDamp;                // true -> пружинка, false -> константная скорость
    [SerializeField] private float turnSpeedDegPerSec = 720f;   // для RotateTowards
    [SerializeField] private float smoothTime = 0.08f;          // для SmoothDampAngle
    [SerializeField] private float deadZoneDeg = 0.25f;         // порог «довернули» (чтобы не дёргалось)

    [Header("Rotation around axis:")]
    [SerializeField] private Vector3 upAxis;                    // по умолчанию (0,1,0)

    private FacingMode _mode;
    private Transform _target;                                  // для TargetTransform
    private Vector3 _targetPos;                                 // для TargetPosition
    private Vector3 _desiredDir;                                // для Direction (мировой)
    private float _yawVel;                                      // скорость для SmoothDampAngle

    private void Awake()
    {
        if (!rotateTarget) rotateTarget = transform;
        if (upAxis == default) upAxis = Vector3.up;
        _mode = FacingMode.None;
    }

    private void Update()
    {
        if (_mode == FacingMode.None || !rotateTarget) return;

        // 1) Вычисляем целевой вектор на плоскости XZ
        var dir = _desiredDir;

        switch (_mode)
        {
            case FacingMode.TargetTransform:
                if (!_target) return;
                dir = _target.position - rotateTarget.position;
                break;
            case FacingMode.TargetPosition:
                dir = _targetPos - rotateTarget.position;
                break;
            case FacingMode.Direction:
                dir = _desiredDir;
                break;
        }

        // проекция на плоскость, чтобы вертеть только по yaw
        dir = Vector3.ProjectOnPlane(dir, upAxis);
        if (dir.sqrMagnitude < 1e-6f) return;

        // 2) целевой yaw
        var targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        var currentYaw = rotateTarget.eulerAngles.y;

        // 3) если уже почти дошли — не дёргаем
        var delta = Mathf.DeltaAngle(currentYaw, targetYaw);
        if (Mathf.Abs(delta) < deadZoneDeg) return;

        // 4) поворачиваем
        var newYaw = useSmoothDamp 
            ? Mathf.SmoothDampAngle(currentYaw, targetYaw, ref _yawVel, smoothTime)
            : Mathf.MoveTowardsAngle(currentYaw, targetYaw, turnSpeedDegPerSec * Time.deltaTime);

        rotateTarget.rotation = Quaternion.Euler(0f, newYaw, 0f);
    }

    // === ПУБЛИЧНОЕ API ===

    /// <summary>Игрок/ИИ: задать мировое направление (обновляй только когда ввод меняется).</summary>
    public void FaceDirection(Vector3 worldDirection)
    {
        worldDirection.y = 0f;
        _desiredDir = worldDirection.normalized;
        _mode = _desiredDir.sqrMagnitude > 0f ? FacingMode.Direction : FacingMode.None;
    }

    /// <summary>Враг: смотреть на трансформ цели (например, на игрока).</summary>
    public void FaceTarget(Transform target)
    {
        _target = target;
        _mode = target ? FacingMode.TargetTransform : FacingMode.None;
    }

    public void StopFacing() => _mode = FacingMode.None;

    // Можно менять поведение на лету:
    public void SetTurnSpeed(float degPerSec) => turnSpeedDegPerSec = Mathf.Max(1f, degPerSec);
    public void SetSmooth(bool isSmoothing, float newSmoothTime = -1f)
    {
        useSmoothDamp = isSmoothing;
        if (newSmoothTime > 0f) smoothTime = newSmoothTime;
    }
}
