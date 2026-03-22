using UnityEngine;
using UnityEngine.AI;

public class EnemyAimController : MonoBehaviour
{
    [SerializeField] private Transform rotateRoot;
    [SerializeField] private Transform aimOrigin;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float rotationSpeedDegrees = 540f;

    private void Awake()
    {
        if (rotateRoot == null)
            rotateRoot = transform;

        if (navMeshAgent != null)
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
        }
    }

    public void AimAt(Vector3 worldPoint)
    {
        if (rotateRoot == null)
            return;

        Vector3 direction = worldPoint - rotateRoot.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        rotateRoot.rotation = Quaternion.RotateTowards(
            rotateRoot.rotation,
            targetRotation,
            rotationSpeedDegrees * Time.deltaTime
        );
    }

    public void LookMoveDirection()
    {
        if (rotateRoot == null || navMeshAgent == null)
            return;

        Vector3 direction = navMeshAgent.desiredVelocity;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        rotateRoot.rotation = Quaternion.RotateTowards(
            rotateRoot.rotation,
            targetRotation,
            rotationSpeedDegrees * Time.deltaTime
        );
    }
}
