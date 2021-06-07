using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadNextScene()
    {
        Time.timeScale = 1;

        int nextSceneNum = SceneManager.GetActiveScene().buildIndex + 1;

        if(nextSceneNum > 1)
        {
            nextSceneNum = 0;
        }
        SceneManager.LoadScene(nextSceneNum);
    }
}
