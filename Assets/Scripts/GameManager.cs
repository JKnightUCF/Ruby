using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Manages game state, including tracking enemies and handling the game over screen
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Enemy tracking
    private int enemiesFixed = 0;
    public int totalEnemies = 4;

    // Game over handling
    private VisualElement gameOverScreen;
    public RestartHandler restartHandler;
    public bool IsGameOver { get; private set; } = false;

    // Ensure only one instance of GameManager exists
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Initialize UI and game over elements
    private void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        restartHandler = GetComponent<RestartHandler>();

        if (uiDocument != null)
        {
            var root = uiDocument.rootVisualElement;
            gameOverScreen = root.Q<VisualElement>("GameOverScreen");

            if (gameOverScreen != null)
                gameOverScreen.style.display = DisplayStyle.None; // Hide initially
            else
                Debug.LogError("GameOverScreen element not found in UI!");
        }
    }

    // Increment fixed enemies and check for game over
    public void EnemyFixed()
    {
        enemiesFixed++;
        Debug.Log($"Enemies fixed: {enemiesFixed}/{totalEnemies}");

        if (enemiesFixed >= totalEnemies)
        {
            ShowGameOver();
        }
    }

    // Display game over screen
    private void ShowGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.style.display = DisplayStyle.Flex;
            Debug.Log("Game Over screen displayed.");

            if (restartHandler != null)
                restartHandler.EnableRestart();

            IsGameOver = true;
        }
        else
        {
            Debug.LogError("GameOverScreen element not assigned or missing!");
        }
    }
}
