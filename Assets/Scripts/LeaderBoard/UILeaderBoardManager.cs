using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class UILeaderBoardManager : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private LeaderBoardUIRow rowPrefab;
    [SerializeField] private TextMeshProUGUI currentPlayerStats;

    private int numberOfRows = 7;
    private int currentRows = 0;

    public void UpdateCurrentPlayerStats(string name, float score)
    {
        currentPlayerStats.text = $"Player {name}, your current score: {score}";
    }
    
    
    public void ResetCurrentPLayer()
    {
        currentPlayerStats.text = "";
    }
    public void AddNewMemberToLeaderBoard(Dictionary<string, float> scoreTable)
    {
        if (currentRows >= numberOfRows)
            return;
        //Create list sorted by value
        var sorted= scoreTable.OrderByDescending(x=> x.Value).ToList();
        var last = sorted.Last();
        LeaderBoardUIRow row = Instantiate(rowPrefab, content);

        row.SetData(sorted.Count, last.Key, last.Value);
        currentRows++;
    }

    //recreation of leaderboard
    public void ShowLeaderboard(Dictionary<string, float> scoreTable)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
       
        var sorted= scoreTable.OrderByDescending(x=> x.Value).Take(numberOfRows).ToList();
        
        for (int i = 0; i < sorted.Count; i++)
        {
            var entry = sorted[i];

            LeaderBoardUIRow row = Instantiate(rowPrefab, content);

            row.SetData(i + 1, entry.Key, entry.Value);
        }
    }
}
