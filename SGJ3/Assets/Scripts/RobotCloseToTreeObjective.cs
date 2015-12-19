using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Robot Close To Tree")]
public class RobotCloseToTreeObjective : QuestObjective
{
    public float Distance;

    private float _treeX;

    private float _maxWaitTime = 20f;
    private float _startTime;

    public override bool IsDone()
    {
        var bots = FindObjectsOfType<BotControl>();
        float closestDistance = float.MaxValue;
        for (int i = 0; i < bots.Length; i++)
        {
            var distance = Mathf.Abs(bots[i].transform.position.x - _treeX);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
        
        return closestDistance <= Distance || _startTime < Time.time;
    }

    public override void UpdateObjective()
    {
		
    }

    protected override void InnerEndObjective()
    {
		
    }

    protected override void InnerStartObjective()
    {
        _treeX = FindObjectOfType<TreeOfLife>().transform.position.x;
        _startTime = Time.time + _maxWaitTime;
    }
}