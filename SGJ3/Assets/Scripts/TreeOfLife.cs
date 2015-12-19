using UnityEngine;
using System.Collections;

public class TreeOfLife : MonoBehaviour
{
    class VisibleState
    {
        public int Segment;
        public GameObject Instance;
    }

    [SerializeField]
    private TreeOfLifeData _data;

    private float _curHealth;
    private float _curGrowth;
    private int _prevSegment;
    private bool _growing;
    private VisibleState _visibleState;

    private VisibleState[] _states;

    public bool Growing { get { return _growing; } }
    public float MaxHealth { get { return _data.Health; } }
    public float MaxGrowth { get { return _data.MaxGrowth; } }
    public float HealthPercentage { get { return Mathf.Clamp01(_curHealth / MaxHealth) * 100; } }
    public float GrowthPercentage { get { return Mathf.Clamp01(_curGrowth / MaxGrowth) * 100; } }
    public float CurrentHealth { get { return _curHealth; } }
    public float CurrentGrowth { get { return _curGrowth; } }

    public int CurrentSegment
    {
        get
        {
            // var prevStart = 0f;
            // for (int i = 0; i < _data.Segments.Length; i++)
            // {
            //     if (_curGrowth <= _data.Segments[i].StartAtGrowthValue + prevStart)
            //     {
            //         return i - 1;
            //     }
            //     prevStart += _data.Segments[i].StartAtGrowthValue;
            // }
            // return _data.Segments.Length - 1;
            return _visibleState.Segment;
        }
    }

    public float NextGrowthStart { get { return GetRelativeStartAtGrowthValue(CurrentSegment + 1); } }
    public float StartGrowthPercentage { get { return _data.StartGrowthPercentage; } }

    private void Awake()
    {
        InitPrefabs();
        SetVisible(0);
        _curHealth = _data.Health;
    }

    private void InitPrefabs()
    {
        _states = new VisibleState[_data.Segments.Length];
        for (int i = 0; i < _data.Segments.Length; i++)
        {
            var instance = Instantiate(_data.Segments[i].Prefab);
            instance.transform.parent = transform;
            instance.transform.position = transform.position;
            instance.SetActive(false);
            _states[i] = new VisibleState()
            {
                Segment = i,
                Instance = instance
            };
        }
    }

    public float GetGrowthValue(int segment)
    {
        if(segment >= _data.Segments.Length) return 100;
        return _data.Segments[segment].GrowthPercentage;
    }

    private void Update()
    {
        if (!_growing) return;

        var growth = _data.GrowthSpeed * Time.deltaTime;
        _curGrowth += growth;
        CheckForSegmentChange();
    }

    public void RecalculateCollider()
    {
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void GoToNextSegment()
    {
        var nextSegment = CurrentSegment + 1;
        if (nextSegment < _data.Segments.Length)
        {
            GoToSegment(_prevSegment, CurrentSegment + 1);
        }
    }

    public float GetStartAtGrowthValue(int segment)
    {
        if (segment >= _data.Segments.Length) return MaxGrowth - GetRelativeStartAtGrowthValue(segment - 1);
        return _data.Segments[segment].StartAtGrowthValue;
    }

    public float GetRelativeStartAtGrowthValue(int segment)
    {
        if (segment >= _data.Segments.Length) return _data.MaxGrowth;

        var start = 0f;
        for (int i = 0; i <= segment; i++)
        {
            start += _data.Segments[i].StartAtGrowthValue;
        }
        return start;
    }

    public void StartGrowth()
    {
        if (GameManager.Instance.OnTreeStartGrowth != null)
        {
            GameManager.Instance.OnTreeStartGrowth();
        }
        _growing = true;
    }

    public void StopGrowth()
    {
        if (GameManager.Instance.OnTreeStopGrowth != null)
        {
            GameManager.Instance.OnTreeStopGrowth();
        }
        _growing = true;
    }

    public void ResetTree()
    {
        _curHealth = _data.Health;
        _curGrowth = 0f;
        _prevSegment = 0;
    }

    public void Damage(float amount, Vector2 hitPoint)
    {
        _curHealth -= amount;
        _curHealth = Mathf.Max(0f, _curHealth);
        if (_data.TakeDamagePrefab)
        {
            Instantiate(_data.TakeDamagePrefab, hitPoint, Quaternion.identity);
        }
        if (GameManager.Instance.OnTreeDamage != null)
        {
            GameManager.Instance.OnTreeDamage(amount, _curHealth);
        }
        if (_curHealth <= 0f)
        {
            // KILL THE MOTTAFAKKA TREE
            if (GameManager.Instance.OnTreeDeath != null)
            {
                GameManager.Instance.OnTreeDeath();
            }
        }
    }

    private void CheckForSegmentChange()
    {
        var nextSegment = CurrentSegment + 1;
        if (nextSegment > _prevSegment)
        {
            GoToSegment(_prevSegment, nextSegment);
        }
    }

    void SetVisible(int segment)
    {
        segment = Mathf.Clamp(segment, 0, _data.Segments.Length);
        if (_visibleState == null)
        {
            _visibleState = _states[segment];
            _visibleState.Instance.SetActive(true);
        }
        if (_visibleState.Segment != segment)
        {
            StartCoroutine(Evolve(segment));
        }
    }

    private IEnumerator Evolve(int toSegment)
    {
        int count = 0;
        var toState = _states[toSegment];
        var red = true;
        while (count <= _data.EvolveBlinkCount)
        {
            count++;
            Color color;
            color = _data.EvolveBlinkGradient.Evaluate(count / (float)_data.EvolveBlinkCount);
            
            _visibleState.Instance.SetActive(false);
            toState.Instance.SetActive(true);
            toState.Instance.GetComponent<SpriteRenderer>().color = red ? color : Color.white;
            yield return new WaitForSeconds(_data.EvolveBlinkWaitTime);
            _visibleState.Instance.SetActive(true);
            _visibleState.Instance.GetComponent<SpriteRenderer>().color = red ? color : Color.white;
            red = !red;
            toState.Instance.SetActive(false);
            yield return new WaitForSeconds(_data.EvolveBlinkWaitTime);
        }
        _visibleState.Instance.SetActive(false);
        _visibleState.Instance.GetComponent<SpriteRenderer>().color = Color.white;
        toState.Instance.GetComponent<SpriteRenderer>().color = Color.white;
        toState.Instance.SetActive(true);
        _visibleState = toState;
    }

    private void GoToSegment(int from, int to)
    {
        if (GameManager.Instance.OnNextTreeSegment != null)
        {
            GameManager.Instance.OnNextTreeSegment(this, from, to);
        }
        SetVisible(to);
        _prevSegment = to;
        Sound.Instance.PlayClipAtPoint(_data.Segments[to - 1].StartSegmentClip, transform.position);
    }
}