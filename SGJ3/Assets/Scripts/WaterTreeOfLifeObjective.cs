using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Water Tree of Life")]
public class WaterTreeOfLifeObjective : QuestObjective
{
    public int HowManyTimes;

    private int _times;

    public override bool IsDone()
    {
        return _times >= HowManyTimes;
    }

    public override void UpdateObjective()
    {
		
    }

    protected override void InnerEndObjective()
    {
        GameManager.Instance.OnWaterTree -= OnWaterTree;
    }

    protected override void InnerStartObjective()
    {
        GameManager.Instance.OnWaterTree += OnWaterTree;
    }

    private void OnWaterTree()
    {
        _times++;
    }
}