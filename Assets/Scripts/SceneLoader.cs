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
        asyncLoad.allowSceneActivation = false;

        float timer = 0.0f;
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1.0f, timer);

                if (progressBar.value >= 1.0f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }
            else
            {
                progressBar.value = asyncLoad.progress;
            }
            
            yield return null;
        }
    } 
}
