using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public bool isTesting;
    public int buildIndex;

    private void Start()
    {
        if (isTesting)
        {
            SavedData.SetSpecialLevelNumber( buildIndex);
            PlayerPrefs.SetInt("SpecialLevelNumber", buildIndex);
            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            /*if (SavedData.GetSpecialLevelNumber() >= 2)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            }*/
            /*if (PlayerPrefs.GetInt("Special", 0) == 1)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 2);
            }*/
            if (SavedData.GetSpecialLevelNumber() >= 4)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            }
            else
            {
                SceneManager.LoadScene(SavedData.GetLevelNumber() >= SceneManager.sceneCountInBuildSettings - 34
                    ? PlayerPrefs.GetInt("ThisLevel")
                    : SavedData.GetLevelNumber());
            }
        }
    }
}
