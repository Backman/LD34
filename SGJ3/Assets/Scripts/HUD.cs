using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour
{
    private struct TreeState
    {
        public float Health;
        public float Growth;
    }

    [SerializeField]
    private float _treeHealthUpdateSpeed = 2f;
    [SerializeField]
    private float _treeGrowthUpdateDuration = 0.5f;

    private Slider _treeHealthSlider;
    private Text _treeHealthText;
    private Text _treeGrowthText;
    private TreeOfLife _tree;

    private TreeState _treeState;

    private static HUD _instance;
    public static HUD Instance { get { return _instance; } }

    public void EnableTexts()
    {
        transform.FindChild("TreeGrowth").gameObject.SetActive(true);
        transform.FindChild("Protocol").gameObject.SetActive(true);
    }
    
    public void DisableTexts()
    {
        transform.FindChild("TreeGrowth").gameObject.SetActive(false);
        transform.FindChild("Protocol").gameObject.SetActive(false);
    }

    private void Awake()
    {
        _instance = this;
        _tree = FindObjectOfType<TreeOfLife>();
        _treeHealthSlider = transform.FindChild("TreeHealth").GetComponent<Slider>();
        _treeGrowthText = transform.FindChild("TreeGrowth").GetComponentInChildren<Text>();
        _treeGrowthText.text = _tree.StartGrowthPercentage.ToString("N0") + "%";
        _treeHealthText = _treeHealthSlider.transform.FindChild("Percentage").GetComponent<Text>();
        _treeHealthSlider.maxValue = _tree.MaxHealth;
        _treeHealthSlider.value = _tree.MaxHealth;
        _treeState.Health = _tree.MaxHealth;

        GameManager.Instance.OnTreeDamage += OnTreeDamage;
        GameManager.Instance.OnNextTreeSegment += OnNextTreeSegment;
        GameManager.Instance.OnTreeStartGrowth += OnTreeStartGrowth;
        GameManager.Instance.OnTreeStopGrowth += OnTreeStopGrowth;

        DisableTexts();
    }

    private void Update()
    {
        if (_treeState.Health > _tree.CurrentHealth)
        {
            _treeState.Health = Mathf.Lerp(_treeState.Health, _tree.CurrentHealth, _treeHealthUpdateSpeed * Time.deltaTime);
            _treeHealthSlider.value = _treeState.Health;
            var healthPercentage = (_treeState.Health / _tree.MaxHealth) * 100f;
            _treeHealthText.text = healthPercentage.ToString("N0") + "%";
        }
    }

    private void OnTreeDamage(float amount, float curHealth)
    {
    }

    private void OnNextTreeSegment(TreeOfLife tree, int prevSegment, int nextSegment)
    {
        var fromGrowth = tree.GetGrowthValue(prevSegment);
        var toGrowth = tree.GetGrowthValue(nextSegment);
        
        StartCoroutine(UpdateGrowthText(fromGrowth, toGrowth));
    }

    private IEnumerator UpdateGrowthText(float fromGrowth, float toGrowth)
    {
        float t = 0f;
        var curGrowth = fromGrowth;
        while (t < _treeGrowthUpdateDuration)
        {
            curGrowth = Mathf.Lerp(curGrowth, toGrowth, t / _treeGrowthUpdateDuration);
            _treeGrowthText.text = curGrowth.ToString("N0") + "%";
            t += Time.deltaTime;
            yield return null;
        }

        _treeGrowthText.text = toGrowth.ToString("N0") + "%";
    }

    private void OnTreeStartGrowth()
    {
        _treeGrowthText.gameObject.SetActive(true);
    }
    
    private void OnTreeStopGrowth()
    {
    }
}
