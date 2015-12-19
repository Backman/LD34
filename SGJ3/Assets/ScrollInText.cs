using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollInText : MonoBehaviour
{
    public float FadeInDuration;
    string _OriginalText;
    Text _Text;
    float _startTime;

    void Awake()
    {
        _Text = GetComponent<Text>();
        _OriginalText = _Text.text;
    }

    void OnEnable()
    {
        _startTime = Time.time;
    }

    void Update()
    {
        float t = Mathf.Clamp01((Time.time - _startTime) / FadeInDuration);
        int charsToShow = Mathf.RoundToInt(t * _OriginalText.Length);
        if (charsToShow != _Text.text.Length)
        {
            _Text.text = _OriginalText.Substring(0, charsToShow);
        }
    }
}
