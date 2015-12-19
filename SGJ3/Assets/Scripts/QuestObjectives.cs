using System.Collections.Generic;
using UnityEngine;

[System.SerializableAttribute]
public class QuestObjectives
{
    public QuestObjectivesData Objectives;
    private Queue<QuestObjective> _objectives;

    private QuestObjective _currentObjective;
    
    public QuestObjective CurrentObjective { get { return _currentObjective; } }

    public void Initialize()
    {
        _objectives = new Queue<QuestObjective>();
        foreach (var objective in Objectives.QuestObjectives)
        {
            var clone = ScriptableObject.Instantiate(objective);
            _objectives.Enqueue(clone);
        }
        GoToNextQuest();
    }

    public void Update()
    {
        if (_currentObjective)
        {
            _currentObjective.UpdateObjective();
            if (_currentObjective.IsDone())
            {
                GoToNextQuest();
            }
        }
    }

    public bool IsDone()
    {
        return _objectives.Count <= 0 && _currentObjective == null;
    }

    private void GoToNextQuest()
    {
        if (_currentObjective)
        {
            _currentObjective.EndObjective();
        }
        if (_objectives.Count > 0)
        {
            _currentObjective = _objectives.Dequeue();
            _currentObjective.StartObjective();
            Debug.Log("Current Objective: " + _currentObjective);
        }
        else
        {
            _currentObjective = null;
        }
    }
}