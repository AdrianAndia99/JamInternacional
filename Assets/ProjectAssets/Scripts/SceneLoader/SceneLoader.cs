using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader")]
public class SceneLoader : ScriptableObject
{
    // --- Synchronous Load ---
    public void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    // --- Asynchronous Load ---
    public async void LoadAsyncScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName)) return;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            await Task.Yield();
        }

        operation.allowSceneActivation = true;
    }

    public async void UnloadAsyncScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded) return;

        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            await Task.Yield();
        }
    }

    // --- Additive Load ---
    public async void LoadAdditiveScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName)) return;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            await Task.Yield();
        }
    }

    public async void UnloadAdditiveScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded) return;

        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            await Task.Yield();
        }
    }
}