using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameInitializer : MonoBehaviour
{
    [SerializeField]
    private string _startScene;

    private IEnumerator Start()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_startScene);
        asyncLoad.allowSceneActivation = false;

        Debug.Log("Началась загрузка сцены");

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Прогресс: {progress * 100}%");

            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Готово! Активарию сцену");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}