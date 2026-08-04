using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class MainSceneRestarter : MonoBehaviour
{
    private const string MainScenePath = "Assets/Scenes/Main.unity";

    private Button restartButton;

    private void Awake()
    {
        restartButton = GetComponent<Button>();
        restartButton.onClick.AddListener(RestartMainScene);
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartMainScene);
        }
    }

    public void RestartMainScene()
    {
        int mainSceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(MainScenePath);
        if (mainSceneBuildIndex < 0)
        {
            Debug.LogError($"Main scene is not enabled in Build Settings: {MainScenePath}");
            return;
        }

        Debug.Log($"Restarting Main scene (build index {mainSceneBuildIndex}).");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainSceneBuildIndex, LoadSceneMode.Single);
    }
}
