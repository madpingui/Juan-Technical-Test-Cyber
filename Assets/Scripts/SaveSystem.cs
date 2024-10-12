using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    private const string saveFile = "savefile.json";

    [SerializeField] private GameController gameController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button saveButton;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFile);

        // Subscribe to game state events to manage button interactability
        GameController.OnMatchStarted += MatchStarted;
        GameController.OnMatchLoaded += MatchStarted;
        GameController.OnMatchFinished += MatchFinished;

        // Enable load button if a save file exists
        if (File.Exists(savePath))
            loadButton.interactable = true;
    }

    private void OnDestroy()
    {
        GameController.OnMatchStarted -= MatchStarted;
        GameController.OnMatchLoaded -= MatchStarted;
        GameController.OnMatchFinished -= MatchFinished;
    }

    private void MatchStarted() => saveButton.interactable = true;
    private void MatchFinished() => saveButton.interactable = false;

#if UNITY_EDITOR
    [ContextMenu("Delete Save File")]
    public void DeleteFileTesting() => File.Delete(savePath); // Method for testing: deletes the save file
#endif

    public void OnSaveButtonClicked()
    {
        GameData data = gameController.GetGameData(scoreManager.Turns, scoreManager.Score, scoreManager.Combo);
        SaveGame(data);
        loadButton.interactable = true;
    }

    public void OnLoadButtonClicked()
    {
        GameData data = LoadGame();
        if (data != null)
        {
            scoreManager.Turns = data.turns;
            scoreManager.Score = data.score;
            scoreManager.Combo = data.combo;
            gameController.LoadGameData(data);
        }
    }

    // Serializes and saves the game data to a JSON file
    private void SaveGame(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved!");
    }

    // Loads and deserializes the game data from a JSON file
    private GameData LoadGame()
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
}

