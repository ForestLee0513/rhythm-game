using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class SceneLoader : MonoBehaviour 
{
    [SerializeField]
    Slider progressBar;

    static string nextScene = "";
    
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScreen");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneAsyncCoroutine());
    }

    private IEnumerator LoadSceneAsyncCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene);

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                progressBar.value = 1.0f;
            }
            else
            {
                progressBar.value = asyncLoad.progress;
            }
            
            yield return null;
        }
    } 
}