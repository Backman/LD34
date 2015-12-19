using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Water")]
public class WaterObjective : QuestObjective
{
    public int WaterToPickUp;
    public int SpawnWaterAmount;

    public int CurrentWaterAmount { get; set; }

    protected override void InnerStartObjective()
    {
        CurrentWaterAmount = 0;
        GameManager.Instance.SpawnWater(SpawnWaterAmount);
    }

    protected override void InnerEndObjective()
    {
    }

    public override void UpdateObjective()
    {
    }

    public override bool IsDone()
    {
        return CurrentWaterAmount >= WaterToPickUp;
    }
}