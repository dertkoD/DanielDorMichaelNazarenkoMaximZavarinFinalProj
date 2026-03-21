using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class LeaderBoardManager : MonoBehaviour
{
    private Dictionary<string, float> scoreTable = new Dictionary<string, float>();
    [SerializeField] private PlayerNameInput _playerNameInput;
    [SerializeField] private UILeaderBoardManager uiLeader;
    [SerializeField] private SaveLeaderBoard saveSystem;

    private string lastName = "";
    
    //Save data for the last playED player
    void Start()
    {
        scoreTable = saveSystem.LoadLeaderBoardData();
        
        if (!string.IsNullOrEmpty(GameSession.CurrentPlayerName))
        {
            SavePlayerScoreInDictionary(
                GameSession.CurrentPlayerName,
                GameSession.CurrentPlayerTime
            );
        }
        
        uiLeader.ShowLeaderboard(scoreTable);
    }
    
    //create "empty" player "account"
    public void SaveNewNameInDictionary()
    {
        string playerName = _playerNameInput.GetPlayerName().ToLower();

        GameSession.CurrentPlayerName = playerName;
        GameSession.CurrentPlayerTime = 0;
        _playerNameInput.CleanInput();
        
        uiLeader.UpdateCurrentPlayerStats(playerName,  GameSession.CurrentPlayerTime);
        
        if (!scoreTable.ContainsKey(playerName))
        {
            scoreTable.Add(playerName, 0);
            
            saveSystem.SaveLeaderBoardData(scoreTable);
            
            uiLeader.AddNewMemberToLeaderBoard(scoreTable);
        }
    }
    
    // Save new score if it's bigger than previous
    private void SavePlayerScoreInDictionary(string playerName, float time)
    {
        if (time > scoreTable[playerName])
        {
            scoreTable[playerName] = time;
        }

        saveSystem.SaveLeaderBoardData(scoreTable);
        
        uiLeader.UpdateCurrentPlayerStats(playerName, scoreTable[playerName]);
        uiLeader.ShowLeaderboard(scoreTable);
    }

    
    public void CleanTheLeaderBoard()
    {
        scoreTable.Clear();
        saveSystem.ClearLeaderBoardData();
        
        uiLeader.ShowLeaderboard(scoreTable);
    }
    
    
}
