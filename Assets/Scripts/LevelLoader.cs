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
            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            /*if (PlayerPrefs.GetInt("Level", 1) >= 2)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            }*/
            /*if (PlayerPrefs.GetInt("Special", 0) == 1)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 2);
            }*/
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            /*if (PlayerPrefs.GetInt("Level", 1) >= 6)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            }
            else
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt("Level") >= SceneManager.sceneCountInBuildSettings 
                    ? PlayerPrefs.GetInt("ThisLevel")
                    : PlayerPrefs.GetInt("Level", 1));
            }*/
        }
    }
}
