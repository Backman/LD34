using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScreenController : MonoBehaviour
{
    public AnimationCurve FadeCurve;
    public float FadeTime;
    public Image BlackOverlay;

    public static MenuScreenController Instance;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (SceneManager.GetActiveScene().name == "StartScreen")
        {
            StartCoroutine(MainScreenState("Tutorial", () =>
            {
                StartCoroutine(MainScreenState("StoryScreen", () =>
                {
                    StartCoroutine(MainScreenState("konrad_coolmap", null,6, false));
                }));
            }));
        }
    }

    public void WinScreen()
    {
        StartCoroutine(MainScreenState("WinScreen", () =>
        {
            StartCoroutine(MainScreenState("konrad_coolmap", null));
        }));
    }

    public void LoseScreen()
    {
        StartCoroutine(MainScreenState("LoseScreen", () =>
        {
            StartCoroutine(MainScreenState("konrad_coolmap", null));
        }));
    }

    IEnumerator MainScreenState(string levelToLoad, System.Action onDone, float waitTime = 0.3f, bool requireKeyPress = true)
    {
        yield return new WaitForSeconds(waitTime);
        if (requireKeyPress)
        {
            while (Input.anyKey == false)
            {
                yield return null;
            }
        }

        float startTime = Time.time;
        while (Time.time < startTime + FadeTime)
        {
            float alpha = FadeCurve.Evaluate((Time.time - startTime) / FadeTime);
            SetOverlayAlpha(alpha);
            yield return null;
        }
        SetOverlayAlpha(1f);
        SceneManager.LoadScene(levelToLoad, LoadSceneMode.Single);
        MusicSystem.Instance.SetGameMusic(onDone == null);
        startTime = Time.time;
        while (Time.time < startTime + FadeTime)
        {
            float alpha = FadeCurve.Evaluate(1 - ((Time.time - startTime) / FadeTime));
            SetOverlayAlpha(alpha);
            yield return null;
        }
        SetOverlayAlpha(0f);
        if (onDone != null)
            onDone();
    }

    private void SetOverlayAlpha(float alpha)
    {
        var color = BlackOverlay.color;
        color.a = alpha;
        BlackOverlay.color = color;
    }
}
