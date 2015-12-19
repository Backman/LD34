using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Next Tree Segment")]
public class NextTreeSegmentObjective : QuestObjective
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
        var tree = FindObjectOfType<TreeOfLife>();
        tree.GoToNextSegment();
    }
}