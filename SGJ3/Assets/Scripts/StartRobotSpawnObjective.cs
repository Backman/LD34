using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Start Robot Spawn")]
public class StartRobotSpawnObjective : QuestObjective
{
    public BotSpawnSettings SpawnSettings;

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
        BotSpawning.Instance.StartSpawn(SpawnSettings);
    }
}