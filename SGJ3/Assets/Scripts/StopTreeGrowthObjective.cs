using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Stop Tree Growth")]
public class StopTreeGrowthObjective : QuestObjective
{
    public override bool IsDone()
    {
        return true;
    }

    public override void UpdateObjective()
    {
		
    }

    protected override void InnerEndObjective()
    {
		
    }

    protected override void InnerStartObjective()
    {
        FindObjectOfType<TreeOfLife>().StopGrowth();
    }
}