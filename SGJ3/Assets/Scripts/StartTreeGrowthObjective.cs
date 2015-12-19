using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Start Tree Growth")]
public class StartTreeGrowthObjective : QuestObjective
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
        FindObjectOfType<TreeOfLife>().StartGrowth();
    }
}