using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    private const string saveFile = "savefile.json";

    [SerializeField] private GameController gameController;
    [SerializeField] private ScoreManager scoreManager;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFile);
    }

    public void SaveGame(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved!");
    }

    public GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game loaded!");
            return gameData;
        }
        Debug.LogError("No save file found!");
        return null;
    }

    public void OnSaveButtonClicked()
    {
        GameData data = gameController.GetGameData(scoreManager.GetScore(), scoreManager.GetCombo());
        SaveGame(data);
    }

    public void OnLoadButtonClicked()
    {
        GameData data = LoadGame();
        if (data != null)
        {
            scoreManager.SetScore(data.score);
            scoreManager.SetCombo(data.combo);
            gameController.LoadGameData(data);
        }
    }
}

