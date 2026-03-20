using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeOutInAndLoad(sceneName));
    }

    private IEnumerator FadeOutInAndLoad(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        
        Time.timeScale = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.blocksRaycasts = true;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - (time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeOut()
    {
        Time.timeScale = 1f;

        fadeCanvasGroup.blocksRaycasts = true;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = time / fadeDuration;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }
}
