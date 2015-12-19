using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Delay")]
public class DelayObjective : QuestObjective
{
    public float Delay;

    private float _startTime;
	
    public override bool IsDone()
    {
        return _startTime <= Time.time;
    }

    public override void UpdateObjective()
    {
		
    }

    protected override void InnerEndObjective()
    {
		
    }

    protected override void InnerStartObjective()
    {
        _startTime = Time.time + Delay;
    }
}