using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Stop Robot Spawn")]
public class StopRobotSpawnObjective : QuestObjective
{
    public float Delay;

    private float _startTime;
    public override bool IsDone()
    {
        var shouldEnd = _startTime <= Time.time;
        if (shouldEnd)
        {
            BotSpawning.Instance.StopSpawn();
        }
        return shouldEnd;
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