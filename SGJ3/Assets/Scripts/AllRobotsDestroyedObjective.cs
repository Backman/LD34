using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/All Robots Destroyed")]
public class AllRobotsDestroyedObjective : QuestObjective
{
    public override bool IsDone()
    {
        var bots = FindObjectsOfType<BotControl>();
        return bots == null || bots.Length <= 0;
    }

    public override void UpdateObjective()
    {
		
    }

    protected override void InnerEndObjective()
    {
		
    }

    protected override void InnerStartObjective()
    {
    }
}
