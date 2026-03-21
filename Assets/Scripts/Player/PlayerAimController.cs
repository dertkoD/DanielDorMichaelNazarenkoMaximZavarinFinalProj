using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Rigidbody rigidbodyComponent;
    [SerializeField] private PlayerWeaponController playerWeaponController;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private bool rotateOnlyWhenHasWeapon = true;

    private Vector2 pointerScreenPosition;
    private Quaternion targetRotation;
    private bool hasValidTargetRotation;

    public Vector3 CurrentAimPoint { get; private set; }
    public bool HasAimPoint { get; private set; }

    public void SetPointerScreenPosition(Vector2 value)
    {
        pointerScreenPosition = value;
    }

    private void Awake()
    {
        if (rigidbodyComponent != null)
        {
            targetRotation = rigidbodyComponent.rotation;
            hasValidTargetRotation = true;
        }
    }

    private void Update()
    {
        if (worldCamera == null) return;
        if (rigidbodyComponent == null) return;

        Ray ray = worldCamera.ScreenPointToRay(pointerScreenPosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0f, rigidbodyComponent.position.y, 0f));

        if (!plane.Raycast(ray, out float enter))
        {
            HasAimPoint = false;
            return;
        }

        Vector3 hitPoint = ray.GetPoint(enter);
        CurrentAimPoint = hitPoint;
        HasAimPoint = true;

        if (rotateOnlyWhenHasWeapon)
        {
            if (playerWeaponController == null) return;
            if (!playerWeaponController.HasWeapon) return;
        }

        Vector3 direction = hitPoint - rigidbodyComponent.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        hasValidTargetRotation = true;
    }

    private void FixedUpdate()
    {
        if (rigidbodyComponent == null) return;
        if (!hasValidTargetRotation) return;

        Quaternion newRotation = Quaternion.RotateTowards(
            rigidbodyComponent.rotation,
            targetRotation,
            rotationSpeed * 360f * Time.fixedDeltaTime
        );

        rigidbodyComponent.MoveRotation(newRotation);
    }
}
