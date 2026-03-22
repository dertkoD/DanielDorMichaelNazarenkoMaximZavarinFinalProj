using System.Collections.Generic;
using UnityEngine;

public class LevelGameOverController : MonoBehaviour
{
    [Header("Events In")]
    [SerializeField] private GameOverActionChannelSO gameOverAction;

    [Header("Events Out")]
    [SerializeField] private LevelResultActionChannelSO levelResultAction;

    [Header("Player")]
    [SerializeField] private int playerObjectId = 0;

    [Header("Enemies")]
    [SerializeField] private List<Health> enemyHealths = new List<Health>();

    private readonly HashSet<int> aliveEnemyIds = new HashSet<int>();
    private bool levelFinished;

    [SerializeField] private Timer _timer;

    private void OnEnable()
    {
        if (gameOverAction != null)
            gameOverAction.OnEvent += OnObjectDied;
    }

    private void OnDisable()
    {
        if (gameOverAction != null)
            gameOverAction.OnEvent -= OnObjectDied;
    }

    private void Start()
    {
        RebuildEnemySet();
    }

    private void RebuildEnemySet()
    {
        aliveEnemyIds.Clear();

        foreach (var health in enemyHealths)
        {
            if (health == null) continue;
            if (health.ObjectId == playerObjectId) continue;
            if (health.IsDead) continue;

            aliveEnemyIds.Add(health.ObjectId);
        }
    }

    private void OnObjectDied(int deadObjectId)
    {
        if (levelFinished)
            return;

        if (deadObjectId == playerObjectId)
        {
            levelFinished = true;
            levelResultAction?.Raise(false);
            _timer.ResetTImer();
            _timer.StopTimerOnFinish();
            return;
        }
            
        if (aliveEnemyIds.Contains(deadObjectId))
            aliveEnemyIds.Remove(deadObjectId);

        if (aliveEnemyIds.Count <= 0)
        {
            levelFinished = true;
            levelResultAction?.Raise(true);
        }
    }
}
