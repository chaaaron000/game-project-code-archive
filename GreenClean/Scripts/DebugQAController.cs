using UnityEngine.SceneManagement;

public sealed class DebugQAController : SingletonComponent<DebugQAController>
{
#if UNITY_EDITOR
    private GridManager gridManager = null;
    private GameManager gameManager = null;

    public void Debug_SetAllTilesPurified()
    {
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                DebugConsole.LogError("[DebugQAController] GridManager not found.");
                return;
            }
        }

        gridManager.Debug_SetAllPurified();
    }

    public void Debug_TriggerGameOver()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                DebugConsole.LogError("[DebugQAController] GameManager not found.");
                return;
            }
        }

        gameManager.Debug_GameOver();
    }

    public static bool Debug_TryGetCurrentSceneType(out SceneType sceneType)
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (!System.Enum.IsDefined(typeof(SceneType), buildIndex))
        {
            sceneType = default;
            return false;
        }

        sceneType = (SceneType)buildIndex;
        return true;
    }
#endif
}
