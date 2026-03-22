using UnityEngine;

public class EnemyPatrolPath : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;

    public int Count => patrolPoints != null ? patrolPoints.Length : 0;

    public Transform GetPoint(int index)
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return null;

        index = Mathf.Clamp(index, 0, patrolPoints.Length - 1);
        return patrolPoints[index];
    }

    private void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            Gizmos.DrawSphere(patrolPoints[i].position, 0.15f);

            int next = (i + 1) % patrolPoints.Length;
            if (patrolPoints[next] != null)
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
        }
    }
}
