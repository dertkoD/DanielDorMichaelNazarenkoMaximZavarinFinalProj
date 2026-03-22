using TMPro;
using UnityEngine;

public class UIHealth : MonoBehaviour
{
    [Header("Channel")]
    [SerializeField] private HealthChangedActionChannelSO healthChangedAction;

    [Header("Player")]
    [SerializeField] private int playerObjectId = 0;
    [SerializeField] private TMP_Text playerText;

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] private TMP_Text enemyText;
    [SerializeField] private string enemyLabel = "Enemy";
    [SerializeField] private bool hideEnemyOnStart = true;
    [SerializeField] private float hideDeadEnemyAfterSeconds = 2f;

    private int shownEnemyObjectId = -1;
    private float hideEnemyAtTime = -1f;

    private void Awake()
    {
        if (enemyPanel != null && hideEnemyOnStart)
            enemyPanel.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log($"UIHealth OnEnable. Channel = {healthChangedAction}");

        if (healthChangedAction != null)
            healthChangedAction.OnEvent += OnHealthChanged;
    }

    private void OnDisable()
    {
        if (healthChangedAction != null)
            healthChangedAction.OnEvent -= OnHealthChanged;
    }

    private void Update()
    {
        if (hideEnemyAtTime < 0f) return;
        if (Time.time < hideEnemyAtTime) return;

        hideEnemyAtTime = -1f;
        shownEnemyObjectId = -1;

        if (enemyPanel != null)
            enemyPanel.SetActive(false);
    }

    private void OnHealthChanged(int objectId, int currentHp, int maxHp)
    {
        Debug.Log($"UIHealth received: objectId={objectId}, hp={currentHp}/{maxHp}");

        if (objectId == playerObjectId)
        {
            UpdatePlayerUI(currentHp, maxHp);
            return;
        }

        UpdateEnemyUI(objectId, currentHp, maxHp);
    }

    private void UpdatePlayerUI(int currentHp, int maxHp)
    {
        if (playerText == null) return;
        playerText.text = $"HP: {currentHp}/{maxHp}";
    }

    private void UpdateEnemyUI(int objectId, int currentHp, int maxHp)
    {
        shownEnemyObjectId = objectId;

        if (enemyPanel != null && !enemyPanel.activeSelf)
            enemyPanel.SetActive(true);

        if (enemyText != null)
            enemyText.text = $"{enemyLabel} {objectId}: {currentHp}/{maxHp}";

        if (currentHp <= 0)
            hideEnemyAtTime = Time.time + hideDeadEnemyAfterSeconds;
        else
            hideEnemyAtTime = -1f;
    }
}
