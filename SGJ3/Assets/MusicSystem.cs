using UnityEngine;
using System.Collections;

public class MusicSystem : MonoBehaviour
{
    public static MusicSystem Instance;
    public GameObject GameMusic;
    public GameObject ScreenMusic;
    public AudioSource NearTree;
    public AudioSource NotNearTree;
    public AnimationCurve FadeCurve;
    public float MinDistance;
    public float MaxDistance;
    public float Volume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            var music = GetComponents<AudioSource>();
            for (int i = 0; i < music.Length; i++)
            {
                music[i].volume = Volume;
            }
            SetGameMusic(false);
        }
    }

    public void SetGameMusic(bool gameMusic)
    {
        if (GameMusic.activeSelf != gameMusic)
        {
            GameMusic.SetActive(gameMusic);
            var sources = GameMusic.GetComponents<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i].Play();
            }
        }
        if (ScreenMusic.activeSelf != (gameMusic == false))
        {
            ScreenMusic.SetActive(gameMusic == false);
            var sources = ScreenMusic.GetComponents<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i].Play();
            }
        }

    }

    void OnLevelWasLoaded()
    {
        SetTreeDistance(1f);
    }


    public void SetTreeDistance(float distance)
    {
        float t = FadeCurve.Evaluate((distance - MinDistance) / (MaxDistance - MinDistance));
        NearTree.volume = t * Volume;
        NotNearTree.volume = (1 - t) * Volume;
    }
}
