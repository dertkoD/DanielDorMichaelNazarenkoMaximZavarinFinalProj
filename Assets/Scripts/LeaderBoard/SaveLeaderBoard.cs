using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLeaderBoard : MonoBehaviour
{
    private const string DATA_SAVE_PATH = "/leaderboard.dat";
    
    //save players count and player's data
    public void SaveLeaderBoardData(Dictionary<string, float> scoreBoard)
    {
        string path = Application.persistentDataPath + DATA_SAVE_PATH;
        
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(scoreBoard.Count);

            foreach (var data in scoreBoard)
            {
                writer.Write(data.Key);
                writer.Write(data.Value);
            }
        }
    }

    //load players count and player's data
    public Dictionary<string, float> LoadLeaderBoardData()
    {
        string path = Application.persistentDataPath + DATA_SAVE_PATH;
        
        Dictionary<string, float> scoreTableToLoad = new Dictionary<string, float>();

        if (!File.Exists(path))
            return scoreTableToLoad;

        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            int playerCount = reader.ReadInt32();

            for (int i = 0; i < playerCount; i++)
            {
                string playerName = reader.ReadString();
                float playerTime = reader.ReadSingle();
                
                scoreTableToLoad.Add(playerName, playerTime);
            }
        }

        return scoreTableToLoad;

    }
    
    
    public void ClearLeaderBoardData()
    {
        string path = Application.persistentDataPath + DATA_SAVE_PATH;

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
