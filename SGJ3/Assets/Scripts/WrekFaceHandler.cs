using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WrekFaceHandler : MonoBehaviour
{
    public float RekDuration = 0.08f;
    [Range(0.0f, 1.0f)]
    public float FreezeValue = 0.0f;
    public static WrekFaceHandler Instance;

    void Awake()
    {
        Instance = this;
    }
    //[System.Serializable]
    //public struct ComboTextData
    //{
    //    public int MinCombo;
    //    public int MaxCombo;
    //    public GameObject Prefab;
    //}
    //public ComboTextData[] ComboData;
    //public float ComboTextYOffset;
    //public float ComboTextCircleSize;
    bool _HasFreezeTime = false;
    //float _LastComboText;

    //public RekCombo RekCombo;

    //void Awake()
    //{
    //    GameLogic.Instance.OnRekFace += OnRekFace;
    //    GameLogic.Instance.OnBlokDamage += OnBlokDamage;
    //    GameLogic.Instance.OnRekComboIncreased += OnRekComboIncreased;

    //    RekCombo.Init();

    //}

    //void OnRekComboIncreased(GameObject obj, int combo)
    //{
    //    if (_LastComboText + 0.5f >= Time.unscaledTime)
    //        return;
    //    if (Random.Range(0f, 1f) > 0.7f)
    //        return;
    //    List<ComboTextData> valid = new List<ComboTextData>();
    //    for (int i = 0; i < ComboData.Length; i++)
    //    {
    //        if (combo >= ComboData[i].MinCombo && combo <= ComboData[i].MaxCombo)
    //        {
    //            valid.Add(ComboData[i]);
    //        }
    //    }
    //    if (valid.Count > 0)
    //    {
    //        ComboTextData comboData = valid[Random.Range(0, valid.Count)];
    //        Vector2 unitCircle = Random.insideUnitCircle * ComboTextCircleSize;
    //        Vector2 pos = (Vector2)obj.transform.position + unitCircle + new Vector2(0, ComboTextYOffset);

    //        TrashMan.spawn(comboData.Prefab, new Vector3(pos.x, pos.y, 30));
    //        _LastComboText = Time.unscaledTime;
    //    }
    //}
    //void OnBlokDamage(int dmg)
    //{
    //    BlinkManager.Instance.AddBlink(Blokfosk.Instance.gameObject, BlinkColor, BlokfoskBlinkDuration);
    //}

    public void OnRekFace()
    {
        var rekDuration = RekDuration;
        var freezeValue = FreezeValue;

        if (_HasFreezeTime == false)
        {
            StartCoroutine(FreezeTime(rekDuration, freezeValue));
            _HasFreezeTime = true;
        }
    }

    IEnumerator FreezeTime(float rekDuration, float freezeValue)
    {
        float startTime = Time.unscaledTime;
        Time.timeScale = freezeValue;
        while (startTime + rekDuration > Time.unscaledTime)
        {
            yield return null;
        }
        Time.timeScale = 1f;
        _HasFreezeTime = false;
    }
}
