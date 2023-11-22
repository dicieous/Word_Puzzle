using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    public static int GetTotalScenesInBuildSettings() => SceneManager.sceneCountInBuildSettings;
   
    public static int GetCurrentSceneByBuildIndex() => SceneManager.GetActiveScene().buildIndex;
   
    public static string GetCurrentSceneByName() => SceneManager.GetActiveScene().name;
   
    public static string GetNextSceneByName() => (SceneManager.GetActiveScene().buildIndex + 1).ToString();

    public static int GetNextSceneByBuildIndex() => SceneManager.GetActiveScene().buildIndex + 1;
   
    public static int GetRandomSceneByBuildIndex() => Random.Range(1, GetTotalScenesInBuildSettings() - 1);

    public static void LoadScene(string SceneName) => SceneManager.LoadScene(SceneName);

    public static void LoadSceneByInt(int sceneIndex) => SceneManager.LoadScene(sceneIndex);

    public static void LoadSceneAsynByIndex(int SceneIndex) => SceneManager.LoadSceneAsync(SceneIndex);

    public static void LoadSceneAsynByName(string SceneName) => SceneManager.LoadSceneAsync(SceneName);

    public static void LoadSameScene() => SceneManager.LoadScene(GetCurrentSceneByName());
   
}
