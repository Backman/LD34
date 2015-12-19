using UnityEngine;

public abstract class QuestObjective : ScriptableObject
{
    public void StartObjective()
    {
        if (GameManager.Instance.OnObjectiveStart != null)
        {
            GameManager.Instance.OnObjectiveStart(this);
        }
        InnerStartObjective();
    }

    public void EndObjective()
    {
        if (GameManager.Instance.OnObjectiveEnd != null)
        {
            GameManager.Instance.OnObjectiveEnd(this);
        }
        InnerEndObjective();
    }
    
    protected abstract void InnerStartObjective();
    protected abstract void InnerEndObjective();
    public abstract void UpdateObjective();
    public abstract bool IsDone();
}